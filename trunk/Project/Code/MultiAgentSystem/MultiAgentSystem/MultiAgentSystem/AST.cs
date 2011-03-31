using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    abstract class AST
    {
        public abstract object visit(Visitor v, object arg);
    }

    abstract class Command : AST
    {
    }

    abstract class Terminal : AST
    {
    }

    class Mainblock : AST
    {
        public Block B;

        public Mainblock(Command C)
        {
            this.B = (Block)C;
        }

        public Mainblock()
        { }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMainBlock(this, arg);
        }
    }
    
    class Block : Command
    {
        public List<Command> commands;

        public Block()
        { commands = new List<Command>(); }

        public override object visit(Visitor v, object arg)
        {
            return v.visitBlock(this, arg);
        }
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

        public Type type;
        public override object visit(Visitor v, object arg)
        {
            return v.visitObjectDecleration(this, arg);
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

        public Type type;
        public override object visit(Visitor v, object arg)
        {
            return v.visitTypeDecleration(this, arg);
        }
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

        public override object visit(Visitor v, object arg)
        {
            return v.visitIfCommand(this, arg);
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

        public override object visit(Visitor v, object arg)
        {
            return v.visitForCommand(this, arg);
        }
    }

    class WhileCommand : Command
    {
        public Expression E;
        public Block B;

        public WhileCommand()
        {

        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitWhileCommand(this, arg);
        }
    }

    class MethodIdentifier : Terminal
    {
        public Identifier I;
        public MethodIdentifier MI;

        public MethodIdentifier()
        {

        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMethodIdentifier(this, arg);
        }
    }

    class MethodCall : Command
    {
        public MethodIdentifier MI;
        public Input In;

        public override object visit(Visitor v, object arg)
        {
            return v.visitMethodCall(this, arg);
        }
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
        
        public override object visit(Visitor v, object arg)
        {
            return v.visitExpression(this, arg);
        }
    }

    class Identifier : Terminal
    {
        string spelling;


        public Identifier(string s)
        {
            spelling = s;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitIdentifier(this, arg);
        }
    }

    class Operator : Terminal
    {
        string spelling;

        public Operator(string s)
        {
            spelling = s;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitOperator(this, arg);
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

        public override object visit(Visitor v, object arg)
        {
            return v.visitInput(this, arg);
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

        public override object visit(Visitor v, object arg)
        {
            return v.visitMASBool(this, arg);
        }
    }

    class MASString : Terminal
    {
        string spelling;

        public MASString(string s)
        {
            spelling = s;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMASString(this, arg);
        }
    }

    class MASNumber : Terminal
    {
        double num;

        public MASNumber(string s)
        {
            num = Double.Parse(s);
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMASNumber(this, arg);
        }
    }

    class MASType : Terminal
    {
        string spelling;

        public MASType(string s)
        {
            spelling = s;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMASType(this, arg);
        }
    }
}
