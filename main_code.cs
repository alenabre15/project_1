using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_1
{
    public partial class Form1 : Form
    { 
        private SolidBrush brush;

        private static List<Point> shapes;

        private int i;

        private bool dragging;

        public Form1()
        {
            InitializeComponent();

            brush = new SolidBrush(Color.DarkGoldenrod);

            i = 0;

            dragging = false;
        }

        static Form1()
        {
            shapes = new List<Point>();
        }

        private void ListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((string)listBox1.SelectedItem == "circle")
            {
                shapes.Add(new Circle());
            }

            if ((string)listBox1.SelectedItem == "triangle")
            {
                shapes.Add(new Triangle());
            }

            if ((string)listBox1.SelectedItem == "square")
            {
                shapes.Add(new Square());
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Point shape in shapes)
            {
                shape.Draw(e.Graphics, brush);
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (listBox1.SelectedItem == null)
                {
                    return;
                }

                if ((string)listBox1.SelectedItem == "circle")
                    shapes.Add(new Circle());

                if ((string)listBox1.SelectedItem == "triangle")
                    shapes.Add(new Triangle());

                if ((string)listBox1.SelectedItem == "square")
                    shapes.Add(new Square());

                Refresh();
            }

            int j = 0;

            if (e.Button == MouseButtons.Right)
            {
                j = 0;

                foreach (Point shape in shapes)
                {
                    ++j;

                    if (shape.Inside(e.X, e.Y))
                    {
                        break;
                    }
                }

                shapes.Remove(shapes[j - 1]);

                Refresh();
            }
        }

        int k = 0;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            k = 0;

            foreach (Point shape in shapes)
            {
                ++k;

                if (shape.Inside(e.X, e.Y))
                {
                    dragging = true;

                    break;
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                shapes[k].X += e.X;
                shapes[k].Y += e.Y;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
    }

    public abstract class Point
    {
        protected double x;
        protected double y;
        protected static Color color;
        protected static double radius;

        public Point()
        {
            x = Cursor.Position.X;
            y = Cursor.Position.Y;
        }

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        static Point()
        {
            radius = 26;
            color = Color.DarkGoldenrod;
        }

        public double X
        {
            get => x;

            set => x = value;
        }

        public double Y
        {
            get => y;

            set => y = value;
        }

        public Color Color
        {
            get => color;

            set => color = value;
        }

        public abstract void Draw(Graphics canvas, Brush brush);

        public abstract bool Inside(int MouseX, int MouseY);
    }

    public class Circle : Point
    { 
        public Circle()
        {
            x = Cursor.Position.X;
            y = Cursor.Position.Y;
        }

        public Circle(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        static Circle()
        {
            radius = 26;
            color = Color.DarkGoldenrod;
        }

        public override void Draw(Graphics canvas, Brush brush)
        {
            Rectangle rect = new Rectangle(Cursor.Position.X - (int)(radius / 2), Cursor.Position.Y - (int)radius - (int)(radius / 3), (int)radius, (int)radius);

            canvas.FillEllipse(brush, rect);
        }

        public override bool Inside(int MouseX, int MouseY)
        {
            return Math.Pow(x - MouseX, 2) + Math.Pow(y - MouseY, 2) <= radius * radius;
        }
    }

    public class Triangle : Point
    {
        public Triangle()
        {
            x = Cursor.Position.X;
            y = Cursor.Position.Y;
        }

        public Triangle(double x, double y, Color color)
        {
            this.x = x;
            this.y = y;
        }

        static Triangle()
        {
            radius = 26;
            color = Color.DarkGoldenrod;
        }

        public override void Draw(Graphics canvas, Brush brush)
        {
            float angle = 0;

            float X = (float)x - (int)radius / 2; 
            float Y = (float)y - (int)radius - (int)radius / 3;

            PointF[] p = new PointF[3];

            p[0].X = X;

            p[0].Y = Y;

            p[1].X = (float)(X + radius * Math.Cos(angle));

            p[1].Y = (float)(Y + radius * Math.Sin(angle));

            p[2].X = (float)(X + radius * Math.Cos(angle + Math.PI / 3));

            p[2].Y = (float)(Y + radius * Math.Sin(angle + Math.PI / 3));

            canvas.FillPolygon(brush, p);
        }

        private int PointOfLine(int[] X, int[] Y)
        {
            try
            {
                int f = (X[1] - X[0]) * (Y[2] - Y[1]) - (X[2] - X[1]) * (Y[1] - Y[0]);

                if (f == 0)
                    return 0;
                if (f > 1)
                    return 1;
                return -1;
            }

            catch
            {
                return -1;
            }
        }

        public override bool Inside(int MouseX, int MouseY)
        {
            int SideLength = (int)((radius * 3) / Math.Sqrt(3.0));
            int f1, f2, f3;

            f1 = PointOfLine(new int[] { MouseX, (int)x - SideLength / 2, (int)x }, new int[] { MouseY, (int)y + (int)radius / 2, (int)y - (int)radius });
            f2 = PointOfLine(new int[] { MouseX, (int)x, (int)x + SideLength / 2 }, new int[] { MouseY, (int)y - (int)radius, (int)y + (int)radius / 2 });
            f3 = PointOfLine(new int[] { MouseX, (int)x + SideLength / 2, (int)x - SideLength / 2 }, new int[] { MouseY, (int)y + (int)radius / 2, (int)y + (int)radius / 2 });

            return (f1 >= 0 && f2 >= 0 && f3 >= 0) || (f1 <= 0 && f2 <= 0 && f3 <= 0);
        }
    }

    public class Square : Point
    {
        private int X;
        private int Y;
        private Rectangle rect;

        public Square()
        {
            X = (int)(Cursor.Position.X + (int)radius / 2 - (int)radius / 6 + radius * Math.Cos(135 * (2 * Math.PI / 360)));
            Y = (int)(Cursor.Position.Y - (int)radius / 2 - radius * Math.Sin(135 * (2 * Math.PI / 360)));
            rect = new Rectangle(X, Y, (int)Math.Sqrt(radius * radius / 2), (int)Math.Sqrt(radius * radius / 2));
            x = rect.X;
            y = rect.Y;
        }

        public Square(double x, double y, int X, int Y, Rectangle rect) : base(x, y)
        {
            this.x = x;
            this.y = y;
            this.X = X;
            this.Y = Y;
            this.rect = rect;
        }

        static Square()
        {
            radius = 26;
            color = Color.DarkGoldenrod;
        }

        public override void Draw(Graphics canvas, Brush brush)
        {
            canvas.FillRectangle(brush, rect);
        }

        public override bool Inside(int MouseX, int MouseY)
        {
            if (MouseX >= x && MouseX <= x + (int)Math.Sqrt(radius * radius / 2) && MouseY >= y && MouseY <= y + (int)Math.Sqrt(radius * radius / 2))
            {
                return true;
            }
            else
                return false;
        }
    }
}
