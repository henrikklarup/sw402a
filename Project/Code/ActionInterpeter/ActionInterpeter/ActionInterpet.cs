using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                try
                {
                    Parse();
                }
                catch (Exception e)
                {
                    Printer.WriteLine(e.Message);
                    return;
                }
                Console.WriteLine("Success");
                Console.ReadKey();
            }
        }

        public static string Compile()
        {
            output = new StringBuilder("");
            Parse();
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

        private static void CodeGen()
        {
            try
            {
                visitor.visitAST(newAst, 1);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                return;
            }
        }
    }
}
