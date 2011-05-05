using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionPatternCompiler
{
    abstract class AST
    {
        public abstract object visit(Visitor v, object arg);
    }

    abstract class Terminal : AST
    { }

    class Action : AST
    {
        public Single_Action single_action;

        public override object visit(Visitor v, object arg)
        {
            return v.visitAction(this, arg);
        }
    }
    
    class Single_Action : AST
    {
        public AST selection;
        public Move_Option move_option;

        public int type;

        public override object visit(Visitor v, object arg)
        {
            return v.visitSingle_Action(this, arg);
        }
    }
        
    class Move_Option : AST
    {
        public AST dir_coord;

        public int type;

        public override object visit(Visitor v, object arg)
        {
            return v.visitMove_Option(this, arg);
        }
    }

    class Identifier : Terminal
    {
        public Token name;

        public override object visit(Visitor v, object arg)
        {
            return v.visitIdentifier(this, arg);
        }
    }

    class MASNumber : Terminal
    {
        public Token ID;

        public override object visit(Visitor v, object arg)
        {
            return v.visitMASNumber(this, arg);
        }
    }

    class Direction : Terminal
    {
        public Token dir;

        public override object visit(Visitor v, object arg)
        {
            return v.visitDirection(this, arg);
        }
    }

    class Coordinate : Terminal
    {
        public Token num1;
        public Token num2;

        public override object visit(Visitor v, object arg)
        {
            return v.visitCoordinate(this, arg);
        }
    }

    class agentID : AST
    {
        public Token num;

        public override object visit(Visitor v, object arg)
        {
            return v.visitagentID(this, arg);
        }
    }

    class teamID : AST
    {
        public Token num;

        public override object visit(Visitor v, object arg)
        {
            return v.visitteamID(this, arg);
        }
    }

    class squadID : AST
    {
        public Token num;

        public override object visit(Visitor v, object arg)
        {
            return v.visitSquadID(this, arg);
        }
    }

    class Type
    {
        public enum Types
        {
            AGENT,
            TEAM,
            SQUAD,
            ACTIONPATTERN,
            AGENTID,
            TEAMID,
            SQUADID,
            DIR,
            COORD,
            IDENTIFIER,
            NUMBER,
        }
    }
}
