using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MultiAgentSystem
{
    class DecorationVisitor : Visitor
    {
        // Exception for catching errors.
        private GrammarException gException = 
            new GrammarException("These errors were found during decoration:");

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

            block.input.visit(this, arg);
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
            if (kind == (int)Token.keywords.TEAM)
            {
                IdentificationTable.enter((int)Token.keywords.STRING, ident + ".color");
            }

            // visit the input and check the spelling.
            if(objectDeclaration.input != null)
                objectDeclaration.input.visit(this, arg);

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
            Token Type = (Token)typeDeclaration.Type.visit(this, arg);
            Token VarName = (Token)typeDeclaration.VarName.visit(this, arg);

            int kind = Type.kind;
            string ident = VarName.spelling;

            object becomes = typeDeclaration.Becomes.visit(this, arg);

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if (Expression.ReferenceEquals(typeDeclaration.Becomes.GetType(),
                new Expression(null, null, null, null).GetType()))
            {
                Expression expression = (Expression)becomes;
                kind = expression.type;
            }
            else
            {
                Token masVariable = (Token)becomes;

                // Checks that the type matches the variable, that the identifier becomes.
                switch (kind)
                {
                    case (int)Token.keywords.STRING:
                        if (masVariable.kind != (int)Token.keywords.ACTUAL_STRING)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER
                                && IdentificationTable.retrieve(masVariable.spelling) != kind)
                            {
                                Printer.ErrorMarker();
                                throwException = true;
                                gException.containedExceptions.Add(
                                    new GrammarException(
                                        "(Line " + masVariable.row + ") " + masVariable.spelling +
                                        " is not valid input for identifier " + ident + ".", masVariable));
                            }
                        }
                        break;
                    case (int)Token.keywords.BOOL:
                        if (masVariable.kind != (int)Token.keywords.TRUE 
                            && masVariable.kind != (int)Token.keywords.FALSE)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER
                                && IdentificationTable.retrieve(masVariable.spelling) != kind)
                            {
                                Printer.ErrorMarker();
                                throwException = true;
                                gException.containedExceptions.Add(
                                    new GrammarException(
                                        "(Line " + masVariable.row + ") " + masVariable.spelling +
                                        " is not valid input for identifier " + ident + ".", masVariable));
                            }
                        }
                        break;
                    case (int)Token.keywords.NUM:
                        if (masVariable.kind != (int)Token.keywords.NUMBER)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER 
                                && IdentificationTable.retrieve(masVariable.spelling) != kind)
                            {
                                Printer.ErrorMarker();
                                throwException = true;
                                gException.containedExceptions.Add(
                                    new GrammarException(
                                        "(Line " + masVariable.row + ") " + masVariable.spelling +
                                        " is not valid input for identifier " + ident + ".", masVariable));
                            }
                        }
                        break;
                    default:
                        Printer.ErrorMarker();
                        throwException = true;
                        gException.containedExceptions.Add(
                            new GrammarException(
                                "(Line " + masVariable.row + ") " + 
                                "The types in the type declaration do not match.", masVariable));
                        break;
                }
            }
            if (IdentificationTable.retrieve(VarName.spelling) == (int)Token.keywords.ERROR)
                IdentificationTable.enter(kind, ident);
            else
            {
                Printer.ErrorMarker();
                throwException = true;
                gException.containedExceptions.Add(
                    new GrammarException(
                        "(Line " + Type.row + ") Identifier " + ident +
                        " has already been declared.", VarName));
            }

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
            Expression expr = (Expression)ifCommand.Expression.visit(this, arg);
            if (expr.type != (int)Token.keywords.BOOL)
            {
                Printer.ErrorMarker();
                throwException = true;
                gException.containedExceptions.Add(
                    new GrammarException(
                        "(Line " + expr.basicToken.row + ")" + 
                        " This expression does not give a boolean value."));
            }

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

            methodCall.linkedIdentifier.visit(this, arg);
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

            // Always a Token of kind number, boolean or identifier.
            Token primExpr1 = (Token)expression.primaryExpression_1.visit(this, arg);

            if (primExpr1.kind == (int)Token.keywords.IDENTIFIER)
            {
                int tempKind = IdentificationTable.retrieve(primExpr1.spelling);

                if (tempKind != (int)Token.keywords.NUM && tempKind != (int)Token.keywords.BOOL)
                {
                    Printer.ErrorMarker();
                    throwException = true;
                    gException.containedExceptions.Add(new GrammarException(
                            "(Line " + primExpr1.row + ") Identifier " + primExpr1.spelling + " is of type "
                            + (Token.keywords)tempKind + " which is illegal in expressions."));
                }
            }

            // Always a Token of kind, operator, if this doesn't exists, 
            // visit the primaryExpression and return null.
            Token _operator = (Token)expression._operator.visit(this, arg);

            // 2nd Primary expression can be both an expression or a token.
            object primExpr2 = expression.primaryExpression_2.visit(this, arg);

            // Check which kind of expression this is, according to the operator.
            switch (_operator.spelling)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                    // If the operator is a mathematic operator,
                    // Save the type as a NUM, since numbers are of type NUMBER.
                    expression.type = (int)Token.keywords.NUM;
                    break;
                case "<":
                case ">":
                case "<=":
                case ">=":
                case "=>":
                case "=<":
                case "==":
                    // If the operator is a boolean operator,
                    // Save the type as BOOL, since boolean types are of type TRUE or FALSE.
                    expression.type = (int)Token.keywords.BOOL;
                    break;
                default:
                    Printer.ErrorMarker();
                    throwException = true;
                    gException.containedExceptions.Add(
                        new GrammarException(
                            "(Line " + _operator.row + ") Failed to identify \"" + 
                            _operator.spelling + "\" as an operator."));
                    break;
            }

            // If this evaluates to true, the primExpr2 is a token and therefor a number, boolean or identifier.
            // primExpr1 is of type Token.
            if (object.ReferenceEquals(primExpr2.GetType(), primExpr1.GetType()))
            {
                int identifier1Kind, identifier2Kind;

                // If primary expression 1 is an identifier, check which type it is.
                if (primExpr1.kind == (int)Token.keywords.IDENTIFIER)
                    identifier1Kind = IdentificationTable.retrieve(primExpr1.spelling);
                else
                    identifier1Kind = primExpr1.kind;

                Token _primExpr2 = (Token)primExpr2;

                // If primary expression 2 is an identifier, check which type it is.
                if (_primExpr2.kind == (int)Token.keywords.IDENTIFIER)
                    identifier2Kind = IdentificationTable.retrieve(_primExpr2.spelling);
                else
                    identifier2Kind = _primExpr2.kind;

                // Ensure primExpr2 matches primExpr1.
                switch (identifier2Kind)
                {
                    case (int)Token.keywords.NUM:
                    case (int)Token.keywords.NUMBER:
                        if (identifier1Kind != (int)Token.keywords.NUM &&
                            identifier1Kind != (int)Token.keywords.NUMBER)
                        {
                            Printer.ErrorMarker();
                            throwException = true;
                            gException.containedExceptions.Add(
                                new GrammarException("(Line " + primExpr1.row +
                                    ") The types in the expression \"" + primExpr1.spelling + " " +
                                    _operator.spelling + " " + _primExpr2.spelling + "\" do not match."));
                        }
                        break;
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                        if (identifier1Kind != (int)Token.keywords.FALSE &&
                            identifier1Kind != (int)Token.keywords.TRUE)
                        {
                            Printer.ErrorMarker();
                            throwException = true;
                            gException.containedExceptions.Add(
                                new GrammarException("(Line " + primExpr1.row +
                                    ") The types in the expression \"" + primExpr1.spelling + " " +
                                    _operator.spelling + " " + _primExpr2.spelling + "\" do not match."));
                        }
                        break;
                    default:
                        Printer.ErrorMarker();
                        throwException = true;
                            gException.containedExceptions.Add(new GrammarException(
                                "(Line " + primExpr1.row + ") Identifier " + 
                                _primExpr2.spelling + " is of type " + (Token.keywords)identifier2Kind +
                                " which is illegal in expressions."));
                        if (identifier1Kind != identifier2Kind)
                        {
                            Printer.ErrorMarker();
                            gException.containedExceptions.Add(
                                new GrammarException("(Line " + primExpr1.row +
                                    ") The types in the expression \"" + primExpr1.spelling + " " +
                                    _operator.spelling + " " + _primExpr2.spelling + "\" do not match."));
                        }
                        break;
                }
            }

            // If primExpr2 is not a token, it must be an expression and will be checked when visited
            else
            {
                Expression _primExpr2 = (Expression)primExpr2;

                int identifierKind;
                // If primary expression 1 is an identifier, check which type it is.
                if (primExpr1.kind == (int)Token.keywords.IDENTIFIER)
                    identifierKind = IdentificationTable.retrieve(primExpr1.spelling);
                else
                    identifierKind = primExpr1.kind;

                // If the expression is a mathematic expression, there should be no true/false expressions.
                if (expression.type == (int)Token.keywords.NUM)
                    if (identifierKind == (int)Token.keywords.TRUE || 
                        identifierKind == (int)Token.keywords.FALSE)
                    {
                        Printer.ErrorMarker();
                        throwException = true;
                        gException.containedExceptions.Add(
                            new GrammarException("(Line " + primExpr1.row +
                                ") The types in the expression " + primExpr1.spelling + " " +
                                _primExpr2._operator.token.spelling + "... do not match."));
                    }

                // If any sub-expression is bool, the entire expression is bool
                // and should only have one boolean operator.
                if (_primExpr2.type == (int)Token.keywords.BOOL)
                {
                    if (expression.type == (int)Token.keywords.BOOL)
                    {
                        Printer.ErrorMarker();
                        throwException = true;
                        gException.containedExceptions.Add(
                            new GrammarException("(Line " + primExpr1.row +
                                ") The types in the expression " + primExpr1.spelling + " " +
                                _primExpr2._operator.token.spelling + "... do not match."));
                    }

                    // If any of the expressions are of type boolean
                    // the entire expression is boolean and is set as boolean.
                    expression.type = (int)Token.keywords.BOOL;
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

            Token firstVar;
            object nextVar;

            if (input.firstVar != null)
            {
                firstVar = (Token)input.firstVar.visit(this, arg);

                if (TypeDeclaration.ReferenceEquals(input.firstVar.GetType(),
                new TypeDeclaration().GetType()))
                {
                    TypeDeclaration typeD = (TypeDeclaration)input.firstVar;
                    firstVar = typeD.VarName.token;
                }

                if (firstVar.kind == (int)Token.keywords.IDENTIFIER)
                {
                    if (IdentificationTable.retrieve(firstVar.spelling) == (int)Token.keywords.ERROR)
                    {
                        Printer.ErrorMarker();
                        throwException = true;
                        gException.containedExceptions.Add(
                            new GrammarException("(Line " + firstVar.row +
                                ") The variable " + firstVar.spelling + "is undeclared."));
                    }
                }
                if (input.nextVar != null)
                {
                    Printer.Collapse();
                    nextVar = (Input)input.nextVar.visit(this, arg);
                    Printer.Expand();
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

            int kind;
            string ident = (string)assignCommand.ident.visit(this, arg);
            object becomes = assignCommand.becomes.visit(this, arg);

            kind = IdentificationTable.retrieve(ident);

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if (Expression.Equals(becomes.GetType(), new Expression(null, null, null, null).GetType()))
            {
                Expression expression = (Expression)becomes;
                kind = expression.type;
            }
            else
            {
                Token masVariable = (Token)becomes;

                switch (kind)
                {
                    case (int)Token.keywords.STRING:
                        if (masVariable.kind != (int)Token.keywords.ACTUAL_STRING)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER && 
                                IdentificationTable.retrieve(masVariable.spelling) != kind)
                            {
                                Printer.ErrorMarker();
                                throwException = true;
                                gException.containedExceptions.Add(
                                    new GrammarException("(Line " + masVariable.row +
                                        ") The types in the assignment of " +
                                        ident + " do not match."));
                            }
                        }
                        break;
                    case (int)Token.keywords.BOOL:
                        if (masVariable.kind != (int)Token.keywords.TRUE || 
                            masVariable.kind != (int)Token.keywords.FALSE)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER &&
                                IdentificationTable.retrieve(masVariable.spelling) != kind)
                            {
                                Printer.ErrorMarker();
                                throwException = true;
                                gException.containedExceptions.Add(
                                    new GrammarException("(Line " + masVariable.row +
                                        ") The types in the assignment of " +
                                        ident + " do not match."));
                            }
                        }
                        break;
                    case (int)Token.keywords.NUM:
                        if (masVariable.kind != (int)Token.keywords.NUMBER)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER && 
                                IdentificationTable.retrieve(masVariable.spelling) != kind)
                            {
                                Printer.ErrorMarker();
                                throwException = true;
                                gException.containedExceptions.Add(
                                    new GrammarException("(Line " + masVariable.row +
                                        ") The types in the assignment of " +
                                        ident + " do not match."));
                            }
                        }
                        break;
                    default:
                        Printer.ErrorMarker();
                        throwException = true;
                        gException.containedExceptions.Add(
                            new GrammarException("(Line " + masVariable.row +
                                ") The types in the assignment of " + ident + " do not match."));
                        break;
                }
            }

            Printer.Collapse();
            return null;
        }

        private LinkedIdentifier CreateLinkedIdentifier(Token t)
        {
            LinkedIdentifier link = new LinkedIdentifier();
            link.Identifier = new Identifier(t);
            return link;
        }
    }
}