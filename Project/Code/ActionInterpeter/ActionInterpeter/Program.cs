using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    class Program
    {

        private static List<Token> Tokens = new List<Token>();
        private static AST newAst;
        public static string input;

        static void Main(string[] args)
        {
            while (true)
            {
                input = Console.ReadLine();

                Parse();

                Console.ReadKey();
            }
        }

        private static void Scan()
        { }

        private static void Parse()
        {
            Parser parser = new Parser();

            try
            {
                newAst = parser.parse();
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Console.Write("Errors were found while PARSING.");
                return;
            }
            Decorate();
        }

        private static void Decorate()
        {
            Visitor visitor = new Visitor();
            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Console.Write("Errors were found while DECORATING.");
                return;
            }
        }
    }
}
