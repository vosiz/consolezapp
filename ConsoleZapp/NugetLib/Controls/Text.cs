using System;

namespace ConsoleZapp
{
    public class Text : IControl
    {
        private string Content;

        // Constructor
        public Text() { }

        // Sets text using format string and arguments
        public void SetText(string fmt, params object[] args)
        {
            Content = string.Format(fmt, args);
        }

        // Renders control content
        public string Render()
        {
            return Content;
        }

        // Writes the rendered content to the console
        public void Print()
        {
            Console.Write(Render());
        }
    }
}
