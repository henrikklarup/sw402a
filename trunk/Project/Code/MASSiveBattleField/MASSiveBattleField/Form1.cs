using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using ActionInterpeter;
using MASClassLibrary;
using System.Threading;
using System.Runtime.InteropServices;

namespace MASSiveBattleField
{
    public partial class WarGame : Form
    {
        #region Props
        Point mousePointGrid;               //Point for mouse grid point
        Size GridSize;                      //GridSize height, width
        Color LineColor;                    //Grid line color
        Color backGroundColor;              //Background color
        int LineWidth;                      //Grid line width
        int Grids;                          //Number of grids. I.e. 13 = 13 x 13 grid
        int turn;                           //Turnswitch
        agent selectedagent;                //Selected agent
        #endregion

        #region Constructor
        public WarGame()
        {

            InitializeComponent();      //Initialize Components

            #region Initi props

            #region GameSettings Dialog
            GameSettings gms = new GameSettings();
            gms.ShowDialog();
            #endregion

            #region Grids
            //Small = 13
            //Medium = 26
            //Large = 46
            Grids = gms.gridSize;
            #endregion

            //Linewidth default 2
            LineWidth = 2;
            //Empty mousepoint
            mousePointGrid = new Point(0, 0);
            //Line color default black
            LineColor = Color.Black;
            //Background color default army green
            backGroundColor = Color.FromArgb(102,153,102);

            //GridSize x,y: (((Width - (2*Lw)) - ((Grids - 1) * lw)) / Grids)
            GridSize = new Size((((dbPanel1.Width - (2 * LineWidth)) - ((Grids - 1) * LineWidth)) / Grids), (((dbPanel1.Height - (2 * LineWidth)) - ((Grids - 1) * LineWidth)) / Grids));

            Lists.moveagents = new List<agent>();
            selectedagent = new agent();
            Lists.teams = new List<team>();
            Lists.agents = new List<agent>();
            Lists.actionPatterns = new List<actionpattern>();
            Lists.squads = new List<squad>();
            Lists.encounters = new List<encounter>();
            #endregion

            #region Folder Browser Dialog
            //Xml-path choosen
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Generate xml data
                XML.returnLists(fbd.SelectedPath);
                placeteams();
            }
            #endregion

            //Turnswitch set to random
            Random rnd = new Random();
            turn = rnd.Next(1,Lists.teams.Count);
            Lists.currentteam = Lists.RetrieveTeam(turn);
            label6.Text = "Team " + turn;

            textBox4.AppendText("WarGame Console");

            DrawTimer.Enabled = true;       //Enable the timer
        }
        #endregion

