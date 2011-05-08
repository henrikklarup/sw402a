using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MultiAgentSystem
{
    public class IntermediateVisitor : Visitor
    {
        // Exception for catching errors.
        private GrammarException gException =
            new GrammarException("These inputs and methods were illegal:");

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

            // Valid input for the mainblock:
            Input ValidInput = new Input();
            ValidInput.firstVar = new Identifier(new Token((int)Token.keywords.NUMBER, "", -1, -1));

            block.input.visit(this, ValidInput);
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
                gException.containedExceptions.Add(new GrammarException(
                        "heu - object declaration"));
                GenerateError();
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

            Token identifier = (Token)methodCall.linkedIdentifier.visit(this, arg);

            /* To test the method, a list of overloads is found that matches the method,
             * and a valid input is derived from the best matching overload. 
             * This input is then tested against what was given in the method. */

            // Find the name of the method called.
            string[] names = identifier.spelling.Split('.');
            string method = names[names.Length - 1];

            // Name of the object the method is called on, and its kind.
            string name = identifier.spelling.Remove(identifier.spelling.Length - (method.Length + 1));
            int kind = IdentificationTable.retrieve(name);

            // The input to test against.
            Input ValidInput = null;

            // The list of overloads for the method.
            List<MASMethod> Methods = MASLibrary.FindMethod(method, kind);

            if (Methods.Count < 1)
            {
                // If no methods were found, the method name isn't correct.
                gException.containedExceptions.Add(new GrammarException(
                    GenerateError(identifier.row, "'" + method + "' is not a valid method.")));
            }
            else if (Methods.Count == 1)
            {
                // If only one method is found, use its valid input to test against.
                ValidInput = Methods.ElementAt(0).ValidInput;
            }
            else
            {
                // If several overloads are found, use OverloadVisit to find the best match for the input.
                ValidInput = methodCall.input.OverloadVisit(this, new List<Input>(), identifier.row);
            }

            // Visit the input if it exists.
            if (methodCall.input != null)
            {
                methodCall.input.visit(this, ValidInput);
            }
            else if (methodCall.input == null && ValidInput != null)
            {
                // If the two are different, but the input doesn't exist, give an error.
                gException.containedExceptions.Add(new GrammarException(
                    Methods.ElementAt(0).PrintInvalidErrorMessage(identifier.row)));
                GenerateError();
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

            /*  */

            // The current valid input is reset.
            Input currentValidInput = new Input();

            if (arg != null)
            {
                // If a new input was given, use it.
                currentValidInput = (Input)arg;
            }

            Token firstVar, dummyVar;

            if (input.firstVar != null)
            {
                // If the next input exists, visit it with the next valid input as arg.
                if (input.nextVar != null)
                {
                    input.nextVar.visit(this, currentValidInput.nextVar);
                }

                firstVar = (Token)input.firstVar.visit(this, arg);
                /* If firstVar turns out to be an identifier, 
                 * look it up in the ID table to get the real kind. */
                if (firstVar.kind == (int)Token.keywords.IDENTIFIER)
                {
                    firstVar.kind = IdentificationTable.retrieve(firstVar.spelling);
                }

                if (currentValidInput.firstVar != null)
                {
                    dummyVar = (Token)currentValidInput.firstVar.visit(this, arg);
                    /* So far in the proces, the valid input matches the given input
                     * and if the kinds match too, then they match perfectly. If not, give an error. */
                    if (firstVar.kind != dummyVar.kind)
                    {
                        gException.containedExceptions.Add(new GrammarException(
                            /*PrintErrorMessage(firstVar.row)*/firstVar.row + "iubhuhy"));
                        throwException = true;
                        Printer.ErrorMarker();
                    }
                }
                // If the given input is different from null, but the valid input is not, give error.
                else
                {
                    gException.containedExceptions.Add(new GrammarException(
                        /*PrintErrorMessage(firstVar.row)*/firstVar.row + "iubhuhy"));
                    throwException = true;
                    Printer.ErrorMarker();
                }
            }

            Printer.Collapse();
            return null;
        }

        internal override Input visitOverload(Input input, List<Input> arg, int line)
        {
            Token firstVar, dummyVar;

            if (input.firstVar != null)
            {
                firstVar = (Token)input.firstVar.visit(this, arg);
                if (firstVar.kind == (int)Token.keywords.IDENTIFIER)
                {
                    firstVar.kind = IdentificationTable.retrieve(firstVar.spelling);
                }

                for(int i = 0; i < arg.Count; i++)
                {
                    dummyVar = (Token)arg.ElementAt(i).firstVar.visit(this, arg);
                    if (dummyVar == null || firstVar.kind != dummyVar.kind)
                    {
                        arg.RemoveAt(i);
                    }
                }
            }
            else if (input.firstVar == null)
            {
                for(int i = 0; i < arg.Count; i++)
                {
                    if (arg.ElementAt(i) != null)
                    {
                        arg.RemoveAt(i);
                    }
                }
            }

            if (input.nextVar != null)
            {
                input.nextVar.visit(this, arg);
            }

            if (arg.Count != 1)
            {
                gException.containedExceptions.Add(new GrammarException(
                    GenerateError(line, "No correct overload was found for this method.")));
                return null;
            }
            else
            {
                return arg.ElementAt(0);
            }
        }

        internal override object visitLinkedIdentifier(LinkedIdentifier LinkedIdentifier, object arg)
        {
            Printer.WriteLine("Method Identifier");
            Printer.Expand();

            Token identifier;

            identifier = (Token)LinkedIdentifier.Identifier.visit(this, arg);

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