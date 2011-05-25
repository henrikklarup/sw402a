using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class agent
    {
        #region Properties
        private int _iD;
        public string name;
        public team team;
        private int _posX;
        private int _posY;
        public double rank;
        #endregion

        #region Get/Set
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
        #endregion

        /// <summary>
        /// Constructor for agent
        /// </summary>
        public agent()
        { }

        /// <summary>
        /// Constructor for agent
        /// </summary>
        /// <param name="name">Name of agent</param>
        /// <param name="rank">Rank of agent</param>
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

        /// <summary>
        /// Constructor for agent
        /// </summary>
        /// <param name="id">Id of agent</param>
        /// <param name="name">Name of agent</param>
        /// <param name="rank">Rank of agent</param>
        /// <param name="team">Team of agent</param>
        public agent(int id, string name, double rank, team team)
        {
            this.name = name;
            this.rank = rank;
            this._posX = 0;
            this._posY = 0;
            this.team = team;
            this._iD = id;
        }

        /// <summary>
        /// Constructor for agent
        /// </summary>
        /// <param name="name">Name of agent</param>
        /// <param name="rank">Rank of agent</param>
        /// <param name="team">Team of agent</param>
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
