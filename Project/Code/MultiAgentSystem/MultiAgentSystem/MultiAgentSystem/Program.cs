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
                Console.Write("Errors were found while SCANNING.");
                Recompile();
            }

            Parse();
        }

        private static void Parse()
        {
            
            Console.CursorLeft = 0; 
            Console.WriteLine("--------------------------------------------------");
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
                Console.Write("Errors were found while PARSING.");
                Recompile();
                return;
            }

            Decorate();
        }

        private static void Decorate()
        {
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
                Console.Write("Errors were found while DECORATING.");
                Recompile();
                return;
            }
            CodeGen();
        }

        private static void CodeGen()
        {
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
                Console.Write("Errors were found while GENERATING CODE.");
                Recompile();
                return;
            }
            Completed();
        }

        private static void Completed()
        {
            Console.CursorLeft = 0;
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("@Compilation has completed");
            Console.WriteLine("--------------------------------------------------");
            Console.Title = "MASS Compiler: Compilation has completed";

            Recompile();
        }

        private static void Recompile()
        {
            ConsoleKeyInfo cki;

            while (true)
            {
                Console.WriteLine(" Would you like to compile again? y/n");
                cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Y)
                {
                    Compile();
                    break;
                }
                if (cki.Key == ConsoleKey.N)
                {
                    Console.WriteLine("Goodbye.");
                    break;
                }

                Console.WriteLine(" is not an option.");
            }
        }
    }
}