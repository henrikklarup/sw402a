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
            block.block.visit(this, arg);
            return null;
        }

        internal object visitBlock(Block block, object arg)
        {
            foreach (Command c in block.commands)
            { 
                c.visit(this, null);
            }
            return null;
        }

        // new object identifier ( input )
        internal object visitObjectDecleration(ObjectDeclaration objectDeclaration, object arg)
        {
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
            // Stores the type and the identifier of the declaration
            int kind = (int)typeDeclaration.Type.visit(this, arg);
            string ident = (string)typeDeclaration.VarName.visit(this, arg);
            
            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if((Expression)typeDeclaration.Becomes != null)
            {
                Expression expression = (Expression)typeDeclaration.Becomes;
                expression.visit(this, arg);
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

            idTable.enter(kind, ident);
            return null;
        }

        // if ( bool-expression ) block
        // if ( bool-expression ) block else block
        internal object visitIfCommand(IfCommand ifCommand, object arg)
        {
            ifCommand.Expression.visit(this, arg);
            ifCommand.IfBlock.visit(this, arg);
            ifCommand.ElseBlock.visit(this, arg);

            return null;
        }

        // for ( type-declaration ; bool-expression ; math-expression ) block
        internal object visitForCommand(ForCommand forCommand, object arg)
        {
            forCommand.CounterDeclaration.visit(this, arg);
            forCommand.LoopExpression.visit(this, arg);
            forCommand.CounterExpression.visit(this, arg);
            forCommand.ForBlock.visit(this, arg);

            return null;
        }

        // while ( bool-expression ) block
        internal object visitWhileCommand(WhileCommand whileCommand, object arg)
        {
            whileCommand.LoopExpression.visit(this, arg);
            whileCommand.WhileBlock.visit(this, arg);
            return null;
        }

        internal object visitMethodIdentifier(MethodIdentifier methodIdentifier, object arg)
        {
            string ident;

            ident = (string)methodIdentifier.Identifier.visit(this, arg);
            methodIdentifier.NextMethodIdentifier.visit(this, arg);
            throw new NotImplementedException();
        }

        // identifier ( input ) | identifier . method-call
        internal object visitMethodCall(MethodCall methodCall, object arg)
        {
            methodCall.methodIdentifier.visit(this, arg);
            methodCall.input.visit(this, arg);

            return null;
        }

        // Syntax: number | identifier | expression | ( expression ) | boolean
        internal object visitExpression(Expression expression, object arg)
        {
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
                    expression.type = Type._bool;
                    break;
                case "+":
                case "-":
                case "/":
                case "*":
                    expression.type = Type._num;
                    break;
                default:
                    Console.WriteLine("Operator {0} at {1}, {2} is not an operator.", _operator.spelling, _operator.col, _operator.row);
                    return null;
            }

            if(expression.type == Type._bool)
            {
                switch (primExpr1.kind)
                { 
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                        break;
                    case (int)Token.keywords.IDENTIFIER:
                        if (idTable.retrieve(primExpr1.spelling) == (int)Type.types.BOOL)
                            break;
                        else
                            return null;
                }
            }

            return null;
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            return identifier.token.spelling;
        }

        internal object visitOperator(Operator p, object arg)
        {
            return p.token;
        }

        internal object visitInput(Input input, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMASNumber(MASNumber mASNumber, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMASType(MASType mASType, object arg)
        {
            return mASType.token.kind;
        }

        internal object visitMASString(MASString mASString, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMASBool(MASBool mASBool, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitObject(Object p, object arg)
        {
            return p.token.kind;
        }

        internal object visitMASVariable(MASVariable mASVariable, object arg)
        {
            throw new NotImplementedException();
        }
    }
}
