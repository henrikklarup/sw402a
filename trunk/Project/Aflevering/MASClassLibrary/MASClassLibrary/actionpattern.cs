using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class actionpattern
    {
        public List<string> actions;
        public string name;

        /// <summary>
        /// Default constructor
        /// </summary>
        public actionpattern()
        { }

        /// <summary>
        /// Constructor for actionPattern
        /// </summary>
        /// <param name="name">Name of actionpattern</param>
        /// <param name="actions">Actions in actionpattern</param>
        public actionpattern(string name, List<string> actions) : this (name)
        {
            foreach (string s in actions)
            {
                this.actions.Add(s);
            }
        }

        /// <summary>
        /// Constructor for actionPattern
        /// </summary>
        /// <param name="name">Name of actionpattern</param>
        /// <param name="action">Action in actionpattern</param>
        public actionpattern(string name, string action) : this (name)
        {
            this.actions.Add(action);
        }

        /// <summary>
        /// Constructor for actionPattern
        /// </summary>
        /// <param name="name">Name of actionpattern</param>
        public actionpattern(string name)
        {
            if (Lists.RetrieveSquad(name) != null)
            {
                this.name = name + Lists.actionPatterns.Count;
            }
            else
            {
                this.name = name;
            }
            this.actions = new List<string>();
            Lists.actionPatterns.Add(this);
        }
    }
}
