using MDS_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MDS_App
{
    public class Visualization
    {
        private Point[] points;
        private Line[] linesDisplayed;
        private Ellipse[] pointsDisplayed;
        private int pointsCount;
        private int linesCount;
        private bool drawPoints = false;
        Polyhedron type;
        Canvas c;

        public Point[] Points { get => points; }
        public bool DrawPoints { get => drawPoints; set => drawPoints = value; }

        public Visualization(int dim, Canvas canvas, Polyhedron polyhedronType)
        {
            c = canvas;
            type = polyhedronType;

            switch (polyhedronType)
            {
                case Polyhedron.Simplex:
                    pointsCount = Simplex.GetVerticesCount(dim);
                    linesCount = Simplex.GetEdgesCount(dim);
                    break;
                case Polyhedron.Hypercube:
                    pointsCount = Hypercube.GetVerticesCount(dim);
                    linesCount = Hypercube.GetEdgesCount(dim);
                    break;
                default:
                    pointsCount = 0;
                    linesCount = 0;
                    break;
            }

            points = new Point[pointsCount];
            for (int i = 0; i < pointsCount; i++)
                points[i] = new Point(0, 0);
        }

        private void InitLines()
        {
            // Edges
            switch (type)
            {
                case Polyhedron.Simplex:
                    InitSimplexEdges();
                    break;
                case Polyhedron.Hypercube:
                    InitHypercubeEdges();
                    break;
                default:
                    break;
            }
        }
        private void InitPoints()
        {
            // Points
            pointsDisplayed = new Ellipse[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                pointsDisplayed[i] = new Ellipse()
                {
                    Stroke = Brushes.MidnightBlue,
                    Fill = Brushes.MidnightBlue,

                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,

                    StrokeThickness = 1,

                    Height = 8,
                    Width = 8
                };

                c.Children.Add(pointsDisplayed[i]);
            }
        }
        public void Draw()
        {
            // Init lines
            if (linesDisplayed == null)
                InitLines();
            // Init points
            if (drawPoints && pointsDisplayed == null)
                InitPoints();
            // Unload points
            if (drawPoints == false && pointsDisplayed != null)
                ClearPoints();

            // Edges
            switch (type)
            {
                case Polyhedron.Simplex:
                    DrawSimplexEdges();
                    break;
                case Polyhedron.Hypercube:
                    DrawHypercubeEdges();
                    break;
                default:
                    break;
            }

            // Points
            if (drawPoints)
            {
                for (int i = 0; i < pointsCount; i++)
                    pointsDisplayed[i].Margin = new System.Windows.Thickness(points[i].x - 4, points[i].y - 4, 0, 0);
            }
            
        }
        public void Clear()
        {
            if (linesDisplayed != null)
            {
                foreach (var l in linesDisplayed)
                    c.Children.Remove(l);
            }
            linesDisplayed = null;

            ClearPoints();
        }
        private void ClearPoints()
        {
            if (pointsDisplayed != null)
            {
                foreach (var p in pointsDisplayed)
                    c.Children.Remove(p);
            }
            pointsDisplayed = null;
        }

        public class Point
        {
            public double x, y;

            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public double Odleglosc(Point p)
            {
                return Math.Sqrt(Math.Pow(x - p.x, 2) + Math.Pow(y - p.y, 2));
            }
        }

        private void InitSimplexEdges()
        {
            if (pointsCount >= 2)
            {
                linesDisplayed = new Line[linesCount];

                int k = 0;
                for (int i = 0; i < pointsCount; i++)
                    for (int j = i + 1; j < pointsCount; j++)
                    {
                        linesDisplayed[k] = new Line()
                        {
                            Stroke = Brushes.Black,
                            Fill = Brushes.Black,
                            StrokeThickness = 1,

                            X1 = points[i].x,
                            Y1 = points[i].y,
                            X2 = points[j].x,
                            Y2 = points[j].y
                        };

                        c.Children.Add(linesDisplayed[k]);
                        k++;
                    }
                
            }
        }
        private void InitHypercubeEdges()
        {
            if (pointsCount >= 2)
            {
                linesDisplayed = new Line[linesCount];

                int k = 0;
                for (int i = 0; i < pointsCount; i++)
                    for (int j = i + 1; j < pointsCount; j++)
                        if (Hypercube.HammingDistance(i,j) == 1)
                        {
                            linesDisplayed[k] = new Line()
                            {
                                Stroke = Brushes.Black,
                                Fill = Brushes.Black,
                                StrokeThickness = 1,

                                X1 = points[i].x,
                                Y1 = points[i].y,
                                X2 = points[j].x,
                                Y2 = points[j].y
                            };

                            c.Children.Add(linesDisplayed[k]);
                            k++;
                        }
            }
        }
        private void DrawSimplexEdges()
        {
            int k = 0;
            for (int i = 0; i < pointsCount; i++)
                for (int j = i + 1; j < pointsCount; j++)
                {
                    linesDisplayed[k].X1 = points[i].x;
                    linesDisplayed[k].Y1 = points[i].y;
                    linesDisplayed[k].X2 = points[j].x;
                    linesDisplayed[k].Y2 = points[j].y;

                    k++;
                }
        }
        private void DrawHypercubeEdges()
        {
            int k = 0;
            for (int i = 0; i < pointsCount; i++)
                for (int j = i + 1; j < pointsCount; j++)
                    if (Hypercube.HammingDistance(i,j) == 1) // Hamming distance == 1  =>  edge present
                    {
                        linesDisplayed[k].X1 = points[i].x;
                        linesDisplayed[k].Y1 = points[i].y;
                        linesDisplayed[k].X2 = points[j].x;
                        linesDisplayed[k].Y2 = points[j].y;

                        k++;
                    }
        }
    }
}
