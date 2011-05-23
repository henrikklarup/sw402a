using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MASClassLibrary;

namespace MASSIVE
{
    class VariableVisitor : Visitor
    {
        // Exception for catching errors.
        private GrammarException gException =
            new GrammarException("These warnings and errors were found during VARIABLE CHECKING:");

        private List<team> teams = new List<team>();
        private List<squad> squads = new List<squad>();
        private List<agent> agents = new List<agent>();

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

            if (throwException)
            {
                throw gException;
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
            Printer.WriteLine("Main Block");
            Printer.Expand();

            block.block.visit(this, arg);

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

            // Everytime a block is visited the block opens 
            // a new scope in the identification table.
            IdentificationTable.openScope();
            foreach (Command c in block.commands)
            {
                c.visit(this, arg);
            }
            // When all commands in the block have been visited
            // the scope is closed in the identification table.
            List<GrammarException> list = IdentificationTable.varCloseScope();
            if (list.Count > 0)
            {
                throwException = true;
                gException.containedExceptions.AddRange(list);
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
            string ident = (string)objectDeclaration.identifier.visit(this, arg);

            int kind = _object.kind;

            // Puts the kind and spelling into the Identification Table.
            IdentificationTable.enter(kind, ident, _object.row);

            List<MASConstructor> builder = MASLibrary.FindConstructor(kind);
            if (builder != null)
            {
                builder.ElementAt(0).InstantiateProperties(ident);
            }

            string input = "";

            // visit the input and check the spelling.
            if (objectDeclaration.input != null)
                input = (string)objectDeclaration.input.visit(this, arg);

            string[] inputStrings = input.Split(',');

            if (inputStrings.Length > 2 && kind == (int)Token.keywords.AGENT)
            {
                IdentificationTable.use(ident.Trim());
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
            Token objectType = (Token)typeDeclaration.Type.visit(this, arg);
            Token VarName = (Token)typeDeclaration.VarName.visit(this, arg);

            int kind = objectType.kind;
            string ident = VarName.spelling;

            IdentificationTable.enter(kind, ident, VarName.row);

            typeDeclaration.Becomes.visit(this, arg);

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

            // visit the expression, if the expression isn't boolean, report and error.
            ifCommand.Expression.visit(this, arg);

            // visit the first block.
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

            // Opens a "virtual" for-scope to be able to remove the counterDeclaration.
            IdentificationTable.openScope();

            // visit the declaration, the two expressions and the block.
            forCommand.CounterDeclaration.visit(this, arg);
            forCommand.LoopExpression.visit(this, arg);
            forCommand.CounterExpression.visit(this, arg);

            forCommand.ForBlock.visit(this, arg);

            // Closes the "virtual" for-scope.
            List<GrammarException> list = IdentificationTable.varCloseScope();
            if (list.Count > 0)
            {
                throwException = true;
                gException.containedExceptions.AddRange(list);
            }

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

            whileCommand.LoopExpression.visit(this, arg);
            whileCommand.WhileBlock.visit(this, arg);

            Printer.Collapse();
            return null;
        }


        // identifier ( input ) | identifier . method-call
        internal override object visitMethodCall(MethodCall methodCall, object arg)
        {
            Printer.WriteLine("Method Call");

            string s = (string)methodCall.linkedIdentifier.visit(this, arg);
            string[] names = s.Split('.');
            s = s.Remove(s.Length - names[names.Length - 1].Length - 1);

            IdentificationTable.use(s);

            if (methodCall.input != null)
            {
                methodCall.input.visit(this, arg);
            }

            return null;
        }

        // Syntax: number | identifier | expression | ( expression ) | boolean
        internal override object visitExpression(Expression expression, object arg)
        {
            Printer.WriteLine("Expression");
            Printer.Expand();

            // If the parent expression is not null, visit it.
            if (expression.parentExpr != null)
            {
                expression.parentExpr.visit(this, arg);
            }
            else
            {
                PrimaryExpression p1 = (PrimaryExpression)expression.primExpr1.visit(this, arg);
                expression.opr.visit(this, arg);
                PrimaryExpression p2 = (PrimaryExpression)expression.primExpr2.visit(this, arg);

                if (p1.var != null)
                {
                    IdentificationTable.use(((Token)p1.var.visit(this, arg)).spelling);
                }

                if (p2.var != null)
                {
                    IdentificationTable.use(((Token)p2.var.visit(this, arg)).spelling);
                }
            }

            Printer.Collapse();
            return expression;
        }

        /// <summary>
        /// Visits an Identifier and returns its token.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal override object visitIdentifier(Identifier identifier, object arg)
        {
            Printer.WriteLine("Identifier: " + identifier.token.spelling);
            return identifier.token;
        }

        /// <summary>
        /// Visits an Operator and returns its token.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal override object visitOperator(Operator p, object arg)
        {
            Printer.WriteLine("Operator: " + p.token.spelling);
            return p.token;
        }

        /// <summary>
        /// Visits and input and visits its variables.
        /// The firstVar is always the first variable and read as a token.
        /// The nextVar is an input if there is more than just the next variable,
        /// and a token if this is the last variable in the input.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal override object visitInput(Input input, object arg)
        {
            Printer.WriteLine("Input");
            Printer.Expand();

            string s = (string)arg;

            // If the first input exists, print it.
            if (input.firstVar != null)
            {
                Token firstVar = (Token)input.firstVar.visit(this, arg);
                s += firstVar.spelling;

                IdentificationTable.use(firstVar.spelling);
            }

            // If more input exists, print it.
            if (input.nextVar != null)
            {
                s += ", ";
                Printer.Collapse();
                s = (string)input.nextVar.visit(this, s);
                Printer.Expand();
            }

            Printer.Collapse();
            return s;
        }

        internal override List<Input> visitOverload(Input input, List<Input> arg, int line)
        {
            return null;
        }

        internal override object visitLinkedIdentifier(LinkedIdentifier LinkedIdentifier, object arg)
        {
            Printer.WriteLine("Linked Identifier");
            Printer.Expand();

            Token identifier;
            string ident;

            identifier = (Token)LinkedIdentifier.Identifier.visit(this, arg);
            ident = identifier.spelling;

            if (LinkedIdentifier.NextLinkedIdentifier != null)
            {
                ident += "." + (string)LinkedIdentifier.NextLinkedIdentifier.visit(this, arg);
            }

            Printer.Collapse();
            return ident;
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

        /// <summary>
        /// Visits the assign command, ensures that identifier and the assigned expression has the same type.
        /// </summary>
        /// <param name="assignCommand"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal override object visitAssignCommand(AssignCommand assignCommand, object arg)
        {
            Printer.WriteLine("Assign Command");
            Printer.Expand();

            assignCommand.ident.visit(this, arg);
            assignCommand.becomes.visit(this, arg);

            Printer.Collapse();
            return null;
        }

        internal override object visitPrimaryExpression(PrimaryExpression primaryExpression, object arg)
        {
            // If var is not null, the primary expression is a variable.
            if (primaryExpression.var != null)
            {
                primaryExpression.var.visit(this, arg);
            }
            // If parentExpression is not null, the primary expression is a parentExpression.
            else if (primaryExpression.parentExpression != null)
            {
                // Set the type to the same type as in its expression.
                primaryExpression.parentExpression.visit(this, arg);
            }
            // If the expression is not null, the primary expression is a new expression.
            else if (primaryExpression.expression != null)
            {
                // Set the type to the same type as in the expression.
                primaryExpression.expression.visit(this, arg);
            }

            return primaryExpression;
        }

        internal override object visitParentExpression(ParentExpression parentExpression, object arg)
        {
            parentExpression.expr.visit(this, arg);

            return parentExpression;
        }

        private LinkedIdentifier CreateLinkedIdentifier(Token t)
        {
            LinkedIdentifier link = new LinkedIdentifier();
            link.Identifier = new Identifier(t);
            return link;
        }
    }
}