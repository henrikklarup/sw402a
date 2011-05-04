using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MASClassLibrary;

namespace ListToXML
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("(R)ead or (S)ave lists: ");

            //Initializing the lists
            Lists.agents = new List<agent>();
            Lists.teams = new List<team>();
            Lists.squads = new List<squad>();
            Lists.actionPatterns = new List<actionpattern>();

            team _team = new team(1, "Team one", "Red");
            agent _agent = new agent(1, "Olsen", 2, _team);
            squad _squad = new squad("first squad");
            _squad.Agents.Add(_agent);
            actionpattern aP = new actionpattern(1, "first action pattern");

            Lists.teams.Add(_team);
            Lists.agents.Add(_agent);
            Lists.actionPatterns.Add(aP);
            Lists.squads.Add(_squad);

            XML.path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //Interface to test saving and loading the xml files
            ConsoleKeyInfo cki = Console.ReadKey();

            while (cki.Key != ConsoleKey.Escape)
            {
                if (cki.Key == ConsoleKey.S)
                {
                    XML.generateXML();
                    Console.WriteLine("Saved");
                }
                if (cki.Key == ConsoleKey.R)
                {
                    XML.returnLists();
                    Console.WriteLine("Read");
                }
                cki = Console.ReadKey();
            }
        }

    }
}
