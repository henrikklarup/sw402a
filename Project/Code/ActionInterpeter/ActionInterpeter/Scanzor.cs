using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ActionInterpeter
{
    class Scanzor
    {
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

        // Exception for catching errors.
        private GrammarException gException =
            new GrammarException("These errors were found by the scanner:");

        /// <summary>
        /// Used to accept characters that match the exact char.
        /// </summary>
        /// <param name="expectedChar">The character the scanner expects.</param>
        private void take(char expectedChar)
        {
            if (currentChar == expectedChar)
            {
                currentSpelling.Append(currentChar);
                currentChar = nextSourceChar();
            }
            else
            {
                Printer.Error(" Error!");
                throw new GrammarException("Expected Char '" + expectedChar + "' didnt match '" + currentChar + "'.");
            }
        }

        /// <summary>
        /// Takes the current Character no matter which one it is and put it in the string
        /// </summary>
        private void takeIt()
        {
            currentSpelling.Append(currentChar);
            currentChar = nextSourceChar();
        }

        /// <summary>
        /// Used to ignore the current Character and get the next char from the source file
        /// </summary>
        private void ignoreIt()
        {
            currentChar = nextSourceChar();
        }

        /// <summary>
        /// Used to check if the char is a digit (0-9)
        /// </summary>
        /// <param name="c">The character being checked</param>
        /// <returns>True if its a digit</returns>
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

        /// <summary>
        /// Checks if the char is a letter (a-z).
        /// </summary>
        /// <param name="c">The character being checked</param>
        /// <returns>True if its a letter.</returns>
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

        /// <summary>
        /// As long as the current character is a digit append it to the string and
        /// read the next untill no digit is read
        /// if a . is read build the last part of the digit
        /// </summary>
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

        
        /// <summary>
        /// Scans the current Character and returns the corresponding byte value 
        /// (to the token) while building the string which is identifying the token
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Initializes the scanner, sets the current char and the current line being read.
        /// </summary>
        public Scanzor()
        {
            //Initializes the string being read by the scanner, and its counters
            charLine = ActionInterpet.input.ToCharArray();
            currentChar = charLine[charCounter++];
        }

        /// <summary>
        /// Scans the scource code.
        /// </summary>
        /// <returns>A Token containing the word, the type and its position.</returns>
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

        /// <summary>
        /// Ignores all seperators (space, indent, and newlines)
        /// </summary>
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