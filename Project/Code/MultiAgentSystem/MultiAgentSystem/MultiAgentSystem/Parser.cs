using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASSIVE
{
    class Parser
    {
        // Holds the current token being checked.
        private Token currentToken;

        // The list of tokens from the scanner.
        private List<Token> tokenList;

        // Counts the current element in the tokenList.
        private int listCount;

        // Exception for catching errors.
        private GrammarException gException = new GrammarException("These errors were found while PARSING:");
        private bool throwException = false;

        /// <summary>
        /// Creates a new parser.
        /// </summary>
        /// <param name="list">A list of tokens produced by the scanner.</param>
        public Parser(List<Token> tokens)
        {
            tokenList = tokens;
            listCount = 0;
            currentToken = tokenList.ElementAt(listCount);
        }

        /// <summary>
        /// Parse the tokens into an abstract syntax tree.
        /// </summary>
        public AST parse()
        {
            AST mainBlock;

            Printer.WriteLine("Parse");
            Printer.Expand();

            mainBlock = parseMainblock();

            Printer.Collapse();

            if (throwException)
            { throw gException; }

            return mainBlock;
        }

        /// <summary>
        /// Parses a mainblock token, and is executed as the first parse.
        /// </summary>
        private AST parseMainblock()
        {
            Mainblock main;

            Printer.WriteLine("Main");
            Printer.Expand();

            accept(Token.keywords.MAIN);
            accept(Token.keywords.LPAREN);
            Input input = (Input)parseInput();
            accept(Token.keywords.RPAREN);
            main = new Mainblock(parseBlock());
            accept(Token.keywords.EOT);

            main.input = input;

            Printer.Collapse();
            return main;
        }

        /// <summary>
        /// Method for parsing a block.
        /// </summary>
        private AST parseBlock()
        {
            Block block = new Block();

            Printer.WriteLine("Block");
            Printer.Expand();

            accept(Token.keywords.LBRACKET);
            // Accept every command in the block, and then accept the right bracket when it's reached.
            while (currentToken.kind != (int)Token.keywords.RBRACKET && currentToken.kind != (int)Token.keywords.EOT)
            {
                Command c = (Command)parseCommand();
                if (c != null)
                    block.commands.Add(c);
            }
            accept(Token.keywords.RBRACKET);

            Printer.Collapse();
            return block;
        }

        /// <summary>
        /// Method for parsing a command.
        /// </summary>
        private Command parseCommand()
        {
            // Object for holding the command that will be returned.
            Command returnObject = null;

            Printer.WriteLine("Command");
            Printer.Expand();

            switch (currentToken.kind)
            {
                // A command can be an object declaration...
                case (int)Token.keywords.NEW:
                    returnObject = parseObjectDeclaration();
                    accept(Token.keywords.SEMICOLON);
                    break;
                // or an if-sentence...
                case (int)Token.keywords.IF_LOOP:
                    returnObject = parseIfCommand();
                    break;
                // for loop...
                case (int)Token.keywords.FOR_LOOP:
                    returnObject = parseForCommand();
                    break;
                // while loop...
                case (int)Token.keywords.WHILE_LOOP:
                    returnObject = parseWhileCommand();
                    break;
                // type declaration...
                case (int)Token.keywords.STRING:
                case (int)Token.keywords.BOOL:
                    returnObject = (TypeDeclaration)parseTypeDeclaration();
                    accept(Token.keywords.SEMICOLON);
                    break;
                // or expression or num declaration...
                case (int)Token.keywords.NUM:
                    /* If the next token is an operator, this is an expression. 
                     * Else it's a num declaration. */
                    if (tokenList.ElementAt(listCount + 1).kind ==
                        (int)Token.keywords.OPERATOR)
                    {
                        returnObject = (Expression)parseExpression();
                    }
                    else if (currentToken.kind ==
                        (int)Token.keywords.LPAREN)
                    {
                        returnObject = (Expression)parseExpression();
                    }
                    else
                    {
                        returnObject = (TypeDeclaration)parseTypeDeclaration();
                    }
                    accept(Token.keywords.SEMICOLON);
                    break;
                // or an expression, assignment or method call.
                case (int)Token.keywords.IDENTIFIER:
                    /* If the next token is an operator, this is an expression. 
                     * Else it's a method call. */
                    if (tokenList.ElementAt(listCount + 1).kind ==
                        (int)Token.keywords.OPERATOR)
                    {
                        returnObject = (Expression)parseExpression();
                    }
                    else if (tokenList.ElementAt(listCount + 1).kind ==
                    (int)Token.keywords.LPAREN)
                    {
                        returnObject = (ParentExpression)parseParentExpression();
                    }
                    else
                    {
                        LinkedIdentifier linked = (LinkedIdentifier)parseLinkedIdentifier();

                        if (currentToken.kind == (int)Token.keywords.BECOMES)
                        {
                            returnObject = (AssignCommand)parseAssignCommand(linked);
                        }
                        else
                        {
                            returnObject = (MethodCall)parseMethodCall(linked);
                        }
                    }
                    accept(Token.keywords.SEMICOLON);
                    break;
                default:
                    // If no valid command is found, this exception is created:
                    Printer.ErrorMarker();
                    throwException = true;
                    gException.containedExceptions.Add(new GrammarException(
                        "(Line " + currentToken.row + ") Token \"" +
                        currentToken.spelling + "\" (" + (Token.keywords)currentToken.kind +
                        ") is not valid for a command.", currentToken));
                    acceptIt();
                    break;
            }

            Printer.Collapse();
            return returnObject;
        }

        /// <summary>
        /// Method for parsing an object declaration.
        /// </summary>
        private Command parseObjectDeclaration()
        {
            Printer.WriteLine("Object declaration");
            Printer.Expand();

            accept(Token.keywords.NEW);
            Object obj = parseObject();
            LinkedIdentifier linked = parseLinkedIdentifier();
            accept(Token.keywords.LPAREN);
            Input input = (Input)parseInput();
            accept(Token.keywords.RPAREN);

            Printer.Collapse();
            return new ObjectDeclaration(obj, linked, input);
        }

        /// <summary>
        /// Method for parsing an object.
        /// </summary>
        private Object parseObject()
        {
            Object tmpObject = null;

            Printer.WriteLine("Object: " + currentToken.spelling);
            Printer.Expand();

            switch (currentToken.kind)
            {
                    // If the token represents an object, accept it.
                case (int)Token.keywords.TEAM:
                case (int)Token.keywords.AGENT:
                case (int)Token.keywords.SQUAD:
                case (int)Token.keywords.ACTION_PATTERN:
                    tmpObject = new Object(currentToken);
                    acceptIt();
                    break;
                default:
                    // Error message
                    Printer.ErrorMarker();
                    throwException = true;
                    gException.containedExceptions.Add(new GrammarException(
                        "(Line " + currentToken.row + ") Token \"" +
                        currentToken.spelling + "\" (" + (Token.keywords)currentToken.kind +
                        ") is not a valid object.", currentToken));
                    acceptIt();
                    break;
            }

            Printer.Collapse();
            return tmpObject;
        }

        /// <summary>
        /// Method for parsing a type.
        /// </summary>
        private Terminal parseType()
        {
            MASType M = null;

            Printer.WriteLine("Type: " + currentToken.spelling);
            Printer.Expand();

            switch (currentToken.kind)
            {
                case (int)Token.keywords.BOOL:
                case (int)Token.keywords.NUM:
                case (int)Token.keywords.STRING:
                    M = new MASType(currentToken);
                    acceptIt();
                    break;
                default:
                    // Error message
                    Printer.ErrorMarker();
                    throwException = true;
                    gException.containedExceptions.Add(new GrammarException(
                        "(Line " + currentToken.row + ") Token \"" +
                        currentToken.spelling + "\" (" + (Token.keywords)currentToken.kind +
                        ") is not a valid type.", currentToken));
                    acceptIt();
                    break;
            }

            Printer.Collapse();
            return M;
        }

        /// <summary>
        /// Method for parsing an if command.
        /// Syntax: if ( expression ) block (else elseBlock)+
        /// </summary>
        private Command parseIfCommand()
        {
            Printer.Write("If");
            Printer.Expand();

            accept(Token.keywords.IF_LOOP);
            accept(Token.keywords.LPAREN);
            Expression expression = (Expression)parseExpression();
            accept(Token.keywords.RPAREN);
            Block block = (Block)parseBlock();
            // Check for an else-statement and react accordingly.
            switch (currentToken.kind)
            {
                case (int)Token.keywords.ELSE_LOOP:
                    Printer.WriteLine(" else");
                    acceptIt();
                    Block elseBlock = (Block)parseBlock();
                    Printer.Collapse();
                    return new IfCommand(expression, block, elseBlock);
                default:
                    Console.WriteLine();
                    Printer.Collapse();
                    return new IfCommand(expression, block);
            }
        }

        /// <summary>
        /// Method for parsing a for-loop.
        /// Syntax: for ( typeDeclaration ; expression_1 ; expression_2 ) block
        /// </summary>
        private Command parseForCommand()
        {
            Printer.WriteLine("For Command");
            Printer.Expand();

            accept(Token.keywords.FOR_LOOP);
            accept(Token.keywords.LPAREN);
            TypeDeclaration typeDeclaration = (TypeDeclaration)parseTypeDeclaration();
            accept(Token.keywords.SEMICOLON);
            Expression expression_1 = (Expression)parseExpression();
            accept(Token.keywords.SEMICOLON);
            AssignCommand assignCommand = (AssignCommand)parseAssignCommand(null);
            accept(Token.keywords.RPAREN);
            Block block = (Block)parseBlock();

            Printer.Collapse();
            return new ForCommand(typeDeclaration, expression_1, assignCommand, block);
        }

        /// <summary>
        /// Method for parsing a while loop.
        /// Syntax: while ( expression ) block
        /// </summary>
        private Command parseWhileCommand()
        {
            Printer.WriteLine("While Command");
            Printer.Expand();

            accept(Token.keywords.WHILE_LOOP);
            accept(Token.keywords.LPAREN);
            Expression LoopExpression = (Expression)parseExpression();
            accept(Token.keywords.RPAREN);

            Block WhileBlock = (Block)parseBlock();

            Printer.Collapse();
            return new WhileCommand(LoopExpression, WhileBlock);
        }

        /// <summary>
        /// Method for parsing a type declaration.
        /// 
        /// </summary>
        private Command parseTypeDeclaration()
        {
            TypeDeclaration typeDeclaration = new TypeDeclaration();
            
            Printer.WriteLine("Type declaration");
            Printer.Expand();

            typeDeclaration.Type = (MASType)parseType();
            typeDeclaration.VarName = parseIdentifier();
            accept(Token.keywords.BECOMES);

            if (tokenList.ElementAt(listCount + 1).kind == (int)Token.keywords.OPERATOR)
            {
                typeDeclaration.Becomes = (Expression)parseExpression();
            }
            else if (currentToken.kind ==
                    (int)Token.keywords.LPAREN)
            {
                typeDeclaration.Becomes = (ParentExpression)parseParentExpression();
            }
            else
            {
                typeDeclaration.Becomes = (MASVariable)parseVariable();
            }

            Printer.Collapse();
            return typeDeclaration;
        }

        /// <summary>
        /// Method for parsing a variable.
        /// </summary>
        /// <returns></returns>
        private Terminal parseVariable()
        {
            MASVariable masVariable = null;

            Printer.WriteLine("Variable: " + currentToken.spelling);
            Printer.Expand();

            switch (currentToken.kind)
            { 
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                case (int)Token.keywords.ACTUAL_STRING:
                case (int)Token.keywords.NUMBER:
                case (int)Token.keywords.IDENTIFIER:
                    masVariable = new MASVariable(currentToken);
                    acceptIt();
                    break;
                default:
                    Printer.ErrorMarker();
                    throwException = true;
                    gException.containedExceptions.Add(new GrammarException(
                        "(Line " + currentToken.row + ") Token \"" +
                        currentToken.spelling + "\" (" + (Token.keywords)currentToken.kind +
                        ") is not a valid variable.", currentToken));
                    acceptIt();
                    break;
            }

            Printer.Collapse();
            return masVariable;
        }

        /// <summary>
        /// Method for parsing a method call.
        /// </summary>
        private Command parseMethodCall(LinkedIdentifier linked)
        {
            Printer.WriteLine("Method call");
            Printer.Expand();

            // If a linked identifier was recieved as input, use that, else parse a new one.
            LinkedIdentifier LinkedIdentifier;
            if (linked != null)
            {
                LinkedIdentifier = linked;
            }
            else
            {
                LinkedIdentifier = (LinkedIdentifier)parseLinkedIdentifier();
            }
            accept(Token.keywords.LPAREN);
            Input input = (Input)parseInput();
            accept(Token.keywords.RPAREN);

            Printer.Collapse();
            return new MethodCall(LinkedIdentifier, input);
        }

        /// <summary>
        /// Method for parsing a method identifier, which contains the identifiers used to "dot"
        /// forward to the method.
        /// </summary>
        /// <returns></returns>
        private LinkedIdentifier parseLinkedIdentifier()
        {
            LinkedIdentifier Linked = new LinkedIdentifier();
            
            Printer.WriteLine("Linked Identifier");
            Printer.Expand();

            Linked.Identifier = parseIdentifier();
            if (currentToken.kind == (int)Token.keywords.PUNCTUATION)
            {
                acceptIt();
                Linked.NextLinkedIdentifier = parseLinkedIdentifier();
            }

            Printer.Collapse();
            return Linked;
        }

        /// <summary>
        /// Method for assigning an identifier.
        /// Syntax: identifier becomes variable | identifier becomes expression
        /// </summary>
        /// <returns></returns>
        private Command parseAssignCommand(LinkedIdentifier linked)
        {
            Printer.WriteLine("Assign Command");
            Printer.Expand();

            LinkedIdentifier ident = null;
            AST becomes;

            // If a linked identifier was recieved as input, use that, else parse a new one.
            if (linked != null)
            {
                ident = linked;
            }
            else
            {
                ident = (LinkedIdentifier)parseLinkedIdentifier();
            }
            accept(Token.keywords.BECOMES);

            if (tokenList.ElementAt(listCount + 1).kind == (int)Token.keywords.OPERATOR)
            {
                becomes = parseExpression();
            }
            else if (currentToken.kind ==
                    (int)Token.keywords.LPAREN)
            {
                becomes = (ParentExpression)parseParentExpression();
            }
            else
            {
                becomes = parseVariable();
            }

            Printer.Collapse();
            return new AssignCommand(ident, becomes);
        }

        /// <summary>
        /// Method for parsing an expression.
        /// Syntax: primary-expression operator primary-expression
        /// </summary>
        private Expression parseExpression()
        {
            Printer.WriteLine("Expression");
            Printer.Expand();

            PrimaryExpression primExpr1;

            // Parse the first Expression.
            primExpr1 = parsePrimaryExpression();

            return parseExpression(primExpr1);
        }

        /// <summary>
        /// Extended method for parsing an expression
        /// </summary>
        /// <param name="primExpr1">Any Primary Expression</param>
        private Expression parseExpression(PrimaryExpression primExpr1)
        {
            Printer.WriteLine("Expression");
            Printer.Expand();

            Operator _operator;
            PrimaryExpression primExpr2;

            // Parse the operator.
            _operator = parseOperator();

            primExpr2 = parsePrimaryExpression();

            if (currentToken.kind == (int)Token.keywords.OPERATOR)
            {
                Expression expr = parseExpression(primExpr2);
                primExpr2 = new PrimaryExpression(expr);
            }

            Printer.Collapse();
            return new Expression(primExpr1, _operator, primExpr2);

        }

        private ParentExpression parseParentExpression()
        {
            Printer.WriteLine("Parent Expression");
            Printer.Expand();

            Expression expr;
            accept(Token.keywords.LPAREN);
            expr = parseExpression();
            accept(Token.keywords.RPAREN);

            Printer.Collapse();
            return new ParentExpression(expr);
        }

        /// <summary>
        /// Method for parsing a Primary Expression
        /// Syntax: number | identifier | expression | ( expression ) | boolean
        /// </summary>
        /// <returns></returns>
        private PrimaryExpression parsePrimaryExpression()
        {            
            Printer.WriteLine("Primary Expression");
            Printer.Expand();

            // Check if it's a number, boolean, parent expression or identifier.
            switch (currentToken.kind)
            { 
                case (int)Token.keywords.NUMBER:
                    MASNumber masnumber = parseMASNumber();
                    Printer.Collapse();
                    return new PrimaryExpression(masnumber);
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                    MASBool masbool = parseMASBool();
                    Printer.Collapse();
                    return new PrimaryExpression(masbool);
                case (int)Token.keywords.IDENTIFIER:
                    Identifier ident = parseIdentifier();
                    Printer.Collapse();
                    return new PrimaryExpression(ident);
                case (int)Token.keywords.ACTUAL_STRING:
                    MASString masstring = parseMASString();
                    Printer.Collapse();
                    return new PrimaryExpression(masstring);
                case (int)Token.keywords.LPAREN:
                    ParentExpression parentExpr = parseParentExpression();
                    Printer.Collapse();
                    return new PrimaryExpression(parentExpr);
                default:
                    Printer.ErrorMarker();
                    throwException = true;
                    gException.containedExceptions.Add(new GrammarException(
                        "(Line " + currentToken.row + ") Token \"" +
                        currentToken.spelling + "\" (" + (Token.keywords)currentToken.kind +
                        ") is not valid in an expression."
                        , currentToken));
                    acceptIt();
                    break;
            }

            return null;
        }

        /// <summary>
        /// Method for parsing an identifier.
        /// </summary>
        private Identifier parseIdentifier()
        {
            Printer.WriteLine("Identifier: " + currentToken.spelling);

            Identifier id = new Identifier(currentToken);
            accept(Token.keywords.IDENTIFIER);

            return id;
        }

        /// <summary>
        /// Method for parsing an operator.
        /// </summary>
        private Operator parseOperator()
        {
            Printer.WriteLine("Operator: " + currentToken.spelling);
            Printer.Expand();

            Operator O = new Operator(currentToken);
            accept(Token.keywords.OPERATOR);

            Printer.Collapse();
            return O;
        }

        /// <summary>
        /// Method for parsing method input.
        /// Syntax: (variable | identifier (, variable | , identifier)* )+
        /// </summary>
        private AST parseInput()
        {
            Input input = new Input();
            
            Printer.WriteLine("Input");
            Printer.Expand();

            switch (currentToken.kind)
            { 
                    // If the first part is a variable parse it as a variable.
                case (int)Token.keywords.NUMBER:
                case (int)Token.keywords.ACTUAL_STRING:
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                    input.firstVar = (MASVariable)parseVariable();
                    break;
                case (int)Token.keywords.IDENTIFIER:
                    input.firstVar = (Identifier)parseIdentifier();
                    break;
                case (int)Token.keywords.NUM:
                case (int)Token.keywords.STRING:
                case (int)Token.keywords.BOOL:
                    input.firstVar = (TypeDeclaration)parseTypeDeclaration();
                    break;
                    // If the current token is a right parenthesis then return.
                default:
                    Printer.Collapse();
                    return null;
            }
            if (currentToken.kind == (int)Token.keywords.COMMA)
            {
                acceptIt();
                input.nextVar = (Input)parseInput();
            }

            Printer.Collapse();
            return input;
        }

        /// <summary>
        /// Method for parsing a number.
        /// </summary>
        /// <returns></returns>
        private MASNumber parseMASNumber()
        { 
            Printer.WriteLine("Number: " + currentToken.spelling);

            MASNumber num = new MASNumber(currentToken);
            accept(Token.keywords.NUMBER);

            return num;
        }

        /// <summary>
        /// Method for parsing a number.
        /// </summary>
        /// <returns></returns>
        private MASString parseMASString()
        {
            Printer.WriteLine("String: " + currentToken.spelling);

            MASString masstring = new MASString(currentToken);
            accept(Token.keywords.ACTUAL_STRING);

            return masstring;
        }

        /// <summary>
        /// Method for parsing a boolean value.
        /// </summary>
        /// <returns></returns>
        private MASBool parseMASBool()
        {
            Printer.WriteLine("Boolean: " + currentToken.spelling);

            MASBool _bool = new MASBool(currentToken);
            if (currentToken.kind != (int)Token.keywords.TRUE &&
                currentToken.kind != (int)Token.keywords.FALSE)
            {
                Printer.ErrorMarker();
                throwException = true;
                gException.containedExceptions.Add(new GrammarException(
                    "(Line " + currentToken.row + ") Token \"" +
                        currentToken.spelling + "\" (" + (Token.keywords)currentToken.kind +
                        ") is not a boolean value.", currentToken));
            }
            acceptIt();

            return _bool;
        }

        /// <summary>
        /// Checks if the kind of the current token matches the expected value, 
        /// it prints an error message is that is not the case.
        /// </summary>
        /// <param name="kind">The token kind to check against</param>
        private void accept(Token.keywords kind)
        {
            if ((int)kind != currentToken.kind)
            {
                Printer.ErrorMarker();
                throwException = true;
                gException.containedExceptions.Add(new GrammarException(
                    "(Line " + currentToken.row + ") Token \"" + currentToken.spelling + "\" (" + 
                    (Token.keywords)currentToken.kind + ") was not legal. \n  A token of kind " + kind +
                    " was expected.", currentToken));
            }

            if (currentToken.kind != (int)Token.keywords.EOT)
            {
                acceptIt();
            }
        }

        /// <summary>
        /// Accepts the current token and updates it to take the next token.
        /// </summary>
        private void acceptIt()
        {
            if (1==1)
            {
                listCount++;
                currentToken = tokenList.ElementAt(listCount);
            }
        }
    }
}
