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
        public Input input;

        public Mainblock(AST C)
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
            return v.visitObjectDeclaration(this, arg);
        }
    }

    // Type VarName = becomes...SomethingSomething...
    class TypeDeclaration : Command
    {
        // What kind of variable is being declared (bool, string or num).
        public MASType Type;

        // Name of the variable being declared.
        public Identifier VarName;

        // The type or variable 
        public AST Becomes;

        public override object visit(Visitor v, object arg)
        {
            return v.visitTypeDeclaration(this, arg);
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
        public AssignCommand CounterExpression;

        // The block with the code that is to be executed in the loop.
        public Block ForBlock;

        public ForCommand(TypeDeclaration T, Expression E, AssignCommand A, Block B)
        {
            this.CounterDeclaration = T;
            this.LoopExpression = E;
            this.CounterExpression = A;
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

        public WhileCommand(Expression loopExpression, Block whileBlock)
        {
            this.LoopExpression = loopExpression;
            this.WhileBlock = whileBlock;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitWhileCommand(this, arg);
        }
    }

    // Identifier.(NextLinkedIdentifier.Identifier).(etc.) (recursive, continues in each LinkedIdentifier object)
    class LinkedIdentifier : Terminal
    {
        // Identifier of the object or method being held here.
        public Identifier Identifier;

        // The LinkedIdentifier containing the next identifier.
        public LinkedIdentifier NextLinkedIdentifier;

        public LinkedIdentifier()
        { }

        public override object visit(Visitor v, object arg)
        {
            return v.visitLinkedIdentifier(this, arg);
        }
    }

    // LinkedIdentifier ( Input )
    class MethodCall : Command
    {
        // Path to the method, including the method name.
        public LinkedIdentifier linkedIdentifier;

        // Input to the method.
        public Input input;

        public MethodCall(LinkedIdentifier LinkedIdentifier, Input input)
        {
            this.linkedIdentifier = LinkedIdentifier;
            this.input = input;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMethodCall(this, arg);
        }
    }

    class AssignCommand : Command
    {
        public LinkedIdentifier ident;
        public AST becomes;

        public AssignCommand(Identifier ident, AST becomes)
        {
            this.ident = new LinkedIdentifier();
            this.ident.Identifier = ident;
            this.becomes = becomes;
        }

        public AssignCommand(LinkedIdentifier linked, AST becomes)
        {
            this.ident = linked;
            this.becomes = becomes;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitAssignCommand(this, arg);
        }
    }

    abstract class ExpressionAST : Command
    { }

    /// <summary>
    /// Can consist of other expressions, or an arbitrary combination of variables and numbers.
    /// Syntax: primary-expression operator primary-expression
    /// </summary>
    class Expression : ExpressionAST
    {
        public Token basicToken = null;
        public AST primaryExpression_1;
        public Operator _operator;
        public AST primaryExpression_2;

        public int type;

        public Expression(AST primaryExpression_1, 
            Operator _operator, AST primaryExpression_2, Token token)
        {
            this.primaryExpression_1 = primaryExpression_1;
            this._operator = _operator;
            this.primaryExpression_2 = primaryExpression_2;
            this.basicToken = token;
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

    /// <summary>
    /// Syntax: (variable | identifier (, variable | , identifier)* )+
    /// </summary>
    class Input : AST
    {
        // The first input variable.
        public AST firstVar = null;

        // The next input variable.
        public Input nextVar = null;

        public Input()
        { }

        public override object visit(Visitor v, object arg)
        {
            return v.visitInput(this, arg);
        }
    }

    class MASVariable : Terminal
    {
        public Token token;

        public MASVariable(Token token)
        {
            this.token = token;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMASVariable(this, arg);
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
