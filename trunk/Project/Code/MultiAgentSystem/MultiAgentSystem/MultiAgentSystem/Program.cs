using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using MASClassLibrary;

namespace MultiAgentSystem
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
            StartUp.Aparture();
            Console.ReadKey();
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
                Console.WriteLine("Would you like to use the standard file (desktop/mass.txt)? y/n");

                ConsoleKeyInfo cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Y)
                {
                    Console.WriteLine("");
                    path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\mass.txt";
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
                "These errors were found by the scanner:");
            bool scanningError = false;

            // The class is called Scanzor, because the class scanner is already in use in the C-language.
            Scanzor scanzor = new Scanzor();
            Token newToken = null;

            Console.WriteLine();
            Console.CursorLeft = 0;
            Printer.CompilationMarker("@Scanning");
            Console.Title = "MASS Compiler: Scanning";
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
            }
            if (scanningError)
            {
                Console.Write("Errors were found while SCANNING.");
                Recompile();
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
            Console.Title = "MASS Compiler: Parsing";

            Parser parser = new Parser(Tokens);

            try
            {
                newAst = parser.parse();
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Console.Write("Errors were found while PARSING.");
                Recompile();
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
            Printer.CompilationMarker("@Decorating");
            Console.Title = "MASS Compiler: Decorating";

            DecorationVisitor visitor = new DecorationVisitor();
            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Console.Write("Errors were found while DECORATING.");
                Recompile();
                return;
            }

            Intermediate();
        }

        private static void Intermediate()
        {
            Console.WriteLine();
            Console.CursorLeft = 0;
            Printer.CompilationMarker("@Intermediate");
            Console.Title = "MASS Compiler: Intermediate";

            IntermediateVisitor visitor = new IntermediateVisitor();
            
            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Console.Write("Errors were found while checking methods and input.");
                Recompile();
                return;
            }

            CodeGen();
        }

        private static void CodeGen()
        {
            Console.WriteLine();
            Console.CursorLeft = 0;
            Printer.CompilationMarker("@Code Generation");
            Console.Title = "MASS Compiler: Code Generation";

            path = Path.GetDirectoryName(path);
            CodeGenerationVisitor visitor = new CodeGenerationVisitor();

            try
            {
                visitor.visitAST(newAst, null);
            }
            catch (GrammarException g)
            {
                g.PrintExceptions();
                Console.Write("Errors were found while GENERATING CODE.");
                Recompile();
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
            Console.Title = "MASS Compiler: Compilation has completed";

            ConsoleKeyInfo cki;
            while (true)
            {
                Console.WriteLine("Would you like to compile the newly compiled *.cs file.? y/n");
                cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Y)
                {
                    // Compiles the output file with the C# compiler.
                    GenerateCSharp();
                    break;
                }
                if (cki.Key == ConsoleKey.N)
                {
                    /* Gives the user the option to compile the source file again,
                     * if the user has made any changes to it. */
                    Recompile();
                    break;
                }

                Console.WriteLine(" is not an option.");
            }
        }

        /// <summary>
        /// Compiles and runs the output file with the C# compiler.
        /// </summary>
        private static void GenerateCSharp()
        {
            string CSharpPath = path + @"\MASSCode.cs";

            CompileCSharpCode.compile(CSharpPath);
            /* Gives the user the option to compile the source file again,
            * if the user has made any changes to it. */
            Recompile();
        }

        /// <summary>
        /// User option to compile the source file again,
        /// in case any changes have been made to the source file.
        /// </summary>
        private static void Recompile()
        {
            ConsoleKeyInfo cki;

            while (true)
            {
                Console.WriteLine(" Would you like to compile again? y/n");
                cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Y)
                {
                    Compile();
                    break;
                }
                if (cki.Key == ConsoleKey.N)
                {
                    Console.WriteLine("Goodbye.");
                    break;
                }

                Console.WriteLine(" is not an option.");
            }
        }
    }
}