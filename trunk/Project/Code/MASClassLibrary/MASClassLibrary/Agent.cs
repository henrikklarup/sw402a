using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class agent
    {
        private int _iD;
        public string name;
        public team team;
        private int _posX;
        private int _posY;
        public double rank;

        public int id
        {
            get { return _iD; }
        }

        public int posx
        {
            get { return _posX; }
            set { _posX = value; }
        }

        public int posy
        {
            get { return _posY; }
            set { _posY = value; }
        }

        public agent()
        { }

        public agent(string name, double rank)
        {
            this._iD = Lists.NextAgentID;
            if (Lists.RetrieveAgent(name) != null)
            {
                this.name = name + this._iD;
            }
            else
            {
                this.name = name;
            }
            this.rank = rank;
            this._posX = 0;
            this._posY = 0;
            Lists.agents.Add(this);
        }

        public agent(int id, string name, double rank, team team)
        {
            this.name = name;
            this.rank = rank;
            this._posX = 0;
            this._posY = 0;
            this.team = team;
            this._iD = id;
        }

        public agent(string name, double rank, team team)
        {
            this.name = name;
            this.rank = rank;
            this._posX = 0;
            this._posY = 0;
            this.team = team;
            this._iD = Lists.NextAgentID;
            Lists.agents.Add(this);
        }
    }
}
