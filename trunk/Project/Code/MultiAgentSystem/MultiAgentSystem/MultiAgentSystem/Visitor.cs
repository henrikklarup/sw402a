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
            Console.WriteLine("AST");
            ast.visit(this, arg);
            return null;
        }

        internal object visitMainBlock(Mainblock block, object arg)
        {
            Console.WriteLine("Main Block");
            block.block.visit(this, arg);
            return null;
        }

        internal object visitBlock(Block block, object arg)
        {
            Console.WriteLine("Block");

            idTable.openScope();
            foreach (Command c in block.commands)
            { 
                c.visit(this, null);
            }
            idTable.closeScope();
            return null;
        }

        // new object identifier ( input )
        internal object visitObjectDecleration(ObjectDeclaration objectDeclaration, object arg)
        {
            Console.WriteLine("Object Declaration");
            // Get the kind of Object and the spelling of the identifier.
            int kind = (int)objectDeclaration._object.visit(this, arg);
            string ident = (string)objectDeclaration.identifier.visit(this, arg);

            // Puts the kind and spelling into the Identification Table.
            idTable.enter(kind, ident);

            // Visit the input and check the spelling.
            objectDeclaration.input.visit(this, arg);
            return null;
        }

        // Type VarName = becomes...SomethingSomething...
        internal object visitTypeDecleration(TypeDeclaration typeDeclaration, object arg)
        {
            Console.WriteLine("Type Declaration");
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
                            Console.WriteLine("Type declaration has not been declared to a variable of type {0}", masVariable.token.kind.ToString());
                            return null;
                        }
                        break;
                    case (int)Token.keywords.BOOL:
                        if (masVariable.token.kind != (int)Token.keywords.TRUE || masVariable.token.kind != (int)Token.keywords.FALSE)
                        {
                            Console.WriteLine("Type declaration has not been declared to a variable of type {0}", masVariable.token.kind.ToString());
                            return null;
                        }
                        break;
                    case (int)Token.keywords.NUM:
                        if (masVariable.token.kind != (int)Token.keywords.NUMBER)
                        {
                            Console.WriteLine("Type declaration has not been declared to a variable of type {0}", masVariable.token.kind.ToString());
                            return null;
                        }
                        break;
                    default:
                        Console.WriteLine("Type declaration has not been declared as a type");
                        return null;
                }
            }
            if (idTable.retrieve(VarName) == (int)Token.keywords.ERROR)
                idTable.enter(kind, ident);
            else
            {
                Console.WriteLine("Identifier {0} at {1}, {2} has already been declared."
                    , VarName.spelling, VarName.col, VarName.row);
            }
            return null;
        }

        // if ( bool-expression ) block
        // if ( bool-expression ) block else block
        internal object visitIfCommand(IfCommand ifCommand, object arg)
        {
            Console.WriteLine("If Command");
            ifCommand.Expression.visit(this, arg);
            ifCommand.IfBlock.visit(this, arg);
            ifCommand.ElseBlock.visit(this, arg);

            return null;
        }

        // for ( type-declaration ; bool-expression ; math-expression ) block
        internal object visitForCommand(ForCommand forCommand, object arg)
        {
            Console.WriteLine("For Command");
            forCommand.CounterDeclaration.visit(this, arg);
            forCommand.LoopExpression.visit(this, arg);
            forCommand.CounterExpression.visit(this, arg);
            forCommand.ForBlock.visit(this, arg);

            return null;
        }

        // while ( bool-expression ) block
        internal object visitWhileCommand(WhileCommand whileCommand, object arg)
        {
            Console.WriteLine("While Command");
            whileCommand.LoopExpression.visit(this, arg);
            whileCommand.WhileBlock.visit(this, arg);
            return null;
        }

        internal object visitMethodIdentifier(MethodIdentifier methodIdentifier, object arg)
        {
            Console.WriteLine("Method Identifier");
            string ident;

            ident = (string)methodIdentifier.Identifier.visit(this, arg);
            methodIdentifier.NextMethodIdentifier.visit(this, arg);
            throw new NotImplementedException();
        }

        // identifier ( input ) | identifier . method-call
        internal object visitMethodCall(MethodCall methodCall, object arg)
        {
            Console.WriteLine("Method Call");
            methodCall.methodIdentifier.visit(this, arg);
            methodCall.input.visit(this, arg);

            return null;
        }

        // Syntax: number | identifier | expression | ( expression ) | boolean
        internal object visitExpression(Expression expression, object arg)
        {
            Console.WriteLine("Expression");
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
                    Console.WriteLine("Operator {0} at {1}, {2} is not an operator.", 
                        _operator.spelling, _operator.col, _operator.row);
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
                        if (idTable.retrieve(primExpr1) == (int)Token.keywords.BOOL)
                            break;
                        return null;
                    default:
                        Console.WriteLine("Boolean expression {0} at {1}, {2} is not of expression type boolean.", 
                            primExpr1.spelling, primExpr1.col, primExpr1.col);
                        return null;
                }

                switch (primExpr2.kind)
                {
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                        break;
                    case (int)Token.keywords.IDENTIFIER:
                        if (idTable.retrieve(primExpr2) == (int)Token.keywords.BOOL)
                            break;
                        return null;
                    default:
                        Console.WriteLine("Boolean expression {0} at {1}, {2} is not of expression type boolean.",
                            primExpr2.spelling, primExpr2.col, primExpr2.col);
                        return null;
                }
            }
            if (expression.kind == (int)Token.keywords.NUM)
            {
                switch (primExpr1.kind)
                { 
                    case (int)Token.keywords.NUMBER:
                        break;
                    case (int)Token.keywords.IDENTIFIER:
                        if (idTable.retrieve(primExpr1) == (int)Token.keywords.NUMBER)
                            break;
                        return null;
                    default:
                        Console.WriteLine("Mathematic expression {0} at {1}, {2} is not of expression type mathematic.",
                            primExpr1.spelling, primExpr1.col, primExpr1.col);
                        return null;
                }

                switch (primExpr2.kind)
                {
                    case (int)Token.keywords.NUMBER:
                        break;
                    case (int)Token.keywords.IDENTIFIER:
                        if (idTable.retrieve(primExpr2) == (int)Token.keywords.NUMBER)
                            break;
                        return null;
                    default:
                        Console.WriteLine("Mathematic expression {0} at {1}, {2} is not of expression type mathematic.",
                            primExpr2.spelling, primExpr2.col, primExpr2.col);
                        return null;
                }
            }
            return null;
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            Console.WriteLine("Identifier");
            return identifier.token;
        }

        internal object visitOperator(Operator p, object arg)
        {
            Console.WriteLine("Operator");
            return p.token;
        }

        internal object visitInput(Input input, object arg)
        {
            Console.WriteLine("Input");
            throw new NotImplementedException();
        }

        internal object visitMASNumber(MASNumber mASNumber, object arg)
        {
            Console.WriteLine("Number");
            return mASNumber.token;
        }

        internal object visitMASType(MASType mASType, object arg)
        {
            Console.WriteLine("Type");
            return mASType.token;
        }

        internal object visitMASString(MASString mASString, object arg)
        {
            Console.WriteLine("String");
            return mASString.token;
        }

        internal object visitMASBool(MASBool mASBool, object arg)
        {
            Console.WriteLine("Bool");
            return mASBool.token;
        }

        internal object visitObject(Object p, object arg)
        {
            Console.WriteLine("Object");
            return p.token;
        }

        internal object visitMASVariable(MASVariable mASVariable, object arg)
        {
            Console.WriteLine("Variable");
            return mASVariable.token;
        }
    }
}
