using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Computer_Graphics
{
    public partial class Exercise1 : Page
    {
        Point startXY;
        Point stopXY;
        ObservableCollection<Line> lines;
        ObservableCollection<Ellipse> ellipses;
        ObservableCollection<Rectangle> rectangles;

        Shape clicked;

        BrushConverter converter;
        public Exercise1()
        {
            InitializeComponent();

            lines = new ObservableCollection<Line>();
            linesList_lb.ItemsSource = lines;
            ellipses = new ObservableCollection<Ellipse>();
            ellipsesList_lb.ItemsSource = ellipses;
            rectangles = new ObservableCollection<Rectangle>();
            rectanglesList_lb.ItemsSource = rectangles;

            converter = new BrushConverter();
            color_l.Content = "#000000";
        }

        private void startPainting_mlbd(object sender, MouseButtonEventArgs e)
        {
            startXY = e.GetPosition((Canvas)sender);
            info_l.Content = ("Start X: " + startXY.X + " Y: " + startXY.Y);
        }

        private void stopPainting_mlbu(object sender, MouseButtonEventArgs e)
        {
            Canvas plotno = (Canvas)sender;

            stopXY = e.GetPosition(plotno);
            info_l.Content += ("\nStop X: " + stopXY.X + " Y: " + stopXY.Y);

            if (clickTool_rb.IsChecked == true)
            {
                clicked = null;

                var mouseWasDownOn = e.Source as FrameworkElement;
                if (mouseWasDownOn != null)
                {
                    string elementName = mouseWasDownOn.Name;
                    foreach (Shape child in canvasExerciseOne_c.Children)
                    {
                        if (child.Name == elementName)
                        {
                            clicked = child;
                            break;
                        }
                    }
                }

                shapeName_tb.Text = "";
                shapeX1_tb.Text = "";
                shapeY1_tb.Text = "";
                shapeX2_tb.Text = "";
                shapeY2_tb.Text = "";
                shapeHeight_tb.Text = "";
                shapeWidth_tb.Text = "";

                if (clicked == null)
                {

                }
                else if (clicked.GetType().ToString() == "System.Windows.Shapes.Line")
                {
                    Line tmp = (Line)clicked;
                    shapeName_tb.Text = tmp.Name;
                    shapeX1_tb.Text = tmp.X1.ToString();
                    shapeY1_tb.Text = tmp.Y1.ToString();
                    shapeX2_tb.Text = tmp.X2.ToString();
                    shapeY2_tb.Text = tmp.Y2.ToString();

                }
                else if (clicked.GetType().ToString() == "System.Windows.Shapes.Ellipse")
                {
                    Ellipse tmp = (Ellipse)clicked;
                    shapeName_tb.Text = tmp.Name;
                    shapeX1_tb.Text = tmp.Margin.Left.ToString();
                    shapeY1_tb.Text = tmp.Margin.Top.ToString();
                    shapeHeight_tb.Text = tmp.Height.ToString();
                    shapeWidth_tb.Text = tmp.Width.ToString();
                }
                else if (clicked.GetType().ToString() == "System.Windows.Shapes.Rectangle")
                {
                    Rectangle tmp = (Rectangle)clicked;
                    shapeName_tb.Text = tmp.Name;
                    shapeX1_tb.Text = tmp.Margin.Left.ToString();
                    shapeY1_tb.Text = tmp.Margin.Top.ToString();
                    shapeHeight_tb.Text = tmp.Height.ToString();
                    shapeWidth_tb.Text = tmp.Width.ToString();
                }
            }
            else if (lineTool_rb.IsChecked == true)
            {
                Line myLine = new Line();
                myLine.Name = "Line_" + lines.Count;
                var brush = (Brush)converter.ConvertFromString(color_l.Content.ToString());
                myLine.Stroke = brush;
                myLine.X1 = startXY.X;
                myLine.X2 = stopXY.X;
                myLine.Y1 = startXY.Y;
                myLine.Y2 = stopXY.Y;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 2;
                canvasExerciseOne_c.Children.Add(myLine);
                lines.Add(myLine);
            }
            else if (circleTool_rb.IsChecked == true)
            {
                Point tmp = new Point();
                if (startXY.X >= stopXY.X)
                    tmp.X = startXY.X;
                else
                    tmp.X = stopXY.X;

                if (startXY.Y >= stopXY.Y)
                    tmp.Y = startXY.Y;
                else
                    tmp.Y = stopXY.Y;

                Ellipse myEllipse = CreateEllipse(Math.Abs(startXY.X - stopXY.X), Math.Abs(startXY.Y - stopXY.Y), tmp.X, tmp.Y);
                myEllipse.Name = "Ellipse_" + ellipses.Count;
                var brush = (Brush)converter.ConvertFromString(color_l.Content.ToString());
                myEllipse.Stroke = brush;
                if (fill_cb.IsChecked == true)
                    myEllipse.Fill = brush;
                myEllipse.HorizontalAlignment = HorizontalAlignment.Left;
                myEllipse.VerticalAlignment = VerticalAlignment.Center;
                myEllipse.StrokeThickness = 2;
                canvasExerciseOne_c.Children.Add(myEllipse);
                ellipses.Add(myEllipse);
            }
            else if (rectangleTool_rb.IsChecked == true)
            {
                Point tmp = new Point();
                if (startXY.X >= stopXY.X)
                    tmp.X = startXY.X;
                else
                    tmp.X = stopXY.X;

                if (startXY.Y >= stopXY.Y)
                    tmp.Y = startXY.Y;
                else
                    tmp.Y = stopXY.Y;

                Rectangle myRectangle = CreateRectangle(Math.Abs(startXY.X - stopXY.X), Math.Abs(startXY.Y - stopXY.Y), tmp.X, tmp.Y);
                myRectangle.Name = "Rectangle_" + rectangles.Count;
                var brush = (Brush)converter.ConvertFromString(color_l.Content.ToString());
                myRectangle.Stroke = brush;
                if (fill_cb.IsChecked == true)
                    myRectangle.Fill = brush;
                myRectangle.HorizontalAlignment = HorizontalAlignment.Left;
                myRectangle.VerticalAlignment = VerticalAlignment.Center;
                myRectangle.StrokeThickness = 2;
                canvasExerciseOne_c.Children.Add(myRectangle);
                rectangles.Add(myRectangle);
            }
        }

        Ellipse CreateEllipse(double width, double height, double desiredCenterX, double desiredCenterY)
        {
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            double left = desiredCenterX - width;
            double top = desiredCenterY - height;

            ellipse.Margin = new Thickness(left, top, 0, 0);
            return ellipse;
        }

        Rectangle CreateRectangle(double width, double height, double desiredCenterX, double desiredCenterY)
        {
            Rectangle rectangle = new Rectangle { Width = width, Height = height };
            double left = desiredCenterX - width;
            double top = desiredCenterY - height;

            rectangle.Margin = new Thickness(left, top, 0, 0);
            return rectangle;
        }

        private void ShapeSave_Click(object sender, RoutedEventArgs e)
        {
            if (clicked == null)
            {

            }
            else if (clicked.GetType().ToString() == "System.Windows.Shapes.Line")
            {
                Line tmp = (Line)clicked;

                if (shapeName_tb.Text != null)
                    tmp.Name = shapeName_tb.Text;
                if (shapeX1_tb.Text != null)
                    tmp.X1 = Double.Parse(shapeX1_tb.Text);
                if (shapeX1_tb.Text != null)
                    tmp.Y1 = Double.Parse(shapeY1_tb.Text);
                if (shapeX1_tb.Text != null)
                    tmp.X2 = Double.Parse(shapeX2_tb.Text);
                if (shapeX1_tb.Text != null)
                    tmp.Y2 = Double.Parse(shapeY2_tb.Text);

                if (newColor_cb.IsChecked == true)
                    tmp.Stroke = (Brush)converter.ConvertFromString(color_l.Content.ToString());

            }
            else if (clicked.GetType().ToString() == "System.Windows.Shapes.Ellipse")
            {
                Ellipse tmp = (Ellipse)clicked;

                if (shapeName_tb.Text != null)
                    tmp.Name = shapeName_tb.Text;
                if (shapeX1_tb.Text != null && shapeX1_tb.Text != null)
                    tmp.Margin = new Thickness(Double.Parse(shapeX1_tb.Text), Double.Parse(shapeY1_tb.Text), 0, 0);
                if (shapeX1_tb.Text != null)
                    tmp.Height = Double.Parse(shapeHeight_tb.Text);
                if (shapeX1_tb.Text != null)
                    tmp.Width = Double.Parse(shapeWidth_tb.Text);

                if (newColor_cb.IsChecked == true)
                {
                    tmp.Stroke = (Brush)converter.ConvertFromString(color_l.Content.ToString());
                    tmp.Fill = (Brush)converter.ConvertFromString(color_l.Content.ToString());
                }
            }
            else if (clicked.GetType().ToString() == "System.Windows.Shapes.Rectangle")
            {
                Rectangle tmp = (Rectangle)clicked;

                if (shapeName_tb.Text != null)
                    tmp.Name = shapeName_tb.Text;
                if (shapeX1_tb.Text != null && shapeX1_tb.Text != null)
                    tmp.Margin = new Thickness(Double.Parse(shapeX1_tb.Text), Double.Parse(shapeY1_tb.Text), 0, 0);
                if (shapeX1_tb.Text != null)
                    tmp.Height = Double.Parse(shapeHeight_tb.Text);
                if (shapeX1_tb.Text != null)
                    tmp.Width = Double.Parse(shapeWidth_tb.Text);

                if (newColor_cb.IsChecked == true)
                {
                    tmp.Stroke = (Brush)converter.ConvertFromString(color_l.Content.ToString());
                    tmp.Fill = (Brush)converter.ConvertFromString(color_l.Content.ToString());
                }
            }
        }
        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            color_l.Content = ClrPcker_Background.SelectedColor.Value;
        }
    }
}