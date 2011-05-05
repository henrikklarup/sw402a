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
        private string _name;
        private Color _color;

        public int id
        {
            get { return _iD; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Color color
        {
            get { return _color; }
            set { _color = value; }
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
            this._name = name;
            this._color = Color.Green;
        }

        public team(string name, string color)
        {
            this.name = name;
            this._color = ColorTranslator.FromHtml(color);
        }

        public team(int ID, string name, string color)
        {
            this._iD = ID;
            this.name = name;
            this._color = ColorTranslator.FromHtml(color);
        }

        public void add(agent Agent)
        {
            Agent.team = this;
        }
    }
}
