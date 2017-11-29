using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;

namespace Computer_Graphics
{
    public partial class Exercise3 : Page
    {
        bool flagRGB = false;
        bool flagCMYK = false;
        public Exercise3()
        {
            InitializeComponent();
            CreatePalette();
        }

        void CreatePalette()
        {
            int width = 1530;
            int height = 200;
            Bitmap bitmap = new Bitmap(width, height);
            byte r = 255;
            byte g = 0;
            byte b = 0;

            for (int y = 0; y < height; y++)
            {
                int phase = -1;
                for (int x = 0; x < width; x++)
                {
                    if (x % 255 == 0)
                        phase++;

                    if (phase == 0)
                        g++;
                    else if (phase == 1)
                        r--;
                    else if (phase == 2)
                        b++;
                    else if (phase == 3)
                        g--;
                    else if (phase == 4)
                        r++;
                    else if (phase == 5)
                        b--;

                    bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            colorPalette_img.Source = ConvertBitmap(bitmap);
        }

        public BitmapImage ConvertBitmap(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        private void colorPalette_Click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            WriteableBitmap imageView = new WriteableBitmap((BitmapSource)image.Source);
            uint[] pixels = new uint[(int)(imageView.PixelWidth * imageView.PixelHeight)];
            int widthInByte = (int)(4 * imageView.PixelWidth);

            imageView.CopyPixels(pixels, widthInByte, 0);

            double px = Math.Floor(e.GetPosition(image).X * imageView.PixelWidth / image.ActualWidth);
            double py = Math.Floor(e.GetPosition(image).Y * imageView.PixelHeight / image.ActualHeight);

            int index = (int)(py * imageView.PixelWidth + px);
            byte startR = (byte)((pixels[index] & 0xff0000) >> 0x10);
            byte startG = (byte)((pixels[index] & 0xff00) >> 8);
            byte startB = (byte)(pixels[index] & 0xff);

            rWithRGB_tb.Text = startR.ToString();
            gWithRGB_tb.Text = startG.ToString();
            bWithRGB_tb.Text = startB.ToString();

            Bitmap tmp = new Bitmap(1, 1);
            tmp.SetPixel(0, 0, Color.FromArgb(startR, startG, startB));
            chosenColor_img.Source = ConvertBitmap(tmp);

            double differencexR = (255 - (double)startR) / 255;
            double differencexG = (255 - (double)startG) / 255;
            double differencexB = (255 - (double)startB) / 255;

            double differenceyR;
            double differenceyG;
            double differenceyB;

            double tmpxR = startR;
            double tmpxG = startG;
            double tmpxB = startB;

            double tmpyR = startR;
            double tmpyG = startG;
            double tmpyB = startB;

            Bitmap bitmap = new Bitmap(255, 255);
            for (int x = 254; x >= 0; x--)
            {
                differenceyR = (double)tmpxR / 255;
                differenceyG = (double)tmpxG / 255;
                differenceyB = (double)tmpxB / 255;

                for (int y = 0; y < 255; y++)
                {
                    if (tmpyR - differenceyR < 0)
                        tmpyR = 0;
                    else
                        tmpyR -= differenceyR;
                    if (tmpyG - differenceyG < 0)
                        tmpyG = 0;
                    else
                        tmpyG -= differenceyG;
                    if (tmpyB - differenceyB < 0)
                        tmpyB = 0;
                    else
                        tmpyB -= differenceyB;

                    bitmap.SetPixel(x, y, Color.FromArgb((byte)tmpyR, (byte)tmpyG, (byte)tmpyB));
                }
                if (tmpxR + differencexR > 256)
                    tmpxR = 255;
                else
                    tmpxR += differencexR;
                if (tmpxG + differencexG > 256)
                    tmpxG = 255;
                else
                    tmpxG += differencexG;
                if (tmpxB + differencexB > 256)
                    tmpxB = 255;
                else
                    tmpxB += differencexB;

                tmpyR = tmpxR;
                tmpyG = tmpxG;
                tmpyB = tmpxB;
            }
            exactColorPalette_img.Source = ConvertBitmap(bitmap);
        }
        private void exactColorPalette_Click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            WriteableBitmap imageView = new WriteableBitmap((BitmapSource)image.Source);
            uint[] pixels = new uint[(int)(imageView.PixelWidth * imageView.PixelHeight)];
            int widthInByte = (int)(4 * imageView.PixelWidth);

            imageView.CopyPixels(pixels, widthInByte, 0);

            double px = Math.Floor(e.GetPosition(image).X * imageView.PixelWidth / image.ActualWidth);
            double py = Math.Floor(e.GetPosition(image).Y * imageView.PixelHeight / image.ActualHeight);

            int index = (int)(py * imageView.PixelWidth + px);
            byte startR = (byte)((pixels[index] & 0xff0000) >> 0x10);
            byte startG = (byte)((pixels[index] & 0xff00) >> 8);
            byte startB = (byte)(pixels[index] & 0xff);

            rWithRGB_tb.Text = startR.ToString();
            gWithRGB_tb.Text = startG.ToString();
            bWithRGB_tb.Text = startB.ToString();
            Bitmap tmp = new Bitmap(1, 1);
            tmp.SetPixel(0, 0, Color.FromArgb(startR, startG, startB));
            chosenColor_img.Source = ConvertBitmap(tmp);
        }


        private double max(double a, double b, double c)
        {
            double tmp;
            if (a > b)
                tmp = a;
            else
                tmp = b;

            if (tmp > c)
                return tmp;
            else
                return c;
        }

        private void rgb_TextChanged(object sender, TextChangedEventArgs e)
        {
            flagRGB = true;
            if (rWithRGB_tb.Text != "" && gWithRGB_tb.Text != "" && bWithRGB_tb.Text != "" && flagCMYK == false)
            {
                try
                {
                    int r = Int32.Parse(rWithRGB_tb.Text);
                    int g = Int32.Parse(gWithRGB_tb.Text);
                    int b = Int32.Parse(bWithRGB_tb.Text);

                    double rr = (double)r / 255;
                    double gr = (double)g / 255;
                    double br = (double)b / 255;

                    double k = 1 - max(rr, gr, br);
                    double c = (1 - rr - k) / (1 - k);
                    double m = (1 - gr - k) / (1 - k);
                    double y = (1 - br - k) / (1 - k);
                    if (k == 1)
                    {
                        c = 0;
                        m = 0;
                        y = 0;
                    }

                    cWithCMYK_tb.Text = c.ToString("N3");
                    mWithCMYK_tb.Text = m.ToString("N3");
                    yWithCMYK_tb.Text = y.ToString("N3");
                    kWithCMYK_tb.Text = k.ToString("N3");

                    Bitmap tmp = new Bitmap(1, 1);
                    tmp.SetPixel(0, 0, Color.FromArgb(r, g, b));
                    chosenColor_img.Source = ConvertBitmap(tmp);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Enter a number!");
                }
            }
            flagRGB = false;
        }

        private void cmyk_TextChanged(object sender, TextChangedEventArgs e)
        {
            flagCMYK = true;
            if (cWithCMYK_tb.Text != "" && mWithCMYK_tb.Text != "" && yWithCMYK_tb.Text != "" && kWithCMYK_tb.Text != "" && flagRGB == false)
            {
                try
                {
                    double c = double.Parse(cWithCMYK_tb.Text);
                    double m = double.Parse(mWithCMYK_tb.Text);
                    double y = double.Parse(yWithCMYK_tb.Text);
                    double k = double.Parse(kWithCMYK_tb.Text);

                    int r = (int)(255 * (1 - c) * (1 - k));
                    int g = (int)(255 * (1 - m) * (1 - k));
                    int b = (int)(255 * (1 - y) * (1 - k));

                    rWithRGB_tb.Text = r.ToString();
                    gWithRGB_tb.Text = g.ToString();
                    bWithRGB_tb.Text = b.ToString();

                    Bitmap tmp = new Bitmap(1, 1);
                    tmp.SetPixel(0, 0, Color.FromArgb(r, g, b));
                    chosenColor_img.Source = ConvertBitmap(tmp);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Enter a number!");
                }
            }
            flagCMYK = false;
        }
    }
}