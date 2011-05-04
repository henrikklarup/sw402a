using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public class agent
    {
        //public int ID;
        //public int posX;
        //public int posY;
        //public string name;
        //public int rank;
        //public team team;

        private int _iD;
        private string _name;
        private team _team;
        private int _posX;
        private int _posY;
        private int _rank;

        public int id
        {
            get { return _iD; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public team team
        {
            get { return _team; }
            set { _team = value; }
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

        public int rank
        {
            get { return _rank; }
        }

        public agent()
        { }

        public agent(string name, int rank)
        {
            this._name = name;
            this._rank = rank;
            this._posX = 0;
            this._posY = 0;
            Lists.agents.Add(this);
        }

        public agent(int id, string name, int rank, team team)
        {
            this._name = name;
            this._rank = rank;
            this._posX = 0;
            this._posY = 0;
            this._team = team;
            this._iD = id;
        }
    }
}
