using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class Agent
    {
        public int ID;
        public int posX;
        public int posY;
        public string name;
        public int rank;
        public Team team;

        public Agent()
        { }

        public Agent(int ID, string name, int rank, Team team)
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
