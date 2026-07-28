using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleZapp.Interop
{
    public static class ConsoleInput
    {
        private const int STD_INPUT_HANDLE = -10;
        private const uint ENABLE_WINDOW_INPUT = 0x0008;
        private const uint ENABLE_MOUSE_INPUT = 0x0010;
        private const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        private const ushort KEY_EVENT = 0x0001;
        private const ushort MOUSE_EVENT = 0x0002;
        private const ushort WINDOW_BUFFER_SIZE_EVENT = 0x0004;
        private const uint MOUSE_WHEELED = 0x0004;
        private const int WHEEL_DELTA = 120;
        private const int MAX_CONSECUTIVE_READ_FAILURES = 20;
        private const int READ_FAILURE_BACKOFF_MS = 50;

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private static IntPtr InputHandle;
        private static bool WindowInputEnabled;

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEY_EVENT_RECORD
        {
            // Field names mirror the native Win32 struct exactly (Hungarian/camelCase) - deliberate exception to the snake_case/PascalCase rule, for 1:1 traceability against Win32 docs
            public int bKeyDown;
            public ushort wRepeatCount;
            public ushort wVirtualKeyCode;
            public ushort wVirtualScanCode;

            // Deliberately ushort, not char: a char field here would marshal under the struct's (default Ansi) CharSet as a single narrowed byte instead of the native 2-byte WCHAR, corrupting this field and misaligning dwControlKeyState right after it.
            public ushort UnicodeChar;

            public uint dwControlKeyState;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOW_BUFFER_SIZE_RECORD
        {
            public COORD dwSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSE_EVENT_RECORD
        {
            // Field names mirror the native Win32 struct exactly (Hungarian/camelCase) - deliberate exception to the snake_case/PascalCase rule, for 1:1 traceability against Win32 docs
            public COORD dwMousePosition;
            public uint dwButtonState;
            public uint dwControlKeyState;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT_RECORD
        {
            [FieldOffset(0)]
            public ushort EventType;

            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;

            [FieldOffset(4)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;

            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool ReadConsoleInput(
            IntPtr hConsoleInput,
            [Out] INPUT_RECORD[] lpBuffer,
            uint nLength,
            out uint lpNumberOfEventsRead);

        public readonly struct NativeKeyEvent
        {
            // A key-down event, translated to what ReadLineFromKeys needs
            public NativeKeyEvent(ushort virtual_key_code, char character, ushort repeat_count)
            {
                VirtualKeyCode = virtual_key_code;
                Char = character;
                RepeatCount = repeat_count;
            }

            public ushort VirtualKeyCode { get; }
            public char Char { get; }
            public ushort RepeatCount { get; }
        }

        // Reads raw Win32 console input
        // - bypasses ReadKey's lossy codepage translation
        // - picks up resize events immediately
        // - blocks natively until a key/resize/wheel event
        // - null return means a resize was handled
        // - wheel events synthesize as PageUp/PageDown
        // - other mouse/menu/focus events are skipped
        public static NativeKeyEvent? ReadKeyOrResize(Action on_resize)
        {
            EnsureWindowInputEnabled();

            var buffer = new INPUT_RECORD[1];
            var consecutive_failures = 0;

            while (true)
            {
                if (!ReadConsoleInput(InputHandle, buffer, 1, out var read))
                {
                    var error = Marshal.GetLastWin32Error();
                    consecutive_failures++;

                    if (consecutive_failures > MAX_CONSECUTIVE_READ_FAILURES)
                        throw new InvalidOperationException($"ReadConsoleInput failed {consecutive_failures} times in a row (last Win32 error {error}).");

                    Thread.Sleep(READ_FAILURE_BACKOFF_MS);
                    continue;
                }

                consecutive_failures = 0;

                if (read == 0)
                    continue;

                var record = buffer[0];

                if (record.EventType == WINDOW_BUFFER_SIZE_EVENT)
                {
                    on_resize?.Invoke();
                    return null;
                }

                if (record.EventType == KEY_EVENT && record.KeyEvent.bKeyDown != 0)
                {
                    return new NativeKeyEvent(
                        record.KeyEvent.wVirtualKeyCode,
                        (char)record.KeyEvent.UnicodeChar,
                        record.KeyEvent.wRepeatCount);
                }

                if (record.EventType == MOUSE_EVENT && (record.MouseEvent.dwEventFlags & MOUSE_WHEELED) != 0)
                {
                    // Wheel delta lives in the high-order 16 bits of dwButtonState, as a signed count of WHEEL_DELTA (120) units - positive means scrolled up/away from the user, negative means scrolled down/towards
                    var delta = (short)(record.MouseEvent.dwButtonState >> 16);

                    if (delta == 0)
                        continue;

                    var virtual_key = delta > 0 ? ConsoleKey.PageUp : ConsoleKey.PageDown;
                    var repeat_count = (ushort)Math.Max(1, Math.Abs(delta) / WHEEL_DELTA);

                    return new NativeKeyEvent((ushort)virtual_key, '\0', repeat_count);
                }
            }
        }

        // Enables window/mouse input on the console mode (once)
        // - preserves other flags, e.g. Ctrl+C keeps working
        // - lets ReadConsoleInput also emit resize/wheel records
        // - clears QuickEdit (mutually exclusive with mouse input)
        // - trade-off: native click-drag text selection stops working
        // - Windows-only by design, see .ideas.md for a future rewrite
        private static void EnsureWindowInputEnabled()
        {
            if (WindowInputEnabled)
                return;

            InputHandle = GetStdHandle(STD_INPUT_HANDLE);

            if (InputHandle == IntPtr.Zero || InputHandle == INVALID_HANDLE_VALUE)
                return;

            if (GetConsoleMode(InputHandle, out var mode))
            {
                var new_mode = (mode | ENABLE_WINDOW_INPUT | ENABLE_MOUSE_INPUT | ENABLE_EXTENDED_FLAGS) & ~ENABLE_QUICK_EDIT_MODE;

                if (SetConsoleMode(InputHandle, new_mode))
                    WindowInputEnabled = true;
            }
        }
    }
}
