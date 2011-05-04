using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class squad
    {
        private int _iD;
        private string _name;

        public int id
        {
            get { return _iD; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<agent> Agents;

        public squad()
        { }

        public squad(string name)
        {
            this._name = name;
            Agents = new List<agent>();
            this._iD = Lists.NextSquadID;
        }

        public squad(string name, List<agent> list) : this(name)
        {
            Agents = list;
        }

        public void add(agent agent)
        {
            Agents.Add(agent);
        }
    }
}
