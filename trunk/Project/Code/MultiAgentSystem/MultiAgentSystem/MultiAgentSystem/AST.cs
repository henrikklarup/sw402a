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

    abstract class Expression : AST
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
}
