using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    public static class Printer
    {
        private static int indent;

        public static void printLogo()
        {
            // Used to place the cursor in the Console.
            int col = 10;
            int row = 0;
            ConsoleColor tmpColor = Console.ForegroundColor;

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

        public static void ErrorLine(string text)
        {
            Error(text);
            Console.WriteLine();
        }

        public static void Error(String error)
        {
            ConsoleColor tmpColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(indent * 2, Console.CursorTop);
            Console.Write(error);
            Console.ForegroundColor = tmpColor;
        }

        public static void WriteLine(string text)
        {
            Write(text);
            Console.WriteLine();
        }

        public static void Write(String text)
        {
            ConsoleColor tmpColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(indent * 2, Console.CursorTop);
            Console.Write(text);
            Console.ForegroundColor = tmpColor;
        }

        public static void Expand()
        {
            indent++;
        }

        public static void Collapse()
        {
            indent--;
        }
    }
}
