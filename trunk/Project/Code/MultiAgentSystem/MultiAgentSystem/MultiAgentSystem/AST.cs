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

    abstract class Declaration : AST
    { }

    abstract class Terminal : AST
    { }

    class Mainblock : AST
    {
        public Block B;
    }

    class Block : AST
    {
        public Commands C;
    }

    class Commands : AST
    {
        public Command C;
        public Commands Cs;
    }

    class ObjectDeclaration : Declaration
    {
        public Object O;
        public Identifier I;
        public Input In;
    }

    class TypeDeclaration : Declaration
    {
        public MASType T;
        public Identifier I;
        public Expression E;
        public Identifier I1;
        public Identifier I2;
        public Number N;
        public MASString S;
        public MASBool B;
    }

    class Object
    {

    }

    class IfCommand : Command
    {
        public Expression E;
        public Block B1;
        public Block B2;
    }

    class ForCommand : Command
    {
        public TypeDeclaration TD;
        public Expression E1;
        public Expression E2;
        public Block B;
    }

    class WhileCommand : Command
    {
        public Expression E;
        public Block B;
    }

    class MethodCall : Command
    {
        public List<Identifier> I;
        public Input In;
    }

    class Expression : AST
    {
        public Expression E;
        public Identifier I1;
        public Identifier I2;
        public Number N1;
        public Number N2;
        public MASString S;
        public MASBool B;
        public Operator O;
    }

    class Identifier : Terminal
    {
        string spelling;
    }

    class Operator : Terminal
    {
        string spelling;
    }

    class Input : AST
    {

    }
}
