using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class actionpattern
    {
        public int ID;
        public List<string> actions;
        public string name;

        public actionpattern()
        { }

        public actionpattern(int ID, string[] actions)
        {
            this.ID = ID;
            this.actions = new List<string>();
            foreach (string s in actions)
            {
                this.actions.Add(s);
            }
        }

        public actionpattern(int ID, string action)
        {
            this.ID = ID;
            this.actions = new List<string>();
            this.actions.Add(action);
        }
    }
}
