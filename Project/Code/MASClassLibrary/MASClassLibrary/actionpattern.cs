using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class actionpattern
    {
        public List<string> actions;
        private string _name;

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public actionpattern()
        { }

        public actionpattern(string name, List<string> actions)
        {
            this._name = name;
            this.actions = new List<string>();
            foreach (string s in actions)
            {
                this.actions.Add(s);
            }
            Lists.actionPatterns.Add(this);
        }

        public actionpattern(string name, string action)
        {
            this._name = name;
            this.actions = new List<string>();
            this.actions.Add(action);
            Lists.actionPatterns.Add(this);
        }
    }
}
