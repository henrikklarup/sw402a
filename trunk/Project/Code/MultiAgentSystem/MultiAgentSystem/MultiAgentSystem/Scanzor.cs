using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MultiAgentSystem
{
    class Scanzor
    {
        //The input file being read
        public string[] fileLines;

        //Counts which line currently being looked at
        public int fileCounter;

        //Holds the current line as an array of chars
        public char[] charLine;

        //Counts the current char
        public int charCounter;

        //The current Char being processed by the scanner.
        public char currentChar;

        //The kind of Token expecting the current char/string to have.
        private byte currentKind;

        //Builds the string
        private StringBuilder currentSpelling;

        //Used to accept characters that match the exact char.
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

        //Checks if the next char is a * and the char after that is a / if this is true it returns false and the loop breaks because the comment has ended
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

        //Checks if the next char is a newline and returns false, to break the loop and end the comment section
        private bool isOneLineComment(char c)
        {
            if (c == '\n')
            {
                ignoreIt();
                return false;
            }
            return true;
        }

        //Ignores the current Character if its a blank space or a newline
        //Ignores everything between /* and */ with the loop using the isMultiLineCommen method
        private void scanSeperator()
        {
            switch (currentChar)
            {
                case ' ':
                case '\t':
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
                case '\n':
                    if (fileCounter == fileLines.Length)
                        currentKind = Token.EOT;
                    if (fileCounter < fileLines.Length)
                    {
                        charLine = fileLines[fileCounter++].ToCharArray();
                        charCounter = 0;
                    }
                    ignoreIt();
                    break;
            }
        }

        private void scanDigit()
        {
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
        }

        private void scanString()
        {
            char lastChar;
            while (true)
            {
                lastChar = currentChar;
                takeIt();
                if (currentChar == '"' && lastChar != '\\')
                {
                    takeIt();
                    break;
                }
            }
        }

        //Scans the current Character and returns the corresponding byte value (to the token) while building the string which is identifying the token
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
                    //Builds a digit, adds "." if its added in the code
                    takeIt();
                    scanDigit();
                    return Token.NUMBER;
                case '+':
                case '-':
                case '*':
                case '/':
                    // returns any of the four usual operators
                    takeIt();
                    return Token.OPERATOR;
                //Checking if the operator is an "expanded" version
                case '<':
                case '>':
                    takeIt();
                    if (currentChar == '=')
                        takeIt();
                    return Token.OPERATOR;
                //Checking if the "=" means become or its an operator e.g. "=="
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
                case '"':
                    takeIt();
                    scanString();
                    return Token.STRING;
                case ';':
                    takeIt();
                    return Token.SEMICOLON;
                case '(':
                    takeIt();
                    return Token.LPAREN;
                case ')':
                    takeIt();
                    return Token.RPAREN;
                case '{':
                    takeIt();
                    return Token.LBRACKET;
                case '}':
                    takeIt();
                    return Token.RBRACKET;
                case ',':
                    takeIt();
                    return Token.COMMA;
                case ':':
                    takeIt();
                    return Token.COLON;
                case '.':
                    takeIt();
                    return Token.PUNCTUATION;
                default:
                    //Someone has screwed up
                    Console.WriteLine("ERROR at line " + fileCounter + " col " + charCounter);
                    return Token.ERROR;
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
            //If looking at a seperator, take the next character and start building a new string
            while (currentChar == ' ' || currentChar == '/' || currentChar == '\n' || currentChar == '\t')
            {                
                scanSeperator();
                if(currentKind == Token.EOT)
                    return new Token(currentKind, "<EOT>");
            }
            currentSpelling = new StringBuilder("");

            //Scan for the next token, e.g. an identifier
            currentKind = scanToken();

            //Returns the token found and the string build while searching for the token
            return new Token(currentKind, currentSpelling.ToString());
        }
    }
}
