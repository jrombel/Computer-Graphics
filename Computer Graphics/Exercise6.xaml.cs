using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System;

namespace Computer_Graphics
{
    public partial class Exercise6 : Page
    {
        List<BezierPoint> points;
        private BezierPoint lastClickedPoint;
        private System.Drawing.Point? clickOffset;
        private int lastClickedControl;
        private int modifyIndex;

        public Exercise6()
        {
            InitializeComponent();

            points = new List<BezierPoint>();
            changeT_s.Value = 0.003;
        }

        private void bezierPoints_c_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (addBezierTool_rb.IsChecked == true)
            {
                BezierPoint tmp = new BezierPoint(new System.Drawing.Point((int)e.GetPosition(bezier_c).X, (int)e.GetPosition(bezier_c).Y));
                points.Add(tmp);
                pointsAmout_tb.Text = points.Count.ToString();

                modifyIndex = points.IndexOf(tmp);
                coordinatePointX_tb.Text = tmp.point.X.ToString();
                coordinatePointY_tb.Text = tmp.point.Y.ToString();
                coordinateControl1X_tb.Text = tmp.control1.X.ToString();
                coordinateControl1Y_tb.Text = tmp.control1.Y.ToString();
                coordinateControl2X_tb.Text = tmp.control2.X.ToString();
                coordinateControl2Y_tb.Text = tmp.control2.Y.ToString();
            }
            else if (moveBezierTool_rb.IsChecked == true)
            {
                Shape clicked = null;

                var mouseWasDownOn = e.Source as FrameworkElement;
                if (mouseWasDownOn != null)
                {

                    foreach (Shape child in bezierPoints_c.Children)
                    {
                        if (child.Margin.Left == mouseWasDownOn.Margin.Left && child.Margin.Top == mouseWasDownOn.Margin.Top)
                        {
                            clicked = child;
                            break;
                        }
                    }
                }

                if (clicked == null)
                {

                }
                else if (clicked.GetType().ToString() == "System.Windows.Shapes.Ellipse")
                {
                    foreach (BezierPoint bezierPoint in points)
                    {
                        if ((bezierPoint.point.X == clicked.Margin.Left) && (bezierPoint.point.Y == clicked.Margin.Top))
                        {
                            lastClickedPoint = bezierPoint;
                            lastClickedControl = 0;
                            modifyIndex = points.IndexOf(bezierPoint);
                            break;
                        }
                        else if ((bezierPoint.control1.X == clicked.Margin.Left) && (bezierPoint.control1.Y == clicked.Margin.Top))
                        {
                            lastClickedPoint = bezierPoint;
                            lastClickedControl = 1;
                            modifyIndex = points.IndexOf(bezierPoint);
                            break;
                        }
                        else if ((bezierPoint.control2.X == clicked.Margin.Left) && (bezierPoint.control2.Y == clicked.Margin.Top))
                        {
                            lastClickedPoint = bezierPoint;
                            lastClickedControl = 2;
                            modifyIndex = points.IndexOf(bezierPoint);
                            break;
                        }
                    }
                }
                clickOffset = new System.Drawing.Point((int)(e.GetPosition(clicked).X), (int)(e.GetPosition(clicked).Y));
            }
            else if (removeBezierTool_rb.IsChecked == true)
            {
                Shape clicked = null;

                var mouseWasDownOn = e.Source as FrameworkElement;
                if (mouseWasDownOn != null)
                {

                    foreach (Shape child in bezierPoints_c.Children)
                    {
                        if (child.Margin.Left == mouseWasDownOn.Margin.Left && child.Margin.Top == mouseWasDownOn.Margin.Top)
                        {
                            clicked = child;
                            break;
                        }
                    }
                }

                if (clicked == null)
                {

                }
                else if (clicked.GetType().ToString() == "System.Windows.Shapes.Ellipse")
                {
                    foreach (BezierPoint bezierPoint in points)
                    {
                        if ((bezierPoint.point.X == clicked.Margin.Left) && (bezierPoint.point.Y == clicked.Margin.Top))
                        {
                            points.Remove(bezierPoint);
                            break;
                        }
                    }
                }
                pointsAmout_tb.Text = points.Count.ToString();
            }

            Draw();
        }

