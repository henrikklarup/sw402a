using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ActionInterpeter
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

        //Coordinates used by the parser to tell where a syntax error has been found
        private int row;
        private int col;

        // Exception for catching errors.
        private GrammarException gException =
            new GrammarException("These errors were found by the scanner:");

        // Used to accept characters that match the exact char.
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

        //Used to take the current Character no matter which one it is and put it in the string
        private void takeIt()
        {
            coords();
            currentSpelling.Append(currentChar);
            currentChar = nextSourceChar();
        }

        //Used to ignore the current Character and get the next char from the source file
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
                return (int)Token.keywords.IDENTIFIER;
            }

            if (isDigit(currentChar))
            {
                //Builds a digit, adds "." if its added in the code
                takeIt();
                scanDigit();
                return (int)Token.keywords.NUMBER;
            }
            switch (char.ToLower(currentChar))
            {
                case '(':
                    takeIt();
                    return (int)Token.keywords.LPAREN;
                case ')':
                    takeIt();
                    return (int)Token.keywords.RPAREN;
                case ',':
                    takeIt();
                    return (int)Token.keywords.COMMA;
                case '.':
                    takeIt();
                    return (int)Token.keywords.PUNCTUATION;
                default:
                    //Someone has screwed up
                    currentChar = '\n';
                    currentSpelling.Append("ERROR at line " + fileCounter + " col " + charCounter);
                    Console.WriteLine(currentSpelling.ToString());
                    row = fileCounter;
                    col = charCounter;
                    return (int)Token.keywords.ERROR;
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

        public Scanzor()
        {
            fileLines = File.ReadAllLines(@"C:/Users/Rasmus/Desktop/test.txt"); //The name of the files input

            //Initializes the string being read by the scanner, and its counters
            charLine = fileLines[fileCounter++].ToCharArray();
            currentChar = charLine[charCounter++];
        }

        public Token scan()
        {
            currentSpelling = new StringBuilder("");

            //Scan for the next token, e.g. an identifier
            currentKind = scanToken();

            //Returns the token found and the string build while searching for the token
            return new Token(currentKind, currentSpelling.ToString(), row, col);
        }
    }
}