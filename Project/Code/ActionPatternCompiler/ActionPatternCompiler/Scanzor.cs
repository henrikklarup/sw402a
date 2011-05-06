using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ActionPatternCompiler
{
    class Scanzor
    {
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

        // Exception for catching errors.
        private GrammarException gException =
            new GrammarException("These errors were found by the scanner:");

        // Used to accept characters that match the exact char.
        private void take(char expectedChar)
        {
            if (currentChar == expectedChar)
            {
                currentSpelling.Append(currentChar);
                currentChar = nextSourceChar();
            }
            else
            {
                throw new GrammarException("Expected Char '" + expectedChar + "' didnt match '" + currentChar + "'.");
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
                case ',':
                    takeIt();
                    return (int)Token.keywords.COMMA;
                case '.':
                    takeIt();
                    return (int)Token.keywords.PUNCTUATION;
                default:
                    // Someone has screwed up
                    takeIt();
                    Printer.Error(" Error!");
                    throw new GrammarException("Char '" +
                        currentChar + "' is not a valid character.");
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
            //Initializes the string being read by the scanner, and its counters
            charLine = ActionPatternCompiler.input.ToCharArray();
            currentChar = charLine[charCounter++];
        }

        public Token scan()
        {
            // If looking at a seperator, take the next character and start building a new string
            while (currentChar == ' ' || currentChar == '\n' || currentChar == '\t')
            {
                scanSeperator();
                if (currentKind == (int)Token.keywords.EOT)
                    return new Token(currentKind, "<EOT>");
            }
            currentSpelling = new StringBuilder("");

            //Scan for the next token, e.g. an identifier
            currentKind = scanToken();

            //Returns the token found and the string build while searching for the token
            return new Token(currentKind, currentSpelling.ToString());
        }

        private void scanSeperator()
        {
            switch (currentChar)
            {
                case ' ':
                case '\t':
                    ignoreIt();
                    break;
                case '\n':
                    currentKind = (int)Token.keywords.EOT;
                    ignoreIt();
                    break;
            }
        }
    }
}