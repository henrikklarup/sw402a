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
        Point Point1;
        Point mousePointGrid;
        Size GridSize;
        Color figureColor;
        Color LineColor;
        Color backGroundColor;
        int LineWidth;
        int Grids;

        //Random Blocks
        Point[] shit = new Point[10];
        #endregion

        #region Constructor
        public WarGame()
        {

            InitializeComponent();

            #region Initi props

            GameSettings gms = new GameSettings();
            gms.ShowDialog();


            //Small = 13
            //Medium = 26
            //Large = 46
            Grids = gms.gridSize;

            LineWidth = 2;
            Point1 = new Point(LineWidth, LineWidth);
            mousePointGrid = new Point(0, 0);
            GridSize = new Size((((dbPanel1.Width - (2 * LineWidth)) - ((Grids - 1) * LineWidth)) / Grids), (((dbPanel1.Height - (2 * LineWidth)) - ((Grids - 1) * LineWidth)) / Grids));
            figureColor = Color.FromArgb(102, 130, 102);
            LineColor = Color.Black;
            backGroundColor = Color.FromArgb(102,153,102);
            #endregion

            //Generate xml data
            getXmlData();
            placeTeams();

            GamerTimer.Enabled = true;

            #region Init random
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                shit[i] = getGridPixelFromGrid(new Point(rnd.Next(Grids), rnd.Next(Grids)));
            }
            #endregion
        }
        #endregion

        #region Paint Event
        private void dbPanel1_Paint(object sender, PaintEventArgs e)
        {
            //Clear Screen to Color
            e.Graphics.Clear(backGroundColor);

            //Draw Grid
            #region DrawGrid
            e.Graphics.DrawLine(new Pen(LineColor, LineWidth), new Point(0, LineWidth/2), new Point(dbPanel1.Width,LineWidth/2));
            e.Graphics.DrawLine(new Pen(LineColor, LineWidth), new Point(LineWidth/2, 0), new Point(LineWidth/2,dbPanel1.Height));


            //((GridSize.Width+LineWidth)*Grids) + (GridSize.Width + 2*LineWidth)
            //eller
            //dbPanel1.Width


            for (int i = GridSize.Width + LineWidth; i < dbPanel1.Width; i += GridSize.Width + LineWidth)
            {
                e.Graphics.DrawLine(new Pen(LineColor, LineWidth), new Point(i + (LineWidth/2), LineWidth), new Point(i +(LineWidth/2), dbPanel1.Height));
            }
            for (int i = GridSize.Height + LineWidth; i < dbPanel1.Height; i += GridSize.Height + LineWidth)
            {
                e.Graphics.DrawLine(new Pen(LineColor, LineWidth), new Point(LineWidth, i + (LineWidth/2)), new Point(dbPanel1.Width, i + (LineWidth/2)));
            }
            #endregion

            //Draw Shit
            #region DrawShit
            foreach (Point p in shit)
            {
                //e.Graphics.FillEllipse(Brushes.Pink, new Rectangle(p,GridSize));
            }
            #endregion

            //Draw Figure
            #region Draw Soldiers
            foreach (Agent a in agents)
            {
                e.Graphics.FillEllipse(new SolidBrush(Color.FromName(a.team.color)), new Rectangle(a.posX, a.posY, GridSize.Width - LineWidth+1, GridSize.Height - LineWidth +1));
                e.Graphics.DrawEllipse(Pens.White, new Rectangle(a.posX, a.posY, GridSize.Width - LineWidth +1, GridSize.Height - LineWidth +1));
            }
            #endregion
        }
        #endregion

        #region Timers
        #region DrawTimer Tick
        private void DrawTimer_Tick(object sender, EventArgs e)
        {
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
            Point1 = getGridPixelFromPixel(e.Location);

            //GetAgent on mouseClick
            foreach(Agent a in agents)
            {
                Point agentPoint = getGridPixelFromPixel(new Point(a.posX, a.posY));
                if (agentPoint == Point1)
                {
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
            label4.Text = "MousePos Grid: " + mousePointGrid.X + "," + mousePointGrid.Y;
        }
        #endregion

        #region Execute
        private void button4_Click(object sender, EventArgs e)
        {
            //Split string to two numbers "x,y" = x y
            string[] text = textBox1.Text.Split(',');
            //Set Figure to x,y
            Point1 = getGridPixelFromGrid(new Point(int.Parse(text[0]) -1, int.Parse(text[1])-1));
        }
        #endregion
        #endregion

        #region Logic
        //GridPointLogic
        #region GridPointLogic
        public Point getGridPixelFromPixel(Point inputPoint)
        {
            //(cut digits) (Point.V / (g+lw)) * (g+lw)   --  DONE!
            int x = (int)(inputPoint.X / (GridSize.Width + LineWidth)) * (GridSize.Width + LineWidth) +LineWidth;
            int y = (int)(inputPoint.Y / (GridSize.Height + LineWidth)) * (GridSize.Height + LineWidth) +LineWidth;

            return new Point(x, y);
        }

        public Point getGridPixelFromGrid(Point inputPoint)
        {
            //Start equals 0,0   -- DONE!

            //(cut digits) ((Point.V * (Gx + Lw)) + Lw)
            int x = (int)((inputPoint.X * (GridSize.Width + LineWidth)) + LineWidth);
            int y = (int)((inputPoint.Y * (GridSize.Height + LineWidth)) + LineWidth);

            return new Point(x, y);
        }

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

        #region Lists
        List<Agent> agents;
        List<ActionPattern> actionPatterns;
        List<Squad> squads;
        List<Team> teams;


        #region GetXMLData
        public void getXmlData()
        {
            using (var sr = new StreamReader(@"C:\agents.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<Agent>));
                agents = (List<Agent>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\teams.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<Team>));
                teams = (List<Team>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\squads.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<Squad>));
                squads = (List<Squad>)deserializer.Deserialize(sr);
            }

            using (var sr = new StreamReader(@"C:\actionPatterns.xml"))
            {
                var deserializer = new XmlSerializer(typeof(List<ActionPattern>));
                actionPatterns = (List<ActionPattern>)deserializer.Deserialize(sr);
            }
        }
        #endregion

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

        #region GameTimer
        private void GamerTimer_Tick(object sender, EventArgs e)
        {
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

            string gameStats = "Team 1: " + agentsOnTeam1 + Environment.NewLine + "Team 2: " + agentsOnTeam2 + Environment.NewLine + "Team 3: " + agentsOnTeam3 + Environment.NewLine + "Team 4: " + agentsOnTeam4;

            if (textBox3.Text != gameStats)
                textBox3.Text = gameStats;

        }
        #endregion
    }
}
