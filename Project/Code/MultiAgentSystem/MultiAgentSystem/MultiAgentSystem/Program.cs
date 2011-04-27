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
            /*StartUp startUp = new StartUp();
            Thread thread = new Thread(new ThreadStart(startUp.first));
            thread.Start();
            Console.ReadKey();
            thread.Abort();*/
             
            Console.ForegroundColor = ConsoleColor.White;

            Compile();
        }

        private static void Compile()
        {
            Tokens.Clear();

            Printer.printLogo();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Compile");
            Console.WriteLine("--------------------------------------------------");
            Console.ReadKey();
            Scan();
        }

        private static void Scan()
        {
            // Exception for errors found in the scanner.
            GrammarException scanException = new GrammarException(
                "These errors were found by the scanner:");
            bool scanningError = false;

            Scanzor scanzor = new Scanzor();
            Token newToken = null;

            Console.CursorTop = Console.CursorTop + 2;
            Console.CursorLeft = 0;
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("@Scanning");
            Console.WriteLine("--------------------------------------------------");
            Console.Title = "MASS Compiler: Scanning";

            while (true)
            {
                try
                {
                    newToken = scanzor.scan();
                }
                catch (GrammarException g)
                {
                    scanningError = true;
                    scanException.containedExceptions.Add(g);
                }

                Tokens.Add(newToken);

                //If the token just found is the End Of Transmission token then break
                if (newToken == null || newToken.kind == (int)Token.keywords.EOT)
                {
                    break;
                }
            }
            try
            {
                if (scanningError)
                {
                    throw scanException;
                }
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
            }
            if (scanningError)
            {
                Console.WriteLine("Errors were found while SCANNING, would you like to compile again?");
                Console.ReadKey();
                
                Compile();
            }

            Parse();
        }

        private static void Parse()
        {
            Console.CursorTop = Console.CursorTop + 2;
            Console.CursorLeft = 0; Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("@Parsing");
            Console.WriteLine("--------------------------------------------------");
            Console.Title = "MASS Compiler: Parsing";

            Parser parser = new Parser(Tokens);

            try
            {
                newAst = parser.parse();
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Console.WriteLine("Errors were found while PARSING, would you like to compile again?");
                Console.ReadKey();
                Compile();
            }

            Decorate();
        }

        private static void Decorate()
        {
            Console.CursorTop = Console.CursorTop + 2;
            Console.CursorLeft = 0;
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("@Decorating");
            Console.WriteLine("--------------------------------------------------");
            Console.Title = "MASS Compiler: Decorating";

            Visitor visitor = new Visitor();
            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Console.WriteLine("Errors were found while DECORATING, would you like to compile again?");
                Console.ReadKey();
                Compile();
            }
            CodeGen();
        }

        private static void CodeGen()
        {
            Console.CursorTop = Console.CursorTop + 2;
            Console.CursorLeft = 0; 
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("@Code Generation");
            Console.WriteLine("--------------------------------------------------");
            Console.Title = "MASS Compiler: Code Generation";

            try
            {
                Console.WriteLine("Not yet implemented!");
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Console.WriteLine("Errors were found while GENERATING CODE, would you like to compile again?");
                Console.ReadKey();
                Compile();
            }
            Completed();
        }

        private static void Completed()
        {
            ConsoleKeyInfo cki;

            Console.CursorTop = Console.CursorTop + 2;
            Console.CursorLeft = 0;
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("@Compilation has completed");
            Console.WriteLine("--------------------------------------------------");
            Console.Title = "MASS Compiler: Compilation has completed";

            while (true)
            {
                Console.WriteLine("Would you like to compile again? y/n");
                cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Y)
                {
                    Compile();
                    break;
                }
                if (cki.Key == ConsoleKey.N)
                {
                    Console.WriteLine("Goodbye");
                    break;
                }

                Console.WriteLine(" is not an option.");
            }
        }
    }
}
