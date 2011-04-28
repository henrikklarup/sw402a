using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAS
{
    public class Team
    {
        private static int IDcount;
        public int ID;
        public string name;
        public string color;

        public Team()
        { }

        public Team(string name, string color)
        {
            IDcount++;
            this.ID = IDcount;
            this.name = name;
            this.color = color;
        }
    }
}
