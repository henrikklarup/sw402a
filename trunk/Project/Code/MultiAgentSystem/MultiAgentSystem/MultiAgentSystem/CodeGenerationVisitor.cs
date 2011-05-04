using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MultiAgentSystem
{
    class CodeGenerationVisitor : Visitor
    {
        private string CodeGenerationPath = Program.path + @"\MASSCode.cs";

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
            File.Delete(Program.path + @"\MASSCode.cs");
            string text = File.ReadAllText(
                Environment.CurrentDirectory + @"\mainTextStart.txt");
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(text);
            }

            Printer.WriteLine("Main Block");
            Printer.Expand();

            block.input.visit(this, arg);

            /* arg will always have boolean values in here. 
             * True is the default state, and false signals a special condition.
             * Here arg is set to false, so visitBlock knows not to write the first "{",
             * as this has already been done manually for the mainblock. */
             
            arg = false;

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine("; ");
            }

            block.block.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine("}");
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

            // If arg is true, start a new block.
            if (!(bool)arg)
            {
                // arg is reset to true, as the false state is not needed after this.
                arg = true;
            }
            else
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine("{");
                }
            }

            // Every command in the block is visited.
            foreach (Command c in block.commands)
            {
                c.visit(this, arg);
            }

            // Finish the block.
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine("}");
            }

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
            Token identifier = (Token)objectDeclaration.identifier.visit(this, arg);

            // Write the C# code for an objectdeclaration, with room for input.
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(_object.spelling.ToLower() + " " +
                    identifier.spelling.ToLower() + " = new " + _object.spelling.ToLower() + "(");
            }

            // Visit the input.
            if (objectDeclaration.input != null)
            {
                objectDeclaration.input.visit(this, arg);
            }

            // Finish the objectdeclaration and add the object to the relevant list in C#.
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine("); ");
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
                new Expression(null, null, null, null).GetType()))
            {
                Token masVariable = (Token)becomes;
                if ((bool)arg)
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
            arg = false;
            // Visit and print the expression.
            Expression expr = (Expression)ifCommand.Expression.visit(this, arg);
            arg = true;

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
            arg = false;
            forCommand.CounterExpression.visit(this, arg);
            arg = true;

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
            arg = false;
            whileCommand.LoopExpression.visit(this, arg);
            arg = true;

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

            // The method identifiers are printed.
            methodCall.linkedIdentifier.visit(this, arg);

            // A parenthesis for the input is printed.
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write("(");
            }

            // The input is printed.
            methodCall.input.visit(this, arg);

            // And the input is ended. If arg is false, then no semicolon should be set.
            if ((bool)arg)
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine("); ");
                }
            }
            else
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine(")");
                }
            }

            return null;
        }

        // Syntax: number | identifier | expression | ( expression ) | boolean
        internal override object visitExpression(Expression expression, object arg)
        {
            Printer.WriteLine("Expression");
            Printer.Expand();

            // Put the first part of the expression in a token.
            Token primExpr1 = (Token)expression.primaryExpression_1.visit(this, arg);

            // Put the operator in a token.
            Token _operator = (Token)expression._operator.visit(this, arg);

            // The second part of the expression can be both a new expression or a single variable.
            object primExpr2 = expression.primaryExpression_2.visit(this, arg);

            // Print the first part of the expression.
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(primExpr1.spelling.ToLower() + " " + _operator.spelling + " ");
            }

            // If the second part of the expression is not a new expression, print it as it is. 
            if (!Expression.ReferenceEquals(primExpr2.GetType(),
                new Expression(null, null, null, null).GetType()))
            {
                Token _primExpr2 = (Token)primExpr2;

                // If arg is false, don't print the semicolon.
                if ((bool)arg)
                {
                    using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                    {
                        file.WriteLine(_primExpr2.spelling.ToLower() + "; ");
                    }
                }
                else
                {
                    using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                    {
                        file.Write(_primExpr2.spelling.ToLower());
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
            return p.token;
        }

        internal override object visitInput(Input input, object arg)
        {
            Printer.WriteLine("Input");
            Printer.Expand();

            arg = false;

            // If the first input exists, print it.
            if (input.firstVar != null)
            {
                Token firstVar = (Token)input.firstVar.visit(this, arg);

                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.Write(firstVar.spelling.ToLower());
                }

                arg = true;
            }

            // If more input exists, print it.
            if (input.nextVar != null)
            {
                Printer.Collapse();
                object nextVar = (Input)input.nextVar.visit(this, arg);
                Printer.Expand();
            }

            Printer.Collapse();
            return null;
        }

        internal override object visitLinkedIdentifier(LinkedIdentifier linkedIdentifier, object arg)
        {
            Printer.WriteLine("Method Identifier");
            Printer.Expand();

            string identifier;

            // Save the first identifier and print it.
            identifier = ((Token)linkedIdentifier.Identifier.visit(this, arg)).spelling;

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(identifier.ToLower());
            }

            // If more identifiers exists, print them.
            if (linkedIdentifier.NextLinkedIdentifier != null)
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.Write(".");
                }
                identifier += "." + linkedIdentifier.NextLinkedIdentifier.visit(this, arg);
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

            string ident = (string)assignCommand.ident.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(" = ");
            }

            object becomes = assignCommand.becomes.visit(this, arg);

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if (!Expression.Equals(becomes.GetType(), new Expression(null, null, null, null).GetType()))
            {
                Token masVariable = (Token)becomes;

                if ((bool)arg)
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
    }
}