using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace ActionInterpeter
{
    public class ActionInterpet
    {
        private static List<Token> Tokens = new List<Token>();      // All tokens found by the scanner.
        private static AST newAst;                                  // The ast created by the parser.
        public static string input;                                 // The input from the GUI console.
        public static StringBuilder output;                         // The output from the interpeter, errors etc.
        private static Visitor visitor;                             // The instance of the visitor class used to decorate and call commands.

        #region Main
        /// <summary>
        /// Main class used for debugging purpose only.
        /// </summary>
        public static void Main()
        {
            while (true)
            {
                input = Console.ReadLine();
                if (input.Count() == 0)
                    continue;
                Parse();
                Console.WriteLine("Success");
                Console.ReadKey();
            }
        }
        #endregion

        #region The compiler
        /// <summary>
        /// Starts the compiler.
        /// </summary>
        /// <returns>Errors and Exceptions</returns>
        public static string Compile()
        {
            output = new StringBuilder("");
            try
            {
                Parse();
            }
            catch (InvalidMoveOptionException e)
            {
                foreach (string s in e.PrintExceptions())
                {
                    Printer.WriteLine(s);
                }
            }
            catch (WrongTeamException e)
            {
                foreach (string s in e.PrintExceptions())
                {
                    Printer.WriteLine(s);
                }
            }
            catch (Exception e)
            {
                Printer.WriteLine(e.Message);
            }
            return output.ToString();
        }

        /// <summary>
        /// Parses the input string.
        /// </summary>
        private static void Parse()
        {
            Parser parser = new Parser();

            try
            {
                newAst = parser.parse();
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                return;
            }
            Decorate();
        }

        /// <summary>
        /// Decorates the ast from the parser, when move option is reached, executes the move command.
        /// </summary>
        private static void Decorate()
        {
            visitor = new Visitor();
            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                return;
            }
        }
        #endregion
    }
}
