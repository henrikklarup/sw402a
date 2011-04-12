using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MultiAgentSystem
{
    class Program
    {
        private static List<Token> Tokens = new List<Token>();
        private static AST newAst;

        static void Main(string[] args)
        {
            Scanzor scanzor = new Scanzor();
            Token newToken;
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

            //Printing the token list, for debuggin purpose only

            Console.WriteLine(string.Format("{0,13} - {1,-30}  {2, 4}", "Kind", "Spelling", "Coords"));
            Console.WriteLine();

            foreach (Token t in Tokens)
            {
                Console.WriteLine(string.Format("{0,13} - {1,-30}  {2, 4},{3,-4}", Enum.GetName(typeof(Token.keywords), t.kind) , t.spelling, t.row, t.col));
            }

            Console.ReadKey();

            Console.WriteLine();
            Console.WriteLine("Parse");
            Parser parser = new Parser(Tokens);
            newAst = parser.parse();

            Console.ReadKey();

            Console.WriteLine();
            Console.WriteLine("Decorate AST");
            Visitor visitor = new Visitor();
            visitor.visitAST(newAst, null);

            Console.ReadKey();
        }
    }
}
