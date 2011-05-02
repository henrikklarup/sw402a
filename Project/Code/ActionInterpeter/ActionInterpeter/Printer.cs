using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    static class Printer
    {

        // Changes the cursor position, to match the indent, and changes the text color to red,
        // to display the error message.
        public static void Error(String error)
        {
            // Writes the error to the console.
            ActionInterpet.output.Append(error);
        }

        // Extention of the Error method, changes the cursor to the next line. 
        public static void ErrorLine(string text)
        {
            ActionInterpet.output.Append("\n");
            Error(text);
        }

        /// <summary>
        /// Prints a mark in the console where an error has occurred.
        /// </summary>
        public static void ErrorMarker()
        {
            Error(" <!>");
        }

        // Changes the cursor position, to match the indent,
        // Changes the text color to green
        // to display that the function went well.
        public static void Write(String text)
        {
            // Writes the message to the console.
            ActionInterpet.output.Append(text);
        }

        // Extention of the Write method.
        public static void WriteLine(string text)
        {
            ActionInterpet.output.Append("\n");
            Write(text);
        }

    }
}
