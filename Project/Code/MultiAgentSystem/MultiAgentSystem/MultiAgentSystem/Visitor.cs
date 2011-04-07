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
            int kind = (int)typeDeclaration.Type.visit(this, arg);
            string ident = (string)typeDeclaration.VarName.visit(this, arg);

            switch (kind)
            { 
                case (int)Token.keywords.IDENTIFIER:
                    typeDeclaration.becomesIdentifier.visit(this, arg);
                    return null;
                case (int)Token.keywords.NUM:
                    typeDeclaration.becomesNumber.visit(this, arg);
                    return null;
                case (int)Token.keywords.STRING:
                    typeDeclaration.becomesString.visit(this, arg);
                    return null;
                case (int)Token.keywords.FALSE:
                case (int)Token.keywords.TRUE:
                    typeDeclaration.becomesBool.visit(this, arg);
                    return null;
                default:
                    break;
            }
            return null;
        }

        internal object visitIfCommand(IfCommand ifCommand, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitForCommand(ForCommand forCommand, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitWhileCommand(WhileCommand whileCommand, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMethodIdentifier(MethodIdentifier methodIdentifier, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMethodCall(MethodCall methodCall, object arg)
        {
            throw new NotImplementedException();
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
    }
}
