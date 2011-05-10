using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using Microsoft.CSharp;

namespace MultiAgentSystem
{
    class CompileCSharpCode
    {
        public static void compile(string path)
        {
            string inputfil = @"";
            string CustomLibraryPath = "";

            /// Gives the directory of our custom XML-file in the SVN Repository.
            /// Hopefully it exists.
            CustomLibraryPath = (Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()).ToString()).ToString()).ToString()).ToString() + "\\MASClassLibrary\\MASClassLibrary\\bin\\Debug\\MASClassLibrary.dll";
            string Output = Path.ChangeExtension(path, ".exe");

            /// Checks whether or not the path for our custom XML-file is correct
            /// and asks for a new one if necessary.
            if (File.Exists(CustomLibraryPath) == false)
            {

                Printer.ErrorLine("Cannot locate MASlibrary at location " + CustomLibraryPath);

                while (File.Exists(CustomLibraryPath) == false)
                {
                    Console.WriteLine("Please enter a path");
                    CustomLibraryPath = Console.ReadLine();
                }

            }

            // Creates a dictionary containing options for the compiler.
            Dictionary<string, string> provOptions = new Dictionary<string, string>();
            provOptions.Add("CompilerVersion", "v4.0");
            
            // Reads entire inputfile input a string.
            StreamReader reader = new StreamReader(path);
            inputfil = reader.ReadToEnd();
            reader.Close();

            /// Creates a codeprovider which uses the dictionary from above.
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp", provOptions);
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            
            //Make sure we generate an EXE, not a DLL
            parameters.GenerateExecutable = true;
            /// Determines where the output file is going to be placed.
            /// In this case it will be placed in our standard "path" Directory. Look above.
            parameters.OutputAssembly = Output;

            /// These two Referenced Assemblies tell the compiler which
            /// assemblies we are going to use in the code it's supposed to compile
            /// so that it can compile the necessary functions with it.
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            parameters.ReferencedAssemblies.Add("System.Collections.Generic.dll");
            parameters.ReferencedAssemblies.Add("System.Linq.dll");
            parameters.ReferencedAssemblies.Add("System.Text.dll");
            parameters.ReferencedAssemblies.Add("System.IO.dll");
            parameters.ReferencedAssemblies.Add(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.XML.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
            parameters.ReferencedAssemblies.Add(CustomLibraryPath);

            // This class gathers all the results from the compilation and accepts the source code.
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, inputfil);

            // If there is more than 0 errors they are printet to the user.
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                    Printer.ErrorLine("Line number " + CompErr.Line +", Error Number: " + CompErr.ErrorNumber +", '" + CompErr.ErrorText + ";");
                }
            }
                // Otherwise the Console tell the users the compilation succeeded and runs the compiled code.
            else
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
                Console.BackgroundColor = ConsoleColor.Black;
                Process.Start(Output);
            }
        }
    }
}
