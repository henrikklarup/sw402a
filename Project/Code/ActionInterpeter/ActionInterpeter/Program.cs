using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    class Program
    {

        private static List<Token> Tokens = new List<Token>();

        static void Main(string[] args)
        {
            Parser parser = new Parser();
            AST ast = parser.parse();
            Console.WriteLine("LOL DONE");
        }
    }
}
