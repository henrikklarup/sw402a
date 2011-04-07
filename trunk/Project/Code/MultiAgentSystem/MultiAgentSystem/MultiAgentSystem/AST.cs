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

    abstract class Terminal : AST
    {
    }

    class Mainblock : AST
    {
        //The block part of the mainblock
        public Block block;

        public Mainblock(Block block)
        {
            this.block = block;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitMainBlock(this, arg);
        }
    }

    #region Block AST
    abstract class BlockAST : AST
    { }

    // { commands }
    class Block : BlockAST
    {
        // All Commands contained in the block.
        public Commands commands;

        public Block(Commands commands)
        {
            this.commands = commands;
        }

        public override object visit(Visitor v, object arg)
        {
            return v.visitBlock(this, arg);
        }
    }
    #endregion

    #region Commands AST
    abstract class CommandsAST : AST
    { }

    class Commands : CommandsAST
    {
        public Command command;
        public Commands commands;

        public Commands(Command command, Commands commands)
        {
            this.command = command;
            this.commands = commands;
        }

        public Commands(Command command)
        {
            this.command = command;
        }
    }
    #endregion

    #region Command AST
    abstract class CommandAST : AST
    { }

    class Command : CommandAST
    {
        public Single_Command single_command;
        public Declaration declaration;
        public Method_Call method_call;

        public Command(Single_Command single_command)
        {
            this.single_command = single_command;
        }

        public Command(Declaration declaration)
        {
            this.declaration = declaration;
        }

        public Command(Method_Call method_call)
        {
            this.method_call = method_call;
        }
    }

    abstract class Single_CommandAST : AST
    { }

    class Single_Command : Single_CommandAST
    {
        public While_Command while_command;
        public If_Command if_command;
        public For_Command for_command;

        public Single_Command(While_Command while_command)
        {
            this.while_command = while_command;
        }

        public Single_Command(If_Command if_command)
        {
            this.if_command = if_command;
        }

        public Single_Command(For_Command for_command)
        {
            this.for_command = for_command;
        }
    }

    class While_Command : Single_CommandAST
    {
        Bool_Expression bool_expression;
        Block block;

        public While_Command(Bool_Expression bool_expression, Block block)
        {
            this.bool_expression = bool_expression;
            this.block = block;
        }
    }

    class If_Command : Single_CommandAST
    {
        public Bool_Expression bool_expression;
        public Block block;

        public If_Command(Bool_Expression bool_expression, Block block)
        {
            this.bool_expression = bool_expression;
            this.block = block;
        }
    }

    class For_Command : Single_CommandAST
    {
        public Type_Declaration type_declaration;
        public Bool_Expression bool_expression;
        public Math_Expression math_expression;
        public Block block;

        public For_Command(
            Type_Declaration type_declaration,
            Bool_Expression bool_expression,
            Math_Expression math_expression,
            Block block            
            )
        {
            this.type_declaration = type_declaration;
            this.bool_expression = bool_expression;
            this.math_expression = math_expression;
            this.block = block;
        }
    }

    #endregion

    #region Expression AST

    abstract class ExpressionAST : AST
    { }

    class Expression : ExpressionAST
    {
        public Math_Expression math_expression;
        public Bool_Expression bool_expression;

        public Expression(Math_Expression math_expression)
        {
            this.math_expression = math_expression;
        }

        public Expression(Bool_Expression bool_expression)
        {
            this.bool_expression = bool_expression;
        }
    }

    class Math_Expression : ExpressionAST
    {
        public Primary_Expression primary_expression_1;
        public Math_Operator math_operator;
        public Primary_Expression primary_Expression_2;

        public Math_Expression(
            Primary_Expression primary_expression_1,
            Math_Operator math_operator,
            Primary_Expression primary_Expression_2)
        {
            this.primary_expression_1 = primary_expression_1;
            this.math_operator = math_operator;
            this.primary_Expression_2 = primary_Expression_2;
        }
    }

    class Primary_Expression : ExpressionAST
    {
        public MASNumber number;
        public Identifier ident;
        public Math_Expression math_expression;
        public Parent_Math_Expression parent_math_expression;

        public Primary_Expression(MASNumber number)
        {
            this.number = number;
        }

        public Primary_Expression(Identifier ident)
        {
            this.ident = ident;
        }

        public Primary_Expression(Math_Expression math_expression)
        {
            this.math_expression = math_expression;
        }

        public Primary_Expression(Parent_Math_Expression parent_math_expression)
        {
            this.parent_math_expression = parent_math_expression;
        }
    }

    class Parent_Math_Expression : ExpressionAST
    {
        public Math_Expression math_expression;

        public Parent_Math_Expression(Math_Expression math_expression)
        {
            this.math_expression = math_expression;
        }
    }

    class Bool_Expression : ExpressionAST
    {
        public MASBool masbool;
        public Bool_Primary_Expression bool_primary_expression;

        public Bool_Expression(MASBool masbool)
        {
            this.masbool = masbool;
        }

        public Bool_Expression(Bool_Primary_Expression bool_primary_expression)
        {
            this.bool_primary_expression = bool_primary_expression;
        }
    }

    class Bool_Primary_Expression : ExpressionAST
    {
        public Primary_Expression primary_expression_1;
        public Bool_Operator bool_operator;
        public Primary_Expression primary_expression_2;

        public Bool_Primary_Expression(Primary_Expression primary_expression_1, Bool_Operator bool_operator, Primary_Expression primary_expression_2)
        {
            this.primary_expression_1 = primary_expression_1;
            this.bool_operator = bool_operator;
            this.primary_expression_2 = primary_expression_2;
        }
    }
    #endregion

    #region Declaration AST

    abstract class DeclarationAST : AST
    { }

    class Declaration : DeclarationAST
    {
        public Object_Declaration object_declaration;
        public Type_Declaration type_declaration;

        public Declaration(Object_Declaration object_declaration)
        {
            this.object_declaration = object_declaration;
        }

        public Declaration(Type_Decleration type_declaration)
        {
            this.type_declaration = type_declaration;
        }
    }

    //new object identifier ( input )
    class Object_Declaration : DeclarationAST
    { 
        public MASObject _object;
        public Identifier ident;
        public Input input;

        public Object_Declaration(MASObject _object, Identifier ident, Input input)
        {
            this._object = _object;
            this.ident = ident;
            this.input = input;
        }
    }

    //type identifier = type
    class Type_Declaration : DeclarationAST
    {
        public MASType type_1;
        public Identifier ident;
        public MASType type_2;

        public Type_Declaration(MASType type_1, Identifier ident, MASType type_2)
        {
            this.type_1 = type_1;
            this.ident = ident;
            this.type_2 = type_2;
        }
    }

    #endregion

    #region Method_Call AST

    abstract class Method_CallAST : AST
    { }

    class Method_Call : Method_CallAST
    {
        public Identifier ident;
        public Input input;
        //Or
        public Method_Call method_call;

        public Method_Call(Identifier ident, Input input)
        {
            this.ident = ident;
            this.input = input;
        }

        public Method_Call(Identifier ident, Method_Call method_call)
        {
            this.ident = ident;
            this.method_call = method_call;
        }
    }

    #endregion

    #region Variables

    class MASBool : Terminal
    {
        public Token true_or_false;

        public MASBool(Token true_or_false)
        {
            this.true_or_false = true_or_false;
        }
    }

    class MASNumber : Terminal
    {
        public Token number;

        public MASNumber(Token number)
        {
            this.number = number;
        }
    }

    class MASString : Terminal
    { 
        public Token _string;

        public MASString(Token _string)
        {
            this._string = _string;
        }
    }

    class Math_Operator : Terminal
    {
        public Token math_operator;

        public Math_Operator(Token math_operator)
        {
            this.math_operator = math_operator;
        }
    }

    class Bool_Operator : Terminal
    {
        public Token bool_operator;

        public Bool_Operator(Token bool_operator)
        {
            this.bool_operator = bool_operator;
        }
    }

    class Identifier : Terminal
    {
        public Token identifier;

        public Identifier(Token identifier)
        {
            this.identifier = identifier;
        }
    }
    #endregion
}
