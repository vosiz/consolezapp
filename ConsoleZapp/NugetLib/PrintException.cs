using System;

namespace ConsoleZapp
{
    public class PrintException : Exception
    {
        public PrintException(string message, Exception inner) : base(message, inner) { }
    }
}
