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
            return parseMainblock();
        }

        /// <summary>
        /// Parses a mainblock token, and is executed as the first parse.
        /// </summary>
        private AST parseMainblock()
        {
            Mainblock main;

            Console.WriteLine("Main");
            switch(currentToken.kind)
            {
                case (int)Token.keywords.MAIN:
                    acceptIt();
                    accept(Token.keywords.LPAREN);
                    accept(Token.keywords.RPAREN);
                    main = new Mainblock(parseBlock());
                    accept(Token.keywords.EOT);
                    return main;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing a block.
        /// </summary>
        private AST parseBlock()
        {
            Block block = new Block();
            // parseCommand is run until the end of the block is reached.
            switch (currentToken.kind)
            {
                case (int)Token.keywords.LBRACKET:
                    Console.WriteLine("Block");
                    acceptIt();
                    while (currentToken.kind != (int)Token.keywords.RBRACKET)
                    {
                        Command c = (Command)parseCommand();
                        if (c != null)
                            block.commands.Add(c);
                    }
                    acceptIt();
                    return block;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing a command.
        /// </summary>
        private Command parseCommand()
        {
            switch (currentToken.kind)
            {
                    // or an object declaration...
                case (int)Token.keywords.NEW:
                    Command objectDeclaration = parseObjectDeclaration();
                    accept(Token.keywords.SEMICOLON);
                    return objectDeclaration;
                    // or an if-sentence...
                case (int)Token.keywords.IF_LOOP:
                    return parseIfCommand();
                    // for loop...
                case (int)Token.keywords.FOR_LOOP:
                    return parseForCommand();
                    // while loop...
                case (int)Token.keywords.WHILE_LOOP:
                    return parseWhileCommand();
                    // type declaration...
                case (int)Token.keywords.NUM:
                case (int)Token.keywords.STRING:
                case (int)Token.keywords.BOOL:
                    TypeDeclaration typeDeclaration = (TypeDeclaration)parseTypeDeclaration();
                    accept(Token.keywords.SEMICOLON);
                    return typeDeclaration;
                    // or expression or method call.
                case (int)Token.keywords.IDENTIFIER:
                    /* If the next token is an operator, this is an expression. 
                     * Else it's a method call. */
                    if (tokenList.ElementAt(listCount + 1).kind ==
                        (int)Token.keywords.OPERATOR)
                    {
                        Expression expression = (Expression)parseExpression();
                        accept(Token.keywords.SEMICOLON);
                        return expression;
                    }
                    else
                    {
                        MethodCall methodCall = (MethodCall)parseMethodCall();
                        accept(Token.keywords.SEMICOLON);
                        return methodCall;
                    }
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing an object declaration.
        /// </summary>
        private Command parseObjectDeclaration()
        {
            Console.WriteLine("Object declaration");

            /* As the current token already have been checked,
             * we can just accept it. */
            accept(Token.keywords.NEW);
            Object obj = parseObject();
            Identifier id = parseIdentifier();
            accept(Token.keywords.LPAREN);
            Input input = (Input)parseInput();
            accept(Token.keywords.RPAREN);
            return new ObjectDeclaration(obj, id, input);
        }

        /// <summary>
        /// Method for parsing an object.
        /// </summary>
        private Object parseObject()
        {
            Console.WriteLine("Object");
            switch (currentToken.kind)
            {
                    // If the token represents an object, accept.
                case (int)Token.keywords.TEAM:
                case (int)Token.keywords.AGENT:
                case (int)Token.keywords.SQUAD:
                    Object tmpObject = new Object(currentToken);
                    acceptIt();
                    return tmpObject;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing a type.
        /// </summary>
        private Terminal parseType()
        {
            switch (currentToken.kind)
            {
                case (int)Token.keywords.BOOL:
                case (int)Token.keywords.NUM:
                case (int)Token.keywords.STRING:
                    Console.WriteLine("Type");
                    MASType M = new MASType(currentToken);
                    acceptIt();
                    return M;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing an if command.
        /// Syntax: if ( expression ) block (else elseBlock)+
        /// </summary>
        private Command parseIfCommand()
        {
            Console.Write("If");
            accept(Token.keywords.IF_LOOP);
            accept(Token.keywords.LPAREN);
            Expression expression = (Expression)parseExpression();
            accept(Token.keywords.RPAREN);
            Block block = (Block)parseBlock();
            // Check for an else-statement and react accordingly.
            switch (currentToken.kind)
            {
                case (int)Token.keywords.ELSE_LOOP:
                    Console.WriteLine(" else");
                    acceptIt();
                    Block elseBlock = (Block)parseBlock();
                    return new IfCommand(expression, block, elseBlock);
                default:
                    Console.WriteLine();
                    return new IfCommand(expression, block);
            }
        }

        /// <summary>
        /// Method for parsing a for-loop.
        /// Syntax: for ( typeDeclaration ; expression_1 ; expression_2 ) block
        /// </summary>
        private Command parseForCommand()
        {
            Console.WriteLine("For");

            accept(Token.keywords.FOR_LOOP);
            accept(Token.keywords.LPAREN);
            TypeDeclaration typeDeclaration = (TypeDeclaration)parseTypeDeclaration();
            accept(Token.keywords.SEMICOLON);
            Expression expression_1 = (Expression)parseExpression();
            accept(Token.keywords.SEMICOLON);
            Expression expression_2 = (Expression)parseExpression();
            accept(Token.keywords.RPAREN);
            
            Block block = (Block)parseBlock();
            
            return new ForCommand(typeDeclaration, expression_1, expression_2, block);
        }

        /// <summary>
        /// Method for parsing a while loop.
        /// Syntax: while ( expression ) block
        /// </summary>
        private Command parseWhileCommand()
        {
            Console.WriteLine("While");
            accept(Token.keywords.WHILE_LOOP);
            accept(Token.keywords.LPAREN);
            Expression LoopExpression = (Expression)parseExpression();
            accept(Token.keywords.RPAREN);

            Block WhileBlock = (Block)parseBlock();

            return new WhileCommand(LoopExpression, WhileBlock);
        }

        /// <summary>
        /// Method for parsing a type declaration.
        /// 
        /// </summary>
        private Command parseTypeDeclaration()
        {
            Console.WriteLine("Type declaration");
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
                        typeDeclaration.Becomes = (MASVariable)parseVariable();
                        break;
                }
            }
            return typeDeclaration;
        }

        /// <summary>
        /// Method for parsing a variable.
        /// </summary>
        /// <returns></returns>
        private Terminal parseVariable()
        {
            switch (currentToken.kind)
            { 
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                case (int)Token.keywords.ACTUAL_STRING:
                case (int)Token.keywords.NUMBER:
                    MASVariable masVariable = new MASVariable(currentToken);
                    acceptIt();
                    return masVariable;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing a method call.
        /// </summary>
        private Command parseMethodCall()
        {
            Console.WriteLine("Method call");
            MethodIdentifier methodIdentifier = (MethodIdentifier)parseMethodIdentifier();
            accept(Token.keywords.LPAREN);
            Input input = (Input)parseInput();
            accept(Token.keywords.RPAREN);
            return new MethodCall(methodIdentifier, input);
        }

        private Terminal parseMethodIdentifier()
        {
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
            return MI;
        }

        /// <summary>
        /// Method for parsing an expression (unfinished).
        /// Syntax: primary-expression operator primary-expression
        /// </summary>
        private Expression parseExpression()
        {
            AST primaryExpression_1;
            Operator _operator;
            AST primaryExpression_2;

            primaryExpression_1 = parsePrimaryExpression();
            _operator = (Operator)parseOperator();
            primaryExpression_2 = parsePrimaryExpression();

            return new Expression(primaryExpression_1, _operator, primaryExpression_2);

            #region OLD expression
            /*Expression E = new Expression();
            // If the expression starts with a parenthesis, parse that.
            if (currentToken.kind == (int)Token.keywords.LPAREN)
            {
                acceptIt();
                E = (Expression)parseExpression();
                accept(Token.keywords.RPAREN);
                Console.WriteLine("Expression");
                return E;
            }
            // Else parse a normal expression.
            else
            {
                switch (currentToken.kind)
                {
                    case (int)Token.keywords.IDENTIFIER:
                        E.firstVariable = parseIdentifier();
                        break;
                    case (int)Token.keywords.NUMBER:
                        E.firstNumber = new MASNumber(currentToken);
                        acceptIt();
                        break;
                    default:
                        accept(Token.keywords.ERROR);
                        break;
                }
                switch (currentToken.kind)
                {
                    case (int)Token.keywords.OPERATOR:
                    case (int)Token.keywords.BECOMES:
                        E.Operator = new Operator(currentToken);
                        acceptIt();
                        break;
                    default:
                        accept(Token.keywords.ERROR);
                        break;
                }

                // If the expression doesn't end after the next token, parse a new expression.
                if (tokenList.ElementAt(listCount + 1).kind == (int)Token.keywords.OPERATOR ||
                    currentToken.kind == (int)Token.keywords.LPAREN)
                {
                    E.innerExpression = (Expression)parseExpression();
                }
                else
                {
                    switch (currentToken.kind)
                    {
                        case (int)Token.keywords.IDENTIFIER:
                            E.secondVariable = parseIdentifier();
                            break;
                        case (int)Token.keywords.NUMBER:
                            E.secondNumber = new MASNumber(currentToken);
                            acceptIt();
                            break;
                        default:
                            accept(Token.keywords.ERROR);
                            break;
                    }
                }
                Console.WriteLine("Expression");
                return E;
            }*/
            #endregion
        }

        /// <summary>
        /// Method for parsing a Primary Expression
        /// Syntax: number | identifier | expression | ( expression ) | boolean
        /// </summary>
        /// <returns></returns>
        private AST parsePrimaryExpression()
        {
            Console.WriteLine("Primary Expression");
            // If the next token is an operator, parse an expression.
            if (tokenList.ElementAt(listCount + 1).kind == (int)Token.keywords.OPERATOR)
            {
                return parseExpression();
            }

            // If the if-loop didnt return, try a switch case on number, boolean, parent expression or identifier.
            switch (currentToken.kind)
            { 
                case (int)Token.keywords.NUMBER:
                    return new MASNumber(currentToken);
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                    return new MASBool(currentToken);
                case (int)Token.keywords.LPAREN:
                    // Accept the LPARENT.
                    acceptIt();
                    Expression expression = parseExpression();
                    accept(Token.keywords.RPAREN);
                    return expression;
                case (int)Token.keywords.IDENTIFIER:
                    return parseIdentifier();
                default:
                    accept(Token.keywords.ERROR);
                    break;
            }
            return null;
        }

        /// <summary>
        /// Method for parsing an identifier.
        /// </summary>
        private Identifier parseIdentifier()
        {
            Console.WriteLine("Identifier");
            Identifier id = new Identifier(currentToken);
            acceptIt();
            return id;
        }

        /// <summary>
        /// Method for parsing an operator.
        /// </summary>
        private Terminal parseOperator()
        {
            switch (currentToken.kind)
            {
                case (int)Token.keywords.OPERATOR:
                    Console.WriteLine("Operator");
                    Operator O = new Operator(currentToken);
                    acceptIt();
                    return O;
                default:
                    accept(Token.keywords.ERROR);
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing method input.
        /// Syntax: (variable | identifier (, variable | , identifier)* )+
        /// </summary>
        private AST parseInput()
        {
            Input input = new Input();

            switch (currentToken.kind)
            { 
                case (int)Token.keywords.NUMBER:
                case (int)Token.keywords.ACTUAL_STRING:
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                    input.firstVar = (MASVariable)parseVariable();
                    input.nextVar = (Input)parseInput();
                    return input;
                case (int)Token.keywords.RPAREN:
                    input.firstVar = null;
                    input.nextVar = null;
                    return input;
                default:
                    accept(Token.keywords.ERROR);
                    return null;
            }
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
                Console.WriteLine("ERROR at line " + currentToken.row + " col " + currentToken.col + 
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
