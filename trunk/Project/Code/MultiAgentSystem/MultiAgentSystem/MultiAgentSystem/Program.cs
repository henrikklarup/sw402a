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
            
            ImBusy imBusyThreads = new ImBusy();
            Thread thread;

            printLogo();
            Console.WriteLine("Compile");

            Console.ReadKey();

            Scanzor scanzor = new Scanzor();
            Token newToken;

            printLogo();

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
            printLogo();

            Console.WriteLine("@Parsing");
            Console.Title = Console.Title + "Parsing";
            Console.WriteLine();
            
            Parser parser = new Parser(Tokens);
            newAst = parser.parse();

            Console.ReadKey();
            printLogo();

            Console.WriteLine("@Decorating");
            Console.Title = Console.Title + "Decorating";
            Console.WriteLine();

            Visitor visitor = new Visitor();
            visitor.visitAST(newAst, null);

            Console.ReadKey();
        }

        private static void printLogo()
        {
            // Used to place the cursor in the Console.
            int col = 10;
            int row = 0;

            Console.Clear();
            Console.Title = "MASS Compiler: ";
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("Multi Agent System");
            Console.SetCursorPosition(col, row++);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" _______ _______ _______ _______");
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("|   |   |   _   |     __|     __|");
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("|       |       |__     |__     |");
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("|__|_|__|___|___|_______|_______|");
            Console.SetCursorPosition(col + 25, row++);
            Console.WriteLine("Compiler");
            Console.ForegroundColor = ConsoleColor.Gray;
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
}
