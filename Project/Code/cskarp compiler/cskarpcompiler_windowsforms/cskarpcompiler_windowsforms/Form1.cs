using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using Microsoft.CSharp;
using MASClassLibrary;

namespace cskarpcompiler_windowsforms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Lists.agents = new List<agent>();

            agent agentbob = new agent("Bob", 5);

            string inputfil = @"";

            Dictionary<string, string> provOptions = new Dictionary<string, string>();
            provOptions.Add("CompilerVersion", "v4.0");

            StreamReader reader = new StreamReader("C:\\program.cs");
            inputfil = reader.ReadToEnd();
            reader.Close();

            textBox1.Text = inputfil;

            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp", provOptions);
            MessageBox.Show(" " + codeProvider.LanguageOptions);
            string Output = "Out.exe";
            Button ButtonObject = (Button)sender;

            textBox2.Text = "";
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            //Make sure we generate an EXE, not a DLL
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = Output;
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, inputfil);

            if (results.Errors.Count > 0)
            {
                textBox2.BackColor = Color.Red;
                foreach (CompilerError CompErr in results.Errors)
                {
                    textBox2.Text = textBox2.Text +
                                "Line number " + CompErr.Line +
                                ", Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";" +
                                Environment.NewLine + Environment.NewLine;
                }
            }
            else
            {
                //Successful Compile
                textBox2.BackColor = Color.Green;
                textBox2.Text = "Success!";
                //If we clicked run then launch our EXE
                if (ButtonObject.Text == "Run") Process.Start(Output);
            }
        }

    }
}
