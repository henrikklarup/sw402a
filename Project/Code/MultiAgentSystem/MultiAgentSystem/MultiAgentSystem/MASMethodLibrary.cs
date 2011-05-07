using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace MultiAgentSystem
{
    public static class MASMethodLibrary
    {
        public static List<MASMethod> MethodLibrary;

        /// <summary>
        /// Finds the method in the library of methods by matching the name.
        /// </summary>
        /// <param name="name">Name of the method to find.</param>
        /// <returns>A MASMethod or null if no match was found.</returns>
        public static MASMethod FindMethod(string name)
        {
            // Find the method that matches the name.
            MASMethod method = MethodLibrary.Find(
                delegate(MASMethod m)
                {
                    return m.Name.ToLower() == name.ToLower();
                });
            return method;
        }
    }

    /// <summary>
    /// Method for adding an agent to a team.
    /// </summary>
    class AddAgentToTeam : MASMethod, ICodeTemplate
    {
        public AddAgentToTeam(string name, int objectkind)
        {
            this._name = name;
            this._objectKind = objectkind;
        }

        public AddAgentToTeam(string name, int objectkind, int returnkind)
            : this(name, objectkind)
        {
            this._returnKind = returnkind;
        }

        /// <summary>
        /// Generates C# code to add an agent to a team.
        /// </summary>
        /// <param name="one">Name of the team.</param>
        /// <param name="two">Name of the agent.</param>
        /// <returns>A string containing the C# code.</returns>
        public string PrintGeneratedCode(string one, string two)
        {
            return two + ".team = " + one + ";";
        }

        /// <summary>
        /// A method that returns a dummy input that can be used to check for correct input with.
        /// </summary>
        /// <returns>Input with the correct properties.</returns>
        public Input GenerateValidInput()
        {
            Input temp = new Input();
            Token t = new Token((int)Token.keywords.AGENT, "", -1, -1);
            temp.firstVar = new Identifier(t);

            return temp;
        }

        /// <summary>
        /// A method that prints an errormessage for when the object isn't used correctly, 
        /// and specifies how it should be used.
        /// </summary>
        /// <returns>A string containing the errormessage.</returns>
        public string PrintInvalidErrorMessage()
        {
            return "The given input was not legal. This method takes an agent as input.";
        }
    }

    public abstract class MASMethod
    {
        internal string _name;

        /// <summary>
        /// The name/spelling of the method.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        internal int _objectKind;

        /// <summary>
        /// The kind of object that this code method should be used with.
        /// Refers to Token.keywords.
        /// </summary>
        public int ObjectKind
        {
            get { return _objectKind; }
        }

        internal int _returnKind;

        /// <summary>
        /// Specifies the type of the object that is returned by the method.
        /// Refers to Token.keywords.
        /// </summary>
        public int ReturnKind
        {
            get { return _returnKind; }
        }
    }

    interface ICodeTemplate
    {
        string PrintGeneratedCode(string one, string two);

        Input GenerateValidInput();

        string PrintInvalidErrorMessage();
    }
}
