using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Computer_Graphics
{
    public partial class Exercise5 : Page
    {
        private WriteableBitmap photo;
        private double height;
        private double width;
        private uint[] pixelData;
        private uint[] pixelDataTmp;
        private uint[] pixelDataUndo;
        private int widthInByte;

        private Polygon luminosity;
        private Polygon red;
        private Polygon green;
        private Polygon blue;

        private int luminosityPixelsMax, luminosityValueMin, luminosityValueMax;
        private int redPixelsMax, redValueMin, redValueMax;
        private int greenPixelsMax, greenValueMin, greenValueMax;
        private int bluePixelsMax, blueValueMin, blueValueMax;

        private int[] percentBlackSelection;
        private int mean;

        public Exercise5()
        {
            InitializeComponent();

            luminosity = new Polygon();
            luminosity.Fill = Brushes.Black;
            luminosity.Stroke = Brushes.Black;
            histogramLuminosity_c.Children.Add(luminosity);
            red = new Polygon();
            red.Fill = Brushes.Red;
            red.Stroke = Brushes.Black;
            histogramRed_c.Children.Add(red);
            green = new Polygon();
            green.Fill = Brushes.Green;
            green.Stroke = Brushes.Black;
            histogramGreen_c.Children.Add(green);
            blue = new Polygon();
            blue.Fill = Brushes.Blue;
            blue.Stroke = Brushes.Black;
            histogramBlue_c.Children.Add(blue);

            percentBlackSelection = new int[256];
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;";

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;

                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.UriSource = new Uri(filename);
                    b.EndInit();

                    image_i.Source = b;

                    photo = new WriteableBitmap(b);
                    height = photo.PixelHeight;
                    width = photo.PixelWidth;

                    pixelData = new uint[(int)(width * height)];
                    pixelDataTmp = new uint[(int)(width * height)];
                    widthInByte = (int)(4 * width);

                    photo.CopyPixels(pixelData, widthInByte, 0);
                    chooseMethod_cbi.IsSelected = true;

                    DrawHistogram();
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("File error!");
            }
        }

        private void DrawHistogram()
        {
            int[][] histogram = { new int[256], new int[256], new int[256], new int[256] };
            luminosityValueMin = 255;
            redValueMin = 255;
            greenValueMin = 255;
            blueValueMin = 255;
            luminosityValueMax = 0;
            redValueMax = 0;
            greenValueMax = 0;
            blueValueMax = 0;

            luminosityPixelsMax = 0;
            redPixelsMax = 0;
            greenPixelsMax = 0;
            bluePixelsMax = 0;

            for (int i = 0; i < pixelData.Length; i++)
            {
                byte redColor = (byte)((pixelData[i] & 0xff0000) >> 16);
                byte greenColor = (byte)((pixelData[i] & 0xff00) >> 8);
                byte blueColor = (byte)((pixelData[i] & 0xff));
                byte luminosityColor = (byte)(0.114 * blueColor + 0.587 * greenColor + 0.299 * redColor);

                histogram[1][redColor]++;
                histogram[2][greenColor]++;
                histogram[3][blueColor]++;
                histogram[0][luminosityColor]++;

                if (luminosityColor < luminosityValueMin)
                    luminosityValueMin = luminosityColor;
                if (luminosityColor > luminosityValueMax)
                    luminosityValueMax = luminosityColor;

                if (redColor < redValueMin)
                    redValueMin = redColor;
                if (redColor > redValueMax)
                    redValueMax = redColor;

                if (greenColor < greenValueMin)
                    greenValueMin = greenColor;
                if (greenColor > greenValueMax)
                    greenValueMax = greenColor;

                if (blueColor < blueValueMin)
                    blueValueMin = blueColor;
                if (blueColor > blueValueMax)
                    blueValueMax = blueColor;
            }
            luminosityPixelsMax = (histogram[0]).Max();
            redPixelsMax = (histogram[1]).Max();
            greenPixelsMax = (histogram[2]).Max();
            bluePixelsMax = (histogram[3]).Max();

            luminosity.Points.Clear();
            red.Points.Clear();
            green.Points.Clear();
            blue.Points.Clear();

            double maxHeight = histogramLuminosity_c.ActualHeight;
            for (int i = 0; i < 256; i++)
            {
                luminosity.Points.Add(new Point(i, maxHeight - maxHeight * histogram[0][i] / luminosityPixelsMax));
                red.Points.Add(new Point(i, maxHeight - maxHeight * histogram[1][i] / redPixelsMax));
                green.Points.Add(new Point(i, maxHeight - maxHeight * histogram[2][i] / greenPixelsMax));
                blue.Points.Add(new Point(i, maxHeight - maxHeight * histogram[3][i] / bluePixelsMax));
            }
            luminosity.Points.Add(new Point(255, maxHeight));
            red.Points.Add(new Point(255, maxHeight));
            green.Points.Add(new Point(255, maxHeight));
            blue.Points.Add(new Point(255, maxHeight));
            luminosity.Points.Add(new Point(0, maxHeight));
            red.Points.Add(new Point(0, maxHeight));
            green.Points.Add(new Point(0, maxHeight));
            blue.Points.Add(new Point(0, maxHeight));

            //mean interactive selection
            int sumLuminosity = 0;
            for (int i = 0; i < 256; i++)
                sumLuminosity += histogram[0][i];
            int meanTmp = sumLuminosity / (luminosityValueMax - luminosityValueMin);
            int nearest = (histogram[0]).OrderBy(x => Math.Abs((long)x - meanTmp)).First();
            mean = Array.IndexOf((histogram[0]), nearest);
        }
        private void HistogramStretch_Click(object sender, RoutedEventArgs e)
        {
            pixelDataUndo = (uint[])pixelData.Clone();
            for (int i = 0; i < pixelData.Length; i++)
            {
                byte red = (byte)((pixelData[i] & 0xff0000) >> 16);
                byte green = (byte)((pixelData[i] & 0xff00) >> 8);
                byte blue = (byte)((pixelData[i] & 0xff));

                byte redNew = (byte)(((double)(red - redValueMin) / (double)(redValueMax - redValueMin)) * 255);
                byte greenNew = (byte)(((double)(green - greenValueMin) / (double)(greenValueMax - greenValueMin)) * 255);
                byte blueNew = (byte)(((double)(blue - blueValueMin) / (double)(blueValueMax - blueValueMin)) * 255);

                byte alpha = 0;

                pixelData[i] = (uint)((alpha << 24) | (redNew << 16) | (greenNew << 8) | (blueNew << 0));
            }
            WriteableBitmap tmp = new WriteableBitmap(photo.PixelWidth, photo.PixelHeight, photo.DpiX, photo.DpiY, photo.Format, photo.Palette);
            tmp.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = tmp;

            DrawHistogram();
        }

        private void HistogramEqualize_Click(object sender, RoutedEventArgs e)
        {
            pixelDataUndo = (uint[])pixelData.Clone();
            int width = (int)photo.Width;
            int height = (int)photo.Height;
            int nPixels = width * height;

            int[][] histogram = { new int[256], new int[256], new int[256] };
            int[][] histogramTransform = { new int[256], new int[256], new int[256] };

            for (int i = 0; i < pixelData.Length; i++)
            {
                byte redColor = (byte)((pixelData[i] & 0xff0000) >> 16);
                byte greenColor = (byte)((pixelData[i] & 0xff00) >> 8);
                byte blueColor = (byte)((pixelData[i] & 0xff));

                histogram[0][redColor]++;
                histogram[1][greenColor]++;
                histogram[2][blueColor]++;
            }

            int sumR = 0;
            int sumG = 0;
            int sumB = 0;

            for (int i = 0; i < 256; ++i)
            {
                sumR += histogram[0][i];
                sumG += histogram[1][i];
                sumB += histogram[2][i];
                histogramTransform[0][i] = sumR;
                histogramTransform[1][i] = sumG;
                histogramTransform[2][i] = sumB;
            }

            int redMin = 0, greenMin = 0, blueMin = 0;
            bool redCheck = true, greenCheck = true, blueCheck = true;

            for (int i = 0; i < 256; ++i)
            {
                if (redCheck == true && histogramTransform[0][i] > 0)
                {
                    redMin = histogramTransform[0][i];
                    redCheck = false;
                }
                if (greenCheck == true && histogramTransform[1][i] > 0)
                {
                    greenMin = histogramTransform[1][i];
                    greenCheck = false;
                }
                if (blueCheck == true && histogramTransform[1][i] > 0)
                {
                    blueMin = histogramTransform[1][i];
                    blueCheck = false;
                }
                if (redCheck == false && greenCheck == false && blueCheck == false)
                    break;
            }

            var I = new byte[width, height];
            double divRed = (double)(nPixels - redMin) / 255d;
            double divGreen = (double)(nPixels - greenMin) / 255d;
            double divBlue = (double)(nPixels - blueMin) / 255d;

            for (int i = 0; i < pixelData.Length; i++)
            {
                byte red = (byte)((pixelData[i] & 0xff0000) >> 16);
                byte green = (byte)((pixelData[i] & 0xff00) >> 8);
                byte blue = (byte)((pixelData[i] & 0xff));

                byte redNew = (byte)Math.Round((histogramTransform[0][red] - redMin) / divRed);
                byte greenNew = (byte)Math.Round((histogramTransform[1][green] - greenMin) / divGreen);
                byte blueNew = (byte)Math.Round((histogramTransform[2][blue] - blueMin) / divBlue);

                byte alpha = 0;

                pixelData[i] = (uint)((alpha << 24) | (redNew << 16) | (greenNew << 8) | (blueNew << 0));
            }

            WriteableBitmap tmp = new WriteableBitmap(photo.PixelWidth, photo.PixelHeight, photo.DpiX, photo.DpiY, photo.Format, photo.Palette);
            tmp.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = tmp;

            DrawHistogram();
        }

        private void manuallyValue_s_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pixelDataUndo = pixelData;
            if (photo != null)
            {
                int value = (int)manuallyValue_s.Value;

                if (value >= 0 && value <= 255)
                {
                    for (int i = 0; i < pixelData.Length; i++)
                    {
                        byte red = (byte)((pixelData[i] & 0xff0000) >> 16);
                        byte green = (byte)((pixelData[i] & 0xff00) >> 8);
                        byte blue = (byte)((pixelData[i] & 0xff));
                        byte alpha = 0;

                        if ((int)(0.114 * blue + 0.587 * green + 0.299 * red) <= value)
                        {
                            red = 0;
                            green = 0;
                            blue = 0;
                        }
                        else
                        {
                            red = 255;
                            green = 255;
                            blue = 255;
                        }

                        pixelDataTmp[i] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                    }

                    WriteableBitmap tmp = new WriteableBitmap(photo.PixelWidth, photo.PixelHeight, photo.DpiX, photo.DpiY, photo.Format, photo.Palette);
                    tmp.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelDataTmp, widthInByte, 0);
                    image_i.Source = tmp;
                }
            }
        }

        private void percentBlackSelectionBinarization_cbi_Selected(object sender, RoutedEventArgs e)
        {
            if (photo != null)
            {
                for (int value = 0; value < 256; value++)
                {
                    int sum = 0;
                    for (int i = 0; i < pixelData.Length; i++)
                    {
                        byte red = (byte)((pixelData[i] & 0xff0000) >> 16);
                        byte green = (byte)((pixelData[i] & 0xff00) >> 8);
                        byte blue = (byte)((pixelData[i] & 0xff));

                        if ((int)(0.114 * blue + 0.587 * green + 0.299 * red) <= value)
                            sum++;
                    }

                    percentBlackSelection[value] = (int)((double)sum / (double)pixelData.Length * 100);
                }
            }
        }

        private void percentBlackPixels_s_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            pixelDataUndo = pixelData;
            if (photo != null)
            {
                int percent = (int)percentBlackPixels_s.Value;
                int value = 0;
                for (int i = 0; i < 256; i++)
                {
                    if (percentBlackSelection[i] < percent)
                        value = i;
                    else
                        break;
                }

                for (int i = 0; i < pixelData.Length; i++)
                {
                    byte red = (byte)((pixelData[i] & 0xff0000) >> 16);
                    byte green = (byte)((pixelData[i] & 0xff00) >> 8);
                    byte blue = (byte)((pixelData[i] & 0xff));
                    byte alpha = 0;

                    if ((int)(0.114 * blue + 0.587 * green + 0.299 * red) <= value)
                    {
                        red = 0;
                        green = 0;
                        blue = 0;
                    }
                    else
                    {
                        red = 255;
                        green = 255;
                        blue = 255;
                    }

                    pixelDataTmp[i] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                }

                WriteableBitmap tmp = new WriteableBitmap(photo.PixelWidth, photo.PixelHeight, photo.DpiX, photo.DpiY, photo.Format, photo.Palette);
                tmp.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelDataTmp, widthInByte, 0);
                image_i.Source = tmp;
            }
        }

        private void meanIteracitveSelection_cbi_Selected(object sender, RoutedEventArgs e)
        {
            meanIteractiveSelection_l.Content = "Mean Iterative Selection: " + mean;

            pixelDataUndo = pixelData;
            if (photo != null)
            {
                int value = mean;

                for (int i = 0; i < pixelData.Length; i++)
                {
                    byte red = (byte)((pixelData[i] & 0xff0000) >> 16);
                    byte green = (byte)((pixelData[i] & 0xff00) >> 8);
                    byte blue = (byte)((pixelData[i] & 0xff));
                    byte alpha = 0;

                    if ((int)(0.114 * blue + 0.587 * green + 0.299 * red) <= value)
                    {
                        red = 0;
                        green = 0;
                        blue = 0;
                    }
                    else
                    {
                        red = 255;
                        green = 255;
                        blue = 255;
                    }

                    pixelDataTmp[i] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                }

                WriteableBitmap tmp = new WriteableBitmap(photo.PixelWidth, photo.PixelHeight, photo.DpiX, photo.DpiY, photo.Format, photo.Palette);
                tmp.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelDataTmp, widthInByte, 0);
                image_i.Source = tmp;
            }
        }

        private void ApplyBinaryzation_Click(object sender, RoutedEventArgs e)
        {
            pixelDataUndo = pixelData;
            pixelData = pixelDataTmp;
            WriteableBitmap tmp = new WriteableBitmap(photo.PixelWidth, photo.PixelHeight, photo.DpiX, photo.DpiY, photo.Format, photo.Palette);
            tmp.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = tmp;

            DrawHistogram();
        }
        private void UndoImage_Click(object sender, RoutedEventArgs e)
        {
            pixelData = (uint[])pixelDataUndo.Clone();
            photo.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = photo;

            DrawHistogram();
        }
    }
}