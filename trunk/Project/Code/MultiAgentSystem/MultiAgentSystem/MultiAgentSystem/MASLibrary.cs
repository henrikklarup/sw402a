using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace MultiAgentSystem
{
    public static class MASLibrary
    {
        // A list of all methods and overloads in the language.
        public static List<MASMethod> MethodLibrary;

        // A list of all constructors and overloads in the language.
        public static List<MASConstructor> ConstructorLibrary;

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

        /// <summary>
        /// Finds a list of constructors that match the name and ObjectKind type.
        /// </summary>
        /// <param name="name">Name of the constructor to find.</param>
        /// <param name="type">Type of the object the constructor builds.</param>
        /// <returns>A list containing every overload for the matching constructor.</returns>
        public static List<MASConstructor> FindConstructor(string name, int objectKind)
        {
            // Find the method that matches the name.
            List<MASConstructor> list = new List<MASConstructor>();
            foreach (MASConstructor m in ConstructorLibrary)
            {
                if (m.Name.ToLower() == name.ToLower() && m.ObjectKind == objectKind)
                {
                    list.Add(m);
                }
            }
            return list;
        }
    }

    /// <summary>
    /// Instantiates every method in the language with the correct input and variables.
    /// </summary>
    class Methods
    {
        public Methods()
        {
            MASLibrary.MethodLibrary = new List<MASMethod>();
            MASLibrary.ConstructorLibrary = new List<MASConstructor>();
            
            // Methods and overloads that only take an agent as input:
            Input i1 = new Input();
            Token t1 = new Token((int)Token.keywords.AGENT, "agent", -1, -1);
            i1.firstVar = new Identifier(t1);
            // Add an agent to a team:
            AddAgentToTeam addAgentToTeam1 = new AddAgentToTeam(i1, "add", (int)Token.keywords.TEAM);
            // Add an agent to a squad:
            AddAgentToSquad addAgentToSquad1 = new AddAgentToSquad(i1, "add", (int)Token.keywords.SQUAD);

            // Methods and overloads that only take a string as input:
            Input i2 = new Input();
            Token t2 = new Token((int)Token.keywords.STRING, "string", -1, -1);
            i2.firstVar = new Identifier(t2);
            // Add an action to an actionpattern:
            AddActionToActionPattern addActionToAP1 = new AddActionToActionPattern(
                i2, "add", (int)Token.keywords.ACTION_PATTERN);

            //Input i3 = new Input();
            //i3.firstVar = new Identifier(t1);
            //i3.nextVar = new Input();
            //i3.nextVar.firstVar = new Identifier(t2);
            //AddAgentToTeam addAgentToTeam2 = new AddAgentToTeam(i3, "add", (int)Token.keywords.TEAM);
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

        /// <summary>
        /// The ID of the method.
        /// </summary>
        public int ID
        {
            get { return _id; }
        }

        protected int _useWith;

        /// <summary>
        /// The kind of object that this method is used with.
        /// Refers to Token.keywords.
        /// </summary>
        public int UseWith
        {
            get { return _useWith; }
        }

        protected int _returnKind;

        /// <summary>
        /// Specifies the type of object that is returned by the method.
        /// Refers to Token.keywords.
        /// </summary>
        public int ReturnKind
        {
            get { return _returnKind; }
        }

        protected int _overloadID;

        /// <summary>
        /// Specifies the ID of the overload.
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
            // ID and overloadID are found by going through the list of methods.
            this._overloadID = MASLibrary.FindMethod(name, useWith).Count;
            this._id = MASLibrary.MethodLibrary.Count;
            this._validInput = input;

            // The printValidInput string is generated from the given input.
            Token temp;
            Input current = input;
            IntermediateVisitor i = new IntermediateVisitor();
            while (current != null)
            {
                temp = (Token)current.firstVar.visit(i, null);
                _printValidInput += temp.spelling;
                if (current.nextVar != null && current.nextVar.nextVar == null)
                {
                    _printValidInput += " and ";
                }
                else if (current.nextVar != null)
                {
                    _printValidInput += ", ";
                }
                current = current.nextVar;
            }

            // The method is added to the list of methods.
            MASLibrary.MethodLibrary.Add(this);
        }

        /// <summary>
        /// A method that prints an errormessage for when the object isn't used correctly, 
        /// and specifies how it should be used.
        /// </summary>
        /// <returns>A string containing the errormessage.</returns>
        public string PrintInvalidErrorMessage(int linenumber)
        {
            return "(Line " + linenumber +
                ") The given input was not legal. The legal input is: " + _printValidInput;
        }
    }

    /// <summary>
    /// Method for adding an agent to a team.
    /// </summary>
    class AddAgentToTeam : MASMethod, ICodeTemplate
    {
        public AddAgentToTeam(Input input, string name, int useWith) : base(input, name, useWith)
        {
            // There is no return value, so it is set to error.
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
            // agent.team = team;
            return two + ".team = " + one + ";";
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
            // There is no return value, so it is set to error.
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
            // squad.agents.add(agent);
            return one + ".agents.add(" + two + ");";
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
            // There is no return value, so it is set to error.
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
            // actionpattern.add(string);
            return one + ".add(" + two + ");";
        }
    }

    public abstract class MASConstructor
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

        /// <summary>
        /// The ID of the method.
        /// </summary>
        public int ID
        {
            get { return _id; }
        }

        protected int _objectKind;

        /// <summary>
        /// The kind of object that this method is used with.
        /// Refers to Token.keywords.
        /// </summary>
        public int ObjectKind
        {
            get { return _objectKind; }
        }

        protected int _overloadID;

        /// <summary>
        /// Specifies the ID of the overload.
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

        public MASConstructor(Input input, string name, int objectKind)
        {
            this._name = name;
            this._objectKind = objectKind;
            // ID and overloadID are found by going through the list of constructors.
            this._overloadID = MASLibrary.FindConstructor(name, objectKind).Count;
            this._id = MASLibrary.ConstructorLibrary.Count;
            this._validInput = input;

            // The printValidInput string is generated from the given input.
            Token temp;
            Input current = input;
            IntermediateVisitor i = new IntermediateVisitor();
            while (current != null)
            {
                temp = (Token)current.firstVar.visit(i, null);
                _printValidInput += temp.spelling;
                if (current.nextVar != null && current.nextVar.nextVar == null)
                {
                    _printValidInput += " and ";
                }
                else if (current.nextVar != null)
                {
                    _printValidInput += ", ";
                }
                current = current.nextVar;
            }

            // The method is added to the list of methods.
            MASLibrary.ConstructorLibrary.Add(this);
        }

        /// <summary>
        /// A method that prints an errormessage for when the object isn't used correctly, 
        /// and specifies how it should be used.
        /// </summary>
        /// <returns>A string containing the errormessage.</returns>
        public string PrintInvalidErrorMessage(int linenumber)
        {
            return "(Line " + linenumber +
                ") The given input was not legal. The legal input is: " + _printValidInput;
        }
    }

    class agentConstructor : MASConstructor, ICodeTemplate
    {

    }

    /// <summary>
    /// Interface to be used with every method and constructor class in the compiler.
    /// </summary>
    interface ICodeTemplate
    {
        /// <summary>
        /// Prints the generated code.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns>A string with code.</returns>
        string PrintGeneratedCode(string one, string two);

        /// <summary>
        /// A property that returns the valid input for the method.
        /// </summary>
        Input ValidInput
        {
            get;
        }

        /// <summary>
        /// Prints an errormessage used when the method or constructor is used wrongly.
        /// </summary>
        /// <param name="linenumber"></param>
        /// <returns>Errormessage.</returns>
        string PrintInvalidErrorMessage(int linenumber);
    }
}
