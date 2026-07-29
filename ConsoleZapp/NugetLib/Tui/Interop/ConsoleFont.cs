using System;
using System.Runtime.InteropServices;

namespace ConsoleZapp.Interop
{
    public static class ConsoleFont
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const int LF_FACESIZE = 32;
        private const uint TMPF_TRUETYPE = 0x04;

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

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

        // Reports whether the console uses a raster (bitmap) font
        // - a raster font garbles multi-byte UTF-8 box-drawing glyphs
        // - returns false (assume fine) if redirected or unreadable
        // - read-only by design, forcing a font live shifts geometry
        public static bool IsRasterFont()
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
