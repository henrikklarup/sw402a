using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionInterpeter
{
    class Visitor
    {
        // Exception for catching errors.
        private GrammarException gException =
            new GrammarException("These errors were found during decoration:");
        private bool throwException = false;

        public object visitAST(AST ast, object arg)
        {
            ast.visit(this, arg);
            return null;
        }

        internal object visitMainProgram(MainProgram mainProgram, object arg)
        {
            Console.WriteLine("LOL");
            mainProgram.action.visit(this, null);
            return null;
        }

        internal object visitAction(Action action, object arg)
        {
            action.single_action.visit(this, null);
            return null;
        }

        internal object visitSingle_Action(Single_Action single_Action, object arg)
        {
            single_Action.identifier.visit(this, arg);
            single_Action.move_action.visit(this, arg);
            return null;
        }

        internal object visitMove_Action(Move_Action move_Action, object arg)
        {
            move_Action.move_Option.visit(this, arg);
            return null;
        }

        internal object visitMove_Option(Move_Option move_Option, object arg)
        {
            //If no direction is recorded, this must be a coordinate
            if (move_Option.direction == null)
            {
                move_Option.coordinate.visit(this, arg);
            }
            return null;
        }

        internal object visitIdentifier(Identifier identifier, object arg)
        {
            //Check if this identifier exists
            Token token = identifier.agent_Name_or_ID;
            if (token.kind == (int)Token.keywords.NUMBER)
            {
                //identifier.type == number
            }
            else
            { 
                //identifier.type
            }
            return null;
        }

        internal object visitCoordinate(Coordinate coordinate, object arg)
        {
            Token firstNum = coordinate.num1;
            Token secondNum = coordinate.num2;

            if (firstNum.kind != (int)Token.keywords.NUMBER )
            {
                throwException = true;
                gException.containedExceptions.Add(
                    new GrammarException(
                        firstNum.spelling +
                        " is not valid input for coordinates.", firstNum));
            }
            if(secondNum.kind != (int)Token.keywords.NUMBER)
            {
                throwException = true;
                gException.containedExceptions.Add(
                    new GrammarException(
                        secondNum.spelling +
                        " is not valid input for coordinates.", secondNum));
            }
            return null;
        }
    }
}
