using System.Windows;
using System.Windows.Media;

namespace DrawingVisualApp
{
    class Particle
    {
        public int col, row;
        public Point pos;
        public float r;
        public bool highlight;
        Brush brush;


        public Particle(int x, int y)
        {
            pos = new Point(x, y);
            r = MainWindow.rnd.Next(3) + 1;
            brush = Brushes.White;
        }

        public void Move()
        {
            var x = pos.X;
            var y = pos.Y;

            x += MainWindow.rnd.Next(-1, 2);
            y += MainWindow.rnd.Next(-1, 2);

            if (x <= 0) x = 1; else
            if (x >= MainWindow.width) x = MainWindow.width - 1; else
            if (y <= 0) y = 1; else
            if (y >= MainWindow.height) y = MainWindow.height - 1;

            pos.X = x;
            pos.Y = y;
        }

        public void Draw(DrawingContext dc)
        {
            if (highlight) 
                brush = Brushes.Red;
            else 
                brush = Brushes.White;

            dc.DrawEllipse(brush, null, pos, r, r);
        }
    }
}
