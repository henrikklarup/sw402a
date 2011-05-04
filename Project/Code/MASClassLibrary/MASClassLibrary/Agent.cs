using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class agent
    {
        public int ID;
        public int posX;
        public int posY;
        public string name;
        public int rank;
        public team team;

        public agent()
        { }

        public agent(int ID, string name, int rank, team team)
        {
            this.ID = ID;
            this.name = name;
            this.rank = rank;
            this.posX = 0;
            this.posY = 0;
            this.team = team;

        }
    }
}
