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

        // Exception for errors found in the scanner.
        private static GrammarException scanException = 
            new GrammarException("These errors were found by the scanner:");
        private static bool scanningError = false;

        static void Main(string[] args)
        {
            StartUp startUp = new StartUp();
            Thread thread = new Thread(new ThreadStart(startUp.first));
            thread.Start();
            Console.ReadKey();
            thread.Abort();
             
            Console.ForegroundColor = ConsoleColor.White;

            Printer.printLogo();
            Console.WriteLine("Compile");

            Console.ReadKey();
            Scanzor scanzor = new Scanzor();
            Token newToken = null;

            Printer.printLogo();

            Console.WriteLine("@Scanning");
            Console.Title = Console.Title + "Scanning";
            Console.WriteLine();

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
                if (newToken.kind == (int)Token.keywords.EOT)
                {
                    break;
                }
            }

            //Printing the token list
            Console.WriteLine(string.Format("{0,10} - {1,-30}  {2, 4}", "Kind", "Spelling", "Coords"));
            Console.WriteLine();

            foreach (Token t in Tokens)
            {
                Console.WriteLine(string.Format("{0,10} - {1,-30}  {2, 4},{3,-4}", 
                    Enum.GetName(typeof(Token.keywords), t.kind), t.spelling, t.row, t.col));
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

            Console.ReadKey();
            Printer.printLogo();

            Console.WriteLine("@Parsing");
            Console.Title = Console.Title + "Parsing";
            Console.WriteLine();
            
            Parser parser = new Parser(Tokens);

            try
            {
                newAst = parser.parse();
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
            }

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
}
