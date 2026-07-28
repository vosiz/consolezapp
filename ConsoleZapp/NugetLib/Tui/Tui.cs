using System;
using System.Collections.Generic;
using System.Text;
using ConsoleZapp.Interop;

namespace ConsoleZapp
{
    public class Tui
    {
        private readonly Header Header;
        private readonly Body Body;
        private readonly int? Width;

        private int LastWidth = -1;
        private int LastHeight = -1;

        // Shrinks the screen buffer to eliminate native scrollback
        // - otherwise scrolling drags the fixed header along
        // - conhost also auto-snaps the view on write, causing jumps
        // - Body's own redraw already reconstructs anything worth keeping
        private static void RemoveScrollback()
        {
            if (Console.IsOutputRedirected)
                return;

            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }

        // Constructor with header, optional body and fixed width
        // - if width is omitted, Print() reads the live width
        public Tui(Header header, Body body = null, int? width = null)
        {
            Header = header;
            Body = body;
            Width = width;

            if (Body != null)
                Body.ResizeCheck = CheckResize;
        }

        // Prints the header, sets up the body's scroll area
        public void Print()
        {
            // console output still goes through Console.Out with the process's OutputEncoding - without forcing UTF-8 here, writing e.g. "€" back out falls back to '?' on most OEM codepages
            Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

            // on a raster (bitmap) console font (the common Windows 7 default), multi-byte UTF-8 box-drawing glyphs get decoded byte-by-byte as separate garbage glyphs instead of one character - degrade to ASCII borders instead of rendering garbage
            if (ConsoleFont.IsRasterFont())
                Header.UseAsciiBorders();

            RemoveScrollback();

            // row tracking assumes the header starts at absolute row 0 - clearing first guarantees that, regardless of what was on screen before this call
            Console.Clear();

            var width = Width ?? Console.WindowWidth;

            Header.Print(width);
            Body?.Init(Header.GetHeight());

            LastWidth = width;
            LastHeight = Console.WindowHeight;
        }

        // Re-prints on resize, redraws Body from its retained buffer
        // - unlike Print(), keeps scrollback instead of starting fresh
        // - called from every drawing method, no resize event exists
        private void CheckResize()
        {
            var width = Width ?? Console.WindowWidth;
            var height = Console.WindowHeight;

            if (width == LastWidth && height == LastHeight)
                return;

            LastWidth = width;
            LastHeight = height;

            RemoveScrollback();

            Console.Clear();
            Header.Print(width);
            Body?.Redraw(Header.GetHeight());
        }

        // Re-renders a single header control in place, defaults to "main" container
        public void UpdateControl(string name, string container_id = "main")
        {
            CheckResize();
            Header.UpdateControl(name, container_id);
        }

        // Overrides border chars for a header container, defaults to "main"
        public void SetBorderChars(
            char horizontal,
            char vertical,
            char top_left,
            char top_right,
            char bottom_left,
            char bottom_right,
            string container_id = "main")
        {
            Header.SetBorderChars(horizontal, vertical, top_left, top_right, bottom_left, bottom_right, container_id);
        }

        // Sets the border color of the given header container, defaults to "main"
        public void SetBorderColor(Cli.Conclr fg, Cli.Conclr bg, string container_id = "main")
        {
            Header.SetBorderColor(fg, bg, container_id);
        }

        // Writes a formatted line to the body's scrolling area, if a body is set
        public void WriteLine(string fmt, params object[] args)
        {
            CheckResize();
            Body?.WriteLine(fmt, args);
        }
        // In the given colors
        public void WriteLine(Cli.Conclr fg, Cli.Conclr bg, string fmt, params object[] args)
        {
            CheckResize();
            Body?.WriteLine(fg, bg, fmt, args);
        }
        // Built from independently colored parts
        public void WriteLine(IEnumerable<Part> parts)
        {
            CheckResize();
            Body?.WriteLine(parts);
        }

        // Prints the body prompt and reads a command, if a body is set
        public string ReadCommand()
        {
            CheckResize();
            return Body?.ReadCommand();
        }

        // Prints a dialog, reads a matching answer, if set
        // - see Body.ReadDialog
        public DialogOption? ReadDialog(Dialog dialog)
        {
            CheckResize();
            return Body?.ReadDialog(dialog);
        }

        // Sets the color the body's prompt is written in, if a body is set
        public void SetPromptColor(Cli.Conclr fg, Cli.Conclr bg)
        {
            Body?.SetPromptColor(fg, bg);
        }

        // Sets the body's scroll mode, if a body is set
        // - see ScrollMode for Manual vs. AutoScroll
        public void SetScrollMode(ScrollMode mode)
        {
            Body?.SetScrollMode(mode);
        }

        // Registers a keyword highlighted wherever typed, if a body is set
        public void AddKeywordColor(string keyword, Cli.Conclr fg, Cli.Conclr bg)
        {
            Body?.AddKeywordColor(keyword, fg, bg);
        }

        // Recolors the last input line, if a body is set
        // - see Body.RecolorLastInput for caveats
        public void RecolorLastInput(Cli.Conclr fg, Cli.Conclr bg)
        {
            CheckResize();
            Body?.RecolorLastInput(fg, bg);
        }
    }
}
