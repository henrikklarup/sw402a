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
            visitBlock(block.B, null);
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
            Type eType = (Type)ifCommand.E.visit(this, null);
            if (!eType.equals(Type._bool))
            {
                Console.WriteLine("IfCommand: The expression is not a boolean.");
            }
            ifCommand.B1.visit(this, null);
            ifCommand.B2.visit(this, null);
            return null;
        }

        internal object visitForCommand(ForCommand forCommand, object arg)
        {
            forCommand.TD.visit(this, null);
            Type e1Type = (Type)forCommand.E1.visit(this, null);
            if (!e1Type.equals(Type._bool))
            {
                Console.WriteLine("ForCommand: The first expression is not a boolean");
            }

            Type e2Type = (Type)forCommand.E2.visit(this, null);
            forCommand.B.visit(this, null);
            return null;
        }

        internal object visitWhileCommand(WhileCommand whileCommand, object arg)
        {
            Type eType = (Type)whileCommand.E.visit(this, null);
            if (!eType.equals(Type._bool))
            {
                Console.WriteLine("WhileCommand: The expression is not a boolean");
            }
            whileCommand.B.visit(this, null);
            return null;
        }

        internal object visitMethodCall(MethodCall methodCall, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitExpression(Expression expression, object arg)
        {
            expression.I1.visit(this, null);
            expression.I2.visit(this, null);
            expression.N1.visit(this, null);
            expression.N2.visit(this, null);
            expression.O.visit(this, null);
            expression.E.visit(this, null);

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
