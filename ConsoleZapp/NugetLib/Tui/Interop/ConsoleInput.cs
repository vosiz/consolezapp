using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConsoleZapp.Interop
{
    internal static class ConsoleInput
    {
        private const int STD_INPUT_HANDLE = -10;
        private const uint ENABLE_WINDOW_INPUT = 0x0008;
        private const ushort KEY_EVENT = 0x0001;
        private const ushort WINDOW_BUFFER_SIZE_EVENT = 0x0004;
        private const int MAX_CONSECUTIVE_READ_FAILURES = 20;
        private const int READ_FAILURE_BACKOFF_MS = 50;

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

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT_RECORD
        {
            [FieldOffset(0)]
            public ushort EventType;

            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;

            [FieldOffset(4)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
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

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private static IntPtr InputHandle;
        private static bool WindowInputEnabled;

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

        // Enables ENABLE_WINDOW_INPUT on the console input mode (once), preserving every other existing flag (notably ENABLE_PROCESSED_INPUT, so Ctrl+C keeps working) - this is what makes ReadConsoleInput also emit WINDOW_BUFFER_SIZE_EVENT records on resize.
        // Windows-only by design - see .ideas.md for a possible future cross-platform native rewrite.
        private static void EnsureWindowInputEnabled()
        {
            if (WindowInputEnabled)
                return;

            InputHandle = GetStdHandle(STD_INPUT_HANDLE);

            if (InputHandle == IntPtr.Zero || InputHandle == INVALID_HANDLE_VALUE)
                return;

            if (GetConsoleMode(InputHandle, out var mode) && SetConsoleMode(InputHandle, mode | ENABLE_WINDOW_INPUT))
                WindowInputEnabled = true;
        }

        // Reads raw Win32 console input, bypassing Console.ReadKey's lossy codepage translation (needed for correct multibyte/non-ASCII typed input, e.g. "€") and picking up resize events immediately instead of only on the next Console.ReadKey-based call.
        // Blocks (natively, no polling) until either a key-down event or a resize event arrives.
        // Returns the key event, or null if a resize was handled (caller should re-render and call this again).
        // Key-up, mouse, menu and focus events are silently skipped.
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
            }
        }
    }
}
