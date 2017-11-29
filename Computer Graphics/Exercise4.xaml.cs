using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Computer_Graphics
{
    public partial class Exercise4 : Page
    {
        public WriteableBitmap photo;
        public double height;
        public double width;
        public uint[] pixelData;
        public uint[] pixelDataUndo;
        private uint[] pixelDataBrightness;
        public int widthInByte;

        public Exercise4()
        {
            InitializeComponent();
            brightnessDegree_tb.Text = "";
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

        private void imagePointTransformation_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            double x = Math.Floor(e.GetPosition(image).X * width / image.ActualWidth);
            double y = Math.Floor(e.GetPosition(image).Y * height / image.ActualHeight);
            int index = GetIndex(x, y);

            double redValue = 0;
            double greenValue = 0;
            double blueValue = 0;
            try
            {
                redValue = Double.Parse(pointTransformationRedValue_tb.Text);
                greenValue = Double.Parse(pointTransformationGreenValue_tb.Text);
                blueValue = Double.Parse(pointTransformationBlueValue_tb.Text);

                if (redValue >= 0 && redValue <= 255 && greenValue >= 0 && greenValue <= 255 && blueValue >= 0 && blueValue <= 255)
                {
                    if (pointTransformationToolAddition_rb.IsChecked == true)
                        pixelData[index] = changeColor(index, redValue, greenValue, blueValue, 0);
                    else if (pointTransformationToolSubtraction_rb.IsChecked == true)
                        pixelData[index] = changeColor(index, redValue, greenValue, blueValue, 1);
                    else if (pointTransformationToolMultiplication_rb.IsChecked == true)
                        pixelData[index] = changeColor(index, redValue, greenValue, blueValue, 2);
                    else if (pointTransformationToolDivision_rb.IsChecked == true)
                        pixelData[index] = changeColor(index, redValue, greenValue, blueValue, 3);

                    photo.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
                    image_i.Source = photo;
                }
                else
                {
                    MessageBox.Show("The value in the 'Value' field should be in the range <0,255>!");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter the value in the 'Value' field!");
            }
        }

        private uint changeColor(int index, double redValue, double greenValue, double blueValue, int type)
        {
            //types
            //0 - addition
            //1 - substraction
            //2 - multiplication
            //3 - division
            byte red = (byte)((pixelData[index] & 0xff0000) >> 0x10);
            byte green = (byte)((pixelData[index] & 0xff00) >> 8);
            byte blue = (byte)(pixelData[index] & 0xff);

            if (type == 0)
            {
                red = addition(red, redValue);
                green = addition(green, greenValue);
                blue = addition(blue, blueValue);
            }
            else if (type == 1)
            {
                red = substraction(red, redValue);
                green = substraction(green, greenValue);
                blue = substraction(blue, blueValue);
            }
            else if (type == 2)
            {
                red = multiplication(red, redValue);
                green = multiplication(green, greenValue);
                blue = multiplication(blue, blueValue);
            }
            else if (type == 3)
            {
                red = division(red, redValue);
                green = division(green, greenValue);
                blue = division(blue, blueValue);
            }

            byte alpha = 0;
            return (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
        }

        private byte addition(byte color, double value)
        {
            byte result;
            if (color + value <= 255)
                result = (byte)(color + value);
            else
                result = 255;

            return result;
        }

        private byte substraction(byte color, double value)
        {
            byte result;
            if (color - value >= 0)
                result = (byte)(color - value);
            else
                result = 0;

            return result;
        }

        private byte multiplication(byte color, double value)
        {
            byte result;
            if (color * value <= 255)
                result = (byte)(color * value);
            else
                result = 255;

            return result;
        }

        private byte division(byte color, double value)
        {
            byte result;
            if (color / value >= 0)
                result = (byte)(color / value);
            else
                result = 0;

            return result;
        }

        private void brightnessDegree_s_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = ((Slider)sender).Value;

            pixelDataBrightness = (uint[])pixelData.Clone();

            for (int i = 0; i < pixelData.Length; i++)
            {
                if (value > 0)
                {
                    byte alpha = 0;
                    byte red = addition((byte)((pixelData[i] & 0xff0000) >> 16), value);
                    byte green = addition((byte)((pixelData[i] & 0xff00) >> 8), value);
                    byte blue = addition((byte)((pixelData[i] & 0xff)), value);

                    pixelDataBrightness[i] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                }
                else if (value < 0)
                {
                    byte alpha = 0;
                    byte red = substraction((byte)((pixelData[i] & 0xff0000) >> 16), (-1 * value));
                    byte green = substraction((byte)((pixelData[i] & 0xff00) >> 8), (-1 * value));
                    byte blue = substraction((byte)((pixelData[i] & 0xff)), (-1 * value));

                    pixelDataBrightness[i] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                }
            }

            WriteableBitmap tmp = new WriteableBitmap(photo.PixelWidth, photo.PixelHeight, photo.DpiX, photo.DpiY, photo.Format, photo.Palette);
            tmp.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelDataBrightness, widthInByte, 0);
            image_i.Source = tmp;
        }

        private void BrightnessSave_Click(object sender, RoutedEventArgs e)
        {
            pixelDataUndo = pixelData;
            pixelData = pixelDataBrightness;
            brightnessDegree_s.Value = 0;
        }

        private void SmoothingFilter_Click(object sender, RoutedEventArgs e)
        {
            uint[] pixelDataTmp = (uint[])pixelData.Clone();
            byte alpha = 0;
            uint red, green, blue;
            int index = 0, tmp = 0;

            for (int x = 1; x < (int)photo.Width - 1; x++)
            {
                for (int y = 1; y < (int)photo.Height - 1; y++)
                {
                    index = GetIndex(x, y);

                    red = 0;
                    green = 0;
                    blue = 0;

                    tmp = GetIndex(x - 1, y - 1);
                    red += (byte)((pixelDataTmp[tmp] & 0xff0000) >> 16);
                    green += (byte)((pixelDataTmp[tmp] & 0xff00) >> 8);
                    blue += (byte)((pixelDataTmp[tmp] & 0xff));

                    tmp = GetIndex(x, y - 1);
                    red += (byte)((pixelDataTmp[tmp] & 0xff0000) >> 16);
                    green += (byte)((pixelDataTmp[tmp] & 0xff00) >> 8);
                    blue += (byte)((pixelDataTmp[tmp] & 0xff));

                    tmp = GetIndex(x + 1, y - 1);
                    red += (byte)((pixelDataTmp[tmp] & 0xff0000) >> 16);
                    green += (byte)((pixelDataTmp[tmp] & 0xff00) >> 8);
                    blue += (byte)((pixelDataTmp[tmp] & 0xff));

                    tmp = GetIndex(x - 1, y);
                    red += (byte)((pixelDataTmp[tmp] & 0xff0000) >> 16);
                    green += (byte)((pixelDataTmp[tmp] & 0xff00) >> 8);
                    blue += (byte)((pixelDataTmp[tmp] & 0xff));

                    tmp = index;
                    red += (byte)((pixelDataTmp[tmp] & 0xff0000) >> 16);
                    green += (byte)((pixelDataTmp[tmp] & 0xff00) >> 8);
                    blue += (byte)((pixelDataTmp[tmp] & 0xff));

                    tmp = GetIndex(x + 1, y);
                    red += (byte)((pixelDataTmp[tmp] & 0xff0000) >> 16);
                    green += (byte)((pixelDataTmp[tmp] & 0xff00) >> 8);
                    blue += (byte)((pixelDataTmp[tmp] & 0xff));

                    tmp = GetIndex(x - 1, y + 1);
                    red += (byte)((pixelDataTmp[tmp] & 0xff0000) >> 16);
                    green += (byte)((pixelDataTmp[tmp] & 0xff00) >> 8);
                    blue += (byte)((pixelDataTmp[tmp] & 0xff));

                    tmp = GetIndex(x, y + 1);
                    red += (byte)((pixelDataTmp[tmp] & 0xff0000) >> 16);
                    green += (byte)((pixelDataTmp[tmp] & 0xff00) >> 8);
                    blue += (byte)((pixelDataTmp[tmp] & 0xff));

                    tmp = GetIndex(x + 1, y + 1);
                    red += (byte)((pixelDataTmp[tmp] & 0xff0000) >> 16);
                    green += (byte)((pixelDataTmp[tmp] & 0xff00) >> 8);
                    blue += (byte)((pixelDataTmp[tmp] & 0xff));

                    pixelDataTmp[index] = (uint)((alpha << 24) | ((byte)(red / 9) << 16) | ((byte)(green / 9) << 8) | ((byte)(blue / 9) << 0));

                }
            }
            pixelDataUndo = pixelData;
            pixelData = pixelDataTmp;
            photo.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = photo;
        }

        private void MedianFilter_Click(object sender, RoutedEventArgs e)
        {
            uint[] pixelDataTmp = (uint[])pixelData.Clone();
            byte alpha = 0;

            int index = 0;

            for (int x = 1; x < (int)photo.Width - 1; x++)
            {
                for (int y = 1; y < (int)photo.Height - 1; y++)
                {
                    index = GetIndex(x, y);

                    Dictionary<int, int> values = new Dictionary<int, int>();
                    values.Add(0, 0);
                    values.Add(1, 0);
                    values.Add(2, 0);
                    values.Add(3, 0);
                    values.Add(4, 0);
                    values.Add(5, 0);
                    values.Add(6, 0);
                    values.Add(7, 0);
                    values.Add(8, 0);

                    int[] tmp = new int[9];
                    tmp[0] = GetIndex(x - 1, y - 1);

                    values[0] += (byte)((pixelDataTmp[tmp[0]] & 0xff0000) >> 16);
                    values[0] += (byte)((pixelDataTmp[tmp[0]] & 0xff00) >> 8);
                    values[0] += (byte)((pixelDataTmp[tmp[0]] & 0xff));

                    tmp[1] = GetIndex(x, y - 1);
                    values[1] += (byte)((pixelDataTmp[tmp[1]] & 0xff0000) >> 16);
                    values[1] += (byte)((pixelDataTmp[tmp[1]] & 0xff00) >> 8);
                    values[1] += (byte)((pixelDataTmp[tmp[1]] & 0xff));

                    tmp[2] = GetIndex(x + 1, y - 1);
                    values[2] += (byte)((pixelDataTmp[tmp[2]] & 0xff0000) >> 16);
                    values[2] += (byte)((pixelDataTmp[tmp[2]] & 0xff00) >> 8);
                    values[2] += (byte)((pixelDataTmp[tmp[2]] & 0xff));

                    tmp[3] = GetIndex(x - 1, y);
                    values[3] += (byte)((pixelDataTmp[tmp[3]] & 0xff0000) >> 16);
                    values[3] += (byte)((pixelDataTmp[tmp[3]] & 0xff00) >> 8);
                    values[3] += (byte)((pixelDataTmp[tmp[3]] & 0xff));

                    tmp[4] = index;
                    values[4] += (byte)((pixelDataTmp[tmp[4]] & 0xff0000) >> 16);
                    values[4] += (byte)((pixelDataTmp[tmp[4]] & 0xff00) >> 8);
                    values[4] += (byte)((pixelDataTmp[tmp[4]] & 0xff));

                    tmp[5] = GetIndex(x + 1, y);
                    values[5] += (byte)((pixelDataTmp[tmp[5]] & 0xff0000) >> 16);
                    values[5] += (byte)((pixelDataTmp[tmp[5]] & 0xff00) >> 8);
                    values[5] += (byte)((pixelDataTmp[tmp[5]] & 0xff));

                    tmp[6] = GetIndex(x - 1, y + 1);
                    values[6] += (byte)((pixelDataTmp[tmp[6]] & 0xff0000) >> 16);
                    values[6] += (byte)((pixelDataTmp[tmp[6]] & 0xff00) >> 8);
                    values[6] += (byte)((pixelDataTmp[tmp[6]] & 0xff));

                    tmp[7] = GetIndex(x, y + 1);
                    values[7] += (byte)((pixelDataTmp[tmp[7]] & 0xff0000) >> 16);
                    values[7] += (byte)((pixelDataTmp[tmp[7]] & 0xff00) >> 8);
                    values[7] += (byte)((pixelDataTmp[tmp[7]] & 0xff));

                    tmp[8] = GetIndex(x + 1, y + 1);
                    values[8] += (byte)((pixelDataTmp[tmp[8]] & 0xff0000) >> 16);
                    values[8] += (byte)((pixelDataTmp[tmp[8]] & 0xff00) >> 8);
                    values[8] += (byte)((pixelDataTmp[tmp[8]] & 0xff));

                    var tmp2 = values.OrderBy(i => i.Value);
                    int tmp3 = tmp2.ElementAt(4).Key;

                    pixelDataTmp[index] = (uint)((alpha << 24) | (pixelDataTmp[tmp[tmp3]] << 16) | (pixelDataTmp[tmp[tmp3]] << 8) | (pixelDataTmp[tmp[tmp3]] << 0));
                }
            }

            pixelDataUndo = pixelData;
            pixelData = pixelDataTmp;
            photo.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = photo;
        }

        private void EdgeDetect_Click(object sender, RoutedEventArgs e)
        {
            uint[] pixelDataTmp = (uint[])pixelData.Clone();

            int[][] Gx = new int[][] {
                new int[] {1,2,1},
                new int[] {0,0,0},
                new int[] {-1,-2,-1},
            };
            int[][] Gy = new int[][] {
                new int[] {1,0,-1},
                new int[] {2,0,-2},
                new int[] {1,0,-1},
            };

            byte alpha = 0;

            for (int i = 1; i < (int)photo.Width - 1; i++)
            {
                for (int j = 1; j < (int)photo.Height - 1; j++)
                {
                    double[] new_x = new double[3];
                    double[] new_y = new double[3];

                    double r, g, b;
                    for (int hw = -1; hw < 2; hw++)
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            int tmp = Gx[hw + 1][wi + 1];
                            r = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff0000) >> 16);
                            new_x[0] += tmp * r;
                            new_y[0] += tmp * r;

                            g = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff00) >> 8);
                            new_x[1] += tmp * g;
                            new_y[1] += tmp * g;

                            b = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff));
                            new_x[2] += tmp * b;
                            new_y[2] += tmp * b;
                        }
                    }
                    byte red = (Convert.ToByte(Convert.ToInt32(Math.Sqrt(new_x[0] * new_x[0] + new_y[0] * new_y[0])) % 255));
                    byte green = (Convert.ToByte(Convert.ToInt32(Math.Sqrt(new_x[1] * new_x[1] + new_y[1] * new_y[1])) % 255));
                    byte blue = (Convert.ToByte(Convert.ToInt32(Math.Sqrt(new_x[2] * new_x[2] + new_y[2] * new_y[2])) % 255));
                    if (edgeNoColor_cb.IsChecked == false)
                        pixelDataTmp[GetIndex(i, j)] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                    else
                        pixelDataTmp[GetIndex(i, j)] = (uint)((alpha << 24) | (blue << 16) | (blue << 8) | (blue << 0));
                }
            }

            pixelDataUndo = pixelData;
            pixelData = pixelDataTmp;
            photo.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = photo;
        }

        private void HighPassSharpening_Click(object sender, RoutedEventArgs e)
        {
            int[][] Gx = new int[][] {
                new int[] {-1,-1,-1},
                new int[] {-1,9,-1},
                new int[] {-1,-1,-1},
            };

            byte alpha = 0;

            uint[] pixelDataTmp = (uint[])pixelData.Clone();

            for (int i = 1; i < (int)photo.Width - 1; i++)
            {
                for (int j = 1; j < (int)photo.Height - 1; j++)
                {
                    double[] new_x = new double[3];

                    double r = 0, g = 0, b = 0;
                    double sw = 0;
                    for (int hw = -1; hw < 2; hw++)
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            int tmp = Gx[hw + 1][wi + 1];
                            double tmp2;
                            tmp2 = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff0000) >> 16);
                            sw += tmp;
                            r += tmp * tmp2;

                            tmp2 = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff00) >> 8);
                            g += tmp * tmp2;

                            tmp2 = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff));
                            b += tmp * tmp2;
                        }
                    }
                    if (sw == 0)
                        sw = 1;
                    if (r < 0)
                        r = 0;
                    if (g < 0)
                        g = 0;
                    if (b < 0)
                        b = 0;

                    byte red = (Convert.ToByte(Convert.ToInt32(r / sw) % 255));
                    byte green = (Convert.ToByte(Convert.ToInt32(g / sw) % 255));
                    byte blue = (Convert.ToByte(Convert.ToInt32(b / sw) % 255));
                    pixelDataTmp[GetIndex(i, j)] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                }
            }

            pixelDataUndo = pixelData;
            pixelData = pixelDataTmp;
            photo.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = photo;
        }

        private void GaussianBlur_Click(object sender, RoutedEventArgs e)
        {
            int[][] Gx = new int[][] {
                new int[] {1,2,1},
                new int[] {2,4,2},
                new int[] {1,2,1},
            };

            byte alpha = 0;

            uint[] pixelDataTmp = (uint[])pixelData.Clone();

            for (int i = 1; i < (int)photo.Width - 1; i++)
            {
                for (int j = 1; j < (int)photo.Height - 1; j++)
                {
                    double[] new_x = new double[3];

                    double r = 0, g = 0, b = 0;
                    double sw = 0;
                    for (int hw = -1; hw < 2; hw++)
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            int tmp = Gx[hw + 1][wi + 1];
                            double tmp2;
                            tmp2 = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff0000) >> 16);
                            sw += tmp;
                            r += tmp * tmp2;

                            tmp2 = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff00) >> 8);
                            g += tmp * tmp2;

                            tmp2 = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff));
                            b += tmp * tmp2;
                        }
                    }

                    if (sw == 0)
                        sw = 1;

                    byte red = (Convert.ToByte(Convert.ToInt32(r / sw) % 255));
                    byte green = (Convert.ToByte(Convert.ToInt32(g / sw) % 255));
                    byte blue = (Convert.ToByte(Convert.ToInt32(b / sw) % 255));
                    pixelDataTmp[GetIndex(i, j)] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                }
            }

            pixelDataUndo = pixelData;
            pixelData = pixelDataTmp;
            photo.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = photo;
        }

        private void WeaveTheMask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int[][] Gx = new int[][] {
                new int[] {Int32.Parse(maskValue00.Text), Int32.Parse(maskValue01.Text), Int32.Parse(maskValue02.Text)},
                new int[] {Int32.Parse(maskValue10.Text), Int32.Parse(maskValue11.Text), Int32.Parse(maskValue12.Text)},
                new int[] {Int32.Parse(maskValue20.Text), Int32.Parse(maskValue21.Text), Int32.Parse(maskValue22.Text)},
            };

                byte alpha = 0;

                uint[] pixelDataTmp = (uint[])pixelData.Clone();

                for (int i = 1; i < (int)photo.Width - 1; i++)
                {
                    for (int j = 1; j < (int)photo.Height - 1; j++)
                    {
                        double[] new_x = new double[3];

                        double r = 0, g = 0, b = 0;
                        double sw = 0;
                        for (int hw = -1; hw < 2; hw++)
                        {
                            for (int wi = -1; wi < 2; wi++)
                            {
                                int tmp = Gx[hw + 1][wi + 1];
                                double tmp2;
                                tmp2 = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff0000) >> 16);
                                sw += tmp;
                                r += tmp * tmp2;

                                tmp2 = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff00) >> 8);
                                g += tmp * tmp2;

                                tmp2 = ((pixelData[GetIndex(i + hw, j + wi)] & 0xff));
                                b += tmp * tmp2;
                            }
                        }

                        if (sw <= 0)
                            sw = 1;
                        if (r < 0)
                            r = 0;
                        if (g < 0)
                            g = 0;
                        if (b < 0)
                            b = 0;

                        byte red = (Convert.ToByte(Convert.ToInt32(r / sw) % 255));
                        byte green = (Convert.ToByte(Convert.ToInt32(g / sw) % 255));
                        byte blue = (Convert.ToByte(Convert.ToInt32(b / sw) % 255));
                        pixelDataTmp[GetIndex(i, j)] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
                    }
                }

                pixelDataUndo = pixelData;
                pixelData = pixelDataTmp;
                photo.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
                image_i.Source = photo;
            }
            catch (FormatException)
            {
                MessageBox.Show("Missing or bad mask values.");
            }
        }

        private void GreyScale_Click(object sender, RoutedEventArgs e)
        {
            uint[] pixelDataTmp = (uint[])pixelData.Clone();
            float rgb;
            byte alpha = 0;
            for (int i = 0; i < pixelDataTmp.Length; i++)
            {
                rgb = ((byte)((pixelDataTmp[i] & 0xff0000) >> 16)) * .21f;
                rgb += ((byte)((pixelDataTmp[i] & 0xff00) >> 8)) * .71f;
                rgb += ((byte)((pixelDataTmp[i] & 0xff))) * .071f;

                pixelDataTmp[i] = (uint)((alpha << 24) | ((byte)rgb << 16) | ((byte)rgb << 8) | ((byte)rgb << 0));
            }

            pixelDataUndo = pixelData;
            pixelData = pixelDataTmp;
            photo.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = photo;
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            pixelData = (uint[])pixelDataUndo.Clone();
            photo.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = photo;
        }
    }
}