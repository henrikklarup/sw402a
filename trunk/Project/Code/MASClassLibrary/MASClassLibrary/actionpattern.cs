using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class actionpattern
    {
        public int ID;
        public string[] actions;
        public string name;

        public actionpattern()
        { }

        public actionpattern(int ID, string[] actions)
        {
            this.ID = ID;
            this.actions = actions;
        }

        public actionpattern(int ID, string action)
        {
            string[] actionString = new string[1];
            actionString[0] = action;

            this.ID = ID;
            this.actions = actionString;
        }
    }
}
