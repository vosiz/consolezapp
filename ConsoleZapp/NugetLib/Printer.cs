using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleZapp
{
    public partial class Printer
    {
        // Legacy - write
        public void Write(string fmt = null, params object[] args)
        {
            Console.Write(string.Format(fmt, args));
        }

        // Legacy - write nl
        public void WriteLine(string fmt = null, params object[] args)
        {
            Console.WriteLine(string.Format(fmt, args));
        }

        // Format string
        public void Sprintf(string fmt, params object[] args)
        {
            Write(string.Format(fmt, args));
        }

        // Format string nl
        public void Sprintfln(string fmt, params object[] args)
        {
            fmt += Environment.NewLine;
            Sprintf(fmt, args);
        }


        // Debug
        public void Debug(string fmt, params object[] args) {

            fmt = "[Debug]: " + fmt;
            Clrprintfln("debug", fmt, args);
        }

        // Info
        public void Info(string fmt, params object[] args) {

            fmt = "[Info]: " + fmt;
            Clrprintfln("info", fmt, args);
        }

        // Warning
        public void Warning(string fmt, params object[] args) {

            fmt = "[Warning]: " + fmt;
            Clrprintfln("warning", fmt, args);
        }

        // Error
        public void Error(string fmt, params object[] args) {

            fmt = "[Error]: " + fmt;
            Clrprintfln("error", fmt, args);
        }

        // Exception
        public void Exception(string fmt, params object[] args) {

            fmt = "[Exc]: " + fmt;
            Clrprintfln("exception", fmt, args);
        }

        // Exception
        public void Exception(Exception exc) {

            Exception(exc.Message);
        }

        // Success
        public void Success(string fmt, params object[] args) {

            Clrprintfln("success", fmt, args);
        }

        // Failure
        public void Fail(string fmt, params object[] args) {

            Clrprintfln("fail", fmt, args);
        }

        // Custom
        public void Custom(string key, string fmt, params object[] args) {

            Clrprintfln(key, fmt, args);
        }

    }
}
