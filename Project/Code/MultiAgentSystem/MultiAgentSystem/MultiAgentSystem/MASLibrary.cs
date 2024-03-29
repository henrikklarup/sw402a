﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MASClassLibrary;

namespace MASSIVE
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
        public static List<MASConstructor> FindConstructor(int objectKind)
        {
            // Find the method that matches the name.
            List<MASConstructor> list = new List<MASConstructor>();
            foreach (MASConstructor c in ConstructorLibrary)
            {
                if (c.ObjectKind == objectKind)
                {
                    list.Add(c);
                }
            }
            return list;
        }

        /// <summary>
        /// Tests two types to see if they match within certain rules.
        /// </summary>
        /// <param name="kind1">The first kind.</param>
        /// <param name="kind2">The second kind.</param>
        /// <returns>True if they are a match, false if not.</returns>
        public static bool MatchingTypes(int kind1, int kind2)
        {
            int textKind = -1;
            int numberKind = -2;
            int boolKind = -3;

            int test1, test2;

            switch (kind1)
            {
                case (int)Token.keywords.STRING:
                case (int)Token.keywords.ACTUAL_STRING:
                    test1 = textKind;
                    break;
                case (int)Token.keywords.NUM:
                case (int)Token.keywords.NUMBER:
                    test1 = numberKind;
                    break;
                case (int)Token.keywords.BOOL:
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                    test1 = boolKind;
                    break;
                default:
                    test1 = kind1;
                    break;
            }

            switch (kind2)
            {
                case (int)Token.keywords.STRING:
                case (int)Token.keywords.ACTUAL_STRING:
                    test2 = textKind;
                    break;
                case (int)Token.keywords.NUM:
                case (int)Token.keywords.NUMBER:
                    test2 = numberKind;
                    break;
                case (int)Token.keywords.BOOL:
                case (int)Token.keywords.TRUE:
                case (int)Token.keywords.FALSE:
                    test2 = boolKind;
                    break;
                default:
                    test2 = kind2;
                    break;
            }

            if (test1 == test2)
            {
                return true;
            }
            else
            {
                return false;
            }
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

            List<Attributes> props = new List<Attributes>();
            
            // Methods and overloads that only take an agent as input:
            Input agentInput = new Input();
            Token agentToken = new Token((int)Token.keywords.AGENT, "agent", -1, -1);
            agentInput.firstVar = new Identifier(agentToken);

            // Add an agent to a team:
            AddAgentToTeam addAgentToTeam1 = new AddAgentToTeam(
                agentInput, "add", (int)Token.keywords.TEAM);

            // Add an agent to a squad:
            AddAgentToSquad addAgentToSquad1 = new AddAgentToSquad(
                agentInput, "add", (int)Token.keywords.SQUAD);

            // Methods and overloads that only take a string as input:
            Input stringInput = new Input();
            Token stringToken = new Token((int)Token.keywords.ACTUAL_STRING, "string", -1, -1);
            
            // Add an action to an actionpattern:
            stringToken.spelling = "action (string)";
            stringInput.firstVar = new Identifier(stringToken);
            AddActionToActionPattern addActionToAP1 = new AddActionToActionPattern(
                stringInput, "add", (int)Token.keywords.ACTION_PATTERN);

            // Squad constructor:
            Attributes squadProp = new Attributes();
            squadProp.ident = "name";
            squadProp.kind = (int)Token.keywords.STRING;
            props.Add(squadProp);

            stringToken.spelling = "name (string)";
            stringInput.firstVar = new Identifier(stringToken);
            squadConstructor squadConstructor1 = new squadConstructor(
                stringInput, (int)Token.keywords.SQUAD, new List<Attributes>(props));
            props.Clear();

            // Team constructor:
            Attributes teamProp1 = new Attributes();
            teamProp1.ident = "name";
            teamProp1.kind = (int)Token.keywords.STRING;
            props.Add(teamProp1);
            Attributes teamProp2 = new Attributes();
            teamProp2.ident = "color";
            teamProp2.kind = (int)Token.keywords.STRING;
            props.Add(teamProp2);

            stringToken.spelling = "name (string)";
            stringInput.firstVar = new Identifier(stringToken);
            teamConstructor teamConstructor1 = new teamConstructor(
                stringInput, (int)Token.keywords.TEAM, new List<Attributes>(props));
            props.Clear();

            // Action pattern constructor:
            Attributes actionPatternProp = new Attributes();
            actionPatternProp.ident = "name";
            actionPatternProp.kind = (int)Token.keywords.STRING;
            props.Add(actionPatternProp);

            stringToken.spelling = "name (string)";
            stringInput.firstVar = new Identifier(stringToken);
            actionPatternConstructor apConstructor1 = new actionPatternConstructor(
                stringInput, (int)Token.keywords.ACTION_PATTERN, new List<Attributes>(props));
            props.Clear();

            // Methods and overloads that take a string and number as input:
            Input stringNumberInput = new Input();
            Token numberToken = new Token((int)Token.keywords.NUMBER, "number", -1, -1);
            stringNumberInput.nextVar = new Input();
            
            // Agent constructor:
            Attributes agentProp = new Attributes();
            agentProp.ident = "name";
            agentProp.kind = (int)Token.keywords.STRING;
            props.Add(agentProp);

            stringToken.spelling = "name (string)";
            stringNumberInput.firstVar = new Identifier(stringToken);
            numberToken.spelling = "rank (number)";
            stringNumberInput.nextVar.firstVar = new Identifier(numberToken);
            agentStringConstructor agentConstructor1 = new agentStringConstructor(
                stringNumberInput, (int)Token.keywords.AGENT, new List<Attributes>(props));
            props.Clear();

            // Methods and overloads that take two strings as input:
            Input stringStringInput = new Input();
            stringStringInput.nextVar = new Input();
            
            // Team constructor:
            props.Add(teamProp1);
            props.Add(teamProp2);

            stringToken.spelling = "name (string)";
            stringStringInput.firstVar = new Identifier(stringToken);
            stringToken.spelling = "color (string - hex)";
            stringStringInput.nextVar.firstVar = new Identifier(stringToken);
            teamConstructor teamConstructor2 = new teamConstructor(
                stringStringInput, (int)Token.keywords.TEAM, new List<Attributes>(props));
            props.Clear();

            // Methods and overloads that take a string, a number and a team as input:
            Input stringNumTeamInput = new Input();
            stringNumTeamInput.nextVar = new Input();
            stringNumTeamInput.nextVar.nextVar = new Input();
            Token teamToken = new Token((int)Token.keywords.TEAM, "team", -1, -1);

            // Agent constructor:
            props.Add(agentProp);

            stringToken.spelling = "name (string)";
            stringNumTeamInput.firstVar = new Identifier(stringToken);
            numberToken.spelling = "rank (number)";
            stringNumTeamInput.nextVar.firstVar = new Identifier(numberToken);
            stringNumTeamInput.nextVar.nextVar.firstVar = new Identifier(teamToken);
            agentStringTeamConstructor agentConstructor2 = new agentStringTeamConstructor(
                stringNumTeamInput, (int)Token.keywords.AGENT, new List<Attributes>(props));
            props.Clear();
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
            InputValidationVisitor i = new InputValidationVisitor();
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

        public abstract string PrintGeneratedCode(string one, string two);
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
        /// <param name="two">Input.</param>
        /// <returns>two.team = one</returns>
        public override string PrintGeneratedCode(string one, string two)
        {
            // two.team = one
            return two.ToLower() + ".team = " + one.ToLower();
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
        /// <param name="two">Input.</param>
        /// <returns>one.agents.add(two)</returns>
        public override string PrintGeneratedCode(string one, string two)
        {
            // one.agents.add(two)
            return one.ToLower() + ".Agents.Add(" + two.ToLower() + ")";
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
        /// <param name="two">Input.</param>
        /// <returns>actionpattern.add</returns>
        public override string PrintGeneratedCode(string one, string two)
        {
            // actionpattern.add(two)
            return one.ToLower() + ".actions.Add(" + two.ToLower() + ")";
        }
    }

    public abstract class MASConstructor
    {
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

        protected List<Attributes> _properties = new List<Attributes>();

        public List<Attributes> Properties
        {
            get { return _properties; }
        }

        public MASConstructor(Input input, int objectKind)
        {
            this._objectKind = objectKind;
            // ID and overloadID are found by going through the list of constructors.
            this._overloadID = MASLibrary.FindConstructor(objectKind).Count;
            this._id = MASLibrary.ConstructorLibrary.Count;
            this._validInput = input;

            // The printValidInput string is generated from the given input.
            Token temp;
            Input current = input;
            InputValidationVisitor i = new InputValidationVisitor();
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

        public MASConstructor(Input input, int objectKind, List<Attributes> properties) 
            : this(input, objectKind)
        {
            this._properties = properties;
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

        public void InstantiateProperties(string name)
        {
            foreach (Attributes a in _properties)
            {
                IdentificationTable.enter(a.kind, name + "." + a.ident);
                IdentificationTable.use(name + "." + a.ident);
            }
        }

        public abstract string PrintGeneratedCode(string name, string input);
    }

    class agentStringConstructor : MASConstructor, ICodeTemplate
    {
        public agentStringConstructor(Input input, int useWith) 
            : base(input, useWith)
        { }

        public agentStringConstructor(Input input, int useWith, List<Attributes> properties)
            : base(input, useWith, properties)
        { }

        /// <summary>
        /// Generates C# code to add an agent to a team.
        /// </summary>
        /// <param name="one">Name of the agent.</param>
        /// <param name="one">Input.</param>
        /// <returns>agent one = new agent(two)</returns>
        public override string PrintGeneratedCode(string one, string two)
        {
            // agent one = new agent(two)
            return "agent " + one.ToLower() + " = new agent(" + two.ToLower() + ")";
        }
    }

    class agentStringTeamConstructor : MASConstructor, ICodeTemplate
    {
        public agentStringTeamConstructor(Input input, int useWith)
            : base(input, useWith)
        { }

        public agentStringTeamConstructor(Input input, int useWith, List<Attributes> properties)
            : base(input, useWith, properties)
        { }

        /// <summary>
        /// Generates C# code to add an agent to a team.
        /// </summary>
        /// <param name="one">Name of the agent.</param>
        /// <param name="one">Input.</param>
        /// <returns>agent one = new agent(two)</returns>
        public override string PrintGeneratedCode(string one, string two)
        {
            string[] input = two.Split(',');
            // agent one = new agent(two);
            // one.team = two
            return "agent " + one.ToLower().Trim() + " = new agent(" +
                input[0].ToLower().Trim() + ", " + input[1].ToLower().Trim() + ");\n" + 
                one.ToLower().Trim() + ".team = " + input[2].ToLower().Trim();
        }
    }

    class squadConstructor : MASConstructor, ICodeTemplate
    {
        public squadConstructor(Input input, int useWith) : base(input, useWith)
        { }

        public squadConstructor(Input input, int useWith, List<Attributes> properties)
            : base(input, useWith, properties)
        { }

        /// <summary>
        /// Generates C# code to add an agent to a team.
        /// </summary>
        /// <param name="one">Name of the squad.</param>
        /// <returns>squad one = new squad(two)</returns>
        public override string PrintGeneratedCode(string one, string two)
        {
            // squad one = new squad(two)
            return "squad " + one.ToLower() + " = new squad(" + two.ToLower() + ")";
        }
    }

    class teamConstructor : MASConstructor, ICodeTemplate
    {
        public teamConstructor(Input input, int useWith) 
            : base(input, useWith)
        { }

        public teamConstructor(Input input, int useWith, List<Attributes> properties)
            : base(input, useWith, properties)
        { }

        /// <summary>
        /// Generates C# code to add an agent to a team.
        /// </summary>
        /// <param name="one">Name of the team.</param>
        /// <param name="two">Input.</param>
        /// <returns>team one = new team</returns>
        public override string PrintGeneratedCode(string one, string two)
        {
            // team one = new team
            return "team " + one.ToLower() + " = new team(" + two.ToLower() + ")";
        }
    }

    class actionPatternConstructor : MASConstructor, ICodeTemplate
    {
        public actionPatternConstructor(Input input, int useWith) 
            : base(input, useWith)
        { }

        public actionPatternConstructor(Input input, int useWith, List<Attributes> properties)
            : base(input, useWith, properties)
        { }

        /// <summary>
        /// Generates C# code to add an agent to a team.
        /// </summary>
        /// <param name="one">Name of the actionpattern.</param>
        /// <param name="two">Input.</param>
        /// <returns>actionpattern one = new actionpattern</returns>
        public override string PrintGeneratedCode(string one, string two)
        {
            // actionpattern one = new actionpattern
            return "actionpattern " + one.ToLower() + " = new actionpattern(" + two.ToLower() + ")";
        }
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
