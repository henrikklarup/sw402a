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
        public String colorStr;

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
            this.color = ColorTranslator.FromHtml("#00ff00");
            Lists.teams.Add(this);
        }

        public team(string name, string color)
        {
            this._iD = Lists.NextTeamID;
            if (Lists.RetrieveTeam(name) != null)
            {
                this.name = name + this._iD;
            }
            else
            {
                this.name = name;
            }
            this.color = ColorTranslator.FromHtml(color);
            this.colorStr = color;
            Lists.teams.Add(this);
        }

        public team(int ID, string name, string color)
        {
            this._iD = ID;
            this.name = name;
            this.color = ColorTranslator.FromHtml(color);
            this.colorStr = color;
        }
    }
}
