using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Computer_Graphics
{
    public partial class Exercise7 : Page
    {
        private Polygon polygon;
        private Ellipse landmark;
        private Point? clickOffset;

        public Exercise7()
        {
            InitializeComponent();
            polygon = new Polygon();
            polygon.Fill = Brushes.Blue;
            polygon.Stroke = Brushes.Black;
            vectorCanvas_c.Children.Add(polygon);

            landmark = new Ellipse();
            landmark.Height = 5;
            landmark.Width = 5;
            landmark.Margin = new Thickness(-10, -10, 0, 0);
            landmark.Fill = new SolidColorBrush(Colors.Red);
            vectorCanvas_c.Children.Add(landmark);

            Load();
        }

        private void vectorCanvas_c_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (createTool_rb.IsChecked == true)
            {
                Point tmp = e.GetPosition(vectorCanvas_c);
                polygon.Points.Add(tmp);
            }
            else if (moveTool_rb.IsChecked == true)
            {
                clickOffset = new Point(e.GetPosition(polygon).X, e.GetPosition(polygon).Y);
            }
            else if (rotateTool_rb.IsChecked == true || resizeTool_rb.IsChecked == true)
            {
                clickOffset = new Point(e.GetPosition(polygon).X, e.GetPosition(polygon).Y);
                if (pointLandmark_rb.IsChecked == true)
                {
                    double x = e.GetPosition(vectorCanvas_c).X;
                    double y = e.GetPosition(vectorCanvas_c).Y;

                    landmarkX_tb.Text = x.ToString();
                    landmarkY_tb.Text = y.ToString();

                    landmark.Margin = new Thickness(x, y, 0, 0);

                    enterLandmark_rb.IsChecked = true;
                    vectorCanvas_c.Cursor = Cursors.SizeNESW;
                }
            }
            Save();
        }

        private void vectorCanvas_c_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (moveTool_rb.IsChecked == true)
                {
                    Point difference = new Point(e.GetPosition(vectorCanvas_c).X - clickOffset.Value.X, e.GetPosition(vectorCanvas_c).Y - clickOffset.Value.Y);
                    for (int i = 0; i < polygon.Points.Count; i++)
                    {
                        polygon.Points[i] = new Point(polygon.Points[i].X + difference.X, polygon.Points[i].Y + difference.Y);
                    }
                    clickOffset = new Point(e.GetPosition(polygon).X, e.GetPosition(polygon).Y);
                }
                else if (rotateTool_rb.IsChecked == true)
                {
                    Point difference = new Point(e.GetPosition(vectorCanvas_c).X - clickOffset.Value.X, e.GetPosition(vectorCanvas_c).Y - clickOffset.Value.Y);

                    double landmarkX = Double.Parse(landmarkX_tb.Text);
                    double landmarkY = Double.Parse(landmarkY_tb.Text);

                    int dx = (int)Math.Abs(difference.X);
                    int dy = (int)Math.Abs(difference.Y);
                    double angleRadians = Math.Atan2(dy, dx);
                    double angleDegrees = ((angleRadians * 180) / Math.PI / 1000);

                    if (difference.X + difference.Y < 0)
                        angleDegrees *= -1;
                    for (int i = 0; i < polygon.Points.Count; i++)
                    {
                        polygon.Points[i] = new Point(landmarkX + (polygon.Points[i].X - landmarkX) * Math.Cos(angleDegrees) - (polygon.Points[i].Y - landmarkY) * Math.Sin(angleDegrees),
                            landmarkY + (polygon.Points[i].X - landmarkX) * Math.Sin(angleDegrees) + (polygon.Points[i].Y - landmarkY) * Math.Cos(angleDegrees));
                    }
                    clickOffset = new Point(e.GetPosition(polygon).X, e.GetPosition(polygon).Y);
                }
                else if (resizeTool_rb.IsChecked == true)
                {
                    Point difference = new Point(e.GetPosition(vectorCanvas_c).X - clickOffset.Value.X, e.GetPosition(vectorCanvas_c).Y - clickOffset.Value.Y);

                    double landmarkX = Double.Parse(landmarkX_tb.Text);
                    double landmarkY = Double.Parse(landmarkY_tb.Text);

                    //double ratioK = (Math.Pow(e.GetPosition(vectorCanvas_c).X - clickOffset.Value.X, 2) + Math.Pow(e.GetPosition(vectorCanvas_c).Y - clickOffset.Value.Y, 2));
                    double ratioK = 1.01;
                    if (difference.X + difference.Y < 0)
                        ratioK = 0.99;

                    for (int i = 0; i < polygon.Points.Count; i++)
                    {
                        polygon.Points[i] = new Point(polygon.Points[i].X * ratioK + (1 - ratioK) * landmarkX, polygon.Points[i].Y * ratioK + (1 - ratioK) * landmarkY);
                    }
                    clickOffset = new Point(e.GetPosition(polygon).X, e.GetPosition(polygon).Y);
                }
            }
            Save();
        }

        private void vectorCanvas_c_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            double x = e.GetPosition(vectorCanvas_c).X;
            double y = e.GetPosition(vectorCanvas_c).Y;

            landmarkX_tb.Text = x.ToString();
            landmarkY_tb.Text = y.ToString();

            landmark.Margin = new Thickness(x, y, 0, 0);
        }

        private void addPoint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double x = Double.Parse(newPointX.Text);
                double y = Double.Parse(newPointY.Text);
                Point tmp = new Point(x, y);
                polygon.Points.Add(tmp);
            }
            catch (FormatException)
            {
                MessageBox.Show("Enter coordinates");
            }
            Save();
        }
        private void removePoints_Click(object sender, RoutedEventArgs e)
        {
            polygon.Points.Clear();
            landmark.Margin = new Thickness(-10, -10, 0, 0);
            landmarkX_tb.Text = "";
            landmarkY_tb.Text = "";
            pointLandmark_rb.IsChecked = true;
            Save();
        }

        private void move_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double x = Double.Parse(moveVectorX.Text);
                double y = Double.Parse(moveVectorY.Text);
                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    polygon.Points[i] = new Point(polygon.Points[i].X + x, polygon.Points[i].Y + y);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Enter vector");
            }
            Save();
        }

        private void vektorToolChange_Click(object sender, RoutedEventArgs e)
        {
            if (createTool_rb.IsChecked == true)
            {
                vectorCanvas_c.Cursor = Cursors.Cross;
            }
            else if (moveTool_rb.IsChecked == true)
            {
                vectorCanvas_c.Cursor = Cursors.SizeAll;
            }
            else if (rotateTool_rb.IsChecked == true)
            {
                if (pointLandmark_rb.IsChecked == true)
                    vectorCanvas_c.Cursor = Cursors.Cross;
                else
                    vectorCanvas_c.Cursor = Cursors.SizeNESW;
            }
            else if (resizeTool_rb.IsChecked == true)
            {
                if (pointLandmark_rb.IsChecked == true)
                    vectorCanvas_c.Cursor = Cursors.Cross;
                else
                    vectorCanvas_c.Cursor = Cursors.SizeNWSE;
            }
            Save();
        }

        private void rotate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double landmarkX = Double.Parse(landmarkX_tb.Text);
                double landmarkY = Double.Parse(landmarkY_tb.Text);
                double angleDegrees = Double.Parse(angle_tb.Text);

                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    polygon.Points[i] = new Point(landmarkX + (polygon.Points[i].X - landmarkX) * Math.Cos(angleDegrees) - (polygon.Points[i].Y - landmarkY) * Math.Sin(angleDegrees),
                                                    landmarkY + (polygon.Points[i].X - landmarkX) * Math.Sin(angleDegrees) + (polygon.Points[i].Y - landmarkY) * Math.Cos(angleDegrees));
                }
                Save();
            }
            catch (FormatException)
            {
                MessageBox.Show("Enter angle and/or landmark point");
            }
        }
        private void resize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double landmarkX = Double.Parse(landmarkX_tb.Text);
                double landmarkY = Double.Parse(landmarkY_tb.Text);
                double ratioK = Double.Parse(ratioK_tb.Text);

                for (int i = 0; i < polygon.Points.Count; i++)
                {
                    polygon.Points[i] = new Point(polygon.Points[i].X * ratioK + (1 - ratioK) * landmarkX, polygon.Points[i].Y * ratioK + (1 - ratioK) * landmarkY);
                }
                Save();
            }
            catch (FormatException)
            {
                MessageBox.Show("Enter ratio K and/or landmark point");
            }
        }

        private void landmark_tb_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                double x = Double.Parse(landmarkX_tb.Text);
                double y = Double.Parse(landmarkY_tb.Text);

                landmark.Margin = new Thickness(x, y, 0, 0);
                Save();
            }
            catch (FormatException)
            {
                MessageBox.Show("Enter landmark");
            }
        }

        private void pointLandmark_rb_Click(object sender, RoutedEventArgs e)
        {
            vectorCanvas_c.Cursor = Cursors.Cross;
            Save();
        }

        private void Load()
        {
            try
            {
                using (StreamReader sr = new StreamReader("polygonPoints.txt"))
                {
                    string line;
                    line = sr.ReadLine();
                    string[] points = line.Split(null);

                    landmarkX_tb.Text = points[0];
                    landmarkY_tb.Text = points[1];
                    landmark.Margin = new Thickness(Double.Parse(points[0]), Double.Parse(points[1]), 0, 0);
                    enterLandmark_rb.IsChecked = true;

                    while (sr.Peek() >= 0)
                    {
                        line = sr.ReadLine();
                        points = line.Split(null);
                        polygon.Points.Add(new Point(Double.Parse(points[0]), Double.Parse(points[1])));
                    }
                }
            }
            catch (Exception)
            { }
        }

        private void Save()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("polygonPoints.txt"))
                {
                    sw.WriteLine(landmarkX_tb.Text.ToString() + " " + landmarkY_tb.Text.ToString());
                    foreach (Point point in polygon.Points)
                    {
                        sw.WriteLine(point.X.ToString() + " " + point.Y.ToString());
                    }
                }
            }
            catch (Exception)
            { }
        }
    }
}