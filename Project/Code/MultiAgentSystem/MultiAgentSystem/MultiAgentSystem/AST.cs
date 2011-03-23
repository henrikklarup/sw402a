using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    abstract class AST
    {
    }

    class Mainblock : AST
    {
        Block block;

        public Mainblock(Block block)
        {
            this.block = block;
        }
    }

    abstract class Command : AST
    {
    }

    abstract class SingleCommand : Command
    {
    }

    abstract class PrimaryExpression : AST
    {
    }

    abstract class Terminal : AST
    {
        public string spelling;
    }
    
    class Block : AST
    {
        List<Command> commandList = new List<Command>();
        Command command;

        public Block(Command command)
        {
            this.command = command;
        }
    }

    class Expression : PrimaryExpression
    {
        PrimaryExpression primaryExpression1;
        MASOperator masOperator;
        PrimaryExpression primaryExpression2;
    }

    class ObjectDeclaration : Command
    {

    }

    class TypeDeclaration : Command
    {

    }

    class MethodCall : Command
    {

    }

    class IfCommand : Command
    {

    }

    class WhileCommand : Command
    {

    }

    class ForCommand : Command
    {
        TypeDeclaration typeD;
        Expression E1, E2;

        public ForCommand(TypeDeclaration type, Expression e1, Expression e2)
        {
            this.typeD = type;
            this.E1 = e1;
            this.E2 = e2;
        }
    }

    class MASOperator : Terminal
    {
        public MASOperator(string op)
        {
            this.spelling = op;
        }
    }

    class Number : Terminal
    {
        public Number(double num)
        {
            spelling = num.ToString();
        }
    }
}
