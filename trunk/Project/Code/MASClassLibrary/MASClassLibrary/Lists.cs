using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASClassLibrary
{
    public static class Lists
    {
        public static List<agent> agents;                 //List of agents
        public static List<ActionPattern> actionPatterns; //List of actionPatterns
        public static List<Squad> squads;                 //List of squads
        public static List<team> teams;                   //List of teams
        public static List<agent> moveagents;             //List of agents to move
        public static team currentteam;                   //Current team


        #region Retrieveagent
        /// <summary>
        /// Finds an agent by its name.
        /// </summary>
        /// <param name="ident">Name of an agent</param>
        /// <returns>agent</returns>
        public static agent Retrieveagent(string ident)
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
        public static agent Retrieveagent(int ident)
        {
            List<agent> results;
            agent agent;

            if (agents != null)
            {
                results = agents.FindAll(delegate(agent a) { return a.ID == ident; });
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

        #region Retrieveteam
        /// <summary>
        /// Finds a team by its name.
        /// </summary>
        /// <param name="ident">Name of a team</param>
        /// <returns>team</returns>
        public static team Retrieveteam(string ident)
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
        public static team Retrieveteam(int ident)
        {
            List<team> results;
            team team;

            results = teams.FindAll(delegate(team t) { return t.ID == ident; });
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
        public static List<agent> RetrieveagentsByteam(team team)
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
        public static oldSquad RetrieveSquad(string ident)
        {
            List<oldSquad> results;
            oldSquad squad;

            results = squads.FindAll(delegate(oldSquad s)
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
        public static oldSquad RetrieveSquad(int ident)
        {
            List<oldSquad> results;
            oldSquad squad;

            results = squads.FindAll(delegate(oldSquad s)
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
        public static oldActionPattern RetrieveActionPattern(string ident)
        {
            List<oldActionPattern> results;
            oldActionPattern actionPattern;
            results = actionPatterns.FindAll(delegate(oldActionPattern ap)
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
        public static oldActionPattern RetrieveActionPattern(int ident)
        {
            List<oldActionPattern> results;
            oldActionPattern actionPattern;

            results = actionPatterns.FindAll(delegate(oldActionPattern ap)
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

        #region IDs



        #endregion
    }
}