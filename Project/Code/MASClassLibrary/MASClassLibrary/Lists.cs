﻿using System;
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


        #region Retrieve Agent
        /// <summary>
        /// Finds an agent by its name.
        /// </summary>
        /// <param name="ident">Name of an agent</param>
        /// <returns>Agent</returns>
        public static Agent RetrieveAgent(string ident)
        {
            List<Agent> results;
            Agent agent;

            results = agents.FindAll(delegate(Agent a) { return a.name == ident; });
            if (results.Count == 0)
            {
                throw new Exception("Agent " + ident + " does not exists.");
            }
            else if (results.Count != 1)
            {
                throw new Exception("Agent " + ident + " has duplicants.");
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

            results = agents.FindAll(delegate(Agent a) { return a.ID == ident; });
            if (results.Count == 0)
            {
                throw new Exception("Agent " + ident + " does not exists.");
            }
            else if (results.Count != 1)
            {
                throw new Exception("Agent " + ident + " has duplicants.");
            }
            else
            {
                agent = results.FirstOrDefault();
            }

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

            results = teams.FindAll(delegate(Team t) { return t.name == ident; });
            if (results.Count == 0)
            {
                throw new Exception("Team " + ident + " does not exists.");
            }
            else if (results.Count != 1)
            {
                throw new Exception("Team " + ident + " has duplicants.");
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
            if (results.Count == 0)
            {
                throw new Exception("Team " + ident + " does not exists.");
            }
            else if (results.Count != 1)
            {
                throw new Exception("Team " + ident + " has duplicants.");
            }
            else
            {
                team = results.FirstOrDefault();
            }

            return team;
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

            results = squads.FindAll(delegate(Squad s) { return s.name == ident; });
            if (results.Count == 0)
            {
                throw new Exception("Squad " + ident + " does not exists.");
            }
            else if (results.Count != 1)
            {
                throw new Exception("Squad " + ident + " has duplicants.");
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

            results = squads.FindAll(delegate(Squad s) { return s.ID == ident; });
            if (results.Count == 0)
            {
                throw new Exception("Squad " + ident + " does not exists.");
            }
            else if (results.Count != 1)
            {
                throw new Exception("Squad " + ident + " has duplicants.");
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

            results = actionPatterns.FindAll(delegate(ActionPattern ap) { return ap.name == ident; });
            if (results.Count == 0)
            {
                throw new Exception("Action Pattern " + ident + " does not exists.");
            }
            else if (results.Count != 1)
            {
                throw new Exception("Action Pattern " + ident + " has duplicants.");
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

            results = actionPatterns.FindAll(delegate(ActionPattern ap) { return ap.ID == ident; });
            if (results.Count == 0)
            {
                throw new Exception("Action Pattern " + ident + " does not exists.");
            }
            else if (results.Count != 1)
            {
                throw new Exception("Action Pattern " + ident + " has duplicants.");
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