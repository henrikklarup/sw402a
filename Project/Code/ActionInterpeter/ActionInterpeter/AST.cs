using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    abstract class AST
    { }

    abstract class Terminal : AST
    {
        public Token token;
    }

    abstract class Action : AST
    { }

    class MainProgram : AST 
    {
        public Action action;

        public MainProgram(Action action)
        {
            this.action = action;
        }
    }

    class Single_Action : Action
    {
        public Move_Action move_action;
        public Identifier identifier;

        public Single_Action(Identifier identifier, Move_Action move_action)
        {
            this.identifier = identifier;
            this.move_action = move_action;
        }
    }

    class Move_Action : Action
    {
        public Move_Option move_Option;

        public Move_Action(Move_Option move_option)
        {
            this.move_Option = move_option;
        }
    }

    class Move_Option : Action
    {
        public Token direction;
        public Coordinate coordinate;

        public Move_Option(Token direction)
        {
            this.direction = direction;
        }

        public Move_Option(Coordinate coord)
        {
            this.coordinate = coord;
        }
    }

    class Identifier : Terminal
    {
        public Token agent_Name_or_ID;

        public Identifier(Token agent_Name_or_ID)
        {
            this.agent_Name_or_ID = agent_Name_or_ID;
        }
    }

    class Coordinate : Terminal
    {
        public Token num1;
        public Token num2;

        public Coordinate(Token num1, Token num2)
        {
            this.num1 = num1;
            this.num2 = num2;
        }
    }

}
