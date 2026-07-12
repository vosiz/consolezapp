namespace ConsoleZapp
{
    public class Tui
    {
        private readonly Header Header;
        private readonly int Width;

        // Constructor with header and optional screen width
        public Tui(Header header, int width = 80)
        {
            Header = header;
            Width = width;
        }

        // Prints the header to the console
        public void Print()
        {
            Header.Print(Width);
        }
    }
}
