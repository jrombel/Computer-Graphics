using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Computer_Graphics
{
    public partial class Exercise9 : Page
    {
        private WriteableBitmap photo;
        private double height;
        private double width;
        private uint[] pixelData;
        private uint[] pixelDataSelection;
        private int widthInByte;

        private int index;
        private int select;

        public Exercise9()
        {
            InitializeComponent();
            index = -1;
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
                    imageSelection_i.Source = b;

                    photo = new WriteableBitmap(b);
                    height = photo.PixelHeight;
                    width = photo.PixelWidth;

                    pixelData = new uint[(int)(width * height)];
                    pixelDataSelection = new uint[(int)(width * height)];
                    widthInByte = (int)(4 * width);

                    photo.CopyPixels(pixelData, widthInByte, 0);
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("File error!");
            }
        }

        public int GetIndex(double x, double y)
        {
            return (int)(y * width + x);
        }

        private void ImageSelection_i_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            double x = Math.Floor(e.GetPosition(image).X * width / image.ActualWidth);
            double y = Math.Floor(e.GetPosition(image).Y * height / image.ActualHeight);
            index = GetIndex(x, y);

            Select();
        }

        private void Select()
        {
            if (index != -1)
            {
                select = 0;
                pixelDataSelection = (uint[])pixelData.Clone();

                byte red = (byte)((pixelData[index] & 0xff0000) >> 16);
                byte green = (byte)((pixelData[index] & 0xff00) >> 8);
                byte blue = (byte)((pixelData[index] & 0xff));

                for (int i = 0; i < pixelData.Length; i++)
                {
                    byte compareRed = (byte)((pixelData[i] & 0xff0000) >> 16);
                    byte compareGreen = (byte)((pixelData[i] & 0xff00) >> 8);
                    byte compareBlue = (byte)((pixelData[i] & 0xff));

                    Byte difference = (byte)difference_s.Value;
                    if (Math.Abs(compareRed - red) < difference && Math.Abs(compareGreen - green) < difference && Math.Abs(compareBlue - blue) < difference)
                    {
                        select++;
                        pixelDataSelection[i] = (uint)((0 << 24) | (0 << 16) | (255 << 8) | (0 << 0));
                    }
                }

                difference_l.Content = "Selected: ~" + Math.Round((((double)select / (double)pixelData.Length) * 100), 2) + "%";

                WriteableBitmap tmp = new WriteableBitmap(photo.PixelWidth, photo.PixelHeight, photo.DpiX, photo.DpiY, photo.Format, photo.Palette);
                tmp.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelDataSelection, widthInByte, 0);
                imageSelection_i.Source = tmp;
            }
        }
    }
}