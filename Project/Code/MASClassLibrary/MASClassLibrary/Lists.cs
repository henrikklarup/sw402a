using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public static class Lists
    {
        public static List<agent> agents;                 //List of agents
        public static List<actionpattern> actionPatterns; //List of actionPatterns
        public static List<squad> squads;                 //List of squads
        public static List<team> teams;                   //List of teams
        public static List<agent> moveagents;             //List of agents to move
        public static team currentteam;                   //Current team

        public static int Points;

        #region RetrieveAgent
        /// <summary>
        /// Finds an agent by its name.
        /// </summary>
        /// <param name="ident">Name of an agent</param>
        /// <returns>agent</returns>
        public static agent RetrieveAgent(string ident)
        {
            List<agent> results;
            agent agent;

            results = agents.FindAll(delegate(agent a) { return a.name.ToLower() == ident.ToLower(); });
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
        /// <returns>agent</returns>
        public static agent RetrieveAgent(int ident)
        {
            List<agent> results;
            agent agent;

            if (agents != null)
            {
                results = agents.FindAll(delegate(agent a) { return a.id == ident; });
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
        /// Finds a team by its name.
        /// </summary>
        /// <param name="ident">Name of a team</param>
        /// <returns>team</returns>
        public static team RetrieveTeam(string ident)
        {
            List<team> results;
            team team;

            results = teams.FindAll(delegate(team t)
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
        /// <returns>team</returns>
        public static team RetrieveTeam(int ident)
        {
            List<team> results;
            team team;

            results = teams.FindAll(delegate(team t) { return t.id == ident; });
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
        public static List<agent> RetrieveAgentsByteam(team team)
        {
            return agents.FindAll(delegate(agent a) { return a.team == team; });
        }
        #endregion

        #region RetrieveSquad
        /// <summary>
        /// Finds a squad by its name.
        /// </summary>
        /// <param name="ident">Name of an squad</param>
        /// <returns>Squad</returns>
        public static squad RetrieveSquad(string ident)
        {
            List<squad> results;
            squad squad;

            results = squads.FindAll(delegate(squad s)
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
        public static squad RetrieveSquad(int ident)
        {
            List<squad> results;
            squad squad;

            results = squads.FindAll(delegate(squad s)
            {
                if (s.name == null)
                    return false;
                return s.id == ident;
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
        public static actionpattern RetrieveActionPattern(string ident)
        {
            List<actionpattern> results;
            actionpattern actionPattern;
            results = actionPatterns.FindAll(delegate(actionpattern ap)
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
        #endregion

        #region IDs

        /// <summary>
        /// Finds the next unused agent id.
        /// </summary>
        public static int NextAgentID
        {
            get { return agents.Count + 1; }
        }

        /// <summary>
        /// Finds the next unused squad id.
        /// </summary>
        public static int NextSquadID
        {
            get { return squads.Count + 1; }
        }

        /// <summary>
        /// Finds the next unused team id.
        /// </summary>
        public static int NextTeamID
        {
            get { return teams.Count + 1; }
        }
        #endregion
    }
}