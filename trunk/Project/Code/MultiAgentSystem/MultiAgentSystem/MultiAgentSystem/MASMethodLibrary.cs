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

            // Methods and overloads that only take an agent as input:
            Input i1 = new Input();
            Token t1 = new Token((int)Token.keywords.AGENT, "", -1, -1);
            i1.firstVar = new Identifier(t1);
            // Add an agent to a team:
            AddAgentToTeam addAgentToTeam1 = new AddAgentToTeam(i1, "add", (int)Token.keywords.TEAM);
            // Add an agent to a squad:
            AddAgentToSquad addAgentToSquad1 = new AddAgentToSquad(i1, "add", (int)Token.keywords.SQUAD);

            // Methods and overloads that only take a string as input:
            Input i2 = new Input();
            Token t2 = new Token((int)Token.keywords.STRING, "", -1, -1);
            i2.firstVar = new Identifier(t2);
            // Add an action to an actionpattern:
            AddActionToActionPattern addActionToAP1 = new AddActionToActionPattern(
                i2, "add", (int)Token.keywords.ACTION_PATTERN);
        }
    }

    /// <summary>
    /// Method for adding an agent to a team.
    /// </summary>
    class AddAgentToTeam : MASMethod, ICodeTemplate
    {
        public AddAgentToTeam(Input input, string name, int useWith) : base(input, name, useWith)
        {
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
        public string PrintInvalidErrorMessage(int linenumber)
        {
            return "(Line " + linenumber + 
                ") The given input was not legal. This method takes an agent as input.";
        }
    }

    /// <summary>
    /// Method for adding an agent to a team.
    /// </summary>
    class AddAgentToSquad : MASMethod, ICodeTemplate
    {
        public AddAgentToSquad(Input input, string name, int useWith)
            : base(input, name, useWith)
        {
            this._returnKind = (int)Token.keywords.ERROR;
        }

        /// <summary>
        /// Generates C# code to add an agent to a team.
        /// </summary>
        /// <param name="one">Name of the squad.</param>
        /// <param name="two">Name of the agent.</param>
        /// <returns>A string containing the C# code.</returns>
        public string PrintGeneratedCode(string one, string two)
        {
            return one + ".agents.add(" + two + ");";
        }

        /// <summary>
        /// A method that prints an errormessage for when the object isn't used correctly, 
        /// and specifies how it should be used.
        /// </summary>
        /// <returns>A string containing the errormessage.</returns>
        public string PrintInvalidErrorMessage(int linenumber)
        {
            return "(Line " + linenumber +
                ") The given input was not legal. This method takes an agent as input.";
        }
    }

    /// <summary>
    /// Method for adding an agent to a team.
    /// </summary>
    class AddActionToActionPattern : MASMethod, ICodeTemplate
    {
        public AddActionToActionPattern(Input input, string name, int useWith)
            : base(input, name, useWith)
        {
            this._returnKind = (int)Token.keywords.ERROR;
        }

        /// <summary>
        /// Generates C# code to add an action to an actionpattern.
        /// </summary>
        /// <param name="one">Name of the action pattern.</param>
        /// <param name="two">String containing the action.</param>
        /// <returns>A string containing the C# code.</returns>
        public string PrintGeneratedCode(string one, string two)
        {
            return one + ".add(" + two + ");";
        }

        /// <summary>
        /// A method that prints an errormessage for when the object isn't used correctly, 
        /// and specifies how it should be used.
        /// </summary>
        /// <returns>A string containing the errormessage.</returns>
        public string PrintInvalidErrorMessage(int linenumber)
        {
            return "(Line " + linenumber +
                ") The given input was not legal. This method takes an agent as input.";
        }
    }

    public abstract class MASMethod
    {
        protected string _name;

        /// <summary>
        /// The name/spelling of the method.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        protected int _id;

        public int ID
        {
            get { return _id; }
        }

        protected int _useWith;

        /// <summary>
        /// The kind of object that this code method should be used with.
        /// Refers to Token.keywords.
        /// </summary>
        public int UseWith
        {
            get { return _useWith; }
        }

        protected int _returnKind;

        /// <summary>
        /// Specifies the type of the object that is returned by the method.
        /// Refers to Token.keywords.
        /// </summary>
        public int ReturnKind
        {
            get { return _returnKind; }
        }

        protected int _overloadID;

        /// <summary>
        /// Specifies the ID of the overload, making it easier to identify individual overloads.
        /// </summary>
        public int OverloadID
        {
            get { return _overloadID; }
        }

        protected Input _validInput;

        /// <summary>
        /// Specifies the valid input for this method.
        /// </summary>
        public Input ValidInput
        {
            get { return _validInput; }
        }

        protected string _printValidInput;

        /// <summary>
        /// A string containing the valid input in written form.
        /// </summary>
        public string PrintValidInput
        {
            get { return _printValidInput; }
        }

        public MASMethod(Input input, string name, int useWith)
        {
            this._name = name;
            this._useWith = useWith;
            this._overloadID = MASMethodLibrary.FindMethod(name, useWith).Count;
            this._id = MASMethodLibrary.MethodLibrary.Count;
            this._validInput = input;

            Token temp;
            Input current = input;
            while (current != null)
            {
                temp = (Token)current.firstVar.visit(new IntermediateVisitor(), null);
                _printValidInput += temp.spelling;
                if (current.nextVar.nextVar == null)
                {
                    _printValidInput += " and ";
                }
                else if (current.nextVar != null)
                {
                    _printValidInput += ", ";
                }
                current = current.nextVar;
            }

            MASMethodLibrary.MethodLibrary.Add(this);
        }
    }

    interface ICodeTemplate
    {
        string PrintGeneratedCode(string one, string two);

        Input ValidInput
        {
            get;
        }

        string PrintInvalidErrorMessage(int linenumber);
    }
}
