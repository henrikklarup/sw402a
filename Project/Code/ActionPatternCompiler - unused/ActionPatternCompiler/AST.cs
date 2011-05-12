﻿using System;
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

    class Single_Action : AST
    {
        public Stance stance;
        public Move_Option move_option;

        public override object visit(Visitor v, object arg)
        {
            return v.visitSingle_Action(this, arg);
        }
    }

    class Move_Option : AST
    {
        public AST dir_coord;

        public int type;        // Direction, coordinate or ActionPattern.
        public int stance;      // Indicates which stance the unit has to be in to use the action.

        public override object visit(Visitor v, object arg)
        {
            return v.visitMove_Option(this, arg);
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

    class Identifier : Terminal
    {
        public Token name;

        public override object visit(Visitor v, object arg)
        {
            return v.visitIdentifier(this, arg);
        }
    }

    class Type
    {
        public enum Types
        {
            DIR,
            COORD,
            NUMBER,
            ACTIONPATTERN,
        }
    }

    class Stance
    {
        public Token stance;

        public enum Stances
        {
            MOVE,
            ENCOUNTER,
        }
    }
}