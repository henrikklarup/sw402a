using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class Parser
    {
        // Holds the current token being checked.
        private Token currentToken;

        // The list of tokens from the scanner.
        private List<Token> tokenList;

        // Counts the current element in the tokenList.
        private int listCount;

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
            Printer.WriteLine("Parse");
            Printer.Expand();
            AST mainBlock = parseMainblock();

            Printer.Collapse();
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
            switch(currentToken.kind)
            {
                case (int)Token.keywords.MAIN:
                    acceptIt();
                    accept(Token.keywords.LPAREN);
                    accept(Token.keywords.RPAREN);
                    main = new Mainblock(parseBlock());
                    accept(Token.keywords.EOT);
                    break;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    return null;
            }

            Printer.Collapse();
            return main;
        }

        /// <summary>
        /// Method for parsing a block.
        /// </summary>
        private AST parseBlock()
        {
            Printer.WriteLine("Block");
            Printer.Expand();
            Block block = new Block();
            // parseCommand is run until the end of the block is reached.
            switch (currentToken.kind)
            {
                case (int)Token.keywords.LBRACKET:
                    acceptIt();
                    while (currentToken.kind != (int)Token.keywords.RBRACKET)
                    {
                        Command c = (Command)parseCommand();
                        if (c != null)
                            block.commands.Add(c);
                    }
                    acceptIt();
                    break;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    return null;
            }
            Printer.Collapse();
            return block;
        }

        /// <summary>
        /// Method for parsing a command.
        /// </summary>
        private Command parseCommand()
        {
            Printer.WriteLine("Command");
            Printer.Expand();
            switch (currentToken.kind)
            {
                    // or an object declaration...
                case (int)Token.keywords.NEW:
                    Command objectDeclaration = parseObjectDeclaration();
                    accept(Token.keywords.SEMICOLON);
                    Printer.Collapse();
                    return objectDeclaration;
                    // or an if-sentence...
                case (int)Token.keywords.IF_LOOP:
                    Command ifCommand = parseIfCommand();
                    Printer.Collapse();
                    return ifCommand;
                    // for loop...
                case (int)Token.keywords.FOR_LOOP:
                    Command forCommand = parseForCommand();
                    Printer.Collapse();
                    return forCommand;
                    // while loop...
                case (int)Token.keywords.WHILE_LOOP:
                    Command whileCommand = parseWhileCommand();
                    Printer.Collapse();
                    return whileCommand;
                    // type declaration...
                case (int)Token.keywords.STRING:
                case (int)Token.keywords.BOOL:
                    TypeDeclaration typeDeclaration = (TypeDeclaration)parseTypeDeclaration();
                    accept(Token.keywords.SEMICOLON);
                    Printer.Collapse();
                    return typeDeclaration;
                    // or expression or num declaration...
                case (int)Token.keywords.NUM:
                    /* If the next token is an operator, this is an expression. 
                     * Else it's a num declaration. */
                    if (tokenList.ElementAt(listCount + 1).kind ==
                        (int)Token.keywords.OPERATOR)
                    {
                        Expression expression = (Expression)parseExpression();
                        accept(Token.keywords.SEMICOLON);
                        Printer.Collapse();
                        return expression;
                    }
                    else
                    {
                        TypeDeclaration numDeclaration = (TypeDeclaration)parseTypeDeclaration();
                        accept(Token.keywords.SEMICOLON);
                        Printer.Collapse();
                        return numDeclaration;
                    }
                    // or expression or method call.
                case (int)Token.keywords.IDENTIFIER:
                    /* If the next token is an operator, this is an expression. 
                     * Else it's a method call. */
                    if (tokenList.ElementAt(listCount + 1).kind ==
                        (int)Token.keywords.OPERATOR)
                    {
                        Expression expression = (Expression)parseExpression();
                        accept(Token.keywords.SEMICOLON);
                        Printer.Collapse();
                        return expression;
                    }
                    else if (tokenList.ElementAt(listCount + 1).kind ==
                        (int)Token.keywords.BECOMES)
                    {
                        AssignCommand assignCommand = (AssignCommand)parseAssignCommand();
                        accept(Token.keywords.SEMICOLON);
                        Printer.Collapse();
                        return assignCommand;
                    }
                    else
                    {
                        MethodCall methodCall = (MethodCall)parseMethodCall();
                        accept(Token.keywords.SEMICOLON);
                        Printer.Collapse();
                        return methodCall;
                    }
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    Printer.Collapse();
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing an object declaration.
        /// </summary>
        private Command parseObjectDeclaration()
        {
            Printer.WriteLine("Object declaration");
            Printer.Expand();
            /* As the current token already have been checked,
             * we can just accept it. */
            accept(Token.keywords.NEW);
            Object obj = parseObject();
            Identifier id = parseIdentifier();
            accept(Token.keywords.LPAREN);
            Input input = (Input)parseInput();
            accept(Token.keywords.RPAREN);
            Printer.Collapse();
            return new ObjectDeclaration(obj, id, input);
        }

        /// <summary>
        /// Method for parsing an object.
        /// </summary>
        private Object parseObject()
        {
            Printer.WriteLine("Object: " + currentToken.spelling);
            Printer.Expand();
            switch (currentToken.kind)
            {
                    // If the token represents an object, accept.
                case (int)Token.keywords.TEAM:
                case (int)Token.keywords.AGENT:
                case (int)Token.keywords.SQUAD:
                    Object tmpObject = new Object(currentToken);
                    acceptIt();
                    Printer.Collapse();
                    return tmpObject;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    Printer.Collapse();
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing a type.
        /// </summary>
        private Terminal parseType()
        {
            Printer.WriteLine("Type: " + currentToken.spelling);
            Printer.Expand();
            switch (currentToken.kind)
            {
                case (int)Token.keywords.BOOL:
                case (int)Token.keywords.NUM:
                case (int)Token.keywords.STRING:
                    MASType M = new MASType(currentToken);
                    acceptIt();
                    Printer.Collapse();
                    return M;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    Printer.Collapse();
                    return null;
            }
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
            Expression expression_2 = (Expression)parseExpression();
            accept(Token.keywords.RPAREN);
            
            Block block = (Block)parseBlock();

            Printer.Collapse();
            return new ForCommand(typeDeclaration, expression_1, expression_2, block);
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
            Printer.WriteLine("Type declaration");
            Printer.Expand();
            TypeDeclaration typeDeclaration = new TypeDeclaration();
            typeDeclaration.Type = (MASType)parseType();
            typeDeclaration.VarName = parseIdentifier();
            accept(Token.keywords.BECOMES);

            if (tokenList.ElementAt(listCount + 1).kind == (int)Token.keywords.OPERATOR)
            {
                typeDeclaration.Becomes = (Expression)parseExpression();
            }
            else
            {
                switch (currentToken.kind)
                {
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                    case (int)Token.keywords.ACTUAL_STRING:
                    case (int)Token.keywords.NUMBER:
                    case (int)Token.keywords.IDENTIFIER:
                        typeDeclaration.Becomes = (MASVariable)parseVariable();
                        break;
                }
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
            Printer.WriteLine("Variable: " + currentToken.spelling);
            Printer.Expand();
            switch (currentToken.kind)
            { 
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                case (int)Token.keywords.ACTUAL_STRING:
                case (int)Token.keywords.NUMBER:
                case (int)Token.keywords.IDENTIFIER:
                    MASVariable masVariable = new MASVariable(currentToken);
                    acceptIt();
                    Printer.Collapse();
                    return masVariable;
                default:
                    Printer.Collapse();
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing a method call.
        /// </summary>
        private Command parseMethodCall()
        {
            Printer.WriteLine("Method call");
            Printer.Expand();
            MethodIdentifier methodIdentifier = (MethodIdentifier)parseMethodIdentifier();
            accept(Token.keywords.LPAREN);
            Input input = (Input)parseInput();
            accept(Token.keywords.RPAREN);
            Printer.Collapse();
            return new MethodCall(methodIdentifier, input);
        }

        private Terminal parseMethodIdentifier()
        {
            Printer.WriteLine("Method Identifier");
            Printer.Expand();
            MethodIdentifier MI = new MethodIdentifier();
            MI.Identifier = parseIdentifier();
            if (currentToken.kind == (int)Token.keywords.PUNCTUATION)
            {
                acceptIt();
                MI.NextMethodIdentifier = (MethodIdentifier)parseMethodIdentifier();
            }
            else
            {
                MI.NextMethodIdentifier = null;
            }
            Printer.Collapse();
            return MI;
        }

        /// <summary>
        /// Method for assigning an identifier.
        /// Syntax: identifier becomes  variable | identifier becomes expression
        /// </summary>
        /// <returns></returns>
        private Command parseAssignCommand()
        {
            Printer.WriteLine("Assign Command");
            Printer.Expand();
            Identifier ident;
            AST becomes;

            if (currentToken.kind == (int)Token.keywords.IDENTIFIER)
                ident = parseIdentifier();
            else
            {
                Printer.ErrorLine("Identifier " + currentToken.spelling + " at " + currentToken.row + ", " + currentToken.col + " is not an identifier.");
                ident = null;
                acceptIt();
            }
            accept(Token.keywords.BECOMES);

            if (tokenList.ElementAt(listCount + 1).kind == (int)Token.keywords.OPERATOR)
                becomes = parseExpression();
            else
            {
                becomes = parseVariable();
            }

            Printer.Collapse();
            return new AssignCommand(ident, becomes);
        }

        /// <summary>
        /// Method for parsing an expression (unfinished).
        /// Syntax: primary-expression operator primary-expression
        /// </summary>
        private Expression parseExpression()
        {
            Printer.WriteLine("Expression");
            Printer.Expand();
            AST primaryExpression_1;
            Operator _operator;
            AST primaryExpression_2;

            primaryExpression_1 = parsePrimaryExpression();
            _operator = (Operator)parseOperator();
            // If the next token is an operator, parse an expression.
            if (tokenList.ElementAt(listCount + 1).kind == (int)Token.keywords.OPERATOR)
            {
                primaryExpression_2 = parseExpression();
            }
            else
            {
                primaryExpression_2 = parsePrimaryExpression();
            }

            Printer.Collapse();
            return new Expression(primaryExpression_1, _operator, primaryExpression_2);
        }

        /// <summary>
        /// Method for parsing a Primary Expression
        /// Syntax: number | identifier | expression | ( expression ) | boolean
        /// </summary>
        /// <returns></returns>
        private AST parsePrimaryExpression()
        {
            Printer.WriteLine("Primary Expression");
            Printer.Expand();
            // If the if-loop didnt return, try a switch case on number, boolean, parent expression or identifier.
            switch (currentToken.kind)
            { 
                case (int)Token.keywords.NUMBER:
                    MASNumber num = parseMASNumber();
                    Printer.Collapse();
                    return num;
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                    MASBool masbool = parseMASBool();
                    Printer.Collapse();
                    return masbool;
                case (int)Token.keywords.LPAREN:
                    // Accept the LPARENT.
                    acceptIt();
                    Expression expression = parseExpression();
                    accept(Token.keywords.RPAREN);
                    Printer.Collapse();
                    return expression;
                case (int)Token.keywords.IDENTIFIER:
                    Identifier ident = parseIdentifier();
                    Printer.Collapse();
                    return ident;
                default:
                    accept(Token.keywords.ERROR);
                    break;
            }
            Printer.Collapse();
            return null;
        }

        /// <summary>
        /// Method for parsing an identifier.
        /// </summary>
        private Identifier parseIdentifier()
        {
            Printer.WriteLine("Identifier: " + currentToken.spelling);
            Identifier id = new Identifier(currentToken);
            acceptIt();
            return id;
        }

        /// <summary>
        /// Method for parsing an operator.
        /// </summary>
        private Terminal parseOperator()
        {
            Printer.WriteLine("Operator: " + currentToken.spelling);
            Printer.Expand();
            switch (currentToken.kind)
            {
                case (int)Token.keywords.OPERATOR:
                    Operator O = new Operator(currentToken);
                    acceptIt();
                    Printer.Collapse();
                    return O;
                default:
                    accept(Token.keywords.ERROR);
                    Printer.Collapse();
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing method input.
        /// Syntax: (variable | identifier (, variable | , identifier)* )+
        /// </summary>
        private AST parseInput()
        {
            Printer.WriteLine("Input");
            Printer.Expand();
            Input input = new Input();
            input.firstVar = null;
            input.nextVar = null;

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
                    // If the current token is a right parenthesis return.
                case (int)Token.keywords.RPAREN:
                    Printer.Collapse();
                    return null;
                default:
                    accept(Token.keywords.ERROR);
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

        private MASNumber parseMASNumber()
        { 
            Printer.WriteLine("Number: " + currentToken.spelling);
            MASNumber num = new MASNumber(currentToken);
            acceptIt();
            return num;
        }

        private MASBool parseMASBool()
        {
            Printer.WriteLine("Boolean: " + currentToken.spelling);
            MASBool _bool = new MASBool(currentToken);
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
            if (kind == Token.keywords.ERROR || (int)kind != currentToken.kind)
            {
                Printer.ErrorLine("ERROR at line " + currentToken.row + " col " + currentToken.col + 
                    ". The recieved token of kind " + (Token.keywords)currentToken.kind + " was not legal.");
                Console.ReadKey();
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
            listCount++;
            currentToken = tokenList.ElementAt(listCount);
        }
    }
}
