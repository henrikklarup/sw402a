using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CalcLang
{
    class Program
    {
        private static List<Token> Tokens = new List<Token>();

        static void Main(string[] args)
        {
            Scanzor scanzor = new Scanzor();
            scanzor.fileLines = File.ReadAllLines(@"C:/Users/Rasmus/Desktop/test.txt");

            /*foreach (string s in scanzor.fileLines)
            {
                Console.WriteLine(s);
            }*/
            scanzor.fileCounter = 0;
            scanzor.charLine = scanzor.fileLines[scanzor.fileCounter++].ToCharArray();
            scanzor.charCounter = 0;
            scanzor.currentChar = scanzor.charLine[scanzor.charCounter++];

            Token newToken;
            while(true)
            {
                newToken = scanzor.scan();
                if (newToken.kind != Token.EOT)
                {
                    Tokens.Add(newToken);
                }
                else
                {
                    break;
                }

            }

            foreach (Token t in Tokens)
            {
                Console.WriteLine(t.kind + " -> " + t.spelling);
            }

            Console.ReadLine();
        }
    }
}
