using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows;

namespace KG5
{
    public partial class Form1 : Form
    {
        const int PIX_IN_ONE = 10;
        //Длина стрелки
        const int ARR_LEN = 10;
        
        private Bitmap _sBitmap;
        private List<Line> _lines = new List<Line>();
        private List<PointF> _rectanglePoints = new List<PointF>();
        private Rect rect;
        public Form1()
        {
            InitializeComponent();

            _sBitmap = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            int w = pictureBox1.ClientSize.Width / 2;
            int h = pictureBox1.ClientSize.Height / 2;
            var g = Graphics.FromImage(_sBitmap);
            g.TranslateTransform(w, h);
            DrawXAxis(new System.Drawing.Point(-w, 0), new System.Drawing.Point(w, 0), g);
            DrawYAxis(new System.Drawing.Point(0, h), new System.Drawing.Point(0, -h), g);
            g.FillEllipse(Brushes.Red, -2, -2, 4, 4);
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var n = 0;
                    var lines = new List<Line>();
                    _lines = lines;
                    using (var stream = new StreamReader(filePath))
                    {
                        n = int.Parse(stream.ReadLine());
                        var tempLine = string.Empty;
                        string[] coords;
                        for (var i = 0; i < n; i++)
                        {
                            tempLine = stream.ReadLine();
                            coords = tempLine.Split(' ');
                            lines.Add(new Line(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]), int.Parse(coords[3])));

                        }
                        tempLine = stream.ReadLine();
                        coords = tempLine.Split(' ');
                        rect = new Rect(float.Parse(coords[0]), float.Parse(coords[1]), Math.Abs((float.Parse(coords[0]) - float.Parse(coords[2]))), Math.Abs((float.Parse(coords[1]) - float.Parse(coords[3]))));

                        for (var i = 0; i < n; i++)
                        {
                            g.DrawLine(Pens.Blue, new System.Drawing.PointF(lines[i].X1 * PIX_IN_ONE, -lines[i].Y1 * PIX_IN_ONE), new System.Drawing.PointF(lines[i].X2 * PIX_IN_ONE, -lines[i].Y2 * PIX_IN_ONE));
                        }

