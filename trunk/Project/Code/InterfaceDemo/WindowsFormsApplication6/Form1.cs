﻿using System;
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
using XMLawesome;
using ActionInterpeter;
using MASClassLibrary;

namespace WindowsFormsApplication6
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
            #endregion

            #region Folder Browser Dialog
            //Xml-path choosen
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Generate xml data
                getXmlData(fbd.SelectedPath);
                placeteams();
            }
            #endregion

            //Turnswitch set to random
            Random rnd = new Random();
            turn = rnd.Next(1,Lists.teams.Count);
            Lists.currentteam = new team();
            Lists.currentteam.ID = turn;
            label6.Text = "team " + turn;

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
                Point drawPoint = new Point(a.posX, a.posY);
                drawPoint = getGridPixelFromGrid(drawPoint);

                //Makeup the agent drawRectangle
                Rectangle drawRect = new Rectangle(drawPoint.X, drawPoint.Y, GridSize.Width - LineWidth + 1, GridSize.Height - LineWidth + 1);

                //Make the font for drawing text
                FontFamily ff = new FontFamily("Arial");
                float fontSizePixel = drawRect.Width;
                Font fnt = new System.Drawing.Font(ff, fontSizePixel, FontStyle.Regular, GraphicsUnit.Pixel);

                //Font rankFont = new System.Drawing.Font(e.Graphics.MeasureString(a.rank.ToString(),new Font(Font,FontStyle.Regular)), FontStyle.Regular);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromName(a.team.color)), drawRect);
                e.Graphics.DrawEllipse(Pens.White, new Rectangle(drawPoint.X, drawPoint.Y, GridSize.Width - LineWidth + 1, GridSize.Height - LineWidth + 1));
                e.Graphics.DrawString(a.rank.ToString(), fnt, Brushes.Black, new PointF(drawPoint.X, drawPoint.Y));


                //Destination point of agent
                #region Despoint
                foreach (agent aa in Lists.moveagents)
                {
                    if (aa.team.ID == Lists.currentteam.ID)
                    {
                        if (aa.ID == selectedagent.ID)
                        {
                            Point desPoint = new Point(aa.posX, aa.posY);
                            desPoint = getGridPixelFromGrid(desPoint);
                            e.Graphics.DrawEllipse(Pens.LightBlue, new Rectangle(desPoint.X, desPoint.Y, GridSize.Width - LineWidth + 1, GridSize.Height - LineWidth + 1));
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
                if (a.team.ID == 1)
                    agentsOnteam1++;
                if (a.team.ID == 2)
                    agentsOnteam2++;
                if (a.team.ID == 3)
                    agentsOnteam3++;
                if (a.team.ID == 4)
                    agentsOnteam4++;
            }

            //Generate gameStats
            string gameStats = "team 1: " + agentsOnteam1 + Environment.NewLine + "team 2: " + agentsOnteam2 + Environment.NewLine + "team 3: " + agentsOnteam3 + Environment.NewLine + "team 4: " + agentsOnteam4;

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
                Point agentPoint = new Point(a.posX, a.posY);
                if (agentPoint == mousePointGrid)
                {
                    selectedagent = a;
                    //Write agent stats
                    textBox2.Text = "Name: " + a.name + Environment.NewLine + "Id: " + a.ID + Environment.NewLine + "team: " + a.team.name + Environment.NewLine + "team Color: " + a.team.color;
                    break;
                }
            }
        }
        #endregion

        #region Endturn
        private void button1_Click(object sender, EventArgs e)
        {
            //Run the game frame
            for(int i = 0; i < 3; i++)
                gameFrame();

            //Switch turn
            if (turn < Lists.teams.Count+1)
                turn++;
            if (turn > Lists.teams.Count)
                turn = 1;
            Lists.currentteam.ID = turn;

            //Update label
            label6.Text = "team " + turn;
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
            Execute();
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
                textBox4.AppendText(output);

                textBox1.Clear();
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
            DrawTimer.Stop();

            //Generate random values, value = rank * (1..100)
            Random rnd = new Random();
            int agent1Value = a1.rank * rnd.Next(100);
            Random rnd1 = new Random(agent1Value);
            int agent2Value = a2.rank * rnd1.Next(100);

            //If agent 1 wins, remove agent 2
            if (agent1Value > agent2Value)
            {
                textBox5.AppendText(a1.name + " beats " + a2.name);
                foreach (agent a in Lists.agents)
                {
                    if (a.ID == a2.ID)
                    {
                        Lists.agents.Remove(a);
                        break;
                    }

                }
            }
            //If agent 2 wins, remove agent 1
            else
            {
                MessageBox.Show(a2.name + " beats " + a1.name);
                foreach (agent a in Lists.agents)
                {
                    if (a.ID == a1.ID)
                    {
                        Lists.agents.Remove(a);
                        break;
                    }

                }
            }


            //Start the timer, and let the game continue
            DrawTimer.Start();
        }
        #endregion

        /// <summary>
        /// Game frame logics
        /// </summary>
        #region gameFrame
        private void gameFrame()
        {
            //Game Logic
            #region GameLogic

            //Update agent posistions
            #region Update agent posistion
            foreach (agent aa in Lists.agents)
            {
                //Need to be current team to move
                if (aa.team.ID == Lists.currentteam.ID)
                {
                    foreach (agent a in Lists.moveagents)
                    {
                        //Checking for agents to move in moveagents
                        if (aa.ID == a.ID)
                        {
                            //Move "Down", keep in bounds
                            if (a.posY > aa.posY && aa.posY + 1 < Grids)
                                aa.posY++;
                            //Move "Up", keep in bounds
                            else if (a.posY < aa.posY && aa.posY - 1 > -1)
                                aa.posY--;
                            //Move "Right", keep in bounds
                            else if (a.posX > aa.posX && aa.posX + 1 < Grids)
                                aa.posX++;
                            //Move "Left", keep in bounds
                            else if (a.posX < aa.posX && aa.posX - 1 > -1)
                                aa.posX--;
                            //At destination, remove agent from moveagents and break;
                            else
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
            bool breakValue = false;
            foreach (agent aa in Lists.agents)
            {
                foreach (agent a in Lists.agents)
                {
                    //Same team doesn't count
                    if (a.team.ID != aa.team.ID)
                    {
                        //Need to have same values of x,y
                        if (a.posX == aa.posX && a.posY == aa.posY)
                        {
                            //Some Logic
                            CombatCompareagents(a, aa);
                            breakValue = !breakValue;
                            break;
                        }
                    }
                }
                if (breakValue)
                    break;
            }
            #endregion
            #endregion
        }
        #endregion

        #endregion

        #region Lists
        /// <summary>
        /// Get xml data from files in folder, and puts it into the various lists
        /// </summary>
        /// <param name="path">Folder containing the xmlfiles</param>
        #region GetXMLData
        public void getXmlData(string path)
        {
            /* XML IMPLEMENTATION!
             * Kristian's shizzle
             * 
             * 
            //Create instance of the XmlReader with a path to the xml file
            XmlReader Reader = new XmlReader(@"C:\WarGame.xml");
            Reader.Mount();
            foreach (XmlType item in Reader.XmlSearch(""))
            {
                Console.WriteLine(item.Tag + item.Value);
            }


            */



            using (var sr = new StreamReader(path + @"\agents.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<agent>));
                Lists.agents = (List<agent>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(path + @"\teams.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<team>));
                Lists.teams = (List<team>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(path + @"\squads.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<oldSquad>));
                List<oldSquad> oldsquad = (List<oldSquad>)deserializer.Deserialize(sr);
                foreach (oldSquad old in oldsquad)
                {
                    List<agent> agents = new List<agent>();
                    foreach (int i in old.agents)
                    {
                        agents.Add(Lists.Retrieveagent(i));
                    }
                    squad newsquad = new squad(old.name);
                    newsquad.Agents = agents;
                    Lists.squads.Add(newsquad);
                }
            }

            using (var sr = new StreamReader(path + @"\actionPatterns.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<oldActionPattern>));
                List<oldActionPattern> oldaction= (List<oldActionPattern>)deserializer.Deserialize(sr);
                foreach (oldActionPattern old in oldaction)
                {
                    List<string> actions = new List<string>();
                    foreach (string i in old.actions)
                    {
                         actions.Add(i);
                    }
                    actionpattern newactionpattern = new actionpattern(old.ID, old.actions);
                    newactionpattern.name = old.name;
                    Lists.actionPatterns.Add(newactionpattern);
                }
            }
        }
        #endregion

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
                if (a.team.ID == 1)
                    agentsOnteam1++;
                if (a.team.ID == 2)
                    agentsOnteam2++;
                if (a.team.ID == 3)
                    agentsOnteam3++;
                if (a.team.ID == 4)
                    agentsOnteam4++;
            }

            int it1 = (Grids / 2) - (agentsOnteam1 / 2);
            int it2 = (Grids / 2) - (agentsOnteam2 / 2);
            int it3 = (Grids / 2) - (agentsOnteam3 / 2);
            int it4 = (Grids / 2) - (agentsOnteam4 / 2);
            foreach (agent a in Lists.agents)
            {
                Point p = new Point();
                if (a.team.ID == 1)
                {
                    p = new Point(it1, 0);
                }
                else if (a.team.ID == 2)
                {
                    p = new Point(Grids - 1, it2);
                }
                else if (a.team.ID == 3)
                {
                    p = new Point(it3, Grids - 1);
                }
                else if (a.team.ID == 4)
                {
                    p = new Point(0, it4);
                }

                a.posX += p.X;
                a.posY += p.Y;

                if (a.team.ID == 1)
                {
                    it1++;
                }
                else if (a.team.ID == 2)
                {
                    it2++;
                }
                else if (a.team.ID == 3)
                {
                    it3++;
                }
                else if (a.team.ID == 4)
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
            Application.Exit();
        }
        #endregion
    }
}