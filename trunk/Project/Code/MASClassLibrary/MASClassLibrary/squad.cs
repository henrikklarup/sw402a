using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class squad
    {
        #region Properties
        private int _iD;
        public string name;
        public List<agent> Agents;
        #endregion

        #region Get/Set
        public int id
        {
            get { return _iD; }
        }
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public squad()
        { }

        /// <summary>
        /// Constructor for squad
        /// </summary>
        /// <param name="name">Name of squad</param>
        public squad(string name)
        {
            this._iD = Lists.NextSquadID;
            if (Lists.RetrieveSquad(name) != null)
            {
                this.name = name + this._iD;
            }
            else
            {
                this.name = name;
            }
            this.Agents = new List<agent>();
            Lists.squads.Add(this);
        }

        /// <summary>
        /// Constructor for squad
        /// </summary>
        /// <param name="name">Name of squad</param>
        /// <param name="list">List of agents</param>
        public squad(string name, List<agent> list) : this(name)
        {
            this.Agents = list;
        }
    }
}
