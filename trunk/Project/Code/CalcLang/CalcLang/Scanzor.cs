using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CalcLang
{
    class Scanzor
    {
        //The input file we want to read
        public string[] fileLines;

        //Counts which line we're currently looking at
        public int fileCounter;

        //Holds the current line as an array of chars
        public char[] charLine;

        //Counts the current char we're at
        public int charCounter;

        //The current Char being processed by the scanner.
        public char currentChar;

        //The kind of Token we expect the current char/string to have.
        private byte currentKind;

        //Builds the string
        private StringBuilder currentSpelling;

        //Used to accept characters that match the exact char we want to accept
        //Not used atm
        private void take(char expectedChar)
        {
            if (currentChar == expectedChar)
            {
                currentSpelling.Append(currentChar);
                currentChar = nextSourceChar();
            }
            else
            {
                Console.WriteLine("Expected Char didnt match");
            }
        }

        //Used to take the current Character no matter which one it is and put it in the string
        private void takeIt()
        {
            currentSpelling.Append(currentChar);
            currentChar = nextSourceChar();
        }

        //Used to ignore the current Character and get the next char from the source file
        private void ignoreIt()
        {
            currentChar = nextSourceChar();
        }

        //Used to check if the char is a digit (0-9)
        private bool isDigit(char c)
        {
            switch (c)
            {
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '0':
                    return true;
            }
            return false;
        }

        //Checks if the char is a letter (a-z)
        private bool isLetter(char c)
        {
            switch (char.ToLower(c))
            {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    return true;
            }
            return false;
        }

        //Checks if
        private bool isMultiLineComment(char c)
        {
            if (c == '*')
            {
                ignoreIt();
                if (currentChar == '/')
                {
                    ignoreIt();
                    return false;
                }
            }
            return true;
        }

        private bool isOneLineComment(char c)
        {
            if (c == '\n')
            {
                ignoreIt();
                return false;
            }
            return true;
        }

        private void scanOperator()
        {
            if (currentChar == '=')
            {
                takeIt();
                if (currentChar == '=')
                {
                    takeIt();
                }
            }
        }

        //Ignores the current Character if its a blank space or a newline
        //Ignores everything between /* and */ with the loop using the isMultiLineCommen method
        private void scanSeperator()
        {
            switch (currentChar)
            {
                case ' ':
                case '\n':
                    ignoreIt();
                    break;
                case '/':
                    ignoreIt();
                    if (currentChar == '/')
                        while (isOneLineComment(currentChar))
                        {
                            ignoreIt();
                        };
                    if (currentChar == '*')
                        while (isMultiLineComment(currentChar))
                        {
                            if (currentChar == '\n')
                            {
                                if (fileCounter < fileLines.Length)
                                {
                                    charLine = fileLines[fileCounter++].ToCharArray();
                                    charCounter = 0;
                                }
                            }
                            ignoreIt();
                        };
                    break;
            }
        }

        private byte scanToken()
        {
            switch (char.ToLower(currentChar))
            {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                    takeIt();
                    while (isLetter(currentChar) || isDigit(currentChar))
                    {
                        takeIt();
                    }
                    return Token.IDENTIFIER;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    takeIt();
                    while (isDigit(currentChar))
                    {
                        takeIt();
                    }
                    if (currentChar == '.')
                    {
                        takeIt();
                        while (isDigit(currentChar))
                            takeIt();
                    }
                    return Token.NUMBER;
                case '+':
                case '-':
                case '*':
                case '/':
                    takeIt();
                    return Token.OPERATOR;
                case '<':
                case '>':
                    takeIt();
                    if (currentChar == '=')
                        takeIt();
                    return Token.OPERATOR;
                case '=':
                    takeIt();
                    switch (currentChar)
                    {
                        case '<':
                        case '>':
                        case '=':
                            takeIt();
                            return Token.OPERATOR;
                    }
                    return Token.BECOMES;
                case ';':
                    takeIt();
                    return Token.SEMICOLON;
                case '(':
                    takeIt();
                    return Token.LPAREN;
                case ')':
                    takeIt();
                    return Token.RPAREN;
                case '\n':
                    char lastChar;
                    for (int i = 0; i < 20; i++)
                    {
                        if (fileCounter < fileLines.Length)
                        {
                            charLine = fileLines[fileCounter++].ToCharArray();
                            charCounter = 0;
                        }
                        lastChar = currentChar;
                        currentChar = nextSourceChar();
                        if (currentChar != lastChar)
                        {
                            return scanToken();
                        }
                    }
                    return Token.EOT;
                default:
                    Console.WriteLine("Somethings wrong");
                    return Token.EOT;
            }
        }

        private char nextSourceChar()
        {
            if (charCounter < charLine.Length)
            {
                return charLine[charCounter++];
            }
            return '\n';
        }

        public Token scan()
        {
            if (currentChar == '\n')
            {
                if (fileCounter < fileLines.Length)
                {
                    charLine = fileLines[fileCounter++].ToCharArray();
                    charCounter = 0;
                    currentChar = nextSourceChar();
                }
            }
            while (currentChar == ' ' || currentChar == '\\' || currentChar == '/')
                scanSeperator();
            currentSpelling = new StringBuilder("");
            currentKind = scanToken();
            return new Token(currentKind, currentSpelling.ToString());
        }
    }
}
