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
        Agent movedAgent;                   //Last moved agent
        List<Agent> agents;                 //List of agents
        List<ActionPattern> actionPatterns; //List of actionPatterns
        List<Squad> squads;                 //List of squads
        List<Team> teams;                   //List of teams
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

            #endregion

            #region Folder Browser Dialog
            //Xml-path choosen
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Generate xml data
                getXmlData(fbd.SelectedPath);
                placeTeams();
            }
            #endregion

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
            foreach (Agent a in agents)
            {
                Rectangle drawRect = new Rectangle(a.posX, a.posY, GridSize.Width - LineWidth + 1, GridSize.Height - LineWidth + 1);

                FontFamily ff = new FontFamily("Arial");
                float fontSizePixel = drawRect.Width;
                Font fnt = new System.Drawing.Font(ff, fontSizePixel, FontStyle.Regular, GraphicsUnit.Pixel);

                //Font rankFont = new System.Drawing.Font(e.Graphics.MeasureString(a.rank.ToString(),new Font(Font,FontStyle.Regular)), FontStyle.Regular);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromName(a.team.color)), drawRect);
                e.Graphics.DrawEllipse(Pens.White, new Rectangle(a.posX, a.posY, GridSize.Width - LineWidth + 1, GridSize.Height - LineWidth + 1));
                e.Graphics.DrawString(a.rank.ToString(), fnt, Brushes.Black, new PointF(a.posX, a.posY));
            }
            #endregion
        }
        #endregion

        #region Timers

        #region DrawTimer Tick
        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            //Game Logic
            #region GameLogic
            //Die Agents
            if (movedAgent != null)
            {
                foreach (Agent a in agents)
                {
                    if (a.team.ID != movedAgent.team.ID)
                    {
                        if (a.posX == movedAgent.posX && a.posY == movedAgent.posY)
                        {
                            //Some Logic

                            CombatCompareAgents(a, movedAgent);
                            break;

                        }
                    }
                }
            }
            #endregion

            //Update progress
            #region Progress
            int agentsOnTeam1 = 0;
            int agentsOnTeam2 = 0;
            int agentsOnTeam3 = 0;
            int agentsOnTeam4 = 0;
            foreach (Agent a in agents)
            {
                if (a.team.ID == 1)
                    agentsOnTeam1++;
                if (a.team.ID == 2)
                    agentsOnTeam2++;
                if (a.team.ID == 3)
                    agentsOnTeam3++;
                if (a.team.ID == 4)
                    agentsOnTeam4++;
            }

            //Generate gameStats
            string gameStats = "Team 1: " + agentsOnTeam1 + Environment.NewLine + "Team 2: " + agentsOnTeam2 + Environment.NewLine + "Team 3: " + agentsOnTeam3 + Environment.NewLine + "Team 4: " + agentsOnTeam4;

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
            mousePointGrid = getGridPixelFromPixel(e.Location);

            //GetAgent on mouseClick
            foreach(Agent a in agents)
            {
                Point agentPoint = getGridPixelFromPixel(new Point(a.posX, a.posY));
                if (agentPoint == mousePointGrid)
                {
                    //Write Agent stats
                    textBox2.Text = "Name: " + a.name + Environment.NewLine + "Id: " + a.ID + Environment.NewLine + "Team: " + a.team.name + Environment.NewLine + "Team Color: " + a.team.color;
                    break;
                }
            }
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
            //Split string to two numbers "x,y" = x y
            string[] text = textBox1.Text.Split(',');


            foreach (Agent a in agents)
            {
                if (a.ID == int.Parse(text[0]))
                {
                    //Set Figure to x,y
                    a.posX = getGridPixelFromGrid(new Point(int.Parse(text[1]) - 1, int.Parse(text[2]) - 1)).X;
                    a.posY = getGridPixelFromGrid(new Point(int.Parse(text[1]) - 1, int.Parse(text[2]) - 1)).Y;
                    //Set movedAgent to just moved agent
                    movedAgent = a;
                }
            }
        }
        #endregion
        #endregion

        #region Logic
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
            int x = (int)((inputPoint.X - LineWidth) / (GridSize.Width + LineWidth)) +1;
            int y = (int)((inputPoint.Y - LineWidth) / (GridSize.Height + LineWidth)) +1;

            return new Point(x, y);
        }
        #endregion
        #endregion

        /// <summary>
        /// Compares two agents, the one which "rolls" the highest wins
        /// </summary>
        /// <param name="a1">Agent 1</param>
        /// <param name="a2">Agent 2</param>
        #region CombatCompareAgents
        private void CombatCompareAgents(Agent a1, Agent a2)
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
                MessageBox.Show(a1.name + " beats " + a2.name);
                foreach (Agent a in agents)
                {
                    if (a.ID == a2.ID)
                    {
                        agents.Remove(a);
                        break;
                    }

                }
            }
            //If agent 2 wins, remove agent 1
            else
            {
                MessageBox.Show(a2.name + " beats " + a1.name);
                foreach (Agent a in agents)
                {
                    if (a.ID == a1.ID)
                    {
                        agents.Remove(a);
                        break;
                    }

                }
            }

            //No agent has now been moved
            movedAgent = null;

            //Start the timer, and let the game continue
            DrawTimer.Start();
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
            using (var sr = new StreamReader(path + @"\agents.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<Agent>));
                agents = (List<Agent>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(path + @"\teams.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<Team>));
                teams = (List<Team>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(path + @"\squads.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<Squad>));
                squads = (List<Squad>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(path + @"\actionPatterns.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<ActionPattern>));
                actionPatterns = (List<ActionPattern>)deserializer.Deserialize(sr);
            }
        }
        #endregion

        /// <summary>
        /// Place teams on the gamearea
        /// </summary>
        #region PlaceTeams
        public void placeTeams()
        {
            switch (teams.Count)
            {
                #region 1Teams
                case 1:
                    int agentsOnTeam = 0;
                    foreach(Agent a in agents)
                    {
                        if (a.team.ID == 1)
                            agentsOnTeam++;
                    }

                    int i = (Grids / 2) - (agentsOnTeam/2);
                    foreach (Agent a in agents)
                    {
                        Point p = getGridPixelFromGrid(new Point(i, Grids-1));

                        a.posX += p.X;
                        a.posY += p.Y;
                        i++;
                    }
                    break;
                #endregion
                #region 2Teams
                case 2:
                    int agentsOnTeam1 = 0;
                    int agentsOnTeam2 = 0;
                    foreach (Agent a in agents)
                    {
                        if (a.team.ID == 1)
                            agentsOnTeam1++;
                        if (a.team.ID == 2)
                            agentsOnTeam2++;
                    }

                    int it1 = (Grids / 2) - (agentsOnTeam1 / 2);
                    int it2 = (Grids / 2) - (agentsOnTeam2 / 2);
                    foreach (Agent a in agents)
                    {
                        Point p = new Point();
                        if (a.team.ID == 1)
                        {
                            p = getGridPixelFromGrid(new Point(it1, Grids - 1));
                        }
                        if (a.team.ID == 2)
                        {
                           p = getGridPixelFromGrid(new Point(it2, 0));
                        }

                        a.posX += p.X;
                        a.posY += p.Y;
                        if (a.team.ID == 1)
                        {
                            it1++;
                        }
                        if (a.team.ID == 2)
                        {
                            it2++;
                        }
                    }
                    break;
                #endregion
                #region 3Teams
                case 3:
                    int agentsOnTeam21 = 0;
                    int agentsOnTeam22 = 0;
                    int agentsOnTeam23 = 0;
                    foreach (Agent a in agents)
                    {
                        if (a.team.ID == 1)
                            agentsOnTeam21++;
                        if (a.team.ID == 2)
                            agentsOnTeam22++;
                        if (a.team.ID == 3)
                            agentsOnTeam23++;
                    }

                    int it21 = (Grids) - (agentsOnTeam21);
                    int it22 = (Grids) - (agentsOnTeam22);
                    int it23 = (Grids / 2) - (agentsOnTeam23 / 2);
                    foreach (Agent a in agents)
                    {
                        Point p = new Point();
                        if (a.team.ID == 1)
                        {
                            p = getGridPixelFromGrid(new Point(it21, Grids - 1));
                        }
                        if (a.team.ID == 2)
                        {
                            p = getGridPixelFromGrid(new Point(it22, 0));
                        }
                        if (a.team.ID == 3)
                        {
                            p = getGridPixelFromGrid(new Point(0, it23));
                        }

                        a.posX += p.X;
                        a.posY += p.Y;

                        if (a.team.ID == 1)
                        {
                            it21++;
                        }
                        if (a.team.ID == 2)
                        {
                            it22++;
                        }
                        if (a.team.ID == 3)
                        {
                            it23++;
                        }
                    }
                    break;
                #endregion
                #region 4Teams
                case 4:
                    int agentsOnTeam31 = 0;
                    int agentsOnTeam32 = 0;
                    int agentsOnTeam33 = 0;
                    int agentsOnTeam34 = 0;
                    foreach (Agent a in agents)
                    {
                        if (a.team.ID == 1)
                            agentsOnTeam31++;
                        if (a.team.ID == 2)
                            agentsOnTeam32++;
                        if (a.team.ID == 3)
                            agentsOnTeam33++;
                        if (a.team.ID == 4)
                            agentsOnTeam34++;
                    }

                    int it31 = (Grids / 2) - (agentsOnTeam31 / 2);
                    int it32 = (Grids / 2) - (agentsOnTeam32 / 2);
                    int it33 = (Grids / 2) - (agentsOnTeam33 / 2);
                    int it34 = (Grids / 2) - (agentsOnTeam34 / 2);
                    foreach (Agent a in agents)
                    {
                        Point p = new Point();
                        if (a.team.ID == 1)
                        {
                            p = getGridPixelFromGrid(new Point(it31, 0));
                        }
                        if (a.team.ID == 2)
                        {
                            p = getGridPixelFromGrid(new Point(Grids-1, it32));
                        }
                        if (a.team.ID == 3)
                        {
                            p = getGridPixelFromGrid(new Point(it33,Grids-1));
                        }
                        if (a.team.ID == 4)
                        {
                            p = getGridPixelFromGrid(new Point(0,it34));
                        }

                        a.posX += p.X;
                        a.posY += p.Y;

                        if (a.team.ID == 1)
                        {
                            it31++;
                        }
                        if (a.team.ID == 2)
                        {
                            it32++;
                        }
                        if (a.team.ID == 3)
                        {
                            it33++;
                        }
                        if (a.team.ID == 4)
                        {
                            it34++;
                        }
                    }
                    break;
                #endregion
                default:
                    break;
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
