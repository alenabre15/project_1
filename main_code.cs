using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_1
{
    public partial class Form1 : Form
    {
        private static List<Point> points;

        private int pointChosen;

        public Form1()
        {
            InitializeComponent();

            pointChosen = 0;
        }

        static Form1()
        {
            points = new List<Point>();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            BackColor = Color.Black;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            points = Polygon.ConvexHull(points);

            foreach (Point point in points)
            {
                point.Draw(e.Graphics);
            }

            List<PointF> pointsF = new List<PointF>();

            for (int i = 0; i < points.Count; ++i)
            {
                pointsF.Add(new PointF((float)points[i].X, (float)points[i].Y));
            }          

            if (points.Count > 2)
                e.Graphics.FillPolygon(new SolidBrush(Color.White), pointsF.ToArray());
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int j = 0;

            if (e.Button == MouseButtons.Left)
            {
                foreach (Point Point in points)
                    if (Point.Inside(e.X, e.Y))
                        return;

                if (pointChosen == 0)
                {
                    return;
                }

                if (pointChosen == 1)
                    points.Add(new Circle(e.X, e.Y));

                if (pointChosen == 2)
                    points.Add(new Triangle(e.X, e.Y));

                if (pointChosen == 3)
                    points.Add(new Square(e.X, e.Y));

                Refresh();
            }

            if (e.Button == MouseButtons.Right)
            {
                j = 0;

                foreach (Point Point in points)
                {
                    ++j;

                    if (Point.Inside(e.X, e.Y))
                    {
                        break;
                    }
                }

                if (points[j - 1].Inside(e.X, e.Y))
                    points.Remove(points[j - 1]);

                Refresh();
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Point point in points)
            {
                if (point.Inside(e.X, e.Y))
                {
                    point.Dragging = true;

                    point.dX = e.X - point.X;
                    point.dY = e.Y - point.Y;
                }
            }

            if (Polygon.IsPointInPolygon(Polygon.ConvexHull(points).ToArray(), new PointF(e.X, e.Y)))
            {
                foreach (Point point in points)
                {
                    point.Dragging = true;

                    point.dX = e.X - point.X;
                    point.dY = e.Y - point.Y;
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (Point point in points)
            {
                if (point.Dragging)
                {
                    point.X = e.X - point.dX;
                    point.Y = e.Y - point.dY;
                }

                Refresh();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (Point Point in points)
            {
                Point.Dragging = false;
            }
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            pointChosen = 1;
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            pointChosen = 2;
        }

        private void ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            pointChosen = 3;
        }
    }

    public abstract class Point
    {
        protected double x;
        protected double y;
        protected double dx;
        protected double dy;
        protected bool move;
        protected bool dragging;
        protected static Color color;
        protected static double radius;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        static Point()
        {
            radius = 25;
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

        public double dX
        {
            get => dx;

            set => dx = value;
        }

        public double dY
        {
            get => dy;

            set => dy = value;
        }

        public bool Move
        {
            get => move;

            set => move = value;
        }

        public Color Color
        {
            get => color;

            set => color = value;
        }

        public bool Dragging
        {
            get => dragging;

            set => dragging = value;
        }

        public abstract void Draw(Graphics canvas);

        public abstract bool Inside(int MouseX, int MouseY);
    }

    public class Circle : Point
    {
        public Circle(double x, double y) : base(x, y)
        {
            this.x = x;
            this.y = y;
        }

        public override void Draw(Graphics canvas)
        {
            Rectangle rect = new Rectangle((int)x - (int)(radius), (int)y - (int)radius, 2 * (int)radius, 2 * (int)radius);

            canvas.FillEllipse(new SolidBrush(color), rect);
        }

        public override bool Inside(int MouseX, int MouseY)
        {
            return Math.Pow(x - MouseX, 2) + Math.Pow(y - MouseY, 2) <= radius * radius;
        }
    }

    public class Triangle : Point
    {
        public Triangle(double x, double y) : base(x, y)
        {
            this.x = x;
            this.y = y;
        }

        public override void Draw(Graphics canvas)
        {
            float Side = (float)(radius * 3) / (float)Math.Sqrt(3.0);

            PointF[] p = new PointF[3];

            p[0].X = (float)x;

            p[0].Y = (float)y - (float)radius;

            p[1].X = (float)(x + Side / 2);

            p[1].Y = (float)(y + radius / 2);

            p[2].X = (float)(x - Side / 2);

            p[2].Y = (float)(y + radius / 2);

            canvas.FillPolygon(new SolidBrush(color), p);
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
            float SideLength = (float)(radius * 3 / Math.Sqrt(3.0));

            int f1, f2, f3;

            f1 = PointOfLine(new int[] { MouseX, (int)x - (int)(SideLength / 2), (int)x }, new int[] { MouseY, (int)y + (int)(radius / 2), (int)y - (int)radius });
            f2 = PointOfLine(new int[] { MouseX, (int)x, (int)x + (int)(SideLength / 2) }, new int[] { MouseY, (int)y - (int)radius, (int)y + (int)(radius / 2) });
            f3 = PointOfLine(new int[] { MouseX, (int)x + (int)(SideLength / 2), (int)x - (int)(SideLength / 2) }, new int[] { MouseY, (int)y + (int)(radius / 2), (int)y + (int)(radius / 2) });

            return (f1 >= 0 && f2 >= 0 && f3 >= 0) || (f1 <= 0 && f2 <= 0 && f3 <= 0);
        }
    }

    public class Square : Point
    {
        public Square(double x, double y) : base(x, y)
        {
            this.x = x;
            this.y = y;
        }

        public override void Draw(Graphics canvas)
        {
            canvas.FillRectangle(new SolidBrush(color), new Rectangle((int)(x - radius * (float)Math.Sqrt(2) / 2), (int)(y - radius * (float)Math.Sqrt(2) / 2), (int)(radius * (float)Math.Sqrt(2)), (int)(radius * (float)Math.Sqrt(2))));
        }

        public override bool Inside(int MouseX, int MouseY)
        {
            if (MouseX >= x - (int)Math.Sqrt(radius * radius / 2) && MouseX <= x + (int)Math.Sqrt(radius * radius / 2) && MouseY >= y - (int)Math.Sqrt(radius * radius / 2) && MouseY <= y + (int)Math.Sqrt(radius * radius / 2))
            {
                return true;
            }
            else
                return false;
        }
    }

    public class Polygon
    {
        public static double Orientation(Point p, Point q, Point r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) -
                    (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0; // collinear 
            return (val > 0) ? 1 : 2; // cw or ccw 
        }

        // returns list of convex hull points
        public static List<Point> ConvexHull(List<Point> points)
        {
            // there must be at least 3 points 
            if (points.Count < 3) return points;

            // initialize result 
            List<Point> hull = new List<Point>();

            // find the leftmost point 
            int l = 0;
            for (int i = 1; i < points.Count; i++)
                if (points[i].X < points[l].X)
                    l = i;

            // start from leftmost point, keep moving  
            // counterclockwise until reach the start point 
            // again. this loop runs O(h) times where h is 
            // number of points in result or output

            int p = l, q;

            do
            {
                // add current point to result 
                hull.Add(points[p]);

                // search for a point 'q' such that  
                // orientation(p, x, q) is counterclockwise  
                // for all points 'x'. the idea is to keep  
                // track of last visited most counterclock- 
                // wise point in q. if any point 'i' is more  
                // counterclock-wise than q, then update q

                q = (p + 1) % points.Count;

                for (int i = 0; i < points.Count; i++)
                {
                    // if i is more counterclockwise than  
                    // current q, then update q 

                    if (Orientation(points[p], points[i], points[q]) == 2)
                        q = i;
                }

                // now q is the most counterclockwise with 
                // respect to p. set p as q for next iteration,  
                // so that q is added to result 'hull' 

                p = q;

            } while (p != l); // while we don't come to first  
                              // point 

            return hull;
        }

        public static bool IsPointInPolygon(Point[] polygon, PointF point)
        {
            bool isInside = false;

            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                    isInside = !isInside;
            }

            return isInside;
        }
    }
}
