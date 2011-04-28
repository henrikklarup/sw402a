using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAS
{
    public class ActionPattern
    {
        private static int IDcount = 0;
        public int ID;
        public string[] actions;

        public ActionPattern()
        { }

        public ActionPattern(string[] actions)
        {
            IDcount++;
            this.ID = IDcount;
            this.actions = actions;
        }

        public ActionPattern(string action)
        {
            string[] actionString = new string[1];
            actionString[0] = action;

            IDcount++;
            this.ID = IDcount;
            this.actions = actionString;
        }
    }
}
