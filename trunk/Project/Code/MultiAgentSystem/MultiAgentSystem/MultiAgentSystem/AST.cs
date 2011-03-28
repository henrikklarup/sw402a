using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    abstract class AST
    {
    }

    abstract class Command : AST
    { }

    abstract class Terminal : AST
    { }

    class Mainblock : AST
    {
        public Block B;

        public Mainblock(Command C)
        {
            this.B = (Block)C;
        }

        public Mainblock()
        { }
    }
    
    class Block : Command
    {
        public List<Command> commands;

        public Block()
        { commands = new List<Command>(); }
    }

    class ObjectDeclaration : Command
    {
        public Object O;
        public Identifier I;
        public Input In;

        public ObjectDeclaration(Object O, Identifier I, Input In)
        {
            this.O = O;
            this.I = I;
            this.In = In;
        }
    }

    class TypeDeclaration : Command
    {
        public MASType T;
        public Identifier I1;
        public Expression E;
        public Identifier I2;
        public MASNumber N;
        public MASString S;
        public MASBool B;
    }

    class Object
    {
        string spelling;

        public Object(string s)
        {
            spelling = s;
        }
    }

    class IfCommand : Command
    {
        public Expression E;
        public Block B1;
        public Block B2;

        public IfCommand(Expression E, Block B1)
        {
            this.E = E;
            this.B1 = B1;
        }

        public IfCommand(Expression E, Block B1, Block B2)
            : this(E, B1)
        {
            this.B2 = B2;
        }
    }

    class ForCommand : Command
    {
        public TypeDeclaration TD;
        public Expression E1;
        public Expression E2;
        public Block B;

        public ForCommand(TypeDeclaration T, Expression E1, Expression E2, Block B)
        {
            this.TD = T;
            this.E1 = E1;
            this.E2 = E2;
            this.B = B;
        }
    }

    class WhileCommand : Command
    {
        public Expression E;
        public Block B;

        public WhileCommand()
        {

        }
    }

    class MethodCall : Command
    {
        public List<Identifier> I = new List<Identifier>();
        public Input In;
    }

    class Expression : Command
    {
        public Expression E;
        public Identifier I1;
        public Identifier I2;
        public MASNumber N1;
        public MASNumber N2;
        public Operator O;

        public Expression()
        {
        }
    }

    class Identifier : Terminal
    {
        string spelling;

        public Identifier(string s)
        {
            spelling = s;
        }
    }

    class Operator : Terminal
    {
        string spelling;

        public Operator(string s)
        {
            spelling = s;
        }
    }

    class Input : AST
    {
        public ObjectDeclaration O;
        public Identifier I;
        public MASBool B;
        public MASString S;
        public MASNumber N;
        public Input In;

        public Input()
        { }

        public Input(Identifier I)
        {
            this.I = I;
        }

        public Input(MASBool B)
        {
            this.B = B;
        }

        public Input(MASString S)
        {
            this.S = S;
        }

        public Input(MASNumber N)
        {
            this.N = N;
        }

        public Input(ObjectDeclaration obj)
        {
            this.O = obj;
        }

        public Input(Identifier I, Input input) 
            : this(I)
        {
            this.In = input;
        }

        public Input(MASBool B, Input input)
            : this(B)
        {
            this.In = input;
        }

        public Input(MASString S, Input input)
            : this(S)
        {
            this.In = input;
        }

        public Input(MASNumber N, Input input)
            : this(N)
        {
            this.In = input;
        }

        public Input(ObjectDeclaration O, Input input)
            : this(O)
        {
            this.In = input;
        }

        public Input(Terminal T)
        {
            if (T is Identifier)
            {
                I = (Identifier)T;
            }
            else if (T is MASBool)
            {
                B = (MASBool)T;
            }
            else if (T is MASString)
            {
                S = (MASString)T;
            }
            else if (T is MASNumber)
            {
                N = (MASNumber)T;
            }
        }

        public Input(Terminal T, Input input) : this(T)
        {
            this.In = input;
        }
    }

    class MASBool : Terminal
    {
        bool content;

        public MASBool(string s)
        {
            if (s.ToLower() == "true")
            {
                content = true;
            }
            else if (s.ToLower() == "false")
            {
                content = false;
            }
        }
    }

    class MASString : Terminal
    {
        string spelling;

        public MASString(string s)
        {
            spelling = s;
        }
    }

    class MASNumber : Terminal
    {
        double num;

        public MASNumber(string s)
        {
            num = Double.Parse(s);
        }
    }

    class MASType : Terminal
    {
        string spelling;

        public MASType(string s)
        {
            spelling = s;
        }
    }
}
