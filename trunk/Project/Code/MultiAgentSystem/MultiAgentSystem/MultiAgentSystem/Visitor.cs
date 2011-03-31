using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Visitor
    {
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

        internal object visitObjectDecleration(ObjectDeclaration objectDeclaration, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitTypeDecleration(TypeDeclaration typeDeclaration, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitIfCommand(IfCommand ifCommand, object arg)
        {
            Type eType = (Type)ifCommand.Expression.visit(this, null);
            if (!eType.equals(Type._bool))
            {
                Console.WriteLine("IfCommand: The expression is not a boolean.");
            }
            ifCommand.IfBlock.visit(this, null);
            ifCommand.ElseBlock.visit(this, null);
            return null;
        }

        internal object visitForCommand(ForCommand forCommand, object arg)
        {
            forCommand.CounterDeclaration.visit(this, null);
            Type e1Type = (Type)forCommand.LoopExpression.visit(this, null);
            if (!e1Type.equals(Type._bool))
            {
                Console.WriteLine("ForCommand: The first expression is not a boolean");
            }

            Type e2Type = (Type)forCommand.CounterExpression.visit(this, null);
            forCommand.ForBlock.visit(this, null);
            return null;
        }

        internal object visitWhileCommand(WhileCommand whileCommand, object arg)
        {
            Type eType = (Type)whileCommand.LoopExpression.visit(this, null);
            if (!eType.equals(Type._bool))
            {
                Console.WriteLine("WhileCommand: The expression is not a boolean");
            }
            whileCommand.WhileBlock.visit(this, null);
            return null;
        }

        internal object visitMethodCall(MethodCall methodCall, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitExpression(Expression expression, object arg)
        {
            expression.firstVariable.visit(this, null);
            expression.secondVariable.visit(this, null);
            expression.firstNumber.visit(this, null);
            expression.secondNumber.visit(this, null);
            expression.Operator.visit(this, null);
            expression.innerExpression.visit(this, null);

            throw new NotImplementedException();
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitOperator(Operator p, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitInput(Input input, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMASBool(MASBool mASBool, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMASString(MASString mASString, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMASNumber(MASNumber mASNumber, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMASType(MASType mASType, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMethodIdentifier(MethodIdentifier methodIdentifier, object arg)
        {
            throw new NotImplementedException();
        }
    }
}
