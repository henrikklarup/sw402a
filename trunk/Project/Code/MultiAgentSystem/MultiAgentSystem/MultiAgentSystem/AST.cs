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
        //The block part of the mainblock
        public Block block;

        public Mainblock(Command C)
        {
            this.block = (Block)C;
        }

        public Mainblock()
        { }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMainBlock(this, arg);
        }
    }
    
    // { commands }
    class Block : AST
    {
        // All Commands contained in the block.
        public List<Command> commands;

        public Block()
        { commands = new List<Command>(); }

        public override object visit(Visitor v, object arg)
        {
            return v.visitBlock(this, arg);
        }
    }

    // new ObjectKind ObjectName ( Input )
    class ObjectDeclaration : Command
    {
        // What kind of object is being declared.
        public Object _object;

        // The name of the new object.
        public Identifier identifier;

        // The input the objectconstructor needs.
        public Input input;

        public ObjectDeclaration(Object O, Identifier I, Input In)
        {
            this._object = O;
            this.identifier = I;
            this.input = In;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitObjectDecleration(this, arg);
        }
    }

    // Type VarName = becomes...SomethingSomething...
    class TypeDeclaration : Command
    {
        // What kind of variable is being declared (bool, string or num).
        public MASType Type;

        // Name of the variable being declared.
        public Identifier VarName;

        // If the variable is instantiated to an expression (1 + 2 for example), this is the expression.
        public Expression becomesExpression;

        // If the variable is instantiated to another variable, it is held here.
        public Identifier becomesIdentifier;

        // If the variable is instantiated to a number, string or boolean value, it is held here.
        public MASNumber becomesNumber;
        public MASString becomesString;
        public MASBool becomesBool;

        public override object visit(Visitor v, object arg)
        {
            return v.visitTypeDecleration(this, arg);
        }
    }

    class Object : Terminal
    {
        // Name of the object (Agent, Team, etc.)
        public Token token;

        public Object(Token token)
        {
            this.token = token;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitObject(this, arg);
        }
    }

    // if ( Expression ) ifBlock elseBlock
    class IfCommand : Command
    {
        // The expression being evaluated.
        public Expression Expression;

        // The block coming after the if-command.
        public Block IfBlock;

        // the block coming after the else-command.
        public Block ElseBlock;

        public IfCommand(Expression E, Block B1)
        {
            this.Expression = E;
            this.IfBlock = B1;
        }

        public IfCommand(Expression E, Block B1, Block B2)
            : this(E, B1)
        {
            this.ElseBlock = B2;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitIfCommand(this, arg);
        }
    }

    // for ( CounterDeclaration ; LoopExpression ; CounterExpression ) ForBlock
    class ForCommand : Command
    {
        // The type declaration with the counter variable.
        public TypeDeclaration CounterDeclaration;

        // The boolean expression that determines if the loop should continue.
        public Expression LoopExpression;

        // The expression that determines what happens to the counter after each run of the loop.
        public Expression CounterExpression;

        // The block with the code that is to be executed in the loop.
        public Block ForBlock;

        public ForCommand(TypeDeclaration T, Expression E1, Expression E2, Block B)
        {
            this.CounterDeclaration = T;
            this.LoopExpression = E1;
            this.CounterExpression = E2;
            this.ForBlock = B;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitForCommand(this, arg);
        }
    }

    // while ( LoopExpression ) WhileBlock
    class WhileCommand : Command
    {
        public Expression LoopExpression;
        public Block WhileBlock;

        public WhileCommand()
        {
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitWhileCommand(this, arg);
        }
    }

    // Identifier.(NextMethodIdentifier.Identifier).(etc.) (recursive, continues in each MethodIdentifier object)
    class MethodIdentifier : Terminal
    {
        // Identifier of the object or method being held here.
        public Identifier Identifier;

        // The MethodIdentifier containing the next identifier.
        public MethodIdentifier NextMethodIdentifier;

        public MethodIdentifier()
        {
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMethodIdentifier(this, arg);
        }
    }

    // MethodPath ( Input )
    class MethodCall : Command
    {
        // Path to the method, including the method name.
        public MethodIdentifier MethodPath;

        // Input to the method.
        public Input Input;

        public override object visit(Visitor v, object arg)
        {
            return v.visitMethodCall(this, arg);
        }
    }

    // Can consist of other expressions, or an arbitrary combination of variables and numbers.
    class Expression : Command
    {
        // If the expression contains another expression, it is kept here.
        public Expression innerExpression;

        // The variable on the left side of the operator.
        public Identifier firstVariable;

        // The variable on the right side of the operator.
        public Identifier secondVariable;

        // The number on the left side of the operator.
        public MASNumber firstNumber;

        // The number on the right side of the operator.
        public MASNumber secondNumber;

        // The operator
        public Operator Operator;

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
        public Token token;
        
        public Identifier(Token token)
        {
            this.token = token;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitIdentifier(this, arg);
        }
    }

    class Operator : Terminal
    {
        public Token token;

        public Operator(Token token)
        {
            this.token = token;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitOperator(this, arg);
        }
    }

    class Input : AST
    {
        // Object declaration that takes place in the input.
        public ObjectDeclaration ObjectDeclaration;

        // An identifier as input.
        public Identifier VarName;

        // True or false as input.
        public MASBool TrueOrFalse;

        // String as input.
        public MASString Text;

        // Number as input.
        public MASNumber Number;

        // The next input variable.
        public Input InputVar;

        public Input()
        { }

        public Input(Identifier I)
        {
            this.VarName = I;
        }

        public Input(MASBool B)
        {
            this.TrueOrFalse = B;
        }

        public Input(MASString S)
        {
            this.Text = S;
        }

        public Input(MASNumber N)
        {
            this.Number = N;
        }

        public Input(ObjectDeclaration obj)
        {
            this.ObjectDeclaration = obj;
        }

        public Input(Identifier I, Input input) 
            : this(I)
        {
            this.InputVar = input;
        }

        public Input(MASBool B, Input input)
            : this(B)
        {
            this.InputVar = input;
        }

        public Input(MASString S, Input input)
            : this(S)
        {
            this.InputVar = input;
        }

        public Input(MASNumber N, Input input)
            : this(N)
        {
            this.InputVar = input;
        }

        public Input(ObjectDeclaration O, Input input)
            : this(O)
        {
            this.InputVar = input;
        }

        public Input(Terminal T)
        {
            if (T is Identifier)
            {
                VarName = (Identifier)T;
            }
            else if (T is MASBool)
            {
                TrueOrFalse = (MASBool)T;
            }
            else if (T is MASString)
            {
                Text = (MASString)T;
            }
            else if (T is MASNumber)
            {
                Number = (MASNumber)T;
            }
        }

        public Input(Terminal T, Input input) : this(T)
        {
            this.InputVar = input;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitInput(this, arg);
        }
    }

    // Booleans of the system.
    class MASBool : Terminal
    {
        public Token token;

        public MASBool(Token token)
        {
            this.token = token;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMASBool(this, arg);
        }
    }

    // Strings of the system.
    class MASString : Terminal
    {
        public Token token;

        public MASString(Token token)
        {
            this.token = token;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMASString(this, arg);
        }
    }

    // Numbers of the system.
    class MASNumber : Terminal
    {
        public Token token;

        public MASNumber(Token token)
        {
            this.token = token;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMASNumber(this, arg);
        }
    }

    // Types of the system (either bool, num or string)
    class MASType : Terminal
    {
        public Token token;

        public MASType(Token token)
        {
            this.token = token;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMASType(this, arg);
        }
    }
}
