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

        public actionpattern()
        { }

        public actionpattern(string name, List<string> actions) : this (name)
        {
            foreach (string s in actions)
            {
                this.actions.Add(s);
            }
        }

        public actionpattern(string name, string action) : this (name)
        {
            this.actions.Add(action);
        }

        public actionpattern(string name)
        {
            this.name = name;
            this.actions = new List<string>();
            Lists.actionPatterns.Add(this);
        }
    }
}
