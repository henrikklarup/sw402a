using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    /* All these classes are used by the parser to create the Abstract Syntax Tree.
     * The descriptions above the classes, in the AST file, is the function syntax. */

    public abstract class AST
    {
        public abstract object visit(Visitor v, object arg);
    }

    public abstract class Command : AST
    {
    }

    public abstract class Terminal : AST
    {
    }

    // Main input block
    public class Mainblock : AST
    {
        public Input input;
        //The block part of the mainblock
        public Block block;

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
    public class Block : AST
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
    public class ObjectDeclaration : Command
    {
        // What kind of object is being declared.
        public Object _object;

        // The name of the new object.
        public LinkedIdentifier identifier;

        // The input the objectconstructor needs.
        public Input input;

        public ObjectDeclaration(Object O, LinkedIdentifier I, Input In)
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

    // Type VarName = Becomes (BOOL / STRING / NUM / IDENTIFIER / EXPRESSION)
    public class TypeDeclaration : Command
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
    
    // AGENT / TEAM / etc.
    public class Object : Terminal
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
    public class IfCommand : Command
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
    public class ForCommand : Command
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
    public class WhileCommand : Command
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
    public class LinkedIdentifier : Terminal
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
    public class MethodCall : Command
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

    // ident = becomes
    public class AssignCommand : Command
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

    public abstract class ExpressionAST : Command
    { }

    /// <summary>
    /// Can consist of other expressions, or an arbitrary combination of variables and numbers.
    /// Syntax: primary-expression operator primary-expression
    /// </summary>
    public class Expression : ExpressionAST
    {
        public PrimaryExpression primExpr1;
        public Operator opr;
        public PrimaryExpression primExpr2;
        public ParentExpression parentExpr;

        public Token basicToken;

        public Type type;
        public Expression(PrimaryExpression primExpr1, Operator _operator, PrimaryExpression primExpr2)
        {
            this.primExpr1 = primExpr1;
            this.opr = _operator;
            this.primExpr2 = primExpr2;
            this.parentExpr = null;
        }

        public Expression(ParentExpression parentExpr)
        {
            this.primExpr1 = null;
            this.opr = null;
            this.primExpr2 = null;
            this.parentExpr = parentExpr;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitExpression(this, arg);
        }
    }

    public class PrimaryExpression : ExpressionAST
    {
        public Terminal var;
        public Expression expression;
        public ParentExpression parentExpression;

        public Type type;

        public PrimaryExpression(Terminal var)
        {
            this.var = var;
            this.expression = null;
            this.parentExpression = null;
        }

        public PrimaryExpression(Expression expr)
        {
            this.var = null;
            this.expression = expr;
            this.parentExpression = null;
        }

        public PrimaryExpression(ParentExpression pexpr)
        {
            this.var = null;
            this.expression = null;
            this.parentExpression = pexpr;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitPrimaryExpression(this, arg);
        }
    }

    public class ParentExpression : ExpressionAST
    {
        public Expression expr;

        public Type type;

        public ParentExpression(Expression expr)
        {
            this.expr = expr;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitParentExpression(this, arg);
        }
    }

    // Any token of kind Identifier.
    public class Identifier : Terminal
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

    // Any token of kind Operator.
    public class Operator : Terminal
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
    public class Input : AST
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

        public List<Input> OverloadVisit(Visitor v, List<Input> arg, int line)
        {
            return v.visitOverload(this, arg, line);
        }

        public string PrintInput()
        {
            Token temp;
            Input current = this;
            string input = "";
            InputValidationVisitor i = new InputValidationVisitor();

            while (current != null)
            {
                temp = (Token)current.firstVar.visit(i, null);
                input += temp.spelling;
                if (current.nextVar != null)
                {
                    input += ", ";
                }
                current = current.nextVar;
            }

            return input;
        }
    }

    // Any variable.
    public class MASVariable : Terminal
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
    public class MASBool : Terminal
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
    public class MASString : Terminal
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
    public class MASNumber : Terminal
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
    public class MASType : Terminal
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
