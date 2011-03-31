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

        // Holds all the tokens produced by the scanner.
        private List<Token> tokenList;

        // Checks what token is being worked on currently.
        private int listCount = 0;

        /// <summary>
        /// Creates a new parser.
        /// </summary>
        /// <param name="list">A list of tokens produced by the scanner.</param>
        public Parser(List<Token> list)
        {
            tokenList = list;
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
            switch(currentToken.kind)
            {
                case (int)Token.keywords.MAIN:
                    acceptIt();
                    accept(Token.keywords.LPAREN);
                    accept(Token.keywords.RPAREN);
                    main = new Mainblock(parseBlock());
                    accept(Token.keywords.EOT);
                    Console.WriteLine("Main");
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
        private Command parseBlock()
        {
            Block block = new Block();
            // parseCommand is run until the end of the block is reached.
            switch (currentToken.kind)
            {
                case (int)Token.keywords.LBRACKET:
                    acceptIt();
                    while (currentToken.kind != (int)Token.keywords.RBRACKET)
                    {
                        Command c = parseCommand();
                        if (c != null)
                            block.commands.Add(c);
                    }
                    acceptIt();
                    Console.WriteLine("Block");
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
                    // The command can be a block...
                case (int)Token.keywords.LBRACKET:
                    return parseBlock();
                    // or an object declaration...
                case (int)Token.keywords.NEW:
                    Command OD = parseObjectDeclaration();
                    accept(Token.keywords.SEMICOLON);
                    return OD;
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
                    TypeDeclaration T = (TypeDeclaration)parseTypeDeclaration();
                    accept(Token.keywords.SEMICOLON);
                    return T;
                    // or expression or method call.
                case (int)Token.keywords.IDENTIFIER:
                    /* If the next token is an operator, this is an expression. 
                     * Else it's a method call. */
                    if (tokenList.ElementAt(listCount + 1).kind ==
                        (int)Token.keywords.OPERATOR)
                    {
                        Expression E = (Expression)parseExpression();
                        accept(Token.keywords.SEMICOLON);
                        return E;
                    }
                    else
                    {
                        MethodCall M = (MethodCall)parseMethodCall();
                        accept(Token.keywords.SEMICOLON);
                        return M;
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
            /* As the current token will already have been checked,
             * we can just accept it. */
            accept(Token.keywords.NEW);
            Object obj = parseObject();
            Identifier id = parseIdentifier();
            accept(Token.keywords.LPAREN);
            Input input = (Input)parseInput();
            accept(Token.keywords.RPAREN);
            Console.WriteLine("Object declaration");
            return new ObjectDeclaration(obj, id, input);
        }

        /// <summary>
        /// Method for parsing an object.
        /// </summary>
        private Object parseObject()
        {
            switch (currentToken.kind)
            {
                    // If the token represents an object, accept.
                case (int)Token.keywords.TEAM:
                case (int)Token.keywords.AGENT:
                case (int)Token.keywords.SQUAD:
                    acceptIt();
                    Console.WriteLine("Object");
                    return new Object(((Token.keywords)currentToken.kind).ToString());
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
                    MASType M = new MASType(currentToken.spelling);
                    acceptIt();
                    Console.WriteLine("Type");
                    return M;
                default:
                    // Error message
                    accept(Token.keywords.ERROR);
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing an if command.
        /// </summary>
        private Command parseIfCommand()
        {
            accept(Token.keywords.IF_LOOP);
            accept(Token.keywords.LPAREN);
            Expression E = (Expression)parseExpression();
            accept(Token.keywords.RPAREN);
            Block B1 = (Block)parseBlock();
            // Check for an else-statement and react accordingly.
            switch (currentToken.kind)
            {
                case (int)Token.keywords.ELSE_LOOP:
                    acceptIt();
                    Block B2 = (Block)parseBlock();
                    Console.WriteLine("If else");
                    return new IfCommand(E, B1, B2);
                default:
                    Console.WriteLine("If");
                    return new IfCommand(E, B1);
            }
        }

        /// <summary>
        /// Method for parsing a for-loop.
        /// </summary>
        private Command parseForCommand()
        {
            accept(Token.keywords.FOR_LOOP);
            accept(Token.keywords.LPAREN);
            TypeDeclaration T = (TypeDeclaration)parseTypeDeclaration();
            accept(Token.keywords.SEMICOLON);
            Expression E1 = (Expression)parseExpression();
            accept(Token.keywords.SEMICOLON);
            Expression E2 = (Expression)parseExpression();
            accept(Token.keywords.RPAREN);
            Block B = (Block)parseBlock();
            Console.WriteLine("For");
            return new ForCommand(T, E1, E2, B);
        }

        /// <summary>
        /// Method for parsing a while loop.
        /// </summary>
        private Command parseWhileCommand()
        {
            WhileCommand W = new WhileCommand();
            accept(Token.keywords.WHILE_LOOP);
            accept(Token.keywords.LPAREN);
            W.LoopExpression = (Expression)parseExpression();
            accept(Token.keywords.RPAREN);
            W.WhileBlock = (Block)parseBlock();
            Console.WriteLine("While");
            return W;
        }

        /// <summary>
        /// Method for parsing a type declaration.
        /// </summary>
        private Command parseTypeDeclaration()
        {
            TypeDeclaration T = new TypeDeclaration();
            T.Type = (MASType)parseType();
            T.VarName = parseIdentifier();
            accept(Token.keywords.BECOMES);
            if (tokenList.ElementAt(listCount + 1).kind == (int)Token.keywords.OPERATOR)
            {
                T.becomesExpression = (Expression)parseExpression();
            }
            else
            {
                switch (currentToken.kind)
                {
                    case (int)Token.keywords.TRUE:
                    case (int)Token.keywords.FALSE:
                        T.becomesBool = new MASBool(currentToken.spelling);
                        acceptIt();
                        break;
                    case (int)Token.keywords.NUMBER:
                        T.becomesNumber = new MASNumber(currentToken.spelling);
                        acceptIt();
                        break;
                    case (int)Token.keywords.ACTUAL_STRING:
                        T.becomesString = new MASString(currentToken.spelling);
                        acceptIt();
                        break;
                    case (int)Token.keywords.IDENTIFIER:
                        T.becomesIdentifier = parseIdentifier();
                        break;
                    default:
                        accept(Token.keywords.ERROR);
                        break;
                }
            }
            Console.WriteLine("Type declaration");
            return T;
        }

        /// <summary>
        /// Method for parsing a method call.
        /// </summary>
        private Command parseMethodCall()
        {
            MethodCall M = new MethodCall();
            M.MethodPath = (MethodIdentifier)parseMethodIdentifier();
            accept(Token.keywords.LPAREN);
            M.Input = (Input)parseInput();
            accept(Token.keywords.RPAREN);
            Console.WriteLine("Method call");
            return M;
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
            return MI;
        }

        /// <summary>
        /// Method for parsing an expression (unfinished).
        /// </summary>
        private Command parseExpression()
        {
            Expression E = new Expression();
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
                        E.firstNumber = new MASNumber(currentToken.spelling);
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
                        E.Operator = new Operator(currentToken.spelling);
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
                            E.secondNumber = new MASNumber(currentToken.spelling);
                            acceptIt();
                            break;
                        default:
                            accept(Token.keywords.ERROR);
                            break;
                    }
                }
                Console.WriteLine("Expression");
                return E;
            }
        }

        /// <summary>
        /// Method for parsing an identifier.
        /// </summary>
        private Identifier parseIdentifier()
        {
            Identifier id = new Identifier(currentToken.spelling);
            acceptIt();
            Console.WriteLine("Identifier");
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
                    Operator O = new Operator(currentToken.spelling);
                    acceptIt();
                    Console.WriteLine("Operator");
                    return O;
                default:
                    accept(Token.keywords.ERROR);
                    return null;
            }
        }

        /// <summary>
        /// Method for parsing method input.
        /// </summary>
        private AST parseInput()
        {
            Terminal T1 = null;
            ObjectDeclaration O = null;
            switch (currentToken.kind)
            {
                case (int)Token.keywords.RPAREN:
                    return null;
                case (int)Token.keywords.IDENTIFIER:
                    T1 = new Identifier(currentToken.spelling);
                    acceptIt();
                    break;
                case (int)Token.keywords.NUMBER:
                    T1 = new MASNumber(currentToken.spelling);
                    acceptIt();
                    break;
                case (int)Token.keywords.ACTUAL_STRING:
                    T1 = new MASString(currentToken.spelling);
                    acceptIt();
                    break;
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                    T1 = new MASBool(currentToken.spelling);
                    acceptIt();
                    break;
                case (int)Token.keywords.NEW:
                    O = (ObjectDeclaration)parseObjectDeclaration();
                    break;
                default:
                    accept(Token.keywords.ERROR);
                    return null;
            }
            // Input variables are seperated by comma.
            if (currentToken.kind == (int)Token.keywords.COMMA)
            {
                acceptIt();
                Console.WriteLine("Input");
                return new Input(T1, (Input)parseInput());
            }
            else if (T1 != null)
            {
                Console.WriteLine("Input");
                return new Input(T1);
            }
            else if (O != null)
            {
                Console.WriteLine("Input");
                return new Input(O);
            }
            else
            {
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
