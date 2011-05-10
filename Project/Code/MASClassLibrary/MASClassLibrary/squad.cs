using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class squad
    {
        private int _iD;
        public string name;
        public List<agent> Agents;

        public int id
        {
            get { return _iD; }
        }

        public squad()
        { }

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

        public squad(string name, List<agent> list) : this(name)
        {
            this.Agents = list;
        }
    }
}
