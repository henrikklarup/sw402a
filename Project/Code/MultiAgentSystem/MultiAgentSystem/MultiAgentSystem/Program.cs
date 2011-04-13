using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace MultiAgentSystem
{
    class Program
    {
        private static List<Token> Tokens = new List<Token>();
        private static AST newAst;

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Printer.printLogo();
            Console.WriteLine("Compile");

            Console.ReadKey();

            Scanzor scanzor = new Scanzor();
            Token newToken;

            Printer.printLogo();

            while (true)
            {
                newToken = scanzor.scan();

                if (newToken.kind == (int)Token.keywords.ERROR)
                {
                    Console.ReadKey();
                }
                Tokens.Add(newToken);

                //If the token just found is the End Of Transmission token then break
                if (newToken.kind == (int)Token.keywords.EOT)
                {
                    break;
                }
            }

            Console.WriteLine("@Scanning");
            Console.Title = Console.Title + "Scanning";
            Console.WriteLine();

            //Printing the token list
            Console.WriteLine(string.Format("{0,10} - {1,-30}  {2, 4}", "Kind", "Spelling", "Coords"));
            Console.WriteLine();

            foreach (Token t in Tokens)
            {
                Console.WriteLine(string.Format("{0,10} - {1,-30}  {2, 4},{3,-4}", Enum.GetName(typeof(Token.keywords), t.kind), t.spelling, t.row, t.col));
            }

            Console.ReadKey();
            Printer.printLogo();

            Console.WriteLine("@Parsing");
            Console.Title = Console.Title + "Parsing";
            Console.WriteLine();
            
            Parser parser = new Parser(Tokens);
            newAst = parser.parse();

            Console.ReadKey();
            Printer.printLogo();

            Console.WriteLine("@Decorating");
            Console.Title = Console.Title + "Decorating";
            Console.WriteLine();

            Visitor visitor = new Visitor();
            visitor.visitAST(newAst, null);

            Console.ReadKey();
        }
    }

    public class ImBusy
    {
        public string task;
        public void waitForUser()
        {
            byte change = 0;

            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            int lenght = task.Length+1;

            while (true)
            {
                Console.SetCursorPosition(lenght, 6);

                Console.CursorVisible = false;
                Thread.Sleep(500);
                switch (change)
                {
                    case 0:
                        Console.WriteLine(".  ");
                        change++;
                        break;
                    case 1:
                        Console.WriteLine(".. ");
                        change++;
                        break;
                    case 2:
                        Console.WriteLine("...");
                        change = 0;
                        break;
                }
            }
        }
    }

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

        public static void ErrorLine(String error)
        {
            Error(error);
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

        public static void WriteLine(String text)
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
