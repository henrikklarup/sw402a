using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MASClassLibrary
{
    public class team
    {
        private int _iD;
        public string name;
        public Color color;

        public int id
        {
            get { return _iD; }
        }

        public int AvailableRank
        {
            get { return Lists.Points / Lists.teams.Count; }
        }

        public team()
        { }

        public team(string name)
        {
            this._iD = Lists.NextTeamID;
            this.name = name;
            this.color = Color.Green;
            Lists.teams.Add(this);
        }

        public team(string name, string color)
        {
            this.name = name;
            this.color = ColorTranslator.FromHtml(color);
            Lists.teams.Add(this);
        }

        public team(int ID, string name, string color)
        {
            this._iD = ID;
            this.name = name;
            this.color = ColorTranslator.FromHtml(color);
        }

        public void add(agent Agent)
        {
            Agent.team = this;
        }
    }
}
