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

            string text = File.ReadAllText(
                Environment.CurrentDirectory + @"\classes.txt");
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine(text);
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
            arg = false;

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

            if ((bool)arg)
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine("{");
                }
            }

            arg = true;

            foreach (Command c in block.commands)
            {
                c.visit(this, arg);
            }

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

            // Get the kind of Object and the spelling of the identifier.
            Token _object = (Token)objectDeclaration._object.visit(this, arg);
            Token identifier = (Token)objectDeclaration.identifier.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(_object.spelling.ToLower() + " " +
                    identifier.spelling.ToLower() + " = new " + _object.spelling.ToLower() + "(");
            }

            // visit the input and check the spelling.
            if (objectDeclaration.input != null)
                objectDeclaration.input.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine("); ");
                file.WriteLine(_object.spelling.ToLower() + "List.Add(" +
                    identifier.spelling.ToLower() + ");");
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

            // Stores the type and the identifier of the declaration
            Token Type = (Token)typeDeclaration.Type.visit(this, arg);
            Token VarName = (Token)typeDeclaration.VarName.visit(this, arg);

            int kind = Type.kind;
            string ident = VarName.spelling;

            // Print the first part of the type declaration:
            // If the type is "num", then it should be "double" in C#. Else use the same type name.
            string type;
            if (kind == (int)Token.keywords.NUM)
            {
                type = "double";
            }
            else
            {
                type = ((Token.keywords)kind).ToString();
            }

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(type.ToLower() + " " + ident.ToLower() + " = ");
            }

            object becomes = typeDeclaration.Becomes.visit(this, arg);

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if (!Expression.ReferenceEquals(typeDeclaration.Becomes.GetType(),
                new Expression(null, null, null, null).GetType()))
            {
                Token masVariable = (Token)becomes;
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.WriteLine(masVariable.spelling.ToLower() + "; ");
                }   
            }

            Printer.Collapse();
            return null;
        }

        /// <summary>
        /// visit an if expression, consists of a boolean expression and two blocks.
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
            
            arg = false;
            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write("if (");
            }

            // visit the expression, if the expression isn't boolean, report and error.
            Expression expr = (Expression)ifCommand.Expression.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine(")");
            }

            arg = true;

            // visit the first block.
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

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write("for (");
            }

            // visit the declaration, the two expressions and the block.
            forCommand.CounterDeclaration.visit(this, arg);
            forCommand.LoopExpression.visit(this, arg);

            arg = false;

            forCommand.CounterExpression.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.WriteLine(")");
            }

            arg = true;

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

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write("while (");
            }

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

            methodCall.methodIdentifier.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write("(");
            }

            methodCall.input.visit(this, arg);

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

            // Always a Token of kind number, boolean or identifier.
            Token primExpr1 = (Token)expression.primaryExpression_1.visit(this, arg);

            // Always a Token of kind, operator, if this doesn't exists, 
            // visit the primaryExpression and return null.
            Token _operator = (Token)expression._operator.visit(this, arg);

            // 2nd Primary expression can be both an expression or a token.
            object primExpr2 = expression.primaryExpression_2.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(primExpr1.spelling.ToLower() + " " + _operator.spelling + " ");
            }

            // If this evaluates to true, the primExpr2 is a token and therefor a number, boolean or identifier.
            // primExpr1 is of type Token.
            if (object.ReferenceEquals(primExpr2.GetType(), primExpr1.GetType()))
            {
                Token _primExpr2 = (Token)primExpr2;

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

            Token firstVar;
            object nextVar;

            if (input.firstVar != null)
            {
                firstVar = (Token)input.firstVar.visit(this, arg);

                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.Write(firstVar.spelling.ToLower());
                }

                arg = true;
            }
            if (input.nextVar != null)
            {
                Printer.Collapse();
                nextVar = (Input)input.nextVar.visit(this, arg);
                Printer.Expand();
            }

            Printer.Collapse();
            return null;
        }

        internal override object visitMethodIdentifier(MethodIdentifier methodIdentifier, object arg)
        {
            Printer.WriteLine("Method Identifier");
            Printer.Expand();
            Token identifier;

            identifier = (Token)methodIdentifier.Identifier.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(identifier.spelling.ToLower());
            }

            if (methodIdentifier.NextMethodIdentifier != null)
            {
                using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
                {
                    file.Write(".");
                }
                methodIdentifier.NextMethodIdentifier.visit(this, arg);
            }

            Printer.Collapse();
            return null;
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

            int kind;
            Token ident = (Token)assignCommand.ident.visit(this, arg);

            using (StreamWriter file = new StreamWriter(CodeGenerationPath, true))
            {
                file.Write(ident.spelling.ToLower() + " = ");
            }

            object becomes = assignCommand.becomes.visit(this, arg);

            kind = IdentificationTable.retrieve(ident);

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