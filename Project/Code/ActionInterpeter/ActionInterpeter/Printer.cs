using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    public static class Printer
    {
        // Used to indent the messages sent by the compiler to the user.
        private static int indent;
        public static bool printCompleted;

        public static void printLogo()
        {
            // Used to place the cursor in the Console.
            int col = 10;
            int row = 0;
            ConsoleColor tmpColor = Console.ForegroundColor;

            // Draw the MASS icon in the console.
            Console.Clear();
            Console.Title = "MASS Compiler: ";
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("Multi Agent System");
            Console.SetCursorPosition(col, row++);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(" _______ _______ _______ _______");
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("|   |   |   _   |     __|     __|");
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("|       |       |__     |__     |");
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("|__|_|__|___|___|_______|_______|");
            Console.SetCursorPosition(col + 25, row++);
            Console.WriteLine("Compiler");
            Console.ForegroundColor = tmpColor;
        }

        // Changes the cursor position, to match the indent, and changes the text color to red,
        // to display the error message.
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

        // Extention of the Error method, changes the cursor to the next line. 
        public static void ErrorLine(string text)
        {
            Console.WriteLine();
            // Changes the cursor position, to match the indent.
            Console.SetCursorPosition(indent * 2, Console.CursorTop);
            Error(text);
        }

        /// <summary>
        /// Prints a mark in the console where an error has occurred.
        /// </summary>
        public static void ErrorMarker()
        {
            Error(" <!>");
        }

        public static void CompilationMarker(string text)
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine(text);
            Console.WriteLine("--------------------------------------------------");
        }

        // Changes the cursor position, to match the indent,
        // Changes the text color to green
        // to display that the function went well.
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

        // Extention of the Write method.
        public static void WriteLine(string text)
        {
            if (!printCompleted)
                return;
            Console.WriteLine();
            // Changes the cursor position, to match the indent.
            Console.SetCursorPosition(indent * 2, Console.CursorTop);
            Write(text);
        }

        // Expand the indention of the messages.
        public static void Expand()
        {
            indent++;
        }

        // Collaps the indention of the messages.
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
