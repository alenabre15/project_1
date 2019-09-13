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
        public Graphics canvas;

        public SolidBrush brush;

        public static int i;

        public Form1()
        {
            InitializeComponent();

            canvas = pictureBox1.CreateGraphics();

            brush = new SolidBrush(Color.White);
        }

        static Form1()
        {
            i = 0;
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (listBox1.SelectedItem == null)
                {
                    Point.shapes[i] = new Circle();

                    brush.Color = Color.DarkRed;
                }

                if ((string)listBox1.SelectedItem == "circle")
                    Point.shapes[i] = new Circle();

                if ((string)listBox1.SelectedItem == "triangle")
                    Point.shapes[i] = new Triangle();

                if ((string)listBox1.SelectedItem == "square")
                    Point.shapes[i] = new Square();

                Point.shapes[i].Draw(canvas, brush);

                ++i;
            }

            if (e.Button == MouseButtons.Right)
            {
                foreach (Point shape in Point.shapes)
                {
                    if (shape is Circle)
                    {

                    }

                    if (shape is Triangle)
                    {

                    }

                    if (shape is Square)
                    {
                        if (e.X >= shape.x && e.X <= shape.x + (int)Math.Sqrt(Point.radius * Point.radius / 2) && e.Y >= shape.y && e.Y <= shape.y + (int)Math.Sqrt(Point.radius * Point.radius / 2))
                        {
                            shape.Erase(canvas);
                        }
                    }
                }
            }
        }

        private void ListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((string)listBox1.SelectedItem == "circle")
            {
                Point.shapes[i] = new Circle();

                brush.Color = Point.shapes[i].color;
            }

            if ((string)listBox1.SelectedItem == "triangle")
            {
                Point.shapes[i] = new Triangle();

                brush.Color = Point.shapes[i].color;
            }

            if ((string)listBox1.SelectedItem == "square")
            {
                Point.shapes[i] = new Square();

                brush.Color = Point.shapes[i].color;
            }
        }

        private void PictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);

            if (data != null)
            {
                var fileNames = data as string[];

                if (fileNames.Length > 0)
                    pictureBox1.Image = Image.FromFile(fileNames[0]);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.AllowDrop = true;
        }

        private void PictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    public abstract class Point
    {
        public double x;
        public double y;
        public Color color;
        public static double radius;
        public static Point[] shapes = new Point[1000];

        public Point()
        {
            x = Cursor.Position.X;
            y = Cursor.Position.Y;
            color = Color.White;
        }

        public Point(double x, double y, Color color)
        {
            this.x = x;
            this.y = y;
            this.color = color;
        }

        static Point()
        {
            radius = 26;

            for (int i = 0; i < shapes.Length; ++i)
            {
                if (shapes[i] != null)
                    shapes[i] = shapes[i];
                else
                    shapes[i] = null;
            }
        }

        public abstract void Draw(Graphics canvas, Brush brush);

        public abstract void Erase(Graphics canvas);
    }

    public class Circle : Point
    { 
        public Circle()
        {
            x = Cursor.Position.X;
            y = Cursor.Position.Y;
            color = Color.DarkRed;
        }

        public Circle(double x, double y, Color color)
        {
            this.x = x;
            this.y = y;
            this.color = color;
        }

        static Circle()
        {
            radius = 26;

            for (int i = 0; i < shapes.Length; ++i)
            {
                if (shapes[i] != null)
                    shapes[i] = shapes[i];
                else
                    shapes[i] = null;
            }
        }

        public override void Draw(Graphics canvas, Brush brush)
        {
            Rectangle rect = new Rectangle(Cursor.Position.X - (int)(radius / 2), Cursor.Position.Y - (int)radius - (int)(radius / 3), (int)radius, (int)radius);

            canvas.FillEllipse(brush, rect);
        }

        public override void Erase(Graphics canvas)
        {

        }
    }

    public class Triangle : Point
    {
        public Triangle()
        {
            x = Cursor.Position.X;
            y = Cursor.Position.Y;
            color = Color.DarkGreen;
        }

        public Triangle(double x, double y, Color color)
        {
            this.x = x;
            this.y = y;
            this.color = color;
        }

        static Triangle()
        {
            radius = 26;

            for (int i = 0; i < shapes.Length; ++i)
            {
                if (shapes[i] != null)
                    shapes[i] = shapes[i];
                else
                    shapes[i] = null;
            }
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

        public override void Erase(Graphics canvas)
        {

        }
    }

    public class Square : Point
    {
        public int X;
        public int Y;
        public Rectangle rect;

        public Square()
        {
            X = (int)(Cursor.Position.X + (int)radius / 2 - (int)radius / 6 + radius * Math.Cos(135 * (2 * Math.PI / 360)));
            Y = (int)(Cursor.Position.Y - (int)radius / 2 - radius * Math.Sin(135 * (2 * Math.PI / 360)));
            color = Color.DarkBlue;
            rect = new Rectangle(X, Y, (int)Math.Sqrt(radius * radius / 2), (int)Math.Sqrt(radius * radius / 2));
            x = rect.X;
            y = rect.Y;
        }

        public Square(double x, double y, int X, int Y, Color color, Rectangle rect) : base(x, y, color)
        {
            this.x = x;
            this.y = y;
            this.X = X;
            this.Y = Y;
            this.color = color;
            this.rect = rect;
        }

        static Square()
        {
            radius = 26;

            for (int i = 0; i < shapes.Length; ++i)
            {
                if (shapes[i] != null)
                    shapes[i] = shapes[i];
                else
                    shapes[i] = null;
            }
        }

        public override void Draw(Graphics canvas, Brush brush)
        {
            canvas.FillRectangle(brush, rect);
        }

        public override void Erase(Graphics canvas)
        {
            canvas.FillRectangle(new SolidBrush(Color.FromArgb(255, 192, 192)), rect);

            shapes[Form1.i - 1] = null;
        }
    }
}
