using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;
using MASClassLibrary;

namespace MASSIVE
{
    class Program
    {
        private static List<Token> Tokens = new List<Token>();
        private static AST newAst;
        public static string path;

        /// <summary>
        /// The main method of the Multi Agent System compiler.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = "MASSIVE (Multiple Agent Simulation System in Virtual Environment) Compiler";
            //StartUp.Aparture();
            //Console.ReadKey();
            /*StartUp startUp = new StartUp();
            Thread thread = new Thread(new ThreadStart(startUp.first));
            thread.Start();
            Console.ReadKey();
            thread.Abort();*/
            
            Console.ForegroundColor = ConsoleColor.White;

            Printer.printLogo();
            Printer.CompilationMarker("Options");

            Methods methodsAndCons = new Methods();

            /* Gives the user, the option to choose to write all errors and actions in the console, 
             * or just leave it. */
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Would you like to print completed procedures? y/n");
                ConsoleKeyInfo cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Y)
                {
                    Printer.printCompleted = true;
                    break;
                }
                if (cki.Key == ConsoleKey.N)
                {
                    Printer.printCompleted = false;
                    break;
                }
                else
                    Console.WriteLine(" is not an option.");
            }

            /* Gives the user the option to change the source file. */
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Would you like to use the standard file (massive.txt)? y/n");

                ConsoleKeyInfo cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");
                    path = Environment.CurrentDirectory + @"\massive.txt";
                    // Checks whether the file exists or not.
                    while (File.Exists(Program.path) == false)
                    {
                        Console.WriteLine("File does not exist");
                        Console.WriteLine("Please input a valid filepath");
                        Program.path = Console.ReadLine();
                    }
                    break;
                }
                if (cki.Key == ConsoleKey.N)
                {
                    path = Directory.GetCurrentDirectory() + @"\";
                    if(getFile())
                        break;
                }
                else
                    Console.WriteLine(" is not an option.");
            }

            Console.WriteLine();
            Console.WriteLine("Compiling " + path);
            Console.WriteLine("Hit any key to continue!");
            Console.ReadKey();

            // Starts the compilation of the choosen file.
            Compile();
        }

        /// <summary>
        /// Method to let the user choose which file in the current directory should be used as source file.
        /// </summary>
        /// <returns>True if a new path has been choosen.</returns>
        private static bool getFile()
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();

            while (true)
            {
                Console.WriteLine(dir.FullName);
                Console.WriteLine();
                foreach (FileInfo fi in files)
                {
                    Console.WriteLine(fi.Name);
                }

                Console.WriteLine();
                Console.WriteLine("Which file would you like to compile, \"back\" to go back? (name.ext)");
                string input = Console.ReadLine();

                if (input.ToLower() == "back")
                    return false;
                if (File.Exists(path + input))
                {
                    path = path + input;
                    return true;
                }
                else
                {
                    Console.WriteLine("The file " + input + " does not exists.");
                }
            }
        }

        /// <summary>
        /// Clears all save tokens from the tokens list, and compiles the choosen file.
        /// </summary>
        private static void Compile()
        {
            Tokens.Clear();

            Printer.printLogo();
            Printer.CompilationMarker("Compile");

            // Starts the scanner.
            Scan();
        }

        /// <summary>
        /// Initializes and starts the scanner.
        /// </summary>
        private static void Scan()
        {
            // Exception for errors found in the scanner.
            GrammarException scanException = new GrammarException(
                "These errors were found while SCANNING:");
            bool scanningError = false;

            // The class is called Scanzor, because the class scanner is already in use in the C-language.
            Scanzor scanzor = new Scanzor();
            Token newToken = null;

            Console.WriteLine();
            Console.CursorLeft = 0;
            Printer.CompilationMarker("@Scanning");
            // Scans the source file, untill the scanner returns an "end of transmission" token.
            while (true)
            {
                try
                {
                    newToken = scanzor.scan();
                }
                catch (GrammarException g)
                {
                    scanningError = true;
                    scanException.containedExceptions.Add(g);
                }

                Tokens.Add(newToken);

                //If the token just found is the End Of Transmission token then break
                if (newToken == null || newToken.kind == (int)Token.keywords.EOT)
                {
                    break;
                }
            }
            try
            {
                if (scanningError)
                {
                    throw scanException;
                }
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Recompile(g.ContainsErrors());
                return;
            }

            // Starts the parser.
            Parse();
        }

        /// <summary>
        /// Initializes the parser, and parses all tokens stored in the tokens list, to an abstract syntax tree.
        /// </summary>
        private static void Parse()
        {
            Console.WriteLine();            
            Console.CursorLeft = 0; 
            Printer.CompilationMarker("@Parsing");

            Parser parser = new Parser(Tokens);

            try
            {
                newAst = parser.parse();
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Recompile(g.ContainsErrors());
                return;
            }

            // Starts decorating the abstract syntax tree.
            Decorate();
        }

        /// <summary>
        /// Initializes the Decorator visitor, and decorates the abstract syntax tree.
        /// </summary>
        private static void Decorate()
        {
            Console.WriteLine();
            Console.CursorLeft = 0;
            Printer.CompilationMarker("@Type and scope checking");

            TypeAndScopeVisitor visitor = new TypeAndScopeVisitor();
            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Recompile(g.ContainsErrors());
                return;
            }

            InputValidation();
        }

        private static void InputValidation()
        {
            Console.WriteLine();
            Console.CursorLeft = 0;
            Printer.CompilationMarker("@Input validation");

            InputValidationVisitor visitor = new InputValidationVisitor();
            
            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Recompile(g.ContainsErrors());
                return;
            }

            CheckVariables();
        }

        private static void CheckVariables()
        {
            Console.WriteLine();
            Console.CursorLeft = 0;
            Printer.CompilationMarker("@Variable checking");

            VariableVisitor visitor = new VariableVisitor();

            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Recompile(g.ContainsErrors());
                if (g.ContainsErrors())
                {
                    return;
                }
            }

            CodeGen();
        }

        private static void CodeGen()
        {
            Console.WriteLine();
            Console.CursorLeft = 0;
            Printer.CompilationMarker("@Code Generation");

            path = Path.GetDirectoryName(path);
            CodeGenerationVisitor visitor = new CodeGenerationVisitor();

            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Recompile(g.ContainsErrors());
                return;
            }
            Completed();
        }

        /// <summary>
        /// Completes the compilation of the source file, by giving the user 
        /// the opportunity to compile and run the output file.
        /// </summary>
        private static void Completed()
        {
            Console.WriteLine();
            Console.CursorLeft = 0;
            Printer.CompilationMarker("@Compilation has completed");

            GenerateCSharp();
        }

        /// <summary>
        /// Compiles and runs the output file with the C# compiler.
        /// </summary>
        private static void GenerateCSharp()
        {
            string CSharpPath = path + @"\MASSIVECode.cs";

            CompileCSharpCode.compile(CSharpPath);

            Process battlefield = new Process();
            battlefield.StartInfo.FileName = Environment.CurrentDirectory + @"\MASSIVEBattleField.exe";
            battlefield.Start();
        }

        /// <summary>
        /// User option to compile the source file again,
        /// in case any changes have been made to the source file.
        /// </summary>
        internal static void Recompile(bool error)
        {
            ConsoleKeyInfo cki;

            string s, g;
            if (error)
            {
                s = "Would you like to compile again? y/n";
                g = "Goodbye.";
            }
            else
            {
                s = "Would you like to compile again? Press 'n' to continue. y/n";
                g = "";
            }

            while (true)
            {
                Console.WriteLine(s);
                cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Y)
                {
                    Compile();
                    break;
                }
                if (cki.Key == ConsoleKey.N)
                {
                    Console.WriteLine(g);
                    break;
                }

                Console.WriteLine(" is not an option.");
            }
        }
    }
}