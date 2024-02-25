using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;


namespace DrawingVisualApp
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        public static Random rnd = new Random();
        public static int width, height;

        DrawingVisual visual;
        DrawingContext dc;
        List<Particle> particles = new List<Particle>();

        int resolution = 10;
        int rows, cols;
        List<Particle>[,] grid;
        QuadTree qtree;
        Rectangle boundary;

        string timervalue;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();

            width = (int)g.Width;
            height = (int)g.Height;

            cols = width / resolution;
            rows = height / resolution;


            grid = new List<Particle>[rows, cols];

            for (int i = 0; i < rows; ++i)
                for(int j = 0; j < cols; ++j)
                    grid[i, j] = new List<Particle>();
            
            for (int i = 0; i < 2000; ++i)
                particles.Add(new Particle(rnd.Next(width), rnd.Next(height)));

            // for Quadtree
            boundary = new Rectangle(0, 0, width, height);


            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            timer.Start();
        }



        private void timerTick(object sender, EventArgs e)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            Stopwatch sw3 = new Stopwatch();
            Stopwatch sw4 = new Stopwatch();

            lbTime1.Content = "0";
            lbTime2.Content = "0";
            lbTime3.Content = "0";
            lbTime4.Content = "0";

            if (cbTest1.IsChecked is true)
            {
                sw1.Start();
                NotOptimized();
                sw1.Stop();
            }

            if (cbTest2.IsChecked is true)
            {
                sw2.Start();
                Optimazed_1();
                sw2.Stop();
            }

            if (cbTest3.IsChecked is true)
            {
                sw3.Start();
                Optimazed_2();
                sw3.Stop();
            }

            if (cbTest4.IsChecked is true)
            {
                sw4.Start();
                Optimazed_3();
                sw4.Stop();
            }

            Drawing();

            lbTime1.Content = sw1.ElapsedMilliseconds + " msec.";
            lbTime2.Content = sw2.ElapsedMilliseconds + " msec."; 
            lbTime3.Content = sw3.ElapsedMilliseconds + " msec.";
            lbTime4.Content = sw4.ElapsedMilliseconds + " msec."; 
        }

        private void NotOptimized()
        {
            foreach (Particle particle in particles)
            {
                // Against every other Thing
                foreach (Particle other in particles)
                {
                    // As long as its not the same one
                    if (other != particle)
                    {
                        var x1 = (int)particle.pos.X;
                        var y1 = (int)particle.pos.Y;
                        var x2 = (int)other.pos.X;
                        var y2 = (int)other.pos.Y;
                        float d = Dist(x1, y1, x2, y2);
                        if (d < particle.r + other.r)
                        {
                            particle.highlight = true;
                        } 
                    }
                }
            }
        }

        private void Optimazed_1()
        {
            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j)
                    grid[i, j].Clear();

            // Register every particle object in the grid according to it's location
            foreach (var p in particles)
            {
                //p.highlight = false;
                int column = (int)p.pos.X / resolution;
                int row = (int)p.pos.Y / resolution;
                grid[row, column].Add(p);
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (grid[i, j].Count == 0) continue;
                    
                    List<Particle> temp = new List<Particle>();
                    foreach(var g in grid[i, j])
                    {
                        temp.Add(g);
                    }
                    Array.Copy(grid[i, j].ToArray(), temp.ToArray(), 0);
                    
                    foreach (Particle t in temp)
                    {
                        foreach (Particle other in temp)
                        {
                            if (other != t)
                            {
                                var x1 = (int)t.pos.X;
                                var y1 = (int)t.pos.Y;
                                var x2 = (int)other.pos.X;
                                var y2 = (int)other.pos.Y;
                                float d = Dist(x1, y1, x2, y2);
                                if (d < t.r + other.r)
                                {
                                    t.highlight = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Optimazed_2()
        {
            foreach (var p in particles)
            {
                //p.highlight = false;
                int column = (int)p.pos.X / resolution;
                int row = (int)p.pos.Y / resolution;

                p.col = column;
                p.row = row;
            }

            foreach (Particle p in particles)
            {
                foreach (Particle other in particles)
                {
                    if (p == other) continue;
                    
                    if (p.col == other.col && p.row == other.row)
                    {
                        var x1 = (int)p.pos.X;
                        var y1 = (int)p.pos.Y;
                        var x2 = (int)other.pos.X;
                        var y2 = (int)other.pos.Y;
                        float d = Dist(x1, y1, x2, y2);

                        if (d < p.r + other.r)
                        {
                            p.highlight = true;
                        }
                    }
                }
            }
        }

        private void Optimazed_3()
        {
            var qtree = new QuadTree(boundary, 4);

            foreach (var p in particles)
            {
                var point = new myPoint(p.pos.X, p.pos.Y, p);
                qtree.Insert(point);
                //p.highlight = false;
            }

            foreach (var p in particles)
            {
                var range = new Circle(p.pos.X, p.pos.Y, (int)p.r * 2);
                var points = qtree.Query(range);
                foreach (var point in points)
                {
                    var other = point.userData;

                    if (p == other) continue;

                    var x1 = (int)p.pos.X;
                    var y1 = (int)p.pos.Y;
                    var x2 = (int)other.pos.X;
                    var y2 = (int)other.pos.Y;
                    float d = Dist(x1, y1, x2, y2);

                    if (d < p.r + other.r)
                    {
                        p.highlight = true;
                    }
                }
            }
        } //  QuadTree

        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                foreach (var particle in particles)
                {
                    particle.Move();
                    particle.Draw(dc);

                    particle.highlight = false; 
                }

                dc.Close();
                g.AddVisual(visual);
            }
        }

        float Dist(int x1, int y1, int x2, int y2) => (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
}
