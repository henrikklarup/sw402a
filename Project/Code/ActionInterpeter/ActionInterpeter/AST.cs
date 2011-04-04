using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    abstract class AST
    { }

    class action : AST
    { }

    class identifier : action
    { }

    class single_action : action
    { }

    class move_action : single_action
    { }

    class move_option : move_action
    { }

    class coordinate : move_option
    {
        public Token num1;
        public Token num2;
    }

}
