using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    class Visitor
    {
        internal object visitAST(AST aST, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMainProgram(MainProgram mainProgram, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitSingle_Action(Single_Action single_Action, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMove_Action(Move_Action move_Action, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitMove_Option(Move_Option move_Option, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            throw new NotImplementedException();
        }

        internal object visitCoordinate(Coordinate coordinate, object arg)
        {
            throw new NotImplementedException();
        }
    }
}
