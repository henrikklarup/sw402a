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

            Dictionary<string, string> provOptions = new Dictionary<string, string>();
            provOptions.Add("CompilerVersion", "v4.0");

            StreamReader reader = new StreamReader(path);
            inputfil = reader.ReadToEnd();
            reader.Close();

            Console.Write(inputfil);

            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp", provOptions);
            string Output = Path.ChangeExtension(path, ".exe");

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            //Make sure we generate an EXE, not a DLL
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = Output;
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, inputfil);

            if (results.Errors.Count > 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                foreach (CompilerError CompErr in results.Errors)
                {
                    Console.WriteLine("Line number " + CompErr.Line +", Error Number: " + CompErr.ErrorNumber +", '" + CompErr.ErrorText + ";");
                }
            }
            else
            {
                //Successful Compile
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
                Process.Start(Output);
            }
        }
    }
}
