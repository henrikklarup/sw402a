using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication6
{
    public partial class WarGame : Form
    {
        Point Point1 = new Point(20,20);
        Size GridSize = new Size(20,20);
        enum Direction 
        {
            Up, Down, Left, Right, None
        }

        Direction direction = Direction.None;


        public WarGame()
        {
            InitializeComponent();
        }

        private void dbPanel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            e.Graphics.FillRectangle(Brushes.Red, new Rectangle(Point1, GridSize));
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            dbPanel1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Point1.X += GridSize.Width;
        }

        private void dbPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            Point1 = new Point(e.Location.X - (e.Location.X % GridSize.Width), e.Location.Y - (e.Location.Y % GridSize.Height));
        }
    }
}
