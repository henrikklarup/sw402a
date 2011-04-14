using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Visitor
    {
        public IdentificationTable idTable = new IdentificationTable();

        private static string _booleanExpression = "boolean.", _mathExpression = "mathematic.",
            _typeDeclaration = "Type Declaration", 
            _notError = " is not of expression type ", 
            _typeErrorText = " has not been declared to a variable of type ",
            _undeclaredErrorText = " has not been declared as a type.";

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
                kind = expression.kind;
            }
            else
            {
                MASVariable masVariable = (MASVariable)typeDeclaration.Becomes;

                switch (kind)
                {
                    case (int)Token.keywords.STRING:
                        if (masVariable.token.kind != (int)Token.keywords.ACTUAL_STRING)
                        {
                            if (idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine(_typeDeclaration + _typeErrorText + ((Token.keywords)masVariable.token.kind).ToString());
                        }
                        break;
                    case (int)Token.keywords.BOOL:
                        if (masVariable.token.kind != (int)Token.keywords.TRUE || masVariable.token.kind != (int)Token.keywords.FALSE)
                        {
                            if (idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine(_typeDeclaration + _typeErrorText + ((Token.keywords)masVariable.token.kind).ToString());
                        }
                        break;
                    case (int)Token.keywords.NUM:
                        if (masVariable.token.kind != (int)Token.keywords.NUMBER)
                        {
                            if (idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine(_typeDeclaration + _typeErrorText + ((Token.keywords)masVariable.token.kind).ToString());
                        }
                        break;
                    default:
                        Printer.ErrorLine(_typeDeclaration + _undeclaredErrorText);
                        break;
                }
            }
            if (idTable.retrieve(VarName) == (int)Token.keywords.ERROR)
                idTable.enter(kind, ident);
            else
            {
                Printer.ErrorLine("Identifier " + VarName.spelling + " at " + VarName.col+ ", " + VarName.row + " has already been declared.");
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
            Token primExpr1 = (Token)expression.primaryExpression_1.visit(this, arg);
            Token _operator = (Token)expression._operator.visit(this, arg);
            Token primExpr2 = (Token)expression.primaryExpression_2.visit(this, arg);

            switch (_operator.spelling)
            { 
                case "<":
                case ">":
                case "<=":
                case ">=":
                case "==":
                    expression.kind = (int)Token.keywords.BOOL;
                    break;
                case "+":
                case "-":
                case "/":
                case "*":
                    expression.kind = (int)Token.keywords.NUM;
                    break;
                default:
                    Printer.ErrorLine("Operator " + _operator.spelling + " at " + _operator.col + ", " + _operator.row + " is not an operator.");
                    Printer.Collapse();
                    return null;
            }

            if(expression.kind == (int)Token.keywords.BOOL)
            {
                switch (primExpr1.kind)
                { 
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                        break;
                    case (int)Token.keywords.IDENTIFIER:
                        if (idTable.retrieve(primExpr1) != (int)Token.keywords.BOOL)
                            Printer.ErrorLine("Identifier " + primExpr1.spelling + " at " + primExpr1.col + ", "
                                + primExpr1.row + _notError + _booleanExpression);
                        break;
                    default:
                        Printer.ErrorLine("Boolean expression " + primExpr1.spelling + " at " + primExpr1.col + ", " 
                            + primExpr1.row + _notError + _booleanExpression);
                        break;
                }

                switch (primExpr2.kind)
                {
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                        break;
                    case (int)Token.keywords.IDENTIFIER:
                        if (idTable.retrieve(primExpr2) != (int)Token.keywords.BOOL)
                            Printer.ErrorLine("Identifier " + primExpr2.spelling + " at " + primExpr2.col + ", " 
                                + primExpr2.row + _notError + _booleanExpression);
                        break;
                    default:
                        Printer.ErrorLine("Boolean expression " + primExpr2.spelling + " at " + primExpr2.col + ", "
                            + primExpr2.row + _notError + _booleanExpression);
                        break;
                }
            }
            if (expression.kind == (int)Token.keywords.NUM)
            {
                switch (primExpr1.kind)
                { 
                    case (int)Token.keywords.NUMBER:
                        break;
                    case (int)Token.keywords.IDENTIFIER:
                        if (idTable.retrieve(primExpr1) != (int)Token.keywords.NUM)
                            Printer.ErrorLine("Identifier " + primExpr1.spelling + " at " + primExpr1.col + ", "
                                + primExpr1.row + _notError + _mathExpression);
                        break;
                    default:
                        Printer.ErrorLine("Mathematic expression " + primExpr1.spelling + " at " + primExpr1.col + ", "
                            + primExpr1.row + _notError + _mathExpression);
                        break;
                }

                switch (primExpr2.kind)
                {
                    case (int)Token.keywords.NUMBER:
                        break;
                    case (int)Token.keywords.IDENTIFIER:
                        if (idTable.retrieve(primExpr2) != (int)Token.keywords.NUM)
                            Printer.ErrorLine("Identifier " + primExpr2.spelling + " at " + primExpr2.col + ", " 
                                + primExpr2.row + _notError + _mathExpression);
                        break;
                    default:
                        Printer.ErrorLine("Mathematic expression " + primExpr2.spelling + " at " + primExpr2.col + ", "
                            + primExpr2.row + _notError + _mathExpression);
                        break;
                }
            }
            Printer.Collapse();
            return null;
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
                kind = expression.kind;
            }
            else
            {
                MASVariable masVariable = (MASVariable)assignCommand.becomes;

                switch (kind)
                {
                    case (int)Token.keywords.STRING:
                        if (masVariable.token.kind != (int)Token.keywords.ACTUAL_STRING)
                        {
                            if (idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine("Type declaration has not been declared to a variable of type " + masVariable.token.kind.ToString());
                        }
                        break;
                    case (int)Token.keywords.BOOL:
                        if (masVariable.token.kind != (int)Token.keywords.TRUE || masVariable.token.kind != (int)Token.keywords.FALSE)
                        {
                            if (idTable.retrieve(masVariable.token) != kind)
                                Printer.ErrorLine("Type declaration has not been declared to a variable of type " + masVariable.token.kind.ToString());
                        }
                        break;
                    case (int)Token.keywords.NUM:
                        if (masVariable.token.kind != (int)Token.keywords.NUMBER)
                        {
                            if (idTable.retrieve(masVariable.token) != kind)
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
