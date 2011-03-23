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

        static void Main(string[] args)
        {
            Scanzor scanzor = new Scanzor();
            scanzor.fileLines = File.ReadAllLines(@"C:/Users/Rasmus/Desktop/test.mass"); //The name of the files input

            //Initializes the string being read by the scanner, and its counters
            scanzor.charLine = scanzor.fileLines[scanzor.fileCounter++].ToCharArray();
            scanzor.currentChar = scanzor.charLine[scanzor.charCounter++];

            Token newToken;
            while (true)
            {
                newToken = scanzor.scan();

                if (newToken.kind == Token.ERROR)
                {
                    Console.ReadKey();
                    return;
                }
                Tokens.Add(newToken);

                //If the token just found is the End Of Transmission token then break
                if (newToken.kind == Token.EOT)
                {
                    break;
                }

            }

            //Printing the token list, for debuggin purpose only
            foreach (Token t in Tokens)
            {
                Console.WriteLine(string.Format("{0,13} - {1,-20}", Token.spellings[t.kind], t.spelling));
            }

            Console.ReadKey();
        }
    }
}
