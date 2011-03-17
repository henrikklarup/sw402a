using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DrawTest
{
    public partial class Form1 : Form
    {
        Point p1;
        Point p2;
        Point mousePoint;
        Bitmap backBuffer;
        Graphics graphics;
        Size buffersize;
        Direction direction = Direction.Down;

        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public Form1()
        {
            InitializeComponent();
            p1 = new Point(20, 20);
            p2 = new Point(40, 40);

            SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint , true);

            this.DoubleBuffered = true;
            buffersize = panel1.Size;
            backBuffer = new Bitmap(buffersize.Width, buffersize.Height);
            graphics = Graphics.FromImage(backBuffer);
            timer1.Start();
        }

        private void DrawScreen()
        {
            Graphics g = Graphics.FromImage(backBuffer);
            g.FillRectangle(Brushes.Red, new Rectangle(p1, new System.Drawing.Size(10, 10)));
            g.DrawRectangle(Pens.Black, new Rectangle(p1, new System.Drawing.Size(10, 10)));
        }

        private void DoFrame()
        {
            panel1.Invalidate();
        }

        //main rendering function
        private void Render(Graphics g)
        {
            //clear back buffer
            //graphics.Clear(Color.Transparent);

            //draw to back buffer
            DrawScreen();

            //present back buffer
            g.DrawImage(backBuffer, new Rectangle(0, 0, buffersize.Width, buffersize.Height), 0, 0, buffersize.Width, buffersize.Height, GraphicsUnit.Pixel);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Render(e.Graphics);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DoFrame();

            p2 = mousePoint;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            mousePoint = panel1.PointToClient(Cursor.Position);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                direction = Direction.Up;
            if (e.KeyCode == Keys.Down)
                direction = Direction.Down;
            if (e.KeyCode == Keys.Left)
                direction = Direction.Left;
            if (e.KeyCode == Keys.Right)
                direction = Direction.Right;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (direction == Direction.Up)
                p1.Y -= 10;
            if (direction == Direction.Down)
                p1.Y += 10;
            if (direction == Direction.Left)
                p1.X -= 10;
            if (direction == Direction.Right)
                p1.X += 10;
        }
    }
}
