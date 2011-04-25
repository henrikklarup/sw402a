﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Visitor
    {
        /// <summary>
        /// Visit the AST, the first method called when visiting the AST.
        /// Visits the Main Block.
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        public object visitAST(AST ast, object arg)
        {
            ast.visit(this, arg);
            return null;
        }

        /// <summary>
        /// Visit the Main Block.
        /// Visits the first Block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal object visitMainBlock(Mainblock block, object arg)
        {
            Printer.WriteLine("Main Block");
            Printer.Expand();
            block.block.visit(this, arg);

            Printer.Collapse();
            return null;
        }
        
        /// <summary>
        /// Visit a Block, holds a list of commands.
        /// Visits all commands in the block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal object visitBlock(Block block, object arg)
        {
            Printer.WriteLine("Block");
            Printer.Expand();

            // Everytime a block is visited the block opens 
            // a new scope in the identification table.
            IdentificationTable.openScope();
            foreach (Command c in block.commands)
            {
                c.visit(this, null);
            }
            // When all commands in the block have been visited
            // the scope is closed in the identification table.
            IdentificationTable.closeScope();

            Printer.Collapse();
            return null;
        }

         
        /// <summary>
        /// Visit an object declaration, consists of an object, an identifier, and an input.
        /// Checks if the identifier already exists, if not, creates a new identifier.
        /// Syntax: new object identifier ( input )
        /// Visits the object, the identifer, and the input.
        /// </summary>
        /// <param name="objectDeclaration"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal object visitObjectDecleration(ObjectDeclaration objectDeclaration, object arg)
        {
            Printer.WriteLine("Object Declaration");
            Printer.Expand();

            // Get the kind of Object and the spelling of the identifier.
            Token _object = (Token)objectDeclaration._object.visit(this, arg);
            Token identifier = (Token)objectDeclaration.identifier.visit(this, arg);

            int kind = _object.kind;
            string ident = identifier.spelling;

            // Puts the kind and spelling into the Identification Table.
            IdentificationTable.enter(kind, ident);

            // Visit the input and check the spelling.
            objectDeclaration.input.visit(this, arg);
            Printer.Collapse();
            return null;
        }

        /// <summary>
        /// Visit a type declaration, consists of a type, an identifier, 
        /// and whatever its declared as (expression or variable).
        /// Syntax: Type VarName = becomes
        /// Visits the type, the identifier, and the expression or variable.
        /// </summary>
        /// <param name="typeDeclaration"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal object visitTypeDecleration(TypeDeclaration typeDeclaration, object arg)
        {
            Printer.WriteLine("Type declaration");
            Printer.Expand();
            // Stores the type and the identifier of the declaration
            Token Type = (Token)typeDeclaration.Type.visit(this, arg);
            Token VarName = (Token)typeDeclaration.VarName.visit(this, arg);
            object becomes = typeDeclaration.Becomes.visit(this, arg);

            int kind = Type.kind;
            string ident = VarName.spelling;

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if (Expression.ReferenceEquals(typeDeclaration.Becomes.GetType(),new Expression(null, null, null).GetType()))
            {
                Expression expression = (Expression)becomes;
                kind = expression.type;
            }
            else
            {
                Token masVariable = (Token)becomes;

                // Checks that the type matches the variable, that the identifier becomes.
                switch (kind)
                {
                    case (int)Token.keywords.STRING:
                        if (masVariable.kind != (int)Token.keywords.ACTUAL_STRING)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER 
                                && IdentificationTable.retrieve(masVariable) != kind)
                                Printer.ErrorLine(((Token.keywords)masVariable.kind).ToString());
                        }
                        break;
                    case (int)Token.keywords.BOOL:
                        if (masVariable.kind != (int)Token.keywords.TRUE 
                            && masVariable.kind != (int)Token.keywords.FALSE)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER 
                                && IdentificationTable.retrieve(masVariable) != kind)
                                Printer.ErrorLine(((Token.keywords)masVariable.kind).ToString());
                        }
                        break;
                    case (int)Token.keywords.NUM:
                        if (masVariable.kind != (int)Token.keywords.NUMBER)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER 
                                && IdentificationTable.retrieve(masVariable) != kind)
                                Printer.ErrorLine(((Token.keywords)masVariable.kind).ToString());
                        }
                        break;
                    default:
                        Printer.ErrorLine("Type declaration did not match.");
                        break;
                }
            }
            if (IdentificationTable.retrieve(VarName) == (int)Token.keywords.ERROR)
                IdentificationTable.enter(kind, ident);
            else
            {
                Printer.ErrorLine("Identifier " + VarName.spelling + " at " + VarName.col + ", " + VarName.row + " has already been declared.");
            }
            Printer.Collapse();
            return null;
        }

        /// <summary>
        /// Visit an if expression, consists of a boolean expression and two blocks.
        /// If the last block (the else block) exists, then visit it.
        /// Syntax: if ( bool-expression ) block
        /// Syntax: if ( bool-expression ) block else block
        /// Visits the expression and both blocks if they exists.
        /// </summary>
        /// <param name="ifCommand"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal object visitIfCommand(IfCommand ifCommand, object arg)
        {
            Printer.WriteLine("If Command");
            Printer.Expand();

            // Visit the expression, if the expression isn't boolean, report and error.
            Expression expr = (Expression)ifCommand.Expression.visit(this, arg);
            if (expr.type != (int)Token.keywords.BOOL)
                Printer.ErrorLine("If expression is not of type boolean.");

            // Visit the first block.
            ifCommand.IfBlock.visit(this, arg);

            // If the second block exists, visit it.
            if (ifCommand.ElseBlock != null)
            {
                ifCommand.ElseBlock.visit(this, arg);
            }

            Printer.Collapse();
            return null;
        }

        
        /// <summary>
        /// Visit for loop, consitst of a declaration, a boolean expression, an expression, and a block.
        /// Syntax: for ( type-declaration ; bool-expression ; math-expression ) block
        /// Visits the declaration, the two expressions and the block.
        /// </summary>
        /// <param name="forCommand"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal object visitForCommand(ForCommand forCommand, object arg)
        {
            Printer.WriteLine("For Command");
            Printer.Expand();

            // Visit the declaration, the two expressions and the block.
            forCommand.CounterDeclaration.visit(this, arg);
            forCommand.LoopExpression.visit(this, arg);
            forCommand.CounterExpression.visit(this, arg);
            forCommand.ForBlock.visit(this, arg);

            Printer.Collapse();
            return null;
        }

        /// <summary>
        /// Visit while loop
        /// Syntax: while ( bool-expression ) block
        /// </summary>
        /// <param name="whileCommand"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal object visitWhileCommand(WhileCommand whileCommand, object arg)
        {
            Printer.WriteLine("While Command");
            Printer.Expand();
            whileCommand.LoopExpression.visit(this, arg);
            whileCommand.WhileBlock.visit(this, arg);

            Printer.Collapse();
            return null;
        }


        // identifier ( input ) | identifier . method-call
        internal object visitMethodCall(MethodCall methodCall, object arg)
        {
            Printer.WriteLine("Method Call");
            methodCall.methodIdentifier.visit(this, arg);
            methodCall.input.visit(this, arg);

            return null;
        }

        // Syntax: number | identifier | expression | ( expression ) | boolean
        internal object visitExpression(Expression expression, object arg)
        {
            Printer.WriteLine("Expression");
            Printer.Expand();

            // Always a Token of kind, number, boolen or identifier.
            Token primExpr1 = (Token)expression.primaryExpression_1.visit(this, arg);

            // Always a Token of kind, operator, if this doesn't exists, visit the primaryExpression and return null.
            Token _operator = (Token)expression._operator.visit(this, arg);

            // 2nd Primary expression can be both, an expression or a token.
            object primExpr2 = expression.primaryExpression_2.visit(this, arg);

            // Check which kind of expression this is, according to the operator.
            switch (_operator.spelling)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                    // If the operator is a mathematic operator,
                    // Save the type as a NUM, since numbers are of type NUMBER.
                    expression.type = (int)Token.keywords.NUM;
                    break;
                case "<=":
                case ">=":
                case "=>":
                case "=<":
                case "==":
                    // If the operator is a boolean operator,
                    // Save the type as BOOL, since boolean types are of type TRUE or FALSE.
                    expression.type = (int)Token.keywords.BOOL;
                    break;
                default:
                    Printer.ErrorLine("Failed to identify operator at " + primExpr1.col + ", " + primExpr1.row + ".");
                    break;
            }

            // If this evaluates to true, the primExpr2 is a token and therefor a number, boolean or identifier.
            // primExpr1 is of type Token.
            if (object.ReferenceEquals(primExpr2.GetType(), primExpr1.GetType()))
            {
                int identifierKind;
                // If primary expression 1 is an identifier, check which type it is.
                if (primExpr1.kind == (int)Token.keywords.IDENTIFIER)
                    identifierKind = IdentificationTable.retrieve(primExpr1);
                else
                    identifierKind = primExpr1.kind;

                Token _primExpr2 = (Token)primExpr2;

                // Ensure primExpr2 matches primExpr1.
                switch (_primExpr2.kind)
                {
                    case (int)Token.keywords.NUMBER:
                        if (identifierKind != (int)Token.keywords.NUM && identifierKind != (int)Token.keywords.NUMBER)
                            Printer.ErrorLine("The type of " + primExpr1.spelling + " does not match.");
                        break;
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                        if (identifierKind != (int)Token.keywords.FALSE && identifierKind != (int)Token.keywords.TRUE)
                            Printer.ErrorLine("The type of " + primExpr1.spelling + " does not match.");
                        break;
                }
            }
            // If primExpr2 is not a token, it must be an expression and will be checked when visited
            else
            {
                Expression _primExpr2 = (Expression)primExpr2;

                int identifierKind;
                // If primary expression 1 is an identifier, check which type it is.
                if (primExpr1.kind == (int)Token.keywords.IDENTIFIER)
                    identifierKind = IdentificationTable.retrieve(primExpr1);
                else
                    identifierKind = primExpr1.kind;

                // If the expression is a mathematic expression, there should be no true/false expressions.
                if (expression.type == (int)Token.keywords.NUM)
                    if (identifierKind == (int)Token.keywords.TRUE || identifierKind == (int)Token.keywords.FALSE)
                        Printer.ErrorLine("The expression at " + primExpr1.col + ", " + primExpr1.row + " is invalid.");

                // If any sub-expression is bool, the entire expression is bool
                // and should only have one boolean operator.
                if (_primExpr2.type == (int)Token.keywords.BOOL)
                {
                    if (expression.type == (int)Token.keywords.BOOL)
                        Printer.ErrorLine("The expression at " + primExpr1.col + ", " + primExpr1.row + " is invalid.");

                    // If any of the expressions are of type boolean
                    // the entire expression is boolean and is set as boolean.
                    expression.type = (int)Token.keywords.BOOL;
                }
            }

            Printer.Collapse();
            return expression;
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            Printer.WriteLine("Identifier: " + identifier.token.spelling);
            return identifier.token;
        }

        internal object visitOperator(Operator p, object arg)
        {
            Printer.WriteLine("Operator: " + p.token.spelling);
            return p.token;
        }

        internal object visitInput(Input input, object arg)
        {
            Printer.WriteLine("Input");
            Printer.Expand();

            Token firstVar;
            object nextVar;

            if (input.firstVar != null)
            {
                firstVar = (Token)input.firstVar.visit(this, arg);

                if (firstVar.kind == (int)Token.keywords.IDENTIFIER)
                {
                    if (IdentificationTable.retrieve(firstVar) == (int)Token.keywords.ERROR)
                    {
                        Printer.ErrorLine("Variable " + firstVar.spelling + " at " + firstVar.col + ", " + firstVar.row + " has not been declared.");
                    }
                }
                if (input.nextVar != null)
                {
                    Printer.Collapse();
                    nextVar = (Input)input.nextVar.visit(this, arg);
                    Printer.Expand();
                }
            }

            Printer.Collapse();
            return null;
        }

        internal object visitMethodIdentifier(MethodIdentifier methodIdentifier, object arg)
        {
            Printer.WriteLine("Method Identifier");
            Printer.Expand();
            Token identifier;
            string ident;

            identifier = (Token)methodIdentifier.Identifier.visit(this, arg);
            ident = identifier.spelling;
            methodIdentifier.NextMethodIdentifier.visit(this, arg);

            Printer.Collapse();
            return null;
        }

        internal object visitMASNumber(MASNumber mASNumber, object arg)
        {
            Printer.WriteLine("Number: " + mASNumber.token.spelling);
            return mASNumber.token;
        }

        internal object visitMASType(MASType mASType, object arg)
        {
            Printer.WriteLine("Type: " + mASType.token.spelling);
            return mASType.token;
        }

        internal object visitMASString(MASString mASString, object arg)
        {
            Printer.WriteLine("String: " + mASString.token.spelling);
            return mASString.token;
        }

        internal object visitMASBool(MASBool mASBool, object arg)
        {
            Printer.WriteLine("Bool: " + mASBool.token.spelling);
            return mASBool.token;
        }

        internal object visitObject(Object p, object arg)
        {
            Printer.WriteLine("Object: " + p.token.spelling);
            return p.token;
        }

        internal object visitMASVariable(MASVariable mASVariable, object arg)
        {
            Printer.WriteLine("Variable: " + mASVariable.token.spelling);
            return mASVariable.token;
        }

        internal object visitAssignCommand(AssignCommand assignCommand, object arg)
        {
            Printer.WriteLine("Assign Command");
            Printer.Expand();

            int kind;
            Token ident = (Token)assignCommand.ident.visit(this, arg);
            object becomes = assignCommand.becomes.visit(this, arg);

            kind = IdentificationTable.retrieve(ident);

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if (Expression.Equals(becomes.GetType(), new Expression(null, null, null).GetType()))
            {
                Expression expression = (Expression)becomes;
                kind = expression.type;
            }
            else
            {
                Token masVariable = (Token)becomes;

                switch (kind)
                {
                    case (int)Token.keywords.STRING:
                        if (masVariable.kind != (int)Token.keywords.ACTUAL_STRING)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER && IdentificationTable.retrieve(masVariable) != kind)
                                Printer.ErrorLine("Type declaration has not been declared to a variable of type " + masVariable.kind.ToString());
                        }
                        break;
                    case (int)Token.keywords.BOOL:
                        if (masVariable.kind != (int)Token.keywords.TRUE || masVariable.kind != (int)Token.keywords.FALSE)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER && IdentificationTable.retrieve(masVariable) != kind)
                                Printer.ErrorLine("Type declaration has not been declared to a variable of type " + masVariable.kind.ToString());
                        }
                        break;
                    case (int)Token.keywords.NUM:
                        if (masVariable.kind != (int)Token.keywords.NUMBER)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER && IdentificationTable.retrieve(masVariable) != kind)
                                Printer.ErrorLine("Type declaration has not been declared to a variable of type " + masVariable.kind.ToString());
                        }
                        break;
                    default:
                        Printer.ErrorLine("Type declaration has not been declared as a type");
                        break;
                }
            }
            Printer.Collapse();
            return null;
        }
    }
}