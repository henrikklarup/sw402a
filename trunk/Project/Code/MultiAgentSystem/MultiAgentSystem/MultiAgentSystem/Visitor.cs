using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Visitor
    {
        public IdentificationTable idTable = new IdentificationTable();

        public object visitAST(AST ast, object arg)
        {
            Printer.WriteLine("AST");
            Printer.Expand();
            ast.visit(this, arg);

            Printer.Collapse();
            return null;
        }

        internal object visitMainBlock(Mainblock block, object arg)
        {
            Printer.WriteLine("Main Block");
            Printer.Expand();
            block.block.visit(this, arg);

            Printer.Collapse();
            return null;
        }

        internal object visitBlock(Block block, object arg)
        {
            Printer.WriteLine("Block");
            Printer.Expand();

            idTable.openScope();
            foreach (Command c in block.commands)
            {
                c.visit(this, null);
            }
            idTable.closeScope();

            Printer.Collapse();
            return null;
        }

        // new object identifier ( input )
        internal object visitObjectDecleration(ObjectDeclaration objectDeclaration, object arg)
        {
            Printer.WriteLine("Object Declaration");
            Printer.Expand();
            // Get the kind of Object and the spelling of the identifier.
            int kind = (int)objectDeclaration._object.visit(this, arg);
            string ident = (string)objectDeclaration.identifier.visit(this, arg);

            // Puts the kind and spelling into the Identification Table.
            idTable.enter(kind, ident);

            // Visit the input and check the spelling.
            objectDeclaration.input.visit(this, arg);
            Printer.Collapse();
            return null;
        }

        // Type VarName = becomes...SomethingSomething...
        internal object visitTypeDecleration(TypeDeclaration typeDeclaration, object arg)
        {
            Printer.WriteLine(_typeDeclaration);
            Printer.Expand();
            // Stores the type and the identifier of the declaration
            Token Type = (Token)typeDeclaration.Type.visit(this, arg);
            Token VarName = (Token)typeDeclaration.VarName.visit(this, arg);

            int kind = Type.kind;
            string ident = VarName.spelling;

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if (typeDeclaration.Becomes.visit(this, arg) == null)
            {
                Expression expression = (Expression)typeDeclaration.Becomes;
                kind = expression.type;
            }
            else
            {
                ///HAS TO BE UPDATED TO MATCH EXPRESSIONS

                /*
                MASVariable masVariable = (MASVariable)typeDeclaration.Becomes;

                switch (kind)
                {
                    case (int)Token.keywords.STRING:
                        if (masVariable.token.kind != (int)Token.keywords.ACTUAL_STRING)
                        {
                            if (masVariable.token.kind == (int)Token.keywords.IDENTIFIER && idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine(_typeDeclaration + _typeErrorText + ((Token.keywords)masVariable.token.kind).ToString());
                        }
                        break;
                    case (int)Token.keywords.BOOL:
                        if (masVariable.token.kind != (int)Token.keywords.TRUE || masVariable.token.kind != (int)Token.keywords.FALSE)
                        {
                            if (masVariable.token.kind == (int)Token.keywords.IDENTIFIER && idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine(_typeDeclaration + _typeErrorText + ((Token.keywords)masVariable.token.kind).ToString());
                        }
                        break;
                    case (int)Token.keywords.NUM:
                        if (masVariable.token.kind != (int)Token.keywords.NUMBER)
                        {
                            if (masVariable.token.kind == (int)Token.keywords.IDENTIFIER && idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine(_typeDeclaration + _typeErrorText + ((Token.keywords)masVariable.token.kind).ToString());
                        }
                        break;
                    default:
                        Printer.ErrorLine(_typeDeclaration + _undeclaredErrorText);
                        break;
                }*/
            }
            if (idTable.retrieve(VarName) == (int)Token.keywords.ERROR)
                idTable.enter(kind, ident);
            else
            {
                Printer.ErrorLine("Identifier " + VarName.spelling + " at " + VarName.col + ", " + VarName.row + " has already been declared.");
            }
            Printer.Collapse();
            return null;
        }

        // if ( bool-expression ) block
        // if ( bool-expression ) block else block
        internal object visitIfCommand(IfCommand ifCommand, object arg)
        {
            Printer.WriteLine("If Command");
            Printer.Expand();
            ifCommand.Expression.visit(this, arg);
            ifCommand.IfBlock.visit(this, arg);
            if (ifCommand.ElseBlock != null)
            {

            }

            Printer.Collapse();
            return null;
        }

        // for ( type-declaration ; bool-expression ; math-expression ) block
        internal object visitForCommand(ForCommand forCommand, object arg)
        {
            Printer.WriteLine("For Command");
            Printer.Expand();
            forCommand.CounterDeclaration.visit(this, arg);
            forCommand.LoopExpression.visit(this, arg);
            forCommand.CounterExpression.visit(this, arg);
            forCommand.ForBlock.visit(this, arg);

            Printer.Collapse();
            return null;
        }

        // while ( bool-expression ) block
        internal object visitWhileCommand(WhileCommand whileCommand, object arg)
        {
            Printer.WriteLine("While Command");
            Printer.Expand();
            whileCommand.LoopExpression.visit(this, arg);
            whileCommand.WhileBlock.visit(this, arg);

            Printer.Collapse();
            return null;
        }

        internal object visitMethodIdentifier(MethodIdentifier methodIdentifier, object arg)
        {
            Printer.WriteLine("Method Identifier");
            Printer.Expand();
            string ident;

            ident = (string)methodIdentifier.Identifier.visit(this, arg);
            methodIdentifier.NextMethodIdentifier.visit(this, arg);

            Printer.Collapse();
            throw new NotImplementedException();
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
                    identifierKind = idTable.retrieve(primExpr1);
                else
                    identifierKind = primExpr1.kind;

                Token _primExpr2 = (Token)primExpr2;

                // Ensure primExpr2 matches primExpr1.
                switch (_primExpr2.kind)
                {
                    case (int)Token.keywords.NUMBER:
                        if (identifierKind != (int)Token.keywords.NUMBER)
                            Printer.ErrorLine("The type of " + primExpr1.spelling + " does not match the type of " + _primExpr2.spelling + ".");
                        break;
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                        if (identifierKind != (int)Token.keywords.FALSE && identifierKind != (int)Token.keywords.TRUE)
                            Printer.ErrorLine("The type of " + primExpr1.spelling + " does not match the type of " + _primExpr2.spelling + ".");
                        break;
                }
            }
            // If primExpr2 is not a token, it must be an expression and will be checked when visited
            else
            {
                Expression _primExpr2 = (Expression)primExpr2;

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
            throw new NotImplementedException();
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
            Printer.WriteLine("Object");
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

            kind = idTable.retrieve(ident);

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.½
            if (assignCommand.becomes.visit(this, arg) == null)
            {
                Expression expression = (Expression)assignCommand.becomes;
                kind = expression.type;
            }
            else
            {
                MASVariable masVariable = (MASVariable)assignCommand.becomes;

                switch (kind)
                {
                    case (int)Token.keywords.STRING:
                        if (masVariable.token.kind != (int)Token.keywords.ACTUAL_STRING)
                        {
                            if (masVariable.token.kind == (int)Token.keywords.IDENTIFIER && idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine("Type declaration has not been declared to a variable of type " + masVariable.token.kind.ToString());
                        }
                        break;
                    case (int)Token.keywords.BOOL:
                        if (masVariable.token.kind != (int)Token.keywords.TRUE || masVariable.token.kind != (int)Token.keywords.FALSE)
                        {
                            if (masVariable.token.kind == (int)Token.keywords.IDENTIFIER && idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine("Type declaration has not been declared to a variable of type " + masVariable.token.kind.ToString());
                        }
                        break;
                    case (int)Token.keywords.NUM:
                        if (masVariable.token.kind != (int)Token.keywords.NUMBER)
                        {
                            if (masVariable.token.kind == (int)Token.keywords.IDENTIFIER && idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine("Type declaration has not been declared to a variable of type " + masVariable.token.kind.ToString());
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
