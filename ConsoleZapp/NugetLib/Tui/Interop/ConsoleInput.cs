using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleZapp.Interop
{
    internal static class ConsoleInput
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

        internal readonly struct NativeKeyEvent
        {
            // A single native key-down event, translated to the pieces ReadLineFromKeys needs
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

        // Enables ENABLE_WINDOW_INPUT and ENABLE_MOUSE_INPUT on the console input mode (once), preserving every other existing flag (notably ENABLE_PROCESSED_INPUT, so Ctrl+C keeps working) - this is what makes ReadConsoleInput also emit WINDOW_BUFFER_SIZE_EVENT records on resize and MOUSE_EVENT records for the wheel.
        // ENABLE_QUICK_EDIT_MODE is cleared and ENABLE_EXTENDED_FLAGS set alongside it - the two mouse-input modes are mutually exclusive on Windows (QuickEdit swallows mouse events before the app ever sees them). Trade-off: native click-drag text selection in the console window stops working once this is enabled.
        // Windows-only by design - see .ideas.md for a possible future cross-platform native rewrite.
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

        // Reads raw Win32 console input, bypassing Console.ReadKey's lossy codepage translation (needed for correct multibyte/non-ASCII typed input, e.g. "€") and picking up resize events immediately instead of only on the next Console.ReadKey-based call.
        // Blocks (natively, no polling) until either a key-down event, a resize event, or a mouse-wheel event arrives.
        // Returns the key event, or null if a resize was handled (caller should re-render and call this again).
        // Mouse wheel events are synthesized into PageUp/PageDown NativeKeyEvents so callers need no mouse-specific handling. Key-up, non-wheel mouse, menu and focus events are silently skipped.
        internal static NativeKeyEvent? ReadKeyOrResize(Action on_resize)
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
    }
}