        #region Paint Event
        private void dbPanel1_Paint(object sender, PaintEventArgs e)
        {
            //Clear Screen to Color
            e.Graphics.Clear(backGroundColor);

            //Draw Grid
            #region DrawGrid
            e.Graphics.DrawLine(new Pen(LineColor, LineWidth), new Point(0, LineWidth / 2), new Point(dbPanel1.Width, LineWidth / 2));
            e.Graphics.DrawLine(new Pen(LineColor, LineWidth), new Point(LineWidth / 2, 0), new Point(LineWidth / 2, dbPanel1.Height));


            //((GridSize.Width+LineWidth)*Grids) + (GridSize.Width + 2*LineWidth)
            //eller
            //dbPanel1.Width


            for (int i = GridSize.Width + LineWidth; i < dbPanel1.Width; i += GridSize.Width + LineWidth)
            {
                e.Graphics.DrawLine(new Pen(LineColor, LineWidth), new Point(i + (LineWidth / 2), LineWidth), new Point(i + (LineWidth / 2), dbPanel1.Height));
            }
            for (int i = GridSize.Height + LineWidth; i < dbPanel1.Height; i += GridSize.Height + LineWidth)
            {
                e.Graphics.DrawLine(new Pen(LineColor, LineWidth), new Point(LineWidth, i + (LineWidth / 2)), new Point(dbPanel1.Width, i + (LineWidth / 2)));
            }
            #endregion

            //Draw Figure
            #region Draw Soldiers
            foreach (agent a in Lists.agents)
            {
                //Calculate the pixels of the x,y from the agent
                Point drawPoint = new Point(a.posx, a.posy);
                drawPoint = getGridPixelFromGrid(drawPoint);

                //Makeup the agent drawRectangle
                Rectangle drawRect = new Rectangle(drawPoint.X, drawPoint.Y, GridSize.Width - LineWidth + 1, GridSize.Height - LineWidth + 1);

                //Make the font for drawing text
                FontFamily ff = new FontFamily("Arial");
                float fontSizePixel = drawRect.Width;
                Font fnt = new System.Drawing.Font(ff, fontSizePixel, FontStyle.Regular, GraphicsUnit.Pixel);

                //Font rankFont = new System.Drawing.Font(e.Graphics.MeasureString(a.rank.ToString(),new Font(Font,FontStyle.Regular)), FontStyle.Regular);
                e.Graphics.FillEllipse(new SolidBrush(a.team.color), drawRect);
                e.Graphics.DrawEllipse(Pens.White, new Rectangle(drawPoint.X, drawPoint.Y, GridSize.Width - LineWidth + 1, GridSize.Height - LineWidth + 1));
                e.Graphics.DrawString(a.rank.ToString(), fnt, Brushes.Black, new PointF(drawPoint.X, drawPoint.Y));


                //Destination point of agent
                #region Despoint
                int desPointCount = 0;
                foreach (agent aa in Lists.moveagents)
                {
                    if (aa.team.id == Lists.currentteam.id)
                    {
                        if (aa.id == selectedagent.id)
                        {
                            Point desPoint = new Point(aa.posx, aa.posy);
                            desPoint = getGridPixelFromGrid(desPoint);
                            e.Graphics.DrawEllipse(Pens.LightBlue, new Rectangle(desPoint.X, desPoint.Y, GridSize.Width - LineWidth + 1, GridSize.Height - LineWidth + 1));
                            desPointCount++;
                            e.Graphics.DrawString(desPointCount.ToString(), fnt, Brushes.Black, new PointF(desPoint.X, desPoint.Y));
                        }
                    }
                }
                #endregion
            }
            #endregion
        }
        #endregion

        #region Timers

        #region DrawTimer Tick
        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            //Update progress
            #region Progress
            int agentsOnteam1 = 0;
            int agentsOnteam2 = 0;
            int agentsOnteam3 = 0;
            int agentsOnteam4 = 0;
            foreach (agent a in Lists.agents)
            {
                if (a.team.id == 1)
                    agentsOnteam1++;
                if (a.team.id == 2)
                    agentsOnteam2++;
                if (a.team.id == 3)
                    agentsOnteam3++;
                if (a.team.id == 4)
                    agentsOnteam4++;
            }

            //Generate gameStats
            string gameStats = string.Empty;
            if (Lists.teams.Count >= 1)
                gameStats += "Team 1: " + agentsOnteam1;
            if (Lists.teams.Count >= 2)
                gameStats += Environment.NewLine + "Team 2: " + agentsOnteam2;
            if (Lists.teams.Count >= 3)
                gameStats += Environment.NewLine + "Team 3: " + agentsOnteam3;
            if (Lists.teams.Count == 4)
                gameStats += Environment.NewLine + "Team 4: " + agentsOnteam4;


            //If Text changed, update textbox
            if (textBox3.Text != gameStats)
                textBox3.Text = gameStats;
            #endregion

