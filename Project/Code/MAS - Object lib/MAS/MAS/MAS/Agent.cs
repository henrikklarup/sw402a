using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAS
{
    public class Agent
    {
        private static int IDcount = 0;
        public int ID;
        public int posX;
        public int posY;
        public string name;
        public int rank;
        public Team team;

        public Agent()
        { }

        public Agent(string name, int rank, Team team, int posX, int posY)
        {
            IDcount++;
            this.ID = IDcount;
            this.name = name;
            this.rank = rank;
            this.posX = posX;
            this.posY = posY;
            this.team = team;

        }

        public Agent(string name, int rank, Team team)
        {
            IDcount++;
            this.ID = IDcount;
            this.name = name;
            this.rank = rank;
            this.team = team;
            this.posX = -1;
            this.posY = -1;

        }

        public Agent(int Id, string name, int rank, Team team, int posX, int posY)
        {
            this.ID = Id;
            this.name = name;
            this.rank = rank;
            this.posX = posX;
            this.posY = posY;
            this.team = team;

        }

        public Agent(int Id, string name, int rank, Team team)
        {
            this.ID = Id;
            this.name = name;
            this.rank = rank;
            this.team = team;
            this.posX = -1;
            this.posY = -1;

        }
    }
}
