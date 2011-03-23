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
    {/*
        SingleCommand singleCommand;
        ObjectDeclaration objectDeclaration;
        TypeDeclaration typeDeclaration;
        MethodCall methodCall;
        Block block;

        public Command(SingleCommand singleCommand)
        {
            this.singleCommand = singleCommand;
        }

        public Command(ObjectDeclaration objectDeclaration)
        {
            this.objectDeclaration = objectDeclaration;
        }

        public Command(TypeDeclaration typeDeclaration)
        {
            this.typeDeclaration = typeDeclaration;
        }

        public Command(MethodCall methodCall)
        {
            this.methodCall = methodCall;
        }

        public Command(Block block)
        {
            this.block = block;
        }*/
    }

    abstract class Commands : Command
    {/*
        Commands commands;
        Command command;
        
        public Commands(Command command)
        {
            this.command = command;
        }

        public Commands(Command command, Commands commands)
            : this(command)
        {
            this.commands = commands;
        }*/
    }

    abstract class SingleCommand : Command
    {/*
        IfCommand ifCommand;
        WhileCommand whileCommand;
        ForCommand forCommand;
        
        public SingleCommand(IfCommand ifCommand)
        {
            this.ifCommand = ifCommand;
        }

        public SingleCommand(WhileCommand whileCommand)
        {
            this.whileCommand = whileCommand;
        }

        public SingleCommand(ForCommand forCommand)
        {
            this.forCommand = forCommand;
        }*/
    }

    abstract class Expression : AST
    {/*
        PrimaryExpression primaryExpression1;
        MASOperator masOperator;
        PrimaryExpression primaryExpression2;*/
    }

    abstract class Declaration : AST
    {
    }
    
    class Block : Command
    {/*
        Commands commands;

        public Block(Commands commands)
        {
            this.commands = commands;
        }*/
    }

    class PrimaryExpression : Expression
    {

    }

    class ObjectDeclaration : Declaration
    {

    }

    class TypeDeclaration : Declaration
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

    }

    class MASOperator : AST
    {

    }
}
