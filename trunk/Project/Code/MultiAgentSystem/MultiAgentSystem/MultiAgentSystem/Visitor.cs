using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Visitor
    {
        public IdentificationTable idTable = new IdentificationTable();

        public object visitMainBlock(Mainblock block, object arg)
        {
            visitBlock(block.block, null);
            return null;
        }

        public object visitBlock(Block block, object arg)
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
                    case (int)Token.keywords.FALSE:
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.NUM:
                        if (masVariable.token.kind != kind)
                        { 
                            Console.WriteLine("Type declaration has not been declared to a variable of type {0}", masVariable.token.kind.ToString());
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

        internal object visitExpression(Expression expression, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            return identifier.token.spelling;
        }

        internal object visitOperator(Operator p, object arg)
        {
            throw new NotImplementedException();
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

        internal object visitPrimaryExpression(PrimaryExpression primaryExpression, object arg)
        {
            throw new NotImplementedException();
        }
    }
}
