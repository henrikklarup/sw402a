﻿using System;
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

            imBusyThreads.task = "Scanning";
            Thread thread = new Thread(new ThreadStart(imBusyThreads.waitForUser));
            thread.Start();

            Console.ReadKey();
            thread.Abort();
            printLogo();

            Console.WriteLine("@Parsing");
            Console.WriteLine();
            Console.Title = Console.Title + "Parsing";

            Parser parser = new Parser(Tokens);
            newAst = parser.parse();

            imBusyThreads.task = "Parsing";
            thread = new Thread(new ThreadStart(imBusyThreads.waitForUser));
            thread.Start();

            Console.ReadKey();
            thread.Abort();
            printLogo();

            Console.WriteLine("@Decorating");
            Console.Title = Console.Title + "Decorating";
            Console.WriteLine();

            Visitor visitor = new Visitor();
            visitor.visitAST(newAst, null);

            imBusyThreads.task = "Decorating";
            thread = new Thread(new ThreadStart(imBusyThreads.waitForUser));
            thread.Start();

            Console.ReadKey();
            thread.Abort();
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
            Console.WriteLine(" _______ _______ _______ _______");
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("|   |   |   _   |     __|     __|");
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("|       |       |__     |__     |");
            Console.SetCursorPosition(col, row++);
            Console.WriteLine("|__|_|__|___|___|_______|_______|");
            Console.SetCursorPosition(col + 25, row++);
            Console.WriteLine("Compiler");
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
            while (true)
            {
                Console.SetCursorPosition(0, 6);

                Console.CursorVisible = false;
                Thread.Sleep(500);
                switch (change)
                {
                    case 0:
                        Console.WriteLine("@" + task + ".  ");
                        change++;
                        break;
                    case 1:
                        Console.WriteLine("@" + task + ".. ");
                        change++;
                        break;
                    case 2:
                        Console.WriteLine("@" + task + "...");
                        change = 0;
                        break;
                }
            }
        }
    }
}
