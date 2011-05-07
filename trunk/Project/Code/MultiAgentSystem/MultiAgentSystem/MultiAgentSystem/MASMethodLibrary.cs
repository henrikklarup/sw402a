using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAgentSystem
{
    public static class MASMethodLibrary
    {
        public static List<MASMethod> MethodLibrary = new List<MASMethod>();

        /// <summary>
        /// Finds the method in the library of methods by matching the name.
        /// </summary>
        /// <param name="name">Name of the method to find.</param>
        /// <returns>A MASMethod or null if no match was found.</returns>
        public MASMethod FindMethod(string name)
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
    class AddAgentToTeam : MASMethod
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
    }

    abstract class MASMethod : ICodeTemplate
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

        // It is set to return null by default for methods, 
        // and then it can be changed by the individual methods if they require input.
        public override Input GenerateValidInput()
        {
            return null;
        }
    }

    interface ICodeTemplate
    {
        /// <summary>
        /// Prints the generated code for this method.
        /// </summary>
        /// <returns>A string containing the code for this method.</returns>
        public virtual string PrintGeneratedCode();

        /// <summary>
        /// A method that returns a dummy input that can be used to check for correct input with.
        /// </summary>
        /// <returns>Input with the correct properties.</returns>
        public virtual Input GenerateValidInput();

        /// <summary>
        /// A method that prints an errormessage for when the object isn't used correctly, 
        /// and specifies how it should be used.
        /// </summary>
        /// <returns>A string containing the errormessage.</returns>
        public virtual string PrintInvalidErrorMessage();
    }
}
