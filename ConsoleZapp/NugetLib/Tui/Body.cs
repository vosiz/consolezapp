using System;

namespace ConsoleZapp
{
    public class Body
    {
        private string Prompt = "> ";

        // Constructor
        public Body()
        {
        }

        // Sets the prompt shown before reading a command
        public void SetPrompt(string prompt)
        {
            Prompt = prompt;
        }

        // Prints the prompt and reads a command line from the console
        public string ReadCommand()
        {
            Console.Write(Prompt);
            return Console.ReadLine();
        }
    }
}
