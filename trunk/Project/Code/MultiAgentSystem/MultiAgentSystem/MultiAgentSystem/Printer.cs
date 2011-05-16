using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASSIVE
{
    public static class Printer
    {
        private static int indent;          // Used to indent the messages sent by the compiler to the user.
        public static bool printCompleted;  // Turns compiler messages on/off.

        /// <summary>
        /// Clears the console and Prints the MASS logo.
        /// </summary>
        public static void printLogo()
        {
            ConsoleColor tmpColor = Console.ForegroundColor;

            // Draw the MASS icon in the console.
            Console.Clear();
            Console.SetCursorPosition(8, Console.CursorTop++);
            Console.WriteLine("Multi Agent System");
            Console.SetCursorPosition(8, Console.CursorTop++);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(@" _______ _______ _______ _______ _______ ___ ___ _______ 
        |   |   |   _   |     __|     __|_     _|   |   |    ___|
        |       |       |__     |__     |_|   |_|   |   |    ___|
        |__|_|__|___|___|_______|_______|_______|\_____/|_______|");
            Console.SetCursorPosition(8, Console.CursorTop++);
            Console.WriteLine("Compiler");
            Console.ForegroundColor = tmpColor;
        }
        
        /// <summary>
        /// Changes the cursor position, to match the indent, and changes the text color to red,
        /// to display an error message. 
        /// </summary>
        /// <param name="error">The message printed.</param>
        public static void Error(String error)
        {
            // Save the current text color.
            ConsoleColor tmpColor = Console.ForegroundColor;

            // Changes the text color to red.
            Console.ForegroundColor = ConsoleColor.Red;

            // Writes the error to the console.
            Console.Write(error);
            // Changes the text color to the original.
            Console.ForegroundColor = tmpColor;
        }

        /// <summary>
        /// Changes the cursor position, to match the indent, and changes the text color to yellow,
        /// to display a warning. 
        /// </summary>
        /// <param name="error">The message printed.</param>
        public static void Warning(String warning)
        {
            // Save the current text color.
            ConsoleColor tmpColor = Console.ForegroundColor;

            // Changes the text color to red.
            Console.ForegroundColor = ConsoleColor.Yellow;

            // Writes the error to the console.
            Console.Write(warning);
            // Changes the text color to the original.
            Console.ForegroundColor = tmpColor;
        }

        /// <summary>
        /// Extention of the Error method, changes the cursor to the next line. 
        /// </summary>
        /// <param name="text"></param>
        public static void ErrorLine(string text)
        {
            Console.WriteLine();
            // Changes the cursor position, to match the indent.
            Console.SetCursorPosition(indent * 2, Console.CursorTop);
            Error(text);
            Console.WriteLine("");
        }

        /// <summary>
        /// Extention of the Warning method, changes the cursor to the next line. 
        /// </summary>
        /// <param name="text"></param>
        public static void WarningLine(string text)
        {
            Console.WriteLine();
            // Changes the cursor position, to match the indent.
            Console.SetCursorPosition(indent * 2, Console.CursorTop);
            Warning(text);
            Console.WriteLine("");
        }

        /// <summary>
        /// Prints a mark in the console where an error has occurred.
        /// </summary>
        public static void ErrorMarker()
        {
            Error(" <!>");
        }

        /// <summary>
        /// Prints a mark in the console where a warning has occurred.
        /// </summary>
        public static void WarningMarker()
        {
            Warning(" <!>");
        }

        /// <summary>
        /// Prints two lines before and after the text.
        /// </summary>
        /// <param name="text">Any string</param>
        public static void CompilationMarker(string text)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine(text);
            Console.WriteLine("--------------------------------------------------");
        }
        
        /// <summary>
        /// Changes the cursor position, to match the indent,
        /// Changes the text color to green
        /// to display that the function went well.
        /// </summary>
        /// <param name="text">Any string</param>
        public static void Write(String text)
        {
            if (!printCompleted)
                return;
            // Saves the current text color.
            ConsoleColor tmpColor = Console.ForegroundColor;

            // Changes the text color to green.
            Console.ForegroundColor = ConsoleColor.Green;

            // Writes the message to the console.
            Console.Write(text);
            // Changes the text color to the original.
            Console.ForegroundColor = tmpColor;
        }

        /// <summary>
        /// Extention of the Write method.
        /// </summary>
        /// <param name="text">Any string</param> 
        public static void WriteLine(string text)
        {
            if (!printCompleted)
                return;
            Console.WriteLine();
            // Changes the cursor position, to match the indent.
            Console.SetCursorPosition(indent * 2, Console.CursorTop);
            Write(text);
        }

        /// <summary>
        /// Expand the indention of the messages.
        /// </summary>
        public static void Expand()
        {
            indent++;
        }

        /// <summary>
        /// Collaps the indention of the messages.
        /// </summary>
        public static void Collapse()
        {
            indent--;
        }

        /* INDENT EXAMPLE:
         * The source code:
         * main()
         * 
         * {
         *      num i = 0;
         * }
         * 
         * Will be shown as:
         * 
         *  main
         *    block
         *      Type Declaration
         *        number: i
         *        becomes: 0
         * */
    }
}
