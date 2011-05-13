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
        private static Visitor visitor;                             // The instance of the visitor class used to decorate and call commands.
        internal static string input;                               // The input from the GUI console.
        internal static StringBuilder output;                       // The output from the interpeter, errors etc.
        internal static agent thisAgent;                            // Agent used by the visitor to determine the placeholder "unit".

        #region Main
        /// <summary>
        /// Main class used for debugging purpose only.
        /// </summary>
        public static void Main()
        {
            string output;
            XML.initLists();
            XML.returnLists(Environment.CurrentDirectory);
            while (true)
            {
                input = Console.ReadLine();
                if (input.Count() == 0)
                    continue;
                output = Compile(input);
                if (!output.Any())
                    Console.WriteLine("Success");
                else
                    Console.WriteLine(output.ToString());
                Console.ReadKey();
            }
        }
        #endregion

        #region The compiler
        /// <summary>
        /// Starts the compilation of an action.
        /// </summary>
        /// <param name="_input">The input to the compiler (e.g. "agent 1 move up")</param>
        /// <returns>Errors</returns>
        public static string Compile(string _input)
        {
            input = _input;
            output = new StringBuilder("");

            StartCompile();

            return output.ToString();
        }
        /// <summary>
        /// Starts the compilation of an action.
        /// </summary>
        /// <param name="_input">The input to the compiler (e.g. "agent 1 move up")</param>
        /// <param name="_agent">The agent, the keyword "unit" represents</param>
        /// <returns>Errors</returns>
        public static string Compile(string _input, agent _agent)
        {
            input = _input;
            output = new StringBuilder("");
            thisAgent = _agent;

            StartCompile();

            return output.ToString();
        }

        /// <summary>
        /// Starts the compiler.
        /// </summary>
        /// <returns>Errors and Exceptions</returns>
        private static void StartCompile()
        {
            try
            {
                Parse();
            }
            catch (Exception e)
            {
                Printer.WriteLine(e.Message);
            }
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
