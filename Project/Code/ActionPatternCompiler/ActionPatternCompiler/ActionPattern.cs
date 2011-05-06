using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace ActionPatternCompiler
{
    public class ActionPattern
    {
        private static List<Token> Tokens = new List<Token>();
        private static AST newAst;
        public static string input;
        public static agent thisAgent;
        public static StringBuilder output;
        private static Visitor visitor;

        public static void Main(String[] args)
        {
            XML.initLists();
            XML.returnLists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            output = new StringBuilder("");
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

        public static string Compile(string _input, agent _agent)
        {
            input = _input;
            thisAgent = _agent;
            output = new StringBuilder("");
            Parse();
            return output.ToString();
        }

        private static void Parse()
        {
            Parser parser = new Parser();

            newAst = parser.parse();
            Decorate();
            return;
        }

        private static void Decorate()
        {
            visitor = new Visitor();
            visitor.visitAST(newAst, null);
            return;
        }
    }
}
