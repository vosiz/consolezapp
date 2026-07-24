using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleZapp
{
    public class Tui
    {
        private readonly Header Header;
        private readonly Body Body;
        private readonly int? Width;

        private int LastWidth = -1;
        private int LastHeight = -1;

        // Constructor with header, optional body and an optional fixed-width override; if width is omitted, Print() reads the console's live width instead of assuming a fixed one
        public Tui(Header header, Body body = null, int? width = null)
        {
            Header = header;
            Body = body;
            Width = width;

            if (Body != null)
                Body.ResizeCheck = CheckResize;
        }

        // Prints the header to the console and sets up the body's scrolling area below it
        public void Print()
        {
            // console output still goes through Console.Out with the process's OutputEncoding - without forcing UTF-8 here, writing e.g. "€" back out falls back to '?' on most OEM codepages
            Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

            // row tracking assumes the header starts at absolute row 0 - clearing first guarantees that, regardless of what was on screen before this call
            Console.Clear();

            var width = Width ?? Console.WindowWidth;

            Header.Print(width);
            Body?.Init(Header.GetHeight());

            LastWidth = width;
            LastHeight = Console.WindowHeight;
        }

        // Re-prints the header and redraws the body from its retained buffer if the console has been resized since the last draw, keeping scrollback history (unlike Print(), which starts fresh).
        // Called from every drawing method, since there's no resize event to hook on this console host.
        private void CheckResize()
        {
            var width = Width ?? Console.WindowWidth;
            var height = Console.WindowHeight;

            if (width == LastWidth && height == LastHeight)
                return;

            LastWidth = width;
            LastHeight = height;

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

        // Overrides the border characters of the given header container, defaults to "main"
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

        // Writes a formatted line in the given colors to the body's scrolling area, if a body is set
        public void WriteLine(Cli.Conclr fg, Cli.Conclr bg, string fmt, params object[] args)
        {
            CheckResize();
            Body?.WriteLine(fg, bg, fmt, args);
        }

        // Writes a line built from independently colored parts to the body's scrolling area, if a body is set
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

        // Sets the color the body's prompt is written in, if a body is set
        public void SetPromptColor(Cli.Conclr fg, Cli.Conclr bg)
        {
            Body?.SetPromptColor(fg, bg);
        }

        // Registers an exact keyword that gets highlighted wherever it occurs in typed input, if a body is set
        public void AddKeywordColor(string keyword, Cli.Conclr fg, Cli.Conclr bg)
        {
            Body?.AddKeywordColor(keyword, fg, bg);
        }

        // Recolors the whole last input line in place, if a body is set - see Body.RecolorLastInput for caveats
        public void RecolorLastInput(Cli.Conclr fg, Cli.Conclr bg)
        {
            CheckResize();
            Body?.RecolorLastInput(fg, bg);
        }
    }
}
