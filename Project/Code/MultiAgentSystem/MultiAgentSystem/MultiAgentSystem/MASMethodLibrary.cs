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
        /// Finds a list of methods that match the name and UseWith type.
        /// </summary>
        /// <param name="name">Name of the method to find.</param>
        /// <param name="type">Type of the object the method is used on.</param>
        /// <returns>A list containing every overload for the matching method.</returns>
        public static List<MASMethod> FindMethod(string name, int useWith)
        {
            // Find the method that matches the name.
            List<MASMethod> list = new List<MASMethod>();
            foreach (MASMethod m in MethodLibrary)
            {
                if (m.Name.ToLower() == name.ToLower() && m.UseWith == useWith)
                {
                    list.Add(m);
                }
            }
            return list;
        }
    }

    class Methods
    {
        public Methods()
        {
            MASMethodLibrary.MethodLibrary = new List<MASMethod>();

            Input temp = new Input();
            Token t = new Token(-1, "", -1, -1);

            t.kind = (int)Token.keywords.AGENT;
            temp.firstVar = new Identifier(t);
            AddAgentToTeam addAgentToTeam1 = new AddAgentToTeam(temp);
        }
    }

    /// <summary>
    /// Method for adding an agent to a team.
    /// </summary>
    class AddAgentToTeam : MASMethod, ICodeTemplate
    {
        public AddAgentToTeam(Input input) : base(input)
        {
            this._name = "add";
            this._useWith = (int)Token.keywords.TEAM;
            this._returnKind = (int)Token.keywords.ERROR;
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

        internal int _useWith;

        /// <summary>
        /// The kind of object that this code method should be used with.
        /// Refers to Token.keywords.
        /// </summary>
        public int UseWith
        {
            get { return _useWith; }
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

        internal int _overloadID;

        /// <summary>
        /// Specifies the ID of the overload, making it easier to identify individual overloads.
        /// </summary>
        public int OverloadID
        {
            get { return _overloadID; }
        }

        internal Input _validInput;

        /// <summary>
        /// Specifies the valid input for this method.
        /// </summary>
        public Input ValidInput
        {
            get { return _validInput; }
        }

        public MASMethod(Input input)
        {
            this._overloadID = GetOverLoadID(_name, _useWith);
            this._validInput = input;
            MASMethodLibrary.MethodLibrary.Add(this);
        }

        /// <summary>
        /// Finds the proper ID for the overload of any method.
        /// </summary>
        /// <param name="name">Name of the method.</param>
        /// <param name="useWith">Type of the object the method is used on.</param>
        /// <returns>The overload ID of the method.</returns>
        public int GetOverLoadID(string name, int useWith)
        {
            return MASMethodLibrary.FindMethod(name, useWith).Count + 1;
        }
    }

    interface ICodeTemplate
    {
        string PrintGeneratedCode(string one, string two);

        Input ValidInput
        {
            get;
        }

        string PrintInvalidErrorMessage();
    }
}
