using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    class OldParser
    {
     /*   // new ObjectKind ObjectName ( Input )
        class ObjectDeclaration
        {
            // What kind of object is being declared.
            public Object ObjectKind;

            // The name of the new object.
            public Identifier ObjectName;

            // The input the objectconstructor needs.
            public Input Input;

            public ObjectDeclaration(Object O, Identifier I, Input In)
            {
                this.ObjectKind = O;
                this.ObjectName = I;
                this.Input = In;
            }

            public Type type;
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

            public Type type;
            public override object visit(Visitor v, object arg)
            {
                return v.visitTypeDecleration(this, arg);
            }
        }

        class Object
        {
            // Name of the object (Agent, Team, etc.)
            string spelling;

            public Object(string s)
            {
                spelling = s;
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
            // A Variable as input.
            public Variable variable;

            // An identifier as input.
            public Identifier ident;

            // The next input variable.
            public Input input;

            public Input()
            { }

            public Input(Variable variable)
            {
                this.variable = variable;
            }

            public Input(Identifier ident)
            {
                this.ident = ident;
            }

            public Input(Variable variable, Input input)
            {
                this.variable = variable;
                this.input = input;
            }

            public Input(Identifier ident, Input input)
            {
                this.ident = ident;
                this.input = input;
            }
        }

        // Booleans of the system.
        class MASBool : Terminal
        {
            bool Value;

            public MASBool(string s)
            {
                if (s.ToLower() == "true")
                {
                    Value = true;
                }
                else if (s.ToLower() == "false")
                {
                    Value = false;
                }
            }

            public override object visit(Visitor v, object arg)
            {
                return v.visitMASBool(this, arg);
            }
        }

        // Strings of the system.
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

        // Numbers of the system.
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

        // Types of the system (either bool, num or string)
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
        } */
    }
}
