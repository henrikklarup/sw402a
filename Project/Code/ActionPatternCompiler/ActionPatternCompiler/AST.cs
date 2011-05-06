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
        
    class Move_Option : AST
    {
        public AST dir_coord;

        public int type;

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

    class Type
    {
        public enum Types
        {
            DIR,
            COORD,
            NUMBER,
        }
    }
}
