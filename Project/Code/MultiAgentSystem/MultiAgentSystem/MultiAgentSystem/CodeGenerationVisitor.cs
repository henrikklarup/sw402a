﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MASSIVE
{
    class CodeGenerationVisitor : Visitor
    {
        private string CodeGenerationPath = Program.path + @"\MASSIVECode.cs";

        private bool Print = true;
        private int blockCount = 0;

        /// <summary>
        /// visit the AST, the first method called when visiting the AST.
        /// visits the Main Block.
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        public override object visitAST(AST ast, object arg)
        {
            ast.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine("}");
                file.Close();
            }

            return null;
        }

        /// <summary>
        /// visit the Main Block.
        /// visits the first Block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal override object visitMainBlock(Mainblock block, object arg)
        {
            File.Delete(Program.path + @"\MASSIVECode.cs");

            string text = "using System; using System.Drawing; using System.Collections.Generic; " +
                "using MASClassLibrary; namespace MultiAgentSystem { class Program { " + 
		        "static void Main(string[] args) { Lists.agents = new List<agent>(); " + 
			    "Lists.squads = new List<squad>(); Lists.teams = new List<team>(); " +
                "Lists.actionPatterns = new List<actionpattern>();";

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(text);
                file.Close();
            }

            Printer.WriteLine("Main Block");
            Printer.Expand();

            /* arg will always have boolean values in here. 
             * True is the default state, and false signals a special condition.
             * Here arg is set to false, so visitBlock knows not to write the first "{",
             * as this has already been done manually for the mainblock. */
             
            Print = false;

            block.block.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine("}");
                file.Close();
            }

            Printer.Collapse();

            return null;
        }

        /// <summary>
        /// visit a Block, holds a list of commands.
        /// visits all commands in the block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal override object visitBlock(Block block, object arg)
        {
            Printer.WriteLine("Block");
            Printer.Expand();

            blockCount++;

            // If arg is true, start a new block.
            if (!Print)
            {
                // arg is reset to true, as the false state is not needed after this.
                Print = true;
            }
            else
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine("{");
                    file.Close();
                }
            }

            // Everytime a block is visited the block opens 
            // a new scope in the identification table.
            IdentificationTable.openScope();
            foreach (Command c in block.commands)
            {
                c.visit(this, arg);
            }
            // When all commands in the block have been visited
            // the scope is closed in the identification table.
            IdentificationTable.closeScope();

            // Finish the block.
            if (blockCount == 1)
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine("XML.generateXML(@\"" + Program.path + "\");");
                    file.WriteLine("Console.WriteLine(\"XML generation complete.\");");
                    file.Close();
                }
            }
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine("}");
                file.Close();
            }

            blockCount--;

            Printer.Collapse();
            return null;
        }


        /// <summary>
        /// visit an object declaration, consists of an object, an identifier, and an input.
        /// Checks if the identifier already exists, if not, creates a new identifier.
        /// Syntax: new object identifier ( input )
        /// visits the object, the identifer, and the input.
        /// </summary>
        /// <param name="objectDeclaration"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal override object visitObjectDeclaration(ObjectDeclaration objectDeclaration, object arg)
        {
            Printer.WriteLine("Object Declaration");
            Printer.Expand();

            // The object type and identifier in the objectdeclaration are saved.
            Token _object = (Token)objectDeclaration._object.visit(this, arg);
            string identifier = ((Token)objectDeclaration.identifier.visit(this, arg)).spelling;

            // Puts the kind and spelling into the Identification Table.
            IdentificationTable.enter(_object.kind, identifier);

            List<MASConstructor> builders = MASLibrary.FindConstructor(_object.kind);
            builders.ElementAt(0).InstantiateProperties(identifier);

            string input = "";

            // Visit the input.
            if (objectDeclaration.input != null)
            {
                input = (string)objectDeclaration.input.visit(this, arg);
            }

            if (builders.Count > 1)
            {
                Token firstVar, dummyVar;

                Input temp1, temp2;

                // Make a copy of arg.
                List<MASConstructor> list = new List<MASConstructor>(builders);

                // For each element in arg, the element is tested against the given input.
                for (int i = 0; i < builders.Count; i++)
                {
                    temp1 = list.ElementAt(i).ValidInput;
                    temp2 = objectDeclaration.input;

                    // Test through the input.
                    while (temp1 != null && temp2 != null)
                    {
                        firstVar = (Token)temp2.firstVar.visit(this, null);
                        if (firstVar.kind == (int)Token.keywords.IDENTIFIER)
                        {
                            firstVar.kind = IdentificationTable.retrieve(firstVar.spelling);
                        }

                        dummyVar = (Token)temp1.firstVar.visit(this, null);

                        // If the inputs don't match, remove it from the list of potential valid inputs.
                        if (!MASLibrary.MatchingTypes(firstVar.kind, dummyVar.kind))
                        {
                            builders.Remove(list.ElementAt(i));
                        }

                        // Update the variables for the next round of testing.
                        temp1 = temp1.nextVar;
                        temp2 = temp2.nextVar;
                    }

                    if (temp1 != null && temp2 == null)
                    {
                        builders.Remove(list.ElementAt(i));
                    }
                    else if (temp1 == null && temp2 != null)
                    {
                        builders.Remove(list.ElementAt(i));
                    }
                }
            }

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(builders.ElementAt(0).PrintGeneratedCode(identifier, input));
                file.Close();
            }

            if (Print)
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine("; ");
                    file.Close();
                }
            }

            Printer.Collapse();
            return null;
        }

        /// <summary>
        /// visit a type declaration, consists of a type, an identifier, 
        /// and whatever its declared as (expression or variable).
        /// Syntax: Type VarName = becomes
        /// visits the type, the identifier, and the expression or variable.
        /// </summary>
        /// <param name="typeDeclaration"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal override object visitTypeDeclaration(TypeDeclaration typeDeclaration, object arg)
        {
            Printer.WriteLine("Type declaration");
            Printer.Expand();

            // Store the type and the identifier of the declaration.
            Token Type = (Token)typeDeclaration.Type.visit(this, arg);
            Token VarName = (Token)typeDeclaration.VarName.visit(this, arg);

            int kind = Type.kind;
            string ident = VarName.spelling;

            string type;
            // If the type is "num", then it should be "double" in C#. Else use the same type name.
            if (kind == (int)Token.keywords.NUM)
            {
                type = "double";
            }
            else
            {
                type = ((Token.keywords)kind).ToString();
            }

            // Print the first part of the type declaration.
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(type.ToLower() + " " + ident.ToLower() + " = ");
            }

            // Visit the object that represents what the variable is to become.
            object becomes = typeDeclaration.Becomes.visit(this, arg);

            /* If the declaration becomes an expression, then it was printed when "becomes" was visited.
             * If that isn't the case, then simply take what the declaration becomes and print it. */
            if (!Expression.ReferenceEquals(typeDeclaration.Becomes.GetType(),
                new Expression(null).GetType()))
            {
                Token masVariable = (Token)becomes;
                if (Print)
                {
                    using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                    {
                        file.WriteLine(masVariable.spelling.ToLower() + "; ");
                    }
                }
                else
                {
                    using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                    {
                        file.WriteLine(masVariable.spelling.ToLower());
                    }
                }
            }

            Printer.Collapse();
            return null;
        }

        /// <summary>
        /// Visit an if expression, consists of a boolean expression and two blocks.
        /// If the last block (the else block) exists, then visit it.
        /// Syntax: if ( bool-expression ) block
        /// Syntax: if ( bool-expression ) block else block
        /// visits the expression and both blocks if they exists.
        /// </summary>
        /// <param name="ifCommand"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal override object visitIfCommand(IfCommand ifCommand, object arg)
        {
            Printer.WriteLine("If Command");
            Printer.Expand();
            
            // Start the if command.
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write("if (");
            }

            // As no semicolon should be printed at the end of the expression, arg is marked false.
            Print = false;
            // Visit and print the expression.
            Expression expr = (Expression)ifCommand.Expression.visit(this, arg);
            Print = true;

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine(")");
            }

            // Visit the first block.
            ifCommand.IfBlock.visit(this, arg);

            // If the second block exists, visit it.
            if (ifCommand.ElseBlock != null)
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine("else");
                }
                ifCommand.ElseBlock.visit(this, arg);
            }

            Printer.Collapse();
            return null;
        }


        /// <summary>
        /// visit for loop, consitst of a declaration, a boolean expression, an expression, and a block.
        /// Syntax: for ( type-declaration ; bool-expression ; math-expression ) block
        /// visits the declaration, the two expressions and the block.
        /// </summary>
        /// <param name="forCommand"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal override object visitForCommand(ForCommand forCommand, object arg)
        {
            Printer.WriteLine("For Command");
            Printer.Expand();

            // Start the for loop.
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write("for (");
            }

            // Visit the declaration, the two expressions and the block.
            forCommand.CounterDeclaration.visit(this, arg);
            forCommand.LoopExpression.visit(this, arg);

            // As no semicolon should be printed at the end of the expression, arg is marked false.
            Print = false;
            forCommand.CounterExpression.visit(this, arg);
            Print = true;

            // Finish printing the conditions for the loop.
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine(")");
            }

            // And visit the block.
            forCommand.ForBlock.visit(this, arg);

            Printer.Collapse();
            return null;
        }

        /// <summary>
        /// visit while loop
        /// Syntax: while ( bool-expression ) block
        /// </summary>
        /// <param name="whileCommand"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal override object visitWhileCommand(WhileCommand whileCommand, object arg)
        {
            Printer.WriteLine("While Command");
            Printer.Expand();

            // Start printing the while loop.
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write("while (");
            }

            // As no semicolon should be printed at the end of the expression, arg is marked false.
            Print = false;
            whileCommand.LoopExpression.visit(this, arg);
            Print = true;

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine(")");
            }

            whileCommand.WhileBlock.visit(this, arg);

            Printer.Collapse();
            return null;
        }

        // identifier ( input ) | identifier . method-call
        internal override object visitMethodCall(MethodCall methodCall, object arg)
        {
            Printer.WriteLine("Method Call");

            Print = false;
            Token identifier = (Token)methodCall.linkedIdentifier.visit(this, arg);
            Print = true;

            string input = "";

            // Find the name of the method called.
            string[] names = identifier.spelling.Split('.');
            string method = names[names.Length - 1];

            // Name of the object the method is called on, and its kind.
            string name = identifier.spelling.Remove(identifier.spelling.Length - (method.Length + 1));
            int kind = IdentificationTable.retrieve(name);

            List<MASMethod> methods = MASLibrary.FindMethod(method, kind);
            
            // Visit the input.
            if (methodCall.input != null)
            {
                input = (string)methodCall.input.visit(this, arg);
            }

            if (methods.Count > 1)
            {
                Token firstVar, dummyVar;

                Input temp1, temp2;

                // Make a copy of arg.
                List<MASMethod> list = new List<MASMethod>(methods);

                // For each element in arg, the element is tested against the given input.
                for (int i = 0; i < methods.Count; i++)
                {
                    temp1 = list.ElementAt(i).ValidInput;
                    temp2 = methodCall.input;

                    // Test through the input.
                    while (temp1 != null && temp2 != null)
                    {
                        firstVar = (Token)temp2.firstVar.visit(this, null);
                        if (firstVar.kind == (int)Token.keywords.IDENTIFIER)
                        {
                            firstVar.kind = IdentificationTable.retrieve(firstVar.spelling);
                        }

                        dummyVar = (Token)temp1.firstVar.visit(this, null);

                        // If the inputs don't match, remove it from the list of potential valid inputs.
                        if (!MASLibrary.MatchingTypes(firstVar.kind, dummyVar.kind))
                        {
                            methods.Remove(list.ElementAt(i));
                        }

                        // Update the variables for the next round of testing.
                        temp1 = temp1.nextVar;
                        temp2 = temp2.nextVar;
                    }

                    if (temp1 != null && temp2 == null)
                    {
                        methods.Remove(list.ElementAt(i));
                    }
                    else if (temp1 == null && temp2 != null)
                    {
                        methods.Remove(list.ElementAt(i));
                    }
                }
            }

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(methods.ElementAt(0).PrintGeneratedCode(name, input));
            }

            if (Print)
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine("; ");
                }
            }

            return null;
        }

        // Syntax: number | identifier | expression | ( expression ) | boolean
        internal override object visitExpression(Expression expression, object arg)
        {
            Printer.WriteLine("Expression");
            Printer.Expand();

            // If the parent expression is not null, visit it.
            if (expression.parentExpr != null)
            {
                ParentExpression parentExpr =
                    (ParentExpression)expression.parentExpr.visit(this, true);
            }
            else
            {
                expression.primExpr1.visit(this, true);

                Token opr = (Token)expression.opr.visit(this, arg);
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine(opr.spelling);
                    file.Close();
                }

                expression.primExpr2.visit(this, true);
            }

            if (Print)
            {
                if (arg == null)
                {
                    using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                    {
                        file.WriteLine("; ");
                        file.Close();
                    }
                }
            }

            Printer.Collapse();
            return expression;
        }

        internal override object visitIdentifier(Identifier identifier, object arg)
        {
            Printer.WriteLine("Identifier: " + identifier.token.spelling);
            return identifier.token;
        }

        internal override object visitOperator(Operator p, object arg)
        {
            Printer.WriteLine("Operator: " + p.token.spelling);
            switch (p.token.spelling)
            {
                case "=<":
                    return new Token(p.token.kind, "<=", p.token.row, p.token.col);
                case "=>":
                    return new Token(p.token.kind, ">=", p.token.row, p.token.col);
            }
            return p.token;
        }

        internal override object visitInput(Input input, object arg)
        {
            Printer.WriteLine("Input");
            Printer.Expand();

            Print = false;

            string s = (string)arg;

            // If the first input exists, print it.
            if (input.firstVar != null)
            {
                Token firstVar = (Token)input.firstVar.visit(this, arg);

                s += firstVar.spelling;

                Print = true;
            }

            // If more input exists, print it.
            if (input.nextVar != null)
            {
                s += ", ";
                Printer.Collapse();
                s = (string)input.nextVar.visit(this, s);
                Printer.Expand();
            }

            Printer.Collapse();
            return s;
        }

        internal override List<Input> visitOverload(Input input, List<Input> arg, int live)
        {
            return null;
        }

        internal override object visitLinkedIdentifier(LinkedIdentifier linkedIdentifier, object arg)
        {
            Printer.WriteLine("Linked Identifier");
            Printer.Expand();

            Token t = (Token)linkedIdentifier.Identifier.visit(this, arg);

            Token identifier = new Token(t.kind, t.spelling, t.row, t.col);

            if (linkedIdentifier.NextLinkedIdentifier != null)
            {
                identifier.spelling += "." +
                    ((Token)linkedIdentifier.NextLinkedIdentifier.visit(this, arg)).spelling;
            }

            Printer.Collapse();
            return identifier;
        }

        internal override object visitMASNumber(MASNumber mASNumber, object arg)
        {
            Printer.WriteLine("Number: " + mASNumber.token.spelling);
            return mASNumber.token;
        }

        internal override object visitMASType(MASType mASType, object arg)
        {
            Printer.WriteLine("Type: " + mASType.token.spelling);
            return mASType.token;
        }

        internal override object visitMASString(MASString mASString, object arg)
        {
            Printer.WriteLine("String: " + mASString.token.spelling);
            return mASString.token;
        }

        internal override object visitMASBool(MASBool mASBool, object arg)
        {
            Printer.WriteLine("Bool: " + mASBool.token.spelling);
            return mASBool.token;
        }

        internal override object visitObject(Object p, object arg)
        {
            Printer.WriteLine("Object: " + p.token.spelling);
            return p.token;
        }

        internal override object visitMASVariable(MASVariable mASVariable, object arg)
        {
            Printer.WriteLine("Variable: " + mASVariable.token.spelling);
            return mASVariable.token;
        }

        internal override object visitAssignCommand(AssignCommand assignCommand, object arg)
        {
            Printer.WriteLine("Assign Command");
            Printer.Expand();

            string ident = ((Token)assignCommand.ident.visit(this, arg)).spelling;

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(ident + " = ");
            }

            object becomes = assignCommand.becomes.visit(this, arg);

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if (!Expression.Equals(becomes.GetType(), new Expression(null).GetType()))
            {
                Token masVariable = (Token)becomes;

                if (Print)
                {
                    using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                    {
                        file.WriteLine(masVariable.spelling.ToLower() + "; ");
                    }
                }
                else
                {
                    using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                    {
                        file.Write(masVariable.spelling.ToLower());
                    }
                }
            }

            Printer.Collapse();
            return null;
        }

        internal override object visitPrimaryExpression(PrimaryExpression primaryExpression, object arg)
        {
            // If var is not null, the primary expression is a variable.
            if (primaryExpression.var != null)
            {
                Token var = (Token)primaryExpression.var.visit(this, arg);
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.Write(var.spelling.ToLower());
                }
            }
            // If parentExpression is not null, the primary expression is a parentExpression.
            else if (primaryExpression.parentExpression != null)
            {
                // Set the type to the same type as in its expression.
                ParentExpression parentExpression =
                    (ParentExpression)primaryExpression.parentExpression.visit(this, arg);
            }
            // If the expression is not null, the primary expression is a new expression.
            else if (primaryExpression.expression != null)
            {
                // Set the type to the same type as in the expression.
                Expression expr = (Expression)primaryExpression.expression.visit(this, arg);
            }

            return primaryExpression;
        }

        internal override object visitParentExpression(ParentExpression parentExpression, object arg)
        {
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine("(");
            }

            parentExpression.expr.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine(")");
            }

            return parentExpression;
        }
    }
}