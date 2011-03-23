using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GDI_Test
{
    public partial class Form1 : Form
    {
        enum mouseState
        {
            Ideal,
            LeftPressed,
            RightPressed,
            LeftReleased,
            RightReleased
        }

        private mouseState Mouse;
        private Point mousePoint;
        private bool ButtonIsPressed;
        private int gridSpacing = 20;
        private ArrayList figures = new ArrayList();

        public Form1()
        {
            InitializeComponent();
            Mouse = mouseState.Ideal;
            AddRectangleAt(5, 6);
            AddRectangleAt(6, 6);
            AddRectangleAt(8, 7);
            AddRectangleAt(10, 10);
        }


        private void Draw(Graphics g)
        {
            //Draw Line
            g.DrawLine(Pens.Black, new Point(20, 20), mousePoint);

            //Location Text
            string MouseLocation = "X:" + mousePoint.X.ToString() + ", Y:" + mousePoint.Y.ToString();
            Font DrawFont = new Font("Arial", 12);
            PointF LocationPoint = new PointF(gameArea.Size.Width - g.MeasureString(MouseLocation, DrawFont).Width,5.0f);
            g.DrawString(MouseLocation, DrawFont, Brushes.Red, LocationPoint);

            //Rectangles
            foreach (Rectangle r in figures)
            {
                g.FillRectangle(Brushes.DarkBlue, r);
            }


            if (ButtonIsPressed)
            {
                Point aprox = new Point(mousePoint.X - (mousePoint.X % gridSpacing), mousePoint.Y - (mousePoint.Y % gridSpacing));

                if (Mouse == mouseState.LeftPressed)
                {
                    g.FillRectangle(Brushes.DarkRed, new Rectangle(aprox, new Size(gridSpacing, gridSpacing)));
                }
                else if (Mouse == mouseState.RightPressed)
                {
                    g.FillEllipse(Brushes.Green, new Rectangle(aprox, new Size(gridSpacing, gridSpacing)));
                }
            }
        }

        private void gameArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse != mouseState.LeftPressed && Mouse != mouseState.RightPressed)
                Mouse = mouseState.Ideal;
            mousePoint = e.Location;
        }

        private void gameArea_Paint(object sender, PaintEventArgs e)
        {
            Graphics dc = e.Graphics;

            Draw(dc);

            if(dc != e.Graphics)
                base.OnPaint(e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            gameArea.Invalidate();
        }

        private void gameArea_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Mouse = mouseState.LeftPressed;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Mouse = mouseState.RightPressed;
            }

            ButtonIsPressed = true;
        }

        private void gameArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Mouse = mouseState.LeftReleased;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Mouse = mouseState.RightReleased;
            }

            ButtonIsPressed = false;
        }

        public void AddRectangleAt(int x, int y)
        {
            figures.Add(new Rectangle(new Point((int)(x * gridSpacing), (int)(y * gridSpacing)), new Size(gridSpacing, gridSpacing)));
        }

        public void AddEllipseAt(int x, int y)
        {
            figures.Add(new Rectangle(new Point((int)(x * gridSpacing), (int)(y * gridSpacing)), new Size(gridSpacing, gridSpacing)));
        }
    }
}
