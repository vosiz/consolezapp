using System;

namespace ConsoleZapp
{
    public class PrintException : Exception
    {
        // Constructor with message and inner exception
        public PrintException(string message, Exception inner) : base(message, inner) { }
    }
}