                        g.DrawRectangle(Pens.Yellow, new Rectangle(int.Parse(coords[0]) * PIX_IN_ONE, -int.Parse(coords[3]) * PIX_IN_ONE, (int)Math.Abs((int.Parse(coords[0]) - int.Parse(coords[2])))*PIX_IN_ONE, (int)Math.Abs((int.Parse(coords[1]) - int.Parse(coords[3])) * PIX_IN_ONE)));
                    }
                }
            }

            var polygon = new List<System.Drawing.Point>()
            {
                new System.Drawing.Point(-20*PIX_IN_ONE,-5*PIX_IN_ONE),
                new System.Drawing.Point(-25*PIX_IN_ONE,-10*PIX_IN_ONE),
                new System.Drawing.Point(-20*PIX_IN_ONE,-15*PIX_IN_ONE),
                new System.Drawing.Point(-10*PIX_IN_ONE,-15*PIX_IN_ONE),
                new System.Drawing.Point(-5*PIX_IN_ONE,-10*PIX_IN_ONE),
                new System.Drawing.Point(-10*PIX_IN_ONE,-5*PIX_IN_ONE)
            };

            var line = new List<System.Drawing.Point>()
            {
                new System.Drawing.Point(-27*PIX_IN_ONE,-2*PIX_IN_ONE),
                new System.Drawing.Point(-2*PIX_IN_ONE,-13*PIX_IN_ONE)
            };

            g.DrawPolygon(Pens.Yellow, new PointF[6]
            {
                new System.Drawing.PointF(polygon[0].X, -polygon[0].Y),
                new System.Drawing.PointF(polygon[1].X, -polygon[1].Y),
                new System.Drawing.PointF(polygon[2].X, -polygon[2].Y),
                new System.Drawing.PointF(polygon[3].X, -polygon[3].Y),
                new System.Drawing.PointF(polygon[4].X, -polygon[4].Y),
                new System.Drawing.PointF(polygon[5].X, -polygon[5].Y)
            });

            
            g.DrawLine(Pens.Blue, new System.Drawing.Point(line[0].X, -line[0].Y), new System.Drawing.Point(line[1].X, -line[1].Y));
            var newLine = CyrusBeck(polygon, line, polygon.Count);
            g.DrawLine(new Pen(Color.Red, 3), new System.Drawing.Point(newLine[0].X, -newLine[0].Y), new System.Drawing.Point(newLine[1].X, -newLine[1].Y));
            pictureBox1.Image = _sBitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void MiddlePointClipping(Line line, Graphics g)
        {
            Helper helper = new Helper();
            if (Math.Sqrt(Math.Pow((line.X2 - line.X1), 2)*PIX_IN_ONE + Math.Pow((line.Y2 - line.Y1), 2)*PIX_IN_ONE) <= 1)
            {
                return;
            }

            if (!rect.Contains(line.X1,line.Y1) && !rect.Contains(line.X2,line.Y2) && !helper.LineIntersectsRect(new System.Drawing.PointF(line.X1, line.Y1), new System.Drawing.PointF(line.X2, line.Y2), rect))
            {
                return;
            }

            if (rect.Contains(line.X1, line.Y1) && rect.Contains(line.X2, line.Y2))
            {
                g.DrawLine(new Pen(Color.Green, 3), line.X1*PIX_IN_ONE, -line.Y1*PIX_IN_ONE, line.X2*PIX_IN_ONE, -line.Y2*PIX_IN_ONE);
                return;
            }
            
            MiddlePointClipping(
                new Line(line.X1, line.Y1, (line.X1 + line.X2) / 2, (line.Y1 + line.Y2) / 2), g);
            MiddlePointClipping(
                new Line((line.X1 + line.X2) / 2, (line.Y1 + line.Y2) / 2, line.X2, line.Y2), g);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            

        }

        #region axis
        //Рисование оси X
        private void DrawXAxis(System.Drawing.Point start, System.Drawing.Point end, Graphics g)
        {
            //Деления в положительном направлении оси
            for (int i = PIX_IN_ONE; i < end.X - ARR_LEN; i += PIX_IN_ONE)
            {
                g.DrawLine(Pens.Gray, i, start.X, i, end.X);
                DrawText(new System.Drawing.Point(i, 5), (i / PIX_IN_ONE).ToString(), g);
            }
            //Деления в отрицательном направлении оси
            for (int i = -PIX_IN_ONE; i > start.X; i -= PIX_IN_ONE)
            {
                g.DrawLine(Pens.Gray, i, start.X, i, end.X);
                DrawText(new System.Drawing.Point(i, 5), (i / PIX_IN_ONE).ToString(), g);
            }
            //Ось
            g.DrawLine(Pens.Black, start, end);
            //Стрелка
            g.DrawLines(Pens.Black, GetArrow(start.X, start.Y, end.X, end.Y, ARR_LEN));
        }

        //Рисование оси Y
        private void DrawYAxis(System.Drawing.Point start, System.Drawing.Point end, Graphics g)
        {
            //Деления в отрицательном направлении оси
            for (int i = PIX_IN_ONE; i < start.Y; i += PIX_IN_ONE)
            {
                g.DrawLine(Pens.Gray, start.Y, i, end.Y, i);
                DrawText(new System.Drawing.Point(5, i), (-i / PIX_IN_ONE).ToString(), g, true);
            }
            //Деления в положительном направлении оси
            for (int i = -PIX_IN_ONE; i > end.Y + ARR_LEN; i -= PIX_IN_ONE)
            {
                g.DrawLine(Pens.Gray, start.Y, i, end.Y, i);
                DrawText(new System.Drawing.Point(5, i), (-i / PIX_IN_ONE).ToString(), g, true);
            }
            //Ось
            g.DrawLine(Pens.Black, start, end);
            //Стрелка
            g.DrawLines(Pens.Black, GetArrow(start.X, start.Y, end.X, end.Y, ARR_LEN));
        }

        //Рисование текста
        private void DrawText(System.Drawing.Point point, string text, Graphics g, bool isYAxis = false)
        {
            var f = new Font(Font.FontFamily, 6);
            var size = g.MeasureString(text, f);
            var pt = isYAxis
                ? new PointF(point.X + 1, point.Y - size.Height / 2)
                : new PointF(point.X - size.Width / 2, point.Y + 1);
            var rect = new RectangleF(pt, size);
            g.DrawString(text, f, Brushes.Black, rect);
        }

        //Вычисление стрелки оси
        private static PointF[] GetArrow(float xe, float ye, float x2, float y2, float len = 10, float width = 4)
        {
            PointF[] result = new PointF[3];
            //направляющий вектор отрезка
            var n = new PointF(x2 - xe, y2 - ye);
            //Длина отрезка
            var l = (float)Math.Sqrt(n.X * n.X + n.Y * n.Y);
            //Единичный вектор
            var v1 = new PointF(n.X / l, n.Y / l);
            //Длина стрелки
            n.X = x2 - v1.X * len;
            n.Y = y2 - v1.Y * len;
            result[0] = new PointF(n.X + v1.Y * width, n.Y - v1.X * width);
            result[1] = new PointF(x2, y2);
            result[2] = new PointF(n.X - v1.Y * width, n.Y + v1.X * width);
            return result;
        }

        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            var g = Graphics.FromImage(_sBitmap);
            int w = pictureBox1.ClientSize.Width / 2;
            int h = pictureBox1.ClientSize.Height / 2;
            g.TranslateTransform(w, h);
            foreach (var i in _lines)
            {
                MiddlePointClipping(i, g);
            }
            pictureBox1.Image = _sBitmap;
        }
        private static int dot(System.Drawing.Point p0, System.Drawing.Point p1)
        {
            return p0.X * p1.X + p0.Y * p1.Y;
        }

        private static double max(List<double> list)
        {
            double maximum = int.MinValue;
            for (int i = 0; i < list.Count; i++)
                if (list[i] > maximum)
                    maximum = list[i];
            return maximum;
        }


        private static double min(List<double> list)
        {
            double minimum = int.MaxValue;
            for (int i = 0; i < list.Count; i++)
                if (list[i] < minimum)
                    minimum = list[i];
            return minimum;
        }

        System.Drawing.Point[] CyrusBeck(List<System.Drawing.Point> vertices, List<System.Drawing.Point> line, int n)
        {
            System.Drawing.Point[] newPair = new System.Drawing.Point[2];

            System.Drawing.Point[] normal = new System.Drawing.Point[n];


            for (int i = 0; i < n; i++)
            {
                normal[i] = new System.Drawing.Point(vertices[i].Y - vertices[(i + 1) % n].Y, vertices[(i + 1) % n].X - vertices[i].X);
            }

            System.Drawing.Point P1_P0 = new System.Drawing.Point(line[1].X - line[0].X, line[1].Y - line[0].Y);


            System.Drawing.Point[] P0_PEi = new System.Drawing.Point[n];

            for (int i = 0; i < n; i++)
            {
                P0_PEi[i] = new System.Drawing.Point(vertices[i].X - line[0].X, vertices[i].Y - line[0].Y);

            }

            int[] numerator = new int[n], denominator = new int[n];

            for (int i = 0; i < n; i++)
            {
                numerator[i] = dot(normal[i], P0_PEi[i]);
                denominator[i] = dot(normal[i], P1_P0);
            }

            double[] t = new double[n];


            List<double> tE = new List<double>(), tL = new List<double>();

            for (int i = 0; i < n; i++)
            {

                t[i] = (double)(numerator[i]) / (denominator[i]);

                if (denominator[i] > 0)
                    tE.Add(t[i]);
                else
                    tL.Add(t[i]);
            }

            double[] temp = new double[2];

            tE.Add(0.0f);
            temp[0] = max(tE);

            tL.Add(1.0f);
            temp[1] = min(tL);

            if (temp[0] > temp[1])
            {
                newPair[0] = new System.Drawing.Point(-1, -1);
                newPair[1] = new System.Drawing.Point(-1, -1);
                return newPair;
            }


            newPair[0] = new System.Drawing.Point((int)((float)line[0].X + (float)P1_P0.X * (float)temp[0]), (int)((float)line[0].Y + (float)P1_P0.Y * (float)temp[0]));
            newPair[1] = new System.Drawing.Point((int)((float)line[0].X + (float)P1_P0.X * (float)temp[1]),(int)((float)line[0].Y + (float)P1_P0.Y * (float)temp[1]));

            return newPair;
        }
        }
    }
