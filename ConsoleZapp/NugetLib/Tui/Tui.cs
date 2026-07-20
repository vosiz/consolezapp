using System;

namespace ConsoleZapp
{
    public class Tui
    {
        private readonly Header Header;
        private readonly Body Body;
        private readonly int Width;

        // Constructor with header, optional body and screen width
        public Tui(Header header, Body body = null, int width = 80)
        {
            Header = header;
            Body = body;
            Width = width;
        }

        // Prints the header to the console and sets up the body's scrolling area below it
        public void Print()
        {
            // Header/Body row tracking assumes the header starts at absolute row 0 - clearing
            // first guarantees that, regardless of whatever was on screen before this call.
            Console.Clear();

            Header.Print(Width);
            Body?.Init(Header.GetHeight());
        }

        // Re-renders a single header control in place, defaults to "main" container
        public void UpdateControl(string name, string container_id = "main")
        {
            Header.UpdateControl(name, container_id);
        }

        // Writes a formatted line to the body's scrolling area, if a body is set
        public void WriteLine(string fmt, params object[] args)
        {
            Body?.WriteLine(fmt, args);
        }

        // Writes a formatted line in the given colors to the body's scrolling area, if a body is set
        public void WriteLine(Cli.Conclr fg, Cli.Conclr bg, string fmt, params object[] args)
        {
            Body?.WriteLine(fg, bg, fmt, args);
        }

        // Prints the body prompt and reads a command, if a body is set
        public string ReadCommand()
        {
            return Body?.ReadCommand();
        }
    }
}
