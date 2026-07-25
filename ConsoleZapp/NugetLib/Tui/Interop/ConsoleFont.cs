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

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        // Reports whether the console is currently using a raster (bitmap) font, which can't decode multi-byte UTF-8 box-drawing glyphs into a single character and garbles them one byte at a time.
        // Returns false (assume fine, no fallback needed) for redirected/non-interactive output or if the font can't be read at all - there's no real console font to inspect there, or no reliable signal to act on.
        // Deliberately read-only: an earlier version also tried to force a TrueType font via SetCurrentConsoleFontEx, but changing the font live could itself shift the console's effective column/row geometry underneath Tui's own row-position math - not worth the risk for a cosmetic upgrade.
        internal static bool IsRasterFont()
        {
            if (Console.IsOutputRedirected)
                return false;

            var handle = GetStdHandle(STD_OUTPUT_HANDLE);

            if (handle == IntPtr.Zero || handle == INVALID_HANDLE_VALUE)
                return false;

            var font = new CONSOLE_FONT_INFO_EX { cbSize = (uint)Marshal.SizeOf(typeof(CONSOLE_FONT_INFO_EX)) };

            if (!GetCurrentConsoleFontEx(handle, false, ref font))
                return false;

            return (font.FontFamily & TMPF_TRUETYPE) == 0;
        }
    }
}
