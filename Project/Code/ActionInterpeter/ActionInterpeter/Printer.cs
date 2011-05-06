using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    static class Printer
    {

        /// <summary>
        /// Appends an error to the output, does the same as write.
        /// </summary>
        /// <param name="error">The error message.</param>
        public static void Error(string error)
        {
            // Writes the error to the console.
            ActionInterpet.output.Append(error);
        }

        /// <summary>
        /// Extention of the Error method, with a newline.
        /// </summary>
        /// <param name="text">The error message.</param>
        public static void ErrorLine(string text)
        {
            ActionInterpet.output.AppendLine();
            Error(text);
        }

        /// <summary>
        /// Prints a mark in the console where an error has occurred.
        /// </summary>
        public static void ErrorMarker()
        {
            Error(" <!>");
        }

        /// <summary>
        /// Appends the text to the output string.
        /// </summary>
        /// <param name="text">Text to be printed.</param>
        public static void Write(String text)
        {
            // Writes the message to the console.
            ActionInterpet.output.Append(text);
        }

        /// <summary>
        /// Extention of the Write method.
        /// </summary>
        /// <param name="text">Text to be printed.</param>
        public static void WriteLine(string text)
        {
            ActionInterpet.output.AppendLine();
            Write(text);
        }

    }
}
