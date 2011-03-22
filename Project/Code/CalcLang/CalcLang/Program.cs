﻿using System;
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
            scanzor.fileLines = File.ReadAllLines(@"C:/Users/Rasmus/Desktop/test.txt"); //The name of the file with the input

            //Initializes the string being read by the scanner, and its counters
            scanzor.fileCounter = 0;
            scanzor.charLine = scanzor.fileLines[scanzor.fileCounter++].ToCharArray();
            scanzor.charCounter = 0;
            scanzor.currentChar = scanzor.charLine[scanzor.charCounter++];

            Token newToken;
            while(true)
            {
                newToken = scanzor.scan();
                //If the token just found is not the End Of Transmission token then add it, else break
                if (newToken.kind != Token.EOT)
                {
                    Tokens.Add(newToken);
                }
                else
                {
                    break;
                }

            }

            //Printing the token list, for debuggin purpose only
            foreach (Token t in Tokens)
            {
                Console.WriteLine(t.kind + " -> " + t.spelling);
            }

            Console.ReadLine();
        }
    }
}