        private void bezierPoints_c_MouseMove(object sender, MouseEventArgs e)
        {
            if (lastClickedPoint == null)
                return;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (lastClickedControl == 0)
                {
                    lastClickedPoint.point.X = (int)e.GetPosition(bezier_c).X - clickOffset.Value.X;
                    lastClickedPoint.point.Y = (int)e.GetPosition(bezier_c).Y - clickOffset.Value.Y;
                    lastClickedPoint.control1.X = (int)e.GetPosition(bezier_c).X - clickOffset.Value.X - 10;
                    lastClickedPoint.control1.Y = (int)e.GetPosition(bezier_c).Y - clickOffset.Value.Y - 10;
                    lastClickedPoint.control2.X = (int)e.GetPosition(bezier_c).X - clickOffset.Value.X + 10;
                    lastClickedPoint.control2.Y = (int)e.GetPosition(bezier_c).Y - clickOffset.Value.Y + 10;
                }
                else if (lastClickedControl == 1)
                {
                    lastClickedPoint.control1.X = (int)e.GetPosition(bezier_c).X - clickOffset.Value.X;
                    lastClickedPoint.control1.Y = (int)e.GetPosition(bezier_c).Y - clickOffset.Value.Y;
                }
                else if (lastClickedControl == 2)
                {
                    lastClickedPoint.control2.X = (int)e.GetPosition(bezier_c).X - clickOffset.Value.X;
                    lastClickedPoint.control2.Y = (int)e.GetPosition(bezier_c).Y - clickOffset.Value.Y;
                }

                coordinatePointX_tb.Text = points[modifyIndex].point.X.ToString();
                coordinatePointY_tb.Text = points[modifyIndex].point.Y.ToString();
                coordinateControl1X_tb.Text = points[modifyIndex].control1.X.ToString();
                coordinateControl1Y_tb.Text = points[modifyIndex].control1.Y.ToString();
                coordinateControl2X_tb.Text = points[modifyIndex].control2.X.ToString();
                coordinateControl2Y_tb.Text = points[modifyIndex].control2.Y.ToString();

                Draw();
                System.Threading.Thread.Sleep(10);
            }
            else
                lastClickedPoint = null;
        }

        private void bezierPoints_c_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            lastClickedPoint = null;
        }

        private void Draw()
        {
            System.Drawing.Point tmp;
            bezier_c.Children.Clear();
            if (points.Count >= 2)
            {
                for (int i = 0; i < (points.Count - 1); i++)
                {
                    for (double t = 0; t <= 1; t += changeT_s.Value)
                    {
                        tmp = GetPoint(t, points[i].point, points[i].control2, points[i + 1].control1, points[i + 1].point);
                        DrawPoint(tmp, Colors.Black, 1, false);
                    }
                }
            }
            bezierPoints_c.Children.Clear();
            foreach (BezierPoint bezierPoint in points)
            {
                DrawPoint(bezierPoint.point, Colors.Red, 5, true);
                if (points.IndexOf(bezierPoint) != 0)
                    DrawPoint(bezierPoint.control1, Colors.Green, 5, true);
                if (points.IndexOf(bezierPoint) != (points.Count - 1))
                    DrawPoint(bezierPoint.control2, Colors.Green, 5, true);
            }
        }

        private void DrawPoint(System.Drawing.Point point, System.Windows.Media.Color color, int size, bool pivotalPoint)
        {
            int dotSize = size;

            Ellipse currentDot = new Ellipse();
            currentDot.Stroke = new SolidColorBrush(color);
            currentDot.StrokeThickness = 3;
            Canvas.SetZIndex(currentDot, 3);
            currentDot.Height = dotSize;
            currentDot.Width = dotSize;
            currentDot.Fill = new SolidColorBrush(color);
            currentDot.Margin = new Thickness(point.X, point.Y, 0, 0);
            //currentDot.Margin = new Thickness(point.X - size / 2, point.Y - size / 2, 0, 0); // Sets the position.
            if (pivotalPoint == false)
                bezier_c.Children.Add(currentDot);
            else
                bezierPoints_c.Children.Add(currentDot);
        }

        private System.Drawing.Point GetPoint(double t, System.Drawing.Point p0, System.Drawing.Point p1, System.Drawing.Point p2, System.Drawing.Point p3)
        {
            double cx = 3 * (p1.X - p0.X);
            double cy = 3 * (p1.Y - p0.Y);
            double bx = 3 * (p2.X - p1.X) - cx;
            double by = 3 * (p2.Y - p1.Y) - cy;
            double ax = p3.X - p0.X - cx - bx;
            double ay = p3.Y - p0.Y - cy - by;
            double Cube = t * t * t;
            double Square = t * t;

            double resX = (ax * Cube) + (bx * Square) + (cx * t) + p0.X;
            double resY = (ay * Cube) + (by * Square) + (cy * t) + p0.Y;

            return new System.Drawing.Point((int)resX, (int)resY);
        }


        private void reduceAmount_Click(object sender, RoutedEventArgs e)
        {
            if (points.Count > 0)
                points.RemoveAt(points.Count - 1);
            pointsAmout_tb.Text = points.Count.ToString();
            Draw();
        }

        private void increaseAmount_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            points.Add(new BezierPoint(new System.Drawing.Point(rnd.Next(0, 300), rnd.Next(0, 300))));
            pointsAmout_tb.Text = points.Count.ToString();
            Draw();
        }

        private void removeAllPoints_Click(object sender, RoutedEventArgs e)
        {
            points.Clear();
            Draw();
        }

        private void coordinate_TextChanged(object sender, RoutedEventArgs e)
        {
            if (coordinatePointX_tb.Text != "")
                points[modifyIndex].point.X = Int32.Parse(coordinatePointX_tb.Text);
            if (coordinatePointY_tb.Text != "")
                points[modifyIndex].point.Y = Int32.Parse(coordinatePointY_tb.Text);
            if (coordinateControl1X_tb.Text != "")
                points[modifyIndex].control1.X = Int32.Parse(coordinateControl1X_tb.Text);
            if (coordinateControl1Y_tb.Text != "")
                points[modifyIndex].control1.Y = Int32.Parse(coordinateControl1Y_tb.Text);
            if (coordinateControl2X_tb.Text != "")
                points[modifyIndex].control2.X = Int32.Parse(coordinateControl2X_tb.Text);
            if (coordinateControl2Y_tb.Text != "")
                points[modifyIndex].control2.Y = Int32.Parse(coordinateControl2Y_tb.Text);
            Draw();
        }
    }
}