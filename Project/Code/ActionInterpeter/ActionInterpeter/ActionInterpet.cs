using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace ActionInterpeter
{
    public class ActionInterpet
    {
        private static List<Token> Tokens = new List<Token>();
        private static AST newAst;
        public static string input;
        public static StringBuilder output;
        private static Visitor visitor;

        public static void Main()
        {
            while (true)
            {
                input = Console.ReadLine();
                if (input.Count() == 0)
                    continue;
                Parse();
                Console.WriteLine("Success");
                Console.ReadKey();
            }
        }

        public static string Compile()
        {
            output = new StringBuilder("");
            try
            {
                Parse();
            }
            catch (InvalidMoveOptionException e)
            {
                foreach (string s in e.PrintExceptions())
                {
                    Printer.WriteLine(s);
                }
            }
            catch (WrongTeamException e)
            {
                foreach (string s in e.PrintExceptions())
                {
                    Printer.WriteLine(s);
                }
            }
            return output.ToString();
        }

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
                return;
            }
            Decorate();
        }

        private static void Decorate()
        {
            visitor = new Visitor();
            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                return;
            }
        }
    }
}
