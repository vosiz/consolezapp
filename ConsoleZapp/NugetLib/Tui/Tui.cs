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

        // Prints the header to the console
        public void Print()
        {
            Header.Print(Width);
        }

        // Prints the body prompt and reads a command, if a body is set
        public string ReadCommand()
        {
            return Body?.ReadCommand();
        }
    }
}
