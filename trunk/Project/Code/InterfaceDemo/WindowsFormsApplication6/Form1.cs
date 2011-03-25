using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

        //Random Blocks
        Point[] shit = new Point[10];
        #endregion

        #region Constructor
        public WarGame()
        {
            #region Initi props
            LineWidth = 1;
            Point1 = new Point(0,0);
            mousePointGrid = new Point(0, 0);
            GridSize = new Size(10, 10);
            figureColor = Color.FromArgb(102, 130, 102);
            LineColor = Color.Black;
            backGroundColor = Color.FromArgb(102,153,102);
            #endregion

            #region Init random
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                shit[i] = getGridPixelFromGrid(new Point(rnd.Next(100), rnd.Next(100)));
            }
            #endregion

            InitializeComponent();
        }
        #endregion

        #region Paint Event
        private void dbPanel1_Paint(object sender, PaintEventArgs e)
        {
            //Clear Screen to Color
            e.Graphics.Clear(backGroundColor);

            //Draw Grid
            #region DrawGrid
            for (int i = -1; i < dbPanel1.Width; i += GridSize.Width + LineWidth)
            {
                e.Graphics.DrawLine(new Pen(LineColor, LineWidth), new Point(i, 0), new Point(i, dbPanel1.Height));
            }
            for (int i = -1; i < dbPanel1.Height; i += GridSize.Height + LineWidth)
            {
                e.Graphics.DrawLine(new Pen(LineColor, LineWidth), new Point(0, i), new Point(dbPanel1.Width, i));
            }
            #endregion

            //Draw Shit
            #region DrawShit
            foreach (Point p in shit)
            {
                e.Graphics.FillEllipse(Brushes.Pink, new Rectangle(p,GridSize));
            }
            #endregion

            //Draw Figure
            #region Draw Figure
            e.Graphics.FillEllipse(new SolidBrush(figureColor), new Rectangle(Point1, GridSize));
            e.Graphics.DrawEllipse(Pens.White, new Rectangle(Point1.X,Point1.Y, GridSize.Width-1, GridSize.Height-1));
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

        #region GameTimer Tick
        private void timer1_Tick(object sender, EventArgs e)
        {
            //Move Block!!RAWR!!
            Point1.X += GridSize.Width + LineWidth;
        }
        #endregion
        #endregion

        #region Raised Events
        #region MouseClick
        private void dbPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            //Move Block to Mouse "chord"
            Point1 = getGridPixelFromPixel(e.Location);
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
            Point1 = getGridPixelFromGrid(new Point(int.Parse(text[0]), int.Parse(text[1])));
        }
        #endregion
        #endregion

        #region Logic
        //GridPointLogic
        #region GridPointLogic
        public Point getGridPixelFromPixel(Point inputPoint)
        {
            //(cut digits) (Point.V / (g+lw)) * (g+lw)
            int x = (int)(inputPoint.X / (GridSize.Width + LineWidth)) * (GridSize.Width + LineWidth);
            int y = (int)(inputPoint.Y / (GridSize.Height + LineWidth)) * (GridSize.Height + LineWidth);

            return new Point(x, y);
        }

        public Point getGridPixelFromGrid(Point inputPoint)
        {
            //Start equals 0,0

            //(cut digits) ((Point.V - 1) * (g+lw))
            int x = (int)((inputPoint.X - 1) * (GridSize.Width + LineWidth));
            int y = (int)((inputPoint.Y - 1) * (GridSize.Height + LineWidth));

            return new Point(x, y);
        }

        public Point getGridFromPixel(Point inputPoint)
        {
            //Start equals 0,0

            //(cut digits) (Point.V / g) +1
            int x = (int)(inputPoint.X / GridSize.Width) +1;
            int y = (int)(inputPoint.Y / GridSize.Height) +1;

            return new Point(x, y);
        }
        #endregion
        #endregion

    }
}
