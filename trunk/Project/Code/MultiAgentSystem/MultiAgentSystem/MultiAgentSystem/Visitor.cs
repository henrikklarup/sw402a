using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MultiAgentSystem
{
    abstract class Visitor
    {
        /// <summary>
        /// visit the AST, the first method called when visiting the AST.
        /// visits the Main Block.
        /// </summary>
        /// <param name="ast"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        public abstract object visitAST(AST ast, object arg);

        /// <summary>
        /// visit the Main Block.
        /// visits the first Block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal abstract object visitMainBlock(Mainblock block, object arg);

        /// <summary>
        /// visit a Block, holds a list of commands.
        /// visits all commands in the block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal abstract object visitBlock(Block block, object arg);

        /// <summary>
        /// visit an object declaration, consists of an object, an identifier, and an input.
        /// Checks if the identifier already exists, if not, creates a new identifier.
        /// Syntax: new object identifier ( input )
        /// visits the object, the identifer, and the input.
        /// </summary>
        /// <param name="objectDeclaration"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal abstract object visitObjectDeclaration(ObjectDeclaration objectDeclaration, object arg);

        /// <summary>
        /// visit a type declaration, consists of a type, an identifier, 
        /// and whatever its declared as (expression or variable).
        /// Syntax: Type VarName = becomes
        /// visits the type, the identifier, and the expression or variable.
        /// </summary>
        /// <param name="typeDeclaration"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal abstract object visitTypeDeclaration(TypeDeclaration typeDeclaration, object arg);

        /// <summary>
        /// visit an if expression, consists of a boolean expression and two blocks.
        /// If the last block (the else block) exists, then visit it.
        /// Syntax: if ( bool-expression ) block
        /// Syntax: if ( bool-expression ) block else block
        /// visits the expression and both blocks if they exists.
        /// </summary>
        /// <param name="ifCommand"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal abstract object visitIfCommand(IfCommand ifCommand, object arg);

        /// <summary>
        /// visit for loop, consitst of a declaration, a boolean expression, an expression, and a block.
        /// Syntax: for ( type-declaration ; bool-expression ; math-expression ) block
        /// visits the declaration, the two expressions and the block.
        /// </summary>
        /// <param name="forCommand"></param>
        /// <param name="arg"></param>
        /// <returns>null</returns>
        internal abstract object visitForCommand(ForCommand forCommand, object arg);

        /// <summary>
        /// visit while loop
        /// Syntax: while ( bool-expression ) block
        /// </summary>
        /// <param name="whileCommand"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal abstract object visitWhileCommand(WhileCommand whileCommand, object arg);

        // identifier ( input ) | identifier . method-call
        internal abstract object visitMethodCall(MethodCall methodCall, object arg);

        // Syntax: number | identifier | expression | ( expression ) | boolean
        internal abstract object visitExpression(Expression expression, object arg);

        internal abstract object visitIdentifier(Identifier identifier, object arg);

        internal abstract object visitOperator(Operator p, object arg);

        internal abstract object visitInput(Input input, object arg);

        internal abstract object visitLinkedIdentifier(LinkedIdentifier LinkedIdentifier, object arg);

        internal abstract object visitMASNumber(MASNumber mASNumber, object arg);

        internal abstract object visitMASType(MASType mASType, object arg);

        internal abstract object visitMASString(MASString mASString, object arg);

        internal abstract object visitMASBool(MASBool mASBool, object arg);

        internal abstract object visitObject(Object p, object arg);

        internal abstract object visitMASVariable(MASVariable mASVariable, object arg);

        internal abstract object visitAssignCommand(AssignCommand assignCommand, object arg);
    }
}