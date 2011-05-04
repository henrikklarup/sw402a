using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class team
    {
        public int ID;
        public string name;
        public string color;

        public team()
        { }

        public team(int ID, string name, string color)
        {
            this.ID = ID;
            this.name = name;
            this.color = color;
        }

        public void add(agent Agent)
        {
            Agent.team = this;
        }
    }
}
