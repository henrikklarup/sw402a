using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    abstract class AST
    {
        public abstract object visit(Visitor v, object arg);
    }

    abstract class Terminal : AST
    { }

    abstract class Action : AST
    { }

    class MainProgram : AST 
    {
        public Action action;

        public MainProgram(Action action)
        {
            this.action = action;
        }
        public override object visit(Visitor v, object arg)
        {
            return v.visitMainProgram(this, arg);
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

        public override object visit(Visitor v, object arg)
        {
            return v.visitSingle_Action(this, arg);
        }
    }

    class Move_Action : Action
    {
        public Move_Option move_Option;

        public Move_Action(Move_Option move_option)
        {
            this.move_Option = move_option;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMove_Action(this, arg);
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

        public override object visit(Visitor v, object arg)
        {
            return v.visitMove_Option(this, arg);
        }
    }

    class Identifier : Terminal
    {
        public Token agent_Name_or_ID;

        public Identifier(Token agent_Name_or_ID)
        {
            this.agent_Name_or_ID = agent_Name_or_ID;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitIdentifier(this, arg);
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

        public override object visit(Visitor v, object arg)
        {
            return v.visitCoordinate(this, arg);
        }
    }

}
