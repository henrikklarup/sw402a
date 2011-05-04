using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public static class Lists
    {
        public static List<Agent> agents;                 //List of agents
        public static List<ActionPattern> actionPatterns; //List of actionPatterns
        public static List<Squad> squads;                 //List of squads
        public static List<Team> teams;                   //List of teams
        public static List<Agent> moveAgents;             //List of agents to move
        public static Team currentTeam;                   //Current team


        #region RetrieveAgent
        /// <summary>
        /// Finds an agent by its name.
        /// </summary>
        /// <param name="ident">Name of an agent</param>
        /// <returns>Agent</returns>
        public static Agent RetrieveAgent(string ident)
        {
            List<Agent> results;
            Agent agent;

            results = agents.FindAll(delegate(Agent a) { return a.name.ToLower() == ident.ToLower(); });
            if (results.Count != 1)
            {
                return null;
            }
            else
            {
                agent = results.FirstOrDefault();
            }

            return agent;
        }

        /// <summary>
        /// Finds an agent by its ID.
        /// </summary>
        /// <param name="ident">ID of an agent</param>
        /// <returns>Agent</returns>
        public static Agent RetrieveAgent(int ident)
        {
            List<Agent> results;
            Agent agent;

            if (agents != null)
            {
                results = agents.FindAll(delegate(Agent a) { return a.ID == ident; });
                if (results.Count != 1)
                {
                    return null;
                }
                else
                {
                    agent = results.FirstOrDefault();
                }
            }
            else
                return null;

            return agent;
        }
        #endregion

        #region RetrieveTeam
        /// <summary>
        /// Finds a Team by its name.
        /// </summary>
        /// <param name="ident">Name of a Team</param>
        /// <returns>Team</returns>
        public static Team RetrieveTeam(string ident)
        {
            List<Team> results;
            Team team;

            results = teams.FindAll(delegate(Team t)
            {
                if (t.name == null)
                    return false;
                return t.name.ToLower() == ident.ToLower();
            });
            if (results.Count != 1)
            {
                return null;
            }
            else
            {
                team = results.FirstOrDefault();
            }

            return team;
        }

        /// <summary>
        /// Finds team by its ID.
        /// </summary>
        /// <param name="ident">ID of a team</param>
        /// <returns>Team</returns>
        public static Team RetrieveTeam(int ident)
        {
            List<Team> results;
            Team team;

            results = teams.FindAll(delegate(Team t) { return t.ID == ident; });
            if (results.Count != 1)
            {
                return null;
            }
            else
            {
                team = results.FirstOrDefault();
            }

            return team;
        }

        /// <summary>
        /// Finds all agents belonging to a certain team.
        /// </summary>
        /// <param name="team">The agents team.</param>
        /// <returns>a list of agents on a team.</returns>
        public static List<Agent> RetrieveAgentsByTeam(Team team)
        {
            return agents.FindAll(delegate(Agent a) { return a.team == team; });
        }
        #endregion

        #region RetrieveSquad
        /// <summary>
        /// Finds a squad by its name.
        /// </summary>
        /// <param name="ident">Name of an squad</param>
        /// <returns>Squad</returns>
        public static Squad RetrieveSquad(string ident)
        {
            List<Squad> results;
            Squad squad;

            results = squads.FindAll(delegate(Squad s)
            {
                if (s.name == null)
                    return false;
                return s.name.ToLower() == ident.ToLower();
            });
            if (results.Count != 1)
            {
                return null;
            }
            else
            {
                squad = results.FirstOrDefault();
            }

            return squad;
        }

        /// <summary>
        /// Finds a squad by its ID.
        /// </summary>
        /// <param name="ident">ID of an squad</param>
        /// <returns>Squad</returns>
        public static Squad RetrieveSquad(int ident)
        {
            List<Squad> results;
            Squad squad;

            results = squads.FindAll(delegate(Squad s)
            {
                if (s.name == null)
                    return false;
                return s.ID == ident;
            });
            if (results.Count != 1)
            {
                return null;
            }
            else
            {
                squad = results.FirstOrDefault();
            }

            return squad;
        }
        #endregion

        #region RetrieveActionPattern
        /// <summary>
        /// Finds an ActionPattern by its name
        /// </summary>
        /// <param name="ident">Name of the ActionPattern</param>
        /// <returns>ActionPattern</returns>
        public static ActionPattern RetrieveActionPattern(string ident)
        {
            List<ActionPattern> results;
            ActionPattern actionPattern;
            results = actionPatterns.FindAll(delegate(ActionPattern ap)
            {
                if (ap.name == null)
                    return false;
                return ap.name.ToLower() == ident.ToLower();
            });
            if (results.Count != 1)
            {
                return null;
            }
            else
            {
                actionPattern = results.FirstOrDefault();
            }

            return actionPattern;
        }

        /// <summary>
        /// Finds an ActionPattern by its ID
        /// </summary>
        /// <param name="ident">ID of the ActionPattern</param>
        /// <returns>ActionPattern</returns>
        public static ActionPattern RetrieveActionPattern(int ident)
        {
            List<ActionPattern> results;
            ActionPattern actionPattern;

            results = actionPatterns.FindAll(delegate(ActionPattern ap)
            {
                if (ap.name == null)
                    return false;
                return ap.ID == ident;
            });
            if (results.Count != 1)
            {
                return null;
            }
            else
            {
                actionPattern = results.FirstOrDefault();
            }

            return actionPattern;
        }
        #endregion
    }
}