using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MultiAgentSystem
{
    class Scanzor
    {
        // The input file being read
        public string[] fileLines;

        // Counts which line currently being looked at
        public int fileCounter = 0;

        // Holds the current line as an array of chars
        public char[] charLine;

        // Counts the current char
        public int charCounter = 0;

        // The current Char being processed by the scanner.
        public char currentChar;

        // The kind of Token expecting the current char/string to have.
        private int currentKind;

        // Builds the string
        private StringBuilder currentSpelling;

        // Coordinates used by the parser to tell where a syntax error has been found
        private int row;
        private int col;

        // Exception for catching errors.
        private GrammarException gException = 
            new GrammarException("These errors were found by the scanner:");

        // Used to accept characters that match the exact char.
        // Not used atm
        private void take(char expectedChar)
        {
            if (currentChar == expectedChar)
            {
                coords();
                currentSpelling.Append(currentChar);
                currentChar = nextSourceChar();
            }
            else
            {
                Printer.Error(" Error!");
                throw new GrammarException("Expected Char '" + expectedChar + "' didnt match '" + currentChar + "' in line " + row + ".");
            }
        }

        // Used to take the current Character no matter which one it is and put it in the string
        private void takeIt()
        {
            coords();
            currentSpelling.Append(currentChar);
            currentChar = nextSourceChar();
        }

        // Used to ignore the current Character and get the next char from the source file
        private void ignoreIt()
        {
            currentChar = nextSourceChar();
        }

        private void coords()
        {
            if (currentSpelling.ToString() == "")
            {
                row = fileCounter;
                col = charCounter;
            }
        }

        // Used to check if the char is a digit (0-9) and returns true if it is
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

        // Checks if the char is a letter (a-z) and returns true if it is
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
                        currentKind = (int)Token.keywords.EOT;
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
                Printer.WriteLine("Identifier");
                /* If the first character read is a letter, spell the word and 
                 * let the Token class decided if its an identifier or a keyword */
                takeIt();
                while (isLetter(currentChar) || isDigit(currentChar))
                {
                    takeIt();
                }
                return (int)Token.keywords.IDENTIFIER;
            }

            if (isDigit(currentChar))
            {
                Printer.WriteLine("Digit");
                // Builds a digit, adds "." if its added in the code
                takeIt();
                scanDigit();
                return (int)Token.keywords.NUMBER;
            }
            switch (char.ToLower(currentChar))
            {
                case '+':
                case '-':
                case '*':
                case '/':
                    Printer.WriteLine("Operator");
                    // returns any of the four usual operators
                    takeIt();
                    return (int)Token.keywords.OPERATOR;
                // Checking if the operator is an "expanded" version
                case '<':
                case '>':
                    Printer.WriteLine("Operator");
                    takeIt();
                    if (currentChar == '=')
                        takeIt();
                    return (int)Token.keywords.OPERATOR;
                // Checking if the "=" means become or its an operator e.g. "=="
                case '=':
                    takeIt();
                    switch (currentChar)
                    {
                        case '<':
                        case '>':
                        case '=':
                            Printer.WriteLine("Operator");
                            takeIt();
                            return (int)Token.keywords.OPERATOR;
                    }
                    Printer.WriteLine("Becomes");
                    return (int)Token.keywords.BECOMES;
                case '"':
                    Printer.WriteLine("Actual String");
                    takeIt();
                    scanString();
                    return (int)Token.keywords.ACTUAL_STRING;
                case ';':
                    Printer.WriteLine("Semicolon");
                    takeIt();
                    return (int)Token.keywords.SEMICOLON;
                case '(':
                    Printer.WriteLine("Left Paranthesis");
                    takeIt();
                    return (int)Token.keywords.LPAREN;
                case ')':
                    Printer.WriteLine("Right Paranthesis");
                    takeIt();
                    return (int)Token.keywords.RPAREN;
                case '{':
                    Printer.WriteLine("Left Bracket");
                    takeIt();
                    return (int)Token.keywords.LBRACKET;
                case '}':
                    Printer.WriteLine("Right Bracket");
                    takeIt();
                    return (int)Token.keywords.RBRACKET;
                case ',':
                    Printer.WriteLine("Comma");
                    takeIt();
                    return (int)Token.keywords.COMMA;
                case ':':
                    Printer.WriteLine("Colon");
                    takeIt();
                    return (int)Token.keywords.COLON;
                case '.':
                    Printer.WriteLine("Period");
                    takeIt();
                    return (int)Token.keywords.PUNCTUATION;
                default:
                    // Someone has screwed up
                    takeIt();
                    Printer.Error(" Error!");
                    throw new GrammarException("Char '" + 
                        currentChar + "' in line " + row + " is not a valid character.");

                    //currentChar = '\n';
                    //currentSpelling.Append("ERROR at line " + fileCounter + " col " + charCounter);
                    //Console.WriteLine(currentSpelling.ToString());
                    //row = fileCounter;
                    //col = charCounter;
                    //return (int)Token.keywords.ERROR;
            }
        }

        // if the next character exists return it, if not return next line char
        private char nextSourceChar()
        {
            if (charCounter < charLine.Length)
            {
                return charLine[charCounter++];
            }
            return '\n';
        }

        public Scanzor()
        {
            fileLines = File.ReadAllLines(Program.path); //The name of the files input
            
            // Initializes the string being read by the scanner, and its counters
            charLine = fileLines[fileCounter++].ToCharArray();
            currentChar = charLine[charCounter++];
        }

        public Token scan()
        {
            // If looking at a seperator, take the next character and start building a new string
            while (currentChar == ' ' || currentChar == '/' || currentChar == '\n' || currentChar == '\t')
            {                
                scanSeperator();
                if (currentKind == (int)Token.keywords.EOT)
                    return new Token(currentKind, "<EOT>", fileCounter, charCounter);
            }
            currentSpelling = new StringBuilder("");

            // Scan for the next token, e.g. an identifier
            currentKind = scanToken();

            Printer.Write(":");
            Console.CursorLeft = 20;
            Printer.Write(currentSpelling.ToString());

            // Returns the token found and the string build while searching for the token
            return new Token(currentKind, currentSpelling.ToString(), row, col);
        }
    }
}
