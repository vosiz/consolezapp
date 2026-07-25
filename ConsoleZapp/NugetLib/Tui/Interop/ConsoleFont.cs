using System;
using System.Runtime.InteropServices;

namespace ConsoleZapp.Interop
{
    internal static class ConsoleFont
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const int LF_FACESIZE = 32;
        private const uint TMPF_TRUETYPE = 0x04;

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public uint FontFamily;
            public uint FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
            public string FaceName;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        // Reads the console's current font into font, returns whether the call succeeded
        private static bool TryReadFont(IntPtr handle, out CONSOLE_FONT_INFO_EX font)
        {
            font = new CONSOLE_FONT_INFO_EX { cbSize = (uint)Marshal.SizeOf(typeof(CONSOLE_FONT_INFO_EX)) };
            return GetCurrentConsoleFontEx(handle, false, ref font);
        }

        // Requests face_name via SetCurrentConsoleFontEx, then re-reads the font to confirm the switch actually took.
        // Windows silently keeps the previous (raster) font instead of failing when the requested face name isn't installed, so the blind "set" return value can't be trusted on its own.
        private static bool TryForceFace(IntPtr handle, CONSOLE_FONT_INFO_EX font, string face_name)
        {
            font.FaceName = face_name;

            if (font.dwFontSize.Y == 0)
                font.dwFontSize.Y = 16;

            if (!SetCurrentConsoleFontEx(handle, false, ref font))
                return false;

            return TryReadFont(handle, out var applied) && (applied.FontFamily & TMPF_TRUETYPE) != 0;
        }

        // Ensures the console uses a TrueType font, forcing preferred_face_name (falling back to fallback_face_name) if a raster font is detected.
        // Returns true if the font is confirmed TrueType (already was, or was successfully switched), false if it's raster and neither face could be forced.
        // Also returns true (no-op) for redirected/non-interactive output, where there's no real console font to inspect or change.
        internal static bool TryEnsureTrueTypeFont(string preferred_face_name = "Consolas", string fallback_face_name = "Lucida Console")
        {
            if (Console.IsOutputRedirected)
                return true;

            var handle = GetStdHandle(STD_OUTPUT_HANDLE);

            if (handle == IntPtr.Zero || handle == INVALID_HANDLE_VALUE)
                return true;

            if (!TryReadFont(handle, out var font))
                return true;

            if ((font.FontFamily & TMPF_TRUETYPE) != 0)
                return true;

            return TryForceFace(handle, font, preferred_face_name) || TryForceFace(handle, font, fallback_face_name);
        }
    }
}
