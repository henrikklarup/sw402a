using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MultiAgentSystem
{
    class IntermediateVisitor : Visitor
    {
        // Exception for catching errors.
        private GrammarException gException =
            new GrammarException("These inputs and methods were illegal:");
        private bool throwException = false;

        private MASNumber dummyNumber = new MASNumber(
            new Token((int)Token.keywords.NUMBER, "", -1, -1));
        private MASString dummyString1 = new MASString(
            new Token((int)Token.keywords.ACTUAL_STRING, "", -1, -1));
        private MASString dummyString2 = new MASString(
            new Token((int)Token.keywords.ACTUAL_STRING, "", -1, -1));
        private Identifier dummyAgent = new Identifier(
            new Token((int)Token.keywords.AGENT, "", -1, -1));

        private Input dummyInput = new Input();

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

            dummyNumber.token.spelling = "points allocated";
            dummyInput.firstVar = dummyNumber;

            block.input.visit(this, dummyInput);
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
            IdentificationTable.closeScope();

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
            Token identifier = (Token)objectDeclaration.identifier.visit(this, arg);

            int kind = _object.kind;
            string ident = identifier.spelling;

            // Puts the kind and spelling into the Identification Table.
            IdentificationTable.enter(kind, ident);
            IdentificationTable.enter((int)Token.keywords.STRING, ident + ".name");

            dummyInput = new Input();

            switch (kind)
            {
                case (int)Token.keywords.AGENT:
                    dummyNumber.token.spelling = "rank";
                    dummyString1.token.spelling = "name";

                    dummyInput.firstVar = dummyString1;
                    dummyInput.nextVar = new Input();
                    dummyInput.nextVar.firstVar = dummyNumber;
                    break;
                case (int)Token.keywords.TEAM:
                    IdentificationTable.enter((int)Token.keywords.STRING, ident + ".color");

                    dummyString1.token.spelling = "name";
                    dummyString2.token.spelling = "color";

                    dummyInput.firstVar = dummyString1;
                    dummyInput.nextVar = new Input();
                    dummyInput.nextVar.Mandatory = false;
                    dummyInput.nextVar.firstVar = dummyString2;
                    break;
                case (int)Token.keywords.SQUAD:
                case (int)Token.keywords.ACTION_PATTERN:
                    dummyString1.token.spelling = "name";

                    dummyInput.firstVar = dummyString1;
                    break;
                default:
                    dummyInput = null;
                    break;
            }

            if (objectDeclaration.input != null)
            {
                objectDeclaration.input.visit(this, dummyInput);
            }
            else if (objectDeclaration.input == null && dummyInput != null)
            {
                throwException = true;
                Printer.ErrorMarker();

                Token temp;
                string errorMessage = "(Line " + _object.row + ") This constructor takes ";
                Input current = dummyInput;
                do
                {
                    temp = (Token)current.firstVar.visit(this, arg);
                    errorMessage += temp.spelling;
                    if (!current.Mandatory)
                    {
                        errorMessage += " (optional)";
                    }
                    errorMessage += ", ";
                    current = current.nextVar;
                }
                while (current != null);
                errorMessage = errorMessage.Remove(errorMessage.Length - 2);
                errorMessage += " as input.";
                gException.containedExceptions.Add(new GrammarException(errorMessage));
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
            typeDeclaration.Type.visit(this, arg);
            typeDeclaration.VarName.visit(this, arg);
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

            IdentificationTable.openScope();

            // visit the declaration, the two expressions and the block.
            forCommand.CounterDeclaration.visit(this, arg);
            forCommand.LoopExpression.visit(this, arg);
            forCommand.CounterExpression.visit(this, arg);

            forCommand.ForBlock.visit(this, arg);

            IdentificationTable.closeScope();

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

            dummyInput = new Input();

            Token identifier = (Token)methodCall.linkedIdentifier.visit(this, arg);

            string method = identifier.spelling.Remove(0, identifier.spelling.Length - 3).ToLower();
            string name = identifier.spelling.Remove(identifier.spelling.Length - 4);

            int kind = IdentificationTable.retrieve(name);

            switch (kind)
            {
                case (int)Token.keywords.SQUAD:
                case (int)Token.keywords.TEAM:
                    if (method != "add")
                    {
                        throwException = true;
                        gException.containedExceptions.Add(new GrammarException("(Line " +
                            identifier.row + ") Only the add-method is valid" +
                            " for action patterns, squads and teams."));
                    }
                    else
                    {
                        dummyAgent.token.spelling = "agent";
                        dummyInput.firstVar = dummyAgent;
                    }
                    break;
                case (int)Token.keywords.ACTION_PATTERN:
                    if (method != "add")
                    {
                        throwException = true;
                        gException.containedExceptions.Add(new GrammarException("(Line " +
                            identifier.row + ") Only the add-method is valid" +
                            " for action patterns, squads and teams."));
                    }
                    else
                    {
                        dummyString1.token.spelling = "action (string)";
                        dummyInput.firstVar = dummyString1;
                    }
                    break;
                default:
                    if (method != "add")
                    {
                        throwException = true;
                        gException.containedExceptions.Add(new GrammarException("(Line " +
                            identifier.row + ") Only the add-method is valid" +
                            " for action patterns, squads and teams."));
                    }
                    dummyInput = null;
                    break;
            }

            if (methodCall.input != null)
            {
                methodCall.input.visit(this, dummyInput);
            }
            else if (methodCall.input == null && dummyInput != null)
            {
                Token temp;
                string errorMessage = "(Line " + identifier.row + ") This method must have an ";
                Input current = dummyInput;
                do
                {
                    temp = (Token)current.firstVar.visit(this, arg);
                    errorMessage += temp.spelling;
                    if (!current.Mandatory)
                    {
                        errorMessage += " (optional)";
                    }
                    errorMessage += ", ";
                    current = current.nextVar;
                }
                while (current != null);
                errorMessage = errorMessage.Remove(errorMessage.Length - 2);
                errorMessage += " as input.";

                gException.containedExceptions.Add(new GrammarException(errorMessage));
                throwException = true;
                Printer.ErrorMarker();
            }
            return null;
        }

        // Syntax: number | identifier | expression | ( expression ) | boolean
        internal override object visitExpression(Expression expression, object arg)
        {
            Printer.WriteLine("Expression");
            Printer.Expand();

            // Always a Token of kind number, boolean or identifier.
            expression.primaryExpression_1.visit(this, arg);

            // Always a Token of kind, operator, if this doesn't exists, 
            // visit the primaryExpression and return null.
            expression._operator.visit(this, arg);

            // 2nd Primary expression can be both an expression or a token.
            expression.primaryExpression_2.visit(this, arg);

            Printer.Collapse();
            return null;
        }

        internal override object visitIdentifier(Identifier identifier, object arg)
        {
            Printer.WriteLine("Identifier: " + identifier.token.spelling);
            return identifier.token;
        }

        internal override object visitOperator(Operator p, object arg)
        {
            Printer.WriteLine("Operator: " + p.token.spelling);
            return p.token;
        }

        internal override object visitInput(Input input, object arg)
        {
            Printer.WriteLine("Input");
            Printer.Expand();

            Input currentDummyInput = new Input();

            if (arg != null)
            {
                currentDummyInput = (Input)arg;
            }

            Token firstVar, dummyVar;

            if (input.firstVar != null)
            {
                if (input.nextVar != null)
                {
                    input.nextVar.visit(this, currentDummyInput.nextVar);
                }

                firstVar = (Token)input.firstVar.visit(this, arg);
                if (firstVar.kind == (int)Token.keywords.IDENTIFIER)
                {
                    firstVar.kind = IdentificationTable.retrieve(firstVar.spelling);
                }

                if (currentDummyInput.firstVar != null)
                {
                    dummyVar = (Token)currentDummyInput.firstVar.visit(this, arg);
                    if (firstVar.kind != dummyVar.kind && currentDummyInput.Mandatory)
                    {
                        Token temp;
                        string errorMessage = "(Line " + firstVar.row + ") ";
                        Input current = input;
                        do
                        {
                            temp = (Token)current.firstVar.visit(this, arg);
                            errorMessage += temp.spelling;
                            if (!current.Mandatory)
                            {
                                errorMessage += " (optional)";
                            }
                            errorMessage += ", ";
                            current = current.nextVar;
                        }
                        while (current != null);
                        errorMessage = errorMessage.Remove(errorMessage.Length - 2);
                        errorMessage += " was not legal input. This is the legal input: ";
                        current = dummyInput;
                        do
                        {
                            temp = (Token)current.firstVar.visit(this, arg);
                            errorMessage += temp.spelling;
                            if (!current.Mandatory)
                            {
                                errorMessage += " (optional)";
                            }
                            errorMessage += ", ";
                            current = current.nextVar;
                        }
                        while (current != null);
                        errorMessage = errorMessage.Remove(errorMessage.Length - 2);
                        errorMessage += ".";

                        gException.containedExceptions.Add(new GrammarException(errorMessage));
                        throwException = true;
                        Printer.ErrorMarker();
                    }
                }
                else
                {
                    string errorMessage = "(Line " + firstVar.row + ") " +
                        "The given input was not legal. This is the legal input: ";
                    Token temp;
                    Input current = dummyInput;
                    do
                    {
                        temp = (Token)current.firstVar.visit(this, arg);
                        errorMessage += temp.spelling;
                        if (!current.Mandatory)
                        {
                            errorMessage += " (optional)";
                        }
                        errorMessage += ", ";
                        current = current.nextVar;
                    }
                    while (current != null);
                    errorMessage = errorMessage.Remove(errorMessage.Length - 2);
                    errorMessage += ".";

                    gException.containedExceptions.Add(new GrammarException(errorMessage));
                    throwException = true;
                    Printer.ErrorMarker();
                }
            }

            Printer.Collapse();
            return null;
        }

        internal override object visitLinkedIdentifier(LinkedIdentifier LinkedIdentifier, object arg)
        {
            Printer.WriteLine("Method Identifier");
            Printer.Expand();

            Token identifier;
            string ident;

            identifier = (Token)LinkedIdentifier.Identifier.visit(this, arg);
            ident = identifier.spelling;

            if (LinkedIdentifier.NextLinkedIdentifier != null)
            {
                identifier.spelling += "." + 
                    ((Token)LinkedIdentifier.NextLinkedIdentifier.visit(this, arg)).spelling;
            }

            Printer.Collapse();
            return identifier;
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

        internal override object visitAssignCommand(AssignCommand assignCommand, object arg)
        {
            Printer.WriteLine("Assign Command");
            Printer.Expand();

            assignCommand.ident.visit(this, arg);
            assignCommand.becomes.visit(this, arg);

            Printer.Collapse();
            return null;
        }
    }
}