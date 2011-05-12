using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MASSIVE
{
    class TypeAndScopeVisitor : Visitor
    {
        // Exception for catching errors.
        private GrammarException gException = 
            new GrammarException("These errors were found during SCOPE AND TYPE CHECKING:");

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
            string ident = (string)objectDeclaration.identifier.visit(this, arg);

            int kind = _object.kind;

            // Puts the kind and spelling into the Identification Table.
            IdentificationTable.enter(kind, ident);

            List<MASConstructor> builder = MASLibrary.FindConstructor(kind);
            if (builder != null)
            {
                builder.ElementAt(0).InstantiateProperties(ident);
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
            Token objectType = (Token)typeDeclaration.Type.visit(this, arg);
            Token VarName = (Token)typeDeclaration.VarName.visit(this, arg);

            int kind = objectType.kind;
            string ident = VarName.spelling;

            object becomes = typeDeclaration.Becomes.visit(this, arg);

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if (Expression.ReferenceEquals(typeDeclaration.Becomes.GetType(),
                new Expression(null).GetType()))
            {
                Expression expression = (Expression)becomes;
                int expressionKind = expression.type.type;

                switch (kind)
                { 
                    case (int)Token.keywords.BOOL:
                        if(expressionKind != (int)Type.types.BOOL)
                        {
                            Printer.ErrorMarker();
                            throwException = true;
                            gException.containedExceptions.Add(
                                new GrammarException("(Line " + objectType.row + ") a " + Enum.GetName(typeof(Type.types), expressionKind) +
                                    " expression is not a valid input for the type " + Enum.GetName(typeof(Token.keywords), kind) + " ."));
                        }
                        break;
                    case (int)Token.keywords.NUM:
                        if(expressionKind != (int)Type.types.NUM)
                        {
                            Printer.ErrorMarker();
                            throwException = true;
                            gException.containedExceptions.Add(
                                new GrammarException("(Line " + objectType.row + ") a " + Enum.GetName(typeof(Type.types), expressionKind) +
                                    " expression is not a valid input for the type " + Enum.GetName(typeof(Token.keywords), kind) + " ."));
                        }
                        break;
                    case (int)Token.keywords.STRING:
                        if(expressionKind != (int)Type.types.STRING)
                        {
                            Printer.ErrorMarker();
                            throwException = true;
                            gException.containedExceptions.Add(
                                new GrammarException("(Line " + objectType.row + ") a " + Enum.GetName(typeof(Type.types), expressionKind) +
                                    " expression is not a valid input for the type " + Enum.GetName(typeof(Token.keywords), kind) + " ."));
                        }
                        break;
                    default:
                        Printer.ErrorMarker();
                        throwException = true;
                        gException.containedExceptions.Add(
                            new GrammarException("(Line " + objectType.row + ") a " + Enum.GetName(typeof(Type.types), expressionKind) + 
                                " expression is not a valid input for the type " + Enum.GetName(typeof(Token.keywords), kind) + " ."));
                        break;
                }
            }
            else
            {
                #region not expression
                Token masVariable = (Token)becomes;

                // Checks that the type matches the variable, that the identifier becomes.
                switch (kind)
                {
                    case (int)Token.keywords.STRING:
                        if (masVariable.kind != (int)Token.keywords.ACTUAL_STRING)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER
                                && IdentificationTable.retrieve(masVariable.spelling) != (int)Type.types.STRING)
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
                                && IdentificationTable.retrieve(masVariable.spelling) != (int)Type.types.BOOL)
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
                                && IdentificationTable.retrieve(masVariable.spelling) != (int)Type.types.NUM)
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
                #endregion
            }
            if (IdentificationTable.retrieve(VarName.spelling) == (int)Token.keywords.ERROR)
                IdentificationTable.enter(kind, ident);
            else
            {
                Printer.ErrorMarker();
                throwException = true;
                gException.containedExceptions.Add(
                    new GrammarException(
                        "(Line " + objectType.row + ") Identifier " + ident +
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
            if (expr.type.type != (int)Type.types.BOOL)
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

            // If the parent expression is not null, visit it.
            if (expression.parentExpr != null)
            {
                ParentExpression parentExpr =
                    (ParentExpression)expression.parentExpr.visit(this, arg);
                expression.type = parentExpr.type;
            }
            else
            {
                PrimaryExpression primExpr1 = (PrimaryExpression)expression.primExpr1.visit(this, arg);
                Token opr = (Token)expression.opr.visit(this, arg);
                PrimaryExpression primExpr2 = (PrimaryExpression)expression.primExpr2.visit(this, arg);

                switch (opr.spelling)
                {
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        // If the operator is a mathematic operator,
                        // Save the type as a NUM, since numbers are of type NUMBER.
                        expression.type = new Type(Type.types.NULL, Type.types.NUM);
                        break;
                    case "<":
                    case ">":
                    case "<=":
                    case ">=":
                    case "=>":
                    case "=<":
                    case "==":
                    case "!=":
                        // If the operator is a boolean operator,
                        // Save the type as BOOL, since boolean types are of type TRUE or FALSE.
                        expression.type = new Type(Type.types.NULL, Type.types.BOOL);
                        break;
                    default:
                        Printer.ErrorMarker();
                        throwException = true;
                        gException.containedExceptions.Add(
                            new GrammarException(
                                "(Line " + opr.row + ") Failed to identify \"" +
                                opr.spelling + "\" as an operator."));
                        break;
                }

                // If the types are equal this expression should be correct if the operator matches the type.
                if (primExpr1.type.kind == primExpr2.type.kind)
                {
                    // The primary expressions are of equal kind.
                    // Check if the primary expressions matches the operator.
                    switch (primExpr1.type.kind)
                    { 
                        case (int)Type.types.NUM:
                            if (expression.type.type == (int)Type.types.NUM)
                                break;
                            else if (expression.type.type == (int)Type.types.BOOL)
                            {
                                break;
                            }
                            Printer.ErrorMarker();
                            throwException = true;
                            gException.containedExceptions.Add(
                            new GrammarException("(Line " + opr.row +
                                ") The variable expression is invalid."));
                            break;
                        case (int)Type.types.BOOL:
                        case (int)Type.types.STRING:
                            if (expression.type.type != (int)Type.types.BOOL)
                            {
                                Printer.ErrorMarker();
                                throwException = true;
                                gException.containedExceptions.Add(
                                new GrammarException("(Line " + opr.row +
                                    ") The expression is invalid."));
                                break;
                            }
                            break;
                        default:
                            Printer.ErrorMarker();
                            throwException = true;
                            gException.containedExceptions.Add(
                            new GrammarException("(Line " + opr.row +
                                ") The expression is invalid."));
                            break;
                    }

                    expression.type.kind = primExpr1.type.kind;
                }
                else 
                {
                    Printer.ErrorMarker();
                    throwException = true;
                    gException.containedExceptions.Add(
                    new GrammarException("(Line " + opr.row +
                            ") The expression is invalid."));
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
                                ") The variable " + firstVar.spelling + " is undeclared."));
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

            int kind;
            string ident = (string)assignCommand.ident.visit(this, arg);
            object becomes = assignCommand.becomes.visit(this, arg);

            kind = IdentificationTable.retrieve(ident);

            // If the declaration becomes an expression, visit the expression.
            // Else check if it becomes the right type.
            if (Expression.Equals(becomes.GetType(), new Expression(null).GetType()))
            {
                Expression expression = (Expression)becomes;
                kind = expression.type.type;
            }
            else
            {
                Token masVariable = (Token)becomes;

                switch (kind)
                {
                    case (int)Type.types.STRING:
                        if (masVariable.kind != (int)Token.keywords.ACTUAL_STRING)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER && 
                                IdentificationTable.retrieve(masVariable.spelling) != kind)
                            {
                                gException.containedExceptions.Add(
                                    new GrammarException(GenerateError(masVariable.row,
                                        "The types in the assignment of '" + ident + "' do not match.")));
                            }
                        }
                        break;
                    case (int)Type.types.BOOL:
                        if (masVariable.kind != (int)Token.keywords.TRUE || 
                            masVariable.kind != (int)Token.keywords.FALSE)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER &&
                                IdentificationTable.retrieve(masVariable.spelling) != kind)
                            {
                                gException.containedExceptions.Add(
                                    new GrammarException(GenerateError(masVariable.row, 
                                        "The types in the assignment of '" + ident + "' do not match.")));
                            }
                        }
                        break;
                    case (int)Type.types.NUM:
                        if (masVariable.kind != (int)Token.keywords.NUMBER)
                        {
                            if (masVariable.kind == (int)Token.keywords.IDENTIFIER && 
                                IdentificationTable.retrieve(masVariable.spelling) != kind)
                            {
                                gException.containedExceptions.Add(
                                    new GrammarException(GenerateError(masVariable.row,
                                        "The types in the assignment of '" + ident + "' do not match.")));
                            }
                        }
                        break;
                    default:
                        gException.containedExceptions.Add(
                            new GrammarException(GenerateError(masVariable.row,
                                "The types in the assignment of '" + ident + "' do not match.")));
                        break;
                }
            }

            Printer.Collapse();
            return null;
        }

        internal override object visitPrimaryExpression(PrimaryExpression primaryExpression, object arg)
        {
            // If var is not null, the primary expression is a variable.
            if (primaryExpression.var != null)
            {
                Token var = (Token)primaryExpression.var.visit(this, arg);

                switch (var.kind)
                { 
                    case (int)Token.keywords.NUMBER:
                        primaryExpression.type = new Type(Type.types.NUM);
                        break;
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                        primaryExpression.type = new Type(Type.types.BOOL);
                        break;
                    case (int)Token.keywords.ACTUAL_STRING:
                        primaryExpression.type = new Type(Type.types.STRING);
                        break;
                    case (int)Token.keywords.IDENTIFIER:
                        int kind = IdentificationTable.retrieve(var.spelling);
                        // Make a switch according to which kind the ID table retrieves.
                        switch (kind)
                        { 
                            case (int)Token.keywords.NUM:
                                primaryExpression.type = new Type(Type.types.NUM);
                                break;
                            case (int)Token.keywords.BOOL:
                                primaryExpression.type = new Type(Type.types.BOOL);
                                break;
                            case (int)Token.keywords.STRING:
                                primaryExpression.type = new Type(Type.types.STRING);
                                break;
                        }
                        break;
                }
            }
            // If parentExpression is not null, the primary expression is a parentExpression.
            else if (primaryExpression.parentExpression != null)
            {
                // Set the type to the same type as in its expression.
                ParentExpression parentExpression = 
                    (ParentExpression)primaryExpression.parentExpression.visit(this, arg);
                primaryExpression.type = parentExpression.type;
            }
            // If the expression is not null, the primary expression is a new expression.
            else if (primaryExpression.expression != null)
            {
                // Set the type to the same type as in the expression.
                Expression expr = (Expression)primaryExpression.expression.visit(this, arg);
                primaryExpression.type = expr.type;
            }

            return primaryExpression;
        }

        internal override object visitParentExpression(ParentExpression parentExpression, object arg)
        {
            Expression expr = (Expression)parentExpression.expr.visit(this, arg);
            parentExpression.type = expr.type;

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