            //Update GameArea
            dbPanel1.Invalidate();

        }
        #endregion

        #endregion

        #region Raised Events
        #region MouseClick
        private void dbPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            //Get Mouse ClickPoint
            mousePointGrid = getGridFromPixel(e.Location);

            //Getagent on mouseClick
            foreach (agent a in Lists.agents)
            {
                Point agentPoint = new Point(a.posx, a.posy);
                if (agentPoint == mousePointGrid)
                {
                    selectedagent = a;
                    //Write agent stats
                    textBox2.Text = "Name: " + a.name +  " (" + a.id + ")" + Environment.NewLine
                        + "Rank: " + a.rank + Environment.NewLine
                        + "Team: " + a.team.name + " (" + a.team.id + ")" + Environment.NewLine
                        + "Position: " + a.posx + "," + a.posy + Environment.NewLine;
                    break;
                }
            }
        }
        #endregion

        #region Endturn
        private void button1_Click(object sender, EventArgs e)
        {
            EndTurn();
        }

        private void EndTurn()
        {
            //Run the game frame
            for (int i = 0; i < 3; i++)
                gameFrame();

            switchTurn();
        }
        #endregion

        #region MouseMove
        private void dbPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            //Print gameArea chords
            mousePointGrid = getGridFromPixel(e.Location);
            //Update grid label
            label4.Text = "MousePos Grid: " + mousePointGrid.X + "," + mousePointGrid.Y;
        }
        #endregion

        #region Execute
        private void button4_Click(object sender, EventArgs e)
        {
            ThreadStart ts1 = new ThreadStart(doGameFrame);
            Thread workThread = new Thread(ts1);
            workThread.Start();
        }

        private void doGameFrame()
        {
            if (Lists.moveagents.Count > 0 && Lists.currentteam.id == turn)
            {
                gameFrame();
                Thread.Sleep(200);
                doGameFrame();
            }
            else
            {
                DrawTimer.Start();
                switchTurn();
                Thread.CurrentThread.Abort();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // char 13 = the enter key.
            if (e.KeyChar == (char)13)
            {
                Execute();
            }
        }

        private void Execute()
        {
            #region Compile whatever is in textBox1
            if (textBox1.Text != string.Empty)
            {
                // Takes the text from the textbox and stores it as a string.
                string text = textBox1.Text;

                ActionInterpet.input = text;
                string output = ActionInterpet.Compile();

                //If there were no errors, write success!
                if (output == "")
                {
                    StringBuilder newOutput = new StringBuilder("");
                    newOutput.AppendLine();
                    newOutput.Append("The command \"" + text + "\" was successfull.");
                    output = newOutput.ToString();
                }
                textBox4.BeginInvoke(new UpdateTextCallback(UpdateTextbox4), output);
                //textBox4.AppendText(output);

                textBox1.Clear();
            }
            #endregion
            else
            {
                EndTurn();
            }
        }

        #endregion
        #endregion

        #region Logic
        //Grid point logics
        #region GridPointLogic

        /// <summary>
        /// Translate x,y to gridstartX,gridstartY
        /// </summary>
        /// <param name="inputPoint">X,Y in pixel</param>
        /// <returns>gridstartX,gridstartY</returns>
        #region getGridPixelFromPixel
        public Point getGridPixelFromPixel(Point inputPoint)
        {
            //(cut digits) (Point.V / (g+lw)) * (g+lw)   --  DONE!
            int x = (int)(inputPoint.X / (GridSize.Width + LineWidth)) * (GridSize.Width + LineWidth) +LineWidth;
            int y = (int)(inputPoint.Y / (GridSize.Height + LineWidth)) * (GridSize.Height + LineWidth) +LineWidth;

            return new Point(x, y);
        }
        #endregion

        /// <summary>
        /// Translate gridX,gridY to gridstartX,gridstartY
        /// </summary>
        /// <param name="inputPoint">gridX,gridY</param>
        /// <returns>gridstartX,gridstartY</returns>
        #region getGridPixelFromGrid
        public Point getGridPixelFromGrid(Point inputPoint)
        {
            //Start equals 0,0   -- DONE!

            //(cut digits) ((Point.V * (Gx + Lw)) + Lw)
            int x = (int)((inputPoint.X * (GridSize.Width + LineWidth)) + LineWidth);
            int y = (int)((inputPoint.Y * (GridSize.Height + LineWidth)) + LineWidth);

            return new Point(x, y);
        }
        #endregion

        /// <summary>
        /// Translate x,y to gridX,gridY
        /// </summary>
        /// <param name="inputPoint">x,y</param>
        /// <returns>gridX,gridY</returns>
        #region getGridFromPixel
        public Point getGridFromPixel(Point inputPoint)
        {
            //Start equals 0,0   -- DONE!

            //(cut digits) (Point.V - lw) / (Gx + lw)
            int x = (int)((inputPoint.X - LineWidth) / (GridSize.Width + LineWidth));
            int y = (int)((inputPoint.Y - LineWidth) / (GridSize.Height + LineWidth));

            return new Point(x, y);
        }
        #endregion
        #endregion

        /// <summary>
        /// Compares two agents, the one which "rolls" the highest wins
        /// </summary>
        /// <param name="a1">agent 1</param>
        /// <param name="a2">agent 2</param>
        #region CombatCompareagents
        private void CombatCompareagents(agent a1, agent a2)
        {
            //Stop the tiemr, so we don't manulipulate the data while executing this
            //DrawTimer.Stop();

            //Generate random values, value = rank * (1..100)
            Random rnd = new Random();
            int agent1Value = a1.rank * rnd.Next(100);
            Random rnd1 = new Random(agent1Value);
            int agent2Value = a2.rank * rnd1.Next(100);

            //If agent 1 wins, remove agent 2
            if (agent1Value > agent2Value)
            {
                string sendtext = a1.name + " beats " + a2.name + Environment.NewLine;
                textBox5.BeginInvoke(new UpdateTextCallback(UpdateTextbox5), sendtext);
                //textBox5.Text += a1.name + " beats " + a2.name + Environment.NewLine;
                foreach (agent a in Lists.agents)
                {
                    if (a.id == a2.id)
                    {
                        Lists.moveagents.RemoveAll(s => s.id == a.id);
                        Lists.agents.Remove(a);
                        break;
                    }

                }
            }

            //If agent 2 wins, remove agent 1
            else
            {
                string sendtext = a2.name + " beats " + a1.name + Environment.NewLine;
                textBox5.BeginInvoke(new UpdateTextCallback(UpdateTextbox5), sendtext);
                //textBox5.Text += a2.name + " beats " + a1.name + Environment.NewLine;
                foreach (agent a in Lists.agents)
                {
                    if (a.id == a1.id)
                    {
                        Lists.moveagents.RemoveAll(s => s.id == a.id);
                        Lists.agents.Remove(a);
                        break;
                    }

                }
            }

            //Start the timer, and let the game continue
            //DrawTimer.Start();
        }
        #endregion

        /// <summary>
        /// Game frame logics
        /// </summary>
        #region gameFrame
        private void gameFrame()
        {
            //DrawTimer.Stop();
            //Game Logic
            #region GameLogic

            #region CheckForEncounters
            foreach (agent testagent in Lists.agents)
            {
                agent encounterAgent = Functions.encounter(testagent);
                if (encounterAgent != null)
                {
                    bool removed = false;
                    foreach (agent inneragent in Lists.agents)
                    {
                        foreach (encounter en in Lists.encounters)
                        {
                            if (en.agentId == inneragent.id)
                            {
                                ActionInterpet.input = en.action;
                                if (!removed)
                                {
                                    Lists.moveagents.RemoveAll(s => s.id == inneragent.id);
                                    removed = true;
                                }
                                ActionInterpet.Compile();
                            }
                        }
                    }
                }
            }
            #endregion

            //Check if the list is empty
            if (!Lists.moveagents.Any())
                return;

            //Update agent posistions
            #region Update agent posistion
            foreach (agent outerAgent in Lists.agents)
            {
                //Need to be current team to move
                if (outerAgent.team.id == Lists.currentteam.id)
                {
                    #region Agents to move
                    agent a = null;

                    foreach (agent moveAgent in Lists.moveagents)
                    {
                        if (moveAgent.id == outerAgent.id)
                        {
                            a = moveAgent;
                            break;
                        }
                    }
                    #endregion

                    //If there is no agents to move, move on
                    if(a != null)
                     {
                        //Checking for agents to move in moveagents
                        if (outerAgent.id == a.id)
                        {
                            #region Calculate next Position
                            Point agentPoint = new Point(outerAgent.posx, outerAgent.posy);
                            //Move "Down", keep in bounds
                            if (a.posy > outerAgent.posy && outerAgent.posy + 1 < Grids && (bumpingIntoAgent(outerAgent, new Point(agentPoint.X, agentPoint.Y+1)) == null))
                                agentPoint.Y++;
                            //Move "Up", keep in bounds
                            else if (a.posy < outerAgent.posy && outerAgent.posy - 1 > -1 && (bumpingIntoAgent(outerAgent, new Point(agentPoint.X, agentPoint.Y-1)) == null))
                                agentPoint.Y--;
                            //Move "Right", keep in bounds
                            else if (a.posx > outerAgent.posx && outerAgent.posx + 1 < Grids && (bumpingIntoAgent(outerAgent, new Point(agentPoint.X+1, agentPoint.Y)) == null))
                                agentPoint.X++;
                            //Move "Left", keep in bounds
                            else if (a.posx < outerAgent.posx && outerAgent.posx - 1 > -1 && (bumpingIntoAgent(outerAgent, new Point(agentPoint.X - 1, agentPoint.Y)) == null))
                                agentPoint.X--;
                            else
                            {
                                Lists.moveagents.Remove(a);
                                string sendtext = Environment.NewLine + a.name + "couldn't move this round";
                                textBox4.BeginInvoke(new UpdateTextCallback(UpdateTextbox4), sendtext);
                            }
                            #endregion

                            /*
                                Moving after id, don't move right if somebody is on your right!
                            */

                            //Check if move is valid, check if anyother agent is on the position where the agent is gonna move
                            /*
                            bool validMove = true;
                            agent bumpingAgent = bumpingIntoAgent(outerAgent, agentPoint);
                            if (bumpingAgent != null)
                            {
                                validMove = false;
                                string sendtext = Environment.NewLine + a.name + " bumped into " + bumpingAgent.name;
                                textBox4.BeginInvoke(new UpdateTextCallback(UpdateTextbox4), sendtext);
                            }
                            */

                            //Move is valid, move agent
                            //if (validMove)
                            //{
                                outerAgent.posx = agentPoint.X;
                                outerAgent.posy = agentPoint.Y;
                            //}
                            //else
                            //{
                            //    Lists.moveagents.Remove(a);
                            //    break;
                            //}

                            //Move valid, or agent at distination, remove from moveAgents list
                            if (a.posx == outerAgent.posx && a.posy == outerAgent.posy)
                            {
                                Lists.moveagents.Remove(a);
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

            //Check agents
            #region Checkagent
            int agentCount = Lists.agents.Count;
            for (int i = 0; i < agentCount; i++)
            {
                bool breakValue = false;
                foreach (agent aa in Lists.agents)
                {
                    foreach (agent a in Lists.agents)
                    {
                        //Same team doesn't count
                        if (a.team.id != aa.team.id)
                        {
                            //Need to have same values of x,y
                            if (a.posx == aa.posx && a.posy == aa.posy)
                            {
                                //Some Logic
                                CombatCompareagents(a, aa);
                                breakValue = !breakValue;
                                DrawTimer.Start();
                                break;
                            }
                        }
                    }
                    if (breakValue)
                        break;
                }
            }
            #endregion
            #endregion
            //DrawTimer.Start();
        }
        #endregion

        //Switch turn
        #region SwitchTurn
        private void switchTurn()
        {
            #region Agent count
            int agentsOnteam1 = 0;
            int agentsOnteam2 = 0;
            int agentsOnteam3 = 0;
            int agentsOnteam4 = 0;
            foreach (agent a in Lists.agents)
            {
                if (a.team.id == 1)
                    agentsOnteam1++;
                if (a.team.id == 2)
                    agentsOnteam2++;
                if (a.team.id == 3)
                    agentsOnteam3++;
                if (a.team.id == 4)
                    agentsOnteam4++;
            }
            #endregion

            #region WIN
            if (agentsOnteam2 == 0 && agentsOnteam3 == 0 && agentsOnteam4 == 0 && Lists.teams.Count > 0)
            {
                MessageBox.Show("Team 1 wins" + Environment.NewLine + "with " + agentsOnteam1.ToString() + " left");
            }
            else if (agentsOnteam1 == 0 && agentsOnteam3 == 0 && agentsOnteam4 == 0 && Lists.teams.Count > 1)
            {
                MessageBox.Show("Team 2 wins" + Environment.NewLine + "with " + agentsOnteam2.ToString() + " left");
            }
            else if (agentsOnteam1 == 0 && agentsOnteam2 == 0 && agentsOnteam4 == 0 && Lists.teams.Count > 2)
            {
                MessageBox.Show("Team 3 wins" + Environment.NewLine + "with " + agentsOnteam3.ToString() + " left");
            }
            else if (agentsOnteam1 == 0 && agentsOnteam2 == 0 && agentsOnteam3 == 0 && Lists.teams.Count > 3)
            {
                MessageBox.Show("Team 4 wins" + Environment.NewLine + "with " + agentsOnteam4.ToString() + " left");
            }
            #endregion

            //Switch turn
            #region Turn switch
            if (turn < Lists.teams.Count + 1)
                turn++;
            if (turn > Lists.teams.Count)
                turn = 1;
            Lists.currentteam = Lists.RetrieveTeam(turn);
            if (Lists.currentteam.id == 1 && agentsOnteam1 == 0)
                switchTurn();
            if (Lists.currentteam.id == 2 && agentsOnteam2 == 0)
                switchTurn();
            if (Lists.currentteam.id == 3 && agentsOnteam3 == 0)
                switchTurn();
            if (Lists.currentteam.id == 4 && agentsOnteam4 == 0)
                switchTurn();
            #endregion

            //Update Label6
            label6.BeginInvoke(new UpdateTextCallback(UpdateLabel6), "Team " + turn);
        }
        #endregion

        /// <summary>
        /// Checking for bumping into another agent
        /// </summary>
        /// <param name="outerAgent">Agent to check with the other agents</param>
        /// <param name="agentPoint">The point the agent is gonna be at</param>
        /// <returns>Agent is returned if found, null for no bump</returns>
        #region BumpintIntoAgent
        public agent bumpingIntoAgent(agent outerAgent, Point agentPoint)
        {
            #region StandingAgent
            foreach (agent standingAgent in Lists.agents)
            {
                if (standingAgent.team.id == outerAgent.team.id && standingAgent.id != outerAgent.id)
                {
                    Point standingAgentPoint = new Point(standingAgent.posx, standingAgent.posy);
                    if (agentPoint == standingAgentPoint)
                    {
                        return standingAgent;
                    }
                }
            }
            #endregion
            return null;
        }
        #endregion

        #endregion

        #region Lists
        /// <summary>
        /// Place teams on the gamearea
        /// </summary>
        #region Placeteams
        public void placeteams()
        {
            int agentsOnteam1 = 0;
            int agentsOnteam2 = 0;
            int agentsOnteam3 = 0;
            int agentsOnteam4 = 0;
            foreach (agent a in Lists.agents)
            {
                if (a.team.id == 1)
                    agentsOnteam1++;
                if (a.team.id == 2)
                    agentsOnteam2++;
                if (a.team.id == 3)
                    agentsOnteam3++;
                if (a.team.id == 4)
                    agentsOnteam4++;
            }

            int it1 = (Grids / 2) - (agentsOnteam1 / 2);
            int it2 = (Grids / 2) - (agentsOnteam2 / 2);
            int it3 = (Grids / 2) - (agentsOnteam3 / 2);
            int it4 = (Grids / 2) - (agentsOnteam4 / 2);
            foreach (agent a in Lists.agents)
            {
                Point p = new Point();
                if (a.team.id == 1)
                {
                    p = new Point(it1, 0);
                }
                else if (a.team.id == 2)
                {
                    p = new Point(Grids - 1, it2);
                }
                else if (a.team.id == 3)
                {
                    p = new Point(it3, Grids - 1);
                }
                else if (a.team.id == 4)
                {
                    p = new Point(0, it4);
                }

                a.posx += p.X;
                a.posy += p.Y;

                if (a.team.id == 1)
                {
                    it1++;
                }
                else if (a.team.id == 2)
                {
                    it2++;
                }
                else if (a.team.id == 3)
                {
                    it3++;
                }
                else if (a.team.id == 4)
                {
                    it4++;
                }
            }
        }
        #endregion
        #endregion

        #region GameHandlers
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void WarGame_Load(object sender, EventArgs e)
        {
            SetForegroundWindow(Handle.ToInt32());
        }

        private void WarGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion

        #region DllImport
        [DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(int hWnd);
        #endregion

        #region Delegates
        public delegate void UpdateTextCallback(string message);

        private void UpdateTextbox4(string message)
        {
            textBox4.AppendText(message);
        }

        private void UpdateTextbox5(string message)
        {
            textBox5.AppendText(message);
        }

        private void UpdateLabel6(string message)
        {
            label6.Text = message;
        }
        #endregion
    }
}