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
        public int fileCounter = 0;

        //Holds the current line as an array of chars
        public char[] charLine;

        //Counts the current char
        public int charCounter = 0;

        //The current Char being processed by the scanner.
        public char currentChar;

        //The kind of Token expecting the current char/string to have.
        private int currentKind;

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

        //Used to check if the char is a digit (0-9) and returns true if it is
        private bool isDigit(char c)
        {
            switch (c)
            {
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
                    return true;
            }
            return false;
        }

        //Checks if the char is a letter (a-z) and returns true if it is
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

        /* Checks if the next char is a * and the char after that is a / 
         * if this is true it returns false and the loop breaks because the comment has ended */
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

        /* Checks if the next char is a newline and returns false 
         * to break the loop and end the comment section */
        private bool isOneLineComment(char c)
        {
            if (c == '\n')
            {
                ignoreIt();
                return false;
            }
            return true;
        }

        /* Ignores the current Character if its a blank space or a newline 
         * Ignores everything between in multiline comments with the loop using the isMultiLineCommen method */
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
                    /* If the next character after the first / 
                     * the current characters are a part of a single line comment */
                    if (currentChar == '/')
                        while (isOneLineComment(currentChar))
                        {
                            ignoreIt();
                        };
                    /* If the next character is * the scanner is reading
                     * a section of commenting */
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
                    /* If the current character is \n, change to the next line read
                     * unless the last line of the file has been reached then return
                     * the End of Transmission token*/
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

        /* As long as the current character is a digit append it to the string and
         * read the next untill no digit is read
         * if a . is read build the last part of the digit */
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

        /* Reads currentChar untill the current character is " and checks if the previous character " was \ (\")
         * If the previous character wasn't \ the string has been completed and is returned */
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

        /* Scans the current Character and returns the corresponding byte value 
         * (to the token) while building the string which is identifying the token */
        private int scanToken()
        {
            if (isLetter(currentChar))
            {
                /* If the first character read is a letter, spell the word and 
                 * let the Token class decided if its an identifier or a keyword */
                takeIt();
                while (isLetter(currentChar) || isDigit(currentChar))
                {
                    takeIt();
                }
                return Token.IDENTIFIER;
            }

            if (isDigit(currentChar))
            {
                //Builds a digit, adds "." if its added in the code
                takeIt();
                scanDigit();
                return Token.NUMBER;
            }
            switch (char.ToLower(currentChar))
            {
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

        //if the next character exists return it, if not return next line char
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
