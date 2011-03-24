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
        Point Point1;
        Size GridSize;


        public WarGame()
        {
            Point1 = new Point(0, 0);
            GridSize = new Size(21, 21);
            InitializeComponent();
        }

        private void dbPanel1_Paint(object sender, PaintEventArgs e)
        {
            //Clear Screen to Color
            e.Graphics.Clear(Color.LightCyan);

            //Draw Grid
            for (int i = GridSize.Width+1; i < dbPanel1.Width; i += GridSize.Width+1)
            {
                e.Graphics.DrawLine(Pens.White, new Point(i-1, 0), new Point(i-1, dbPanel1.Height));
            }
            for (int i = GridSize.Height + 1; i < dbPanel1.Height; i += GridSize.Height + 1)
            {
                e.Graphics.DrawLine(Pens.White, new Point(0, i-1), new Point(dbPanel1.Width, i-1));
            }

            //Draw Rectangles
            e.Graphics.FillRectangle(Brushes.Red, new Rectangle(Point1, GridSize));
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            dbPanel1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Point1.X += GridSize.Width+1;
        }

        private void dbPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.Location.X + (e.Location.X % GridSize.Width)+((e.Location.X - (e.Location.X % GridSize.Width))/GridSize.Width);
            int y = e.Location.Y - (e.Location.Y % GridSize.Height);

            Point1 = new Point(x,y);
        }
    }
}
