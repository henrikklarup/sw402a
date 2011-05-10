using System; 
using System.Drawing; 
using System.Collections.Generic;
using MASClassLibrary;

namespace MultiAgentSystem 
{ 
	class Program 
	{ 
		static void Main(string[] args)
		{ 
			/* Here the program starts. */ 
			Lists.agents = new List<agent>();
			Lists.squads = new List<squad>();
			Lists.teams = new List<team>();
			Lists.actionPatterns = new List<actionpattern>();
			Lists.Points = 400; 
double totalagents = 10; 
team team1 = new team("First Team", "#97f5c8"); 
team team2 = new team("Second Team"); 
squad theSquad = new squad("tove"); 
double n = 1 + 2; 
for (double i = 0; 
i < totalagents; 
i = i + 1)
{
agent newAgent = new agent("tove", 2); 
newAgent.name = "name"; 
theSquad.Agents.Add(newAgent); 
newAgent.team = team1; 
}
team t2 = new team("name"); 
for (double i = 0; 
i < totalagents; 
i = i + 1)
{
agent newAgent = new agent("g", 2); 
newAgent.team = team2; 
}
XML.generateXML(@"C:\Users\Kasper\Desktop\MAS\Files");
}
}
}
