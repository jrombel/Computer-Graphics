using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Computer_Graphics
{
    public partial class Exercise8 : Page
    {
        private WriteableBitmap photo;
        private double height;
        private double width;
        private uint[] pixelData;
        private bool[] pixels;//false - white; true - black
        private bool[] pixelsTmp;
        private int widthInByte;
        private bool[][] mask = { new bool[3], new bool[3], new bool[3] };

        public Exercise8()
        {
            InitializeComponent();
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
                    pixels = new bool[(int)(width * height)];
                    pixelsTmp = new bool[(int)(width * height)];
                    widthInByte = (int)(4 * width);

                    photo.CopyPixels(pixelData, widthInByte, 0);

                    for (int i = 0; i < pixelData.Length; i++)
                    {
                        byte red = (byte)((pixelData[i] & 0xff0000) >> 16);
                        byte green = (byte)((pixelData[i] & 0xff00) >> 8);
                        byte blue = (byte)((pixelData[i] & 0xff));

                        if ((byte)(0.114 * blue + 0.587 * green + 0.299 * red) <= 128)
                            pixelsTmp[i] = true;
                        else
                            pixelsTmp[i] = false;
                    }

                    CreateImage();
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("File error!");
            }
        }

        private void CreateImage()
        {
            pixels = (bool[])pixelsTmp.Clone();
            for (int i = 0; i < pixels.Length; i++)
            {
                byte red, green, blue, alpha = 0;

                if (pixels[i] == false)
                {
                    red = 255;
                    green = 255;
                    blue = 255;
                }
                else
                {
                    red = 0;
                    green = 0;
                    blue = 0;
                }

                pixelData[i] = (uint)((alpha << 24) | (red << 16) | (green << 8) | (blue << 0));
            }
            WriteableBitmap tmp = new WriteableBitmap(photo.PixelWidth, photo.PixelHeight, photo.DpiX, photo.DpiY, photo.Format, photo.Palette);
            tmp.WritePixels(new Int32Rect(0, 0, (int)width, (int)height), pixelData, widthInByte, 0);
            image_i.Source = tmp;
        }

        private void LoadMask()
        {
            mask[0][0] = (bool)mask00_cb.IsChecked;
            mask[0][1] = (bool)mask01_cb.IsChecked;
            mask[0][2] = (bool)mask02_cb.IsChecked;
            mask[1][0] = (bool)mask10_cb.IsChecked;
            mask[1][1] = (bool)mask11_cb.IsChecked;
            mask[1][2] = (bool)mask12_cb.IsChecked;
            mask[2][0] = (bool)mask20_cb.IsChecked;
            mask[2][1] = (bool)mask21_cb.IsChecked;
            mask[2][2] = (bool)mask22_cb.IsChecked;
        }

        private int GetIndex(double x, double y)
        {
            return (int)(y * width + x);
        }

        private void DilationMorphology_Click(object sender, RoutedEventArgs e)
        {
            DilationMorphology();
        }

        private void ErosionMorphology_Click(object sender, RoutedEventArgs e)
        {
            ErosionMorphology();
        }

        private void OpeningMorphology_Click(object sender, RoutedEventArgs e)
        {
            ErosionMorphology();
            DilationMorphology();
        }

        private void ClosingMorphology_Click(object sender, RoutedEventArgs e)
        {
            DilationMorphology();
            ErosionMorphology();
        }

        private void HitOrMissThinningMorphology_Click(object sender, RoutedEventArgs e)
        {
            int[][,] E = new int[8][,]
            {
                new int[3,3] { { -1, -1, -1 }, { 0, 1, 0 }, { 1, 1, 1 } },
            new int[3, 3] { { 0, -1, -1 }, { 1, 1, -1 }, { 0, 1, 0 } },
            new int[3, 3] { { 1, 0, -1 }, { 1, 1, -1 }, { 1, 0, -1 } },
            new int[3, 3] { { 0, 1, 0 }, { 1, 1, -1 }, { 0, -1, -1 } },
            new int[3, 3] { { 1, 1, 1 }, { 0, 1, 0 }, { -1, -1, -1 } },
            new int[3, 3] { { 0, 1, 0 }, { -1, 1, 1 }, { -1, -1, 0 } },
            new int[3, 3] { { -1, 0, 1 }, { -1, 1, 1 }, { -1, 0, 1 } },
            new int[3, 3] { { -1, -1, 0 }, { -1, 1, 1 }, { 0, 1, 0 } }
        };

            bool[] I = (bool[])pixels.Clone();
            bool[] HoM = (bool[])pixels.Clone();//Hit or Miss(I,E)

            for (int i = 0; i < 8; i++)
            {
                for (int x = 0; x < photo.Width; x++)
                {
                    for (int y = 0; y < photo.Height; y++)
                    {
                        int xMaskStart = -1, yMaskStart = -1;
                        int xMaskStop = 2, yMaskStop = 2;

                        if (x == 0)
                            xMaskStart = 0;
                        else if (x == (photo.Width - 1))
                            xMaskStop = 1;

                        if (y == 0)
                            yMaskStart = 0;
                        else if (y == (photo.Height - 1))
                            yMaskStop = 1;

                        bool find = true;
                        if (xMaskStart != -1 || yMaskStart != -1 || xMaskStop != 2 || yMaskStop != 2)
                            find = false;

                        for (; xMaskStart < xMaskStop; xMaskStart++)
                        {
                            for (; yMaskStart < yMaskStop; yMaskStart++)
                            {

                                if (E[i][xMaskStart + 1, yMaskStart + 1] == 1 && pixels[GetIndex(x + xMaskStart, y + yMaskStart)] == false)
                                {
                                    find = false;
                                    break;
                                }
                                else if (E[i][xMaskStart + 1, yMaskStart + 1] == -1 && pixels[GetIndex(x + xMaskStart, y + yMaskStart)] == true)
                                {
                                    find = false;
                                    break;
                                }
                            }
                            if (find == false)
                                break;
                        }
                        if (find == true)
                            HoM[GetIndex(x, y)] = true;
                        else
                            HoM[GetIndex(x, y)] = false;

                        int index = GetIndex(x, y);
                        if (I[index] == true || (!HoM[index]) == true)
                            pixelsTmp[index] = true;
                        else
                            pixelsTmp[index] = false;
                    }
                }
            }

            CreateImage();
        }

        private void HitOrMissThickeningMorphology_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("It doesn't work well!");

            int[][,] E = new int[8][,]
            {
                new int[3,3] { { 1, 1, 0 }, { 1, -1, 0 }, { 1, 0, -1 } },
            new int[3, 3] { { 0, 1, 1 }, { 0, -1, 1 }, { -1, 0, 1 } },
            new int[3, 3] { { 1, 1, 1 }, { 0, -1, 1 }, { -1, 0, 0 } },
            new int[3, 3] { { -1, 0, 0 }, { 0, -1, 1 }, { 1, 1, 1 } },
            new int[3, 3] { { -1, 0, 1 }, { 0, -1, 1 }, { 0, 1, 1 } },
            new int[3, 3] { { 1, 0, -1 }, { 1, -1, 0 }, { 1, 1, 0 } },
            new int[3, 3] { { 0, 0, -1 }, { 1, -1, 0 }, { 1, 1, 1 } },
            new int[3, 3] { { 1, 1, 1 }, { 1, -1, 0 }, { 0, 0, -1 } }
        };

            bool[] I = (bool[])pixels.Clone();
            bool[] HoM = (bool[])pixels.Clone();//Hit or Miss(I,E)

            for (int i = 0; i < 8; i++)
            {
                for (int x = 0; x < photo.Width; x++)
                {
                    for (int y = 0; y < photo.Height; y++)
                    {
                        int xMaskStart = -1, yMaskStart = -1;
                        int xMaskStop = 2, yMaskStop = 2;

                        if (x == 0)
                            xMaskStart = 0;
                        else if (x == (photo.Width - 1))
                            xMaskStop = 1;

                        if (y == 0)
                            yMaskStart = 0;
                        else if (y == (photo.Height - 1))
                            yMaskStop = 1;


                        bool find = true;
                        if (xMaskStart != -1 || yMaskStart != -1 || xMaskStop != 2 || yMaskStop != 2)
                            find = false;
                        for (; xMaskStart < xMaskStop; xMaskStart++)
                        {
                            for (; yMaskStart < yMaskStop; yMaskStart++)
                            {
                                if (E[i][xMaskStart + 1, yMaskStart + 1] == 1 && pixels[GetIndex(x + xMaskStart, y + yMaskStart)] == false)
                                {
                                    find = false;
                                    break;
                                }
                                else if (E[i][xMaskStart + 1, yMaskStart + 1] == -1 && pixels[GetIndex(x + xMaskStart, y + yMaskStart)] == true)
                                {
                                    find = false;
                                    break;
                                }
                            }
                            if (find == false)
                                break;
                        }
                        if (find == true)
                            HoM[GetIndex(x, y)] = true;
                        else
                            HoM[GetIndex(x, y)] = false;

                        int index = GetIndex(x, y);
                        if (I[index] == true || HoM[index] == true)
                            pixelsTmp[index] = true;
                        else
                            pixelsTmp[index] = false;

                    }
                }
                pixels = (bool[])pixelsTmp.Clone();
            }

            pixelsTmp = (bool[])pixels.Clone();

            CreateImage();
        }

        private void DilationMorphology()
        {
            LoadMask();

            for (int x = 0; x < photo.Width; x++)
            {
                for (int y = 0; y < photo.Height; y++)
                {
                    int xMaskStart = -1, yMaskStart = -1;
                    int xMaskStop = 2, yMaskStop = 2;

                    if (x == 0)
                        xMaskStart = 0;
                    else if (x == (photo.Width - 1))
                        xMaskStop = 1;

                    if (y == 0)
                        yMaskStart = 0;
                    else if (y == (photo.Height - 1))
                        yMaskStop = 1;

                    bool find = false;
                    for (; xMaskStart < xMaskStop; xMaskStart++)
                    {
                        for (; yMaskStart < yMaskStop; yMaskStart++)
                        {
                            if (mask[xMaskStart + 1][yMaskStart + 1] == true && pixels[GetIndex(x + xMaskStart, y + yMaskStart)] == true)
                            {
                                find = true;
                                break;
                            }
                        }
                        if (find == true)
                            break;
                    }
                    if (find == true)
                        pixelsTmp[GetIndex(x, y)] = true;
                }
            }

            CreateImage();
        }

        private void ErosionMorphology()
        {
            LoadMask();

            for (int x = 0; x < photo.Width; x++)
            {
                for (int y = 0; y < photo.Height; y++)
                {
                    int xMaskStart = -1, yMaskStart = -1;
                    int xMaskStop = 2, yMaskStop = 2;

                    if (x == 0)
                        xMaskStart = 0;
                    else if (x == (photo.Width - 1))
                        xMaskStop = 1;

                    if (y == 0)
                        yMaskStart = 0;
                    else if (y == (photo.Height - 1))
                        yMaskStop = 1;

                    bool find = true;
                    if (xMaskStart != -1 || yMaskStart != -1 || xMaskStop != 2 || yMaskStop != 2)
                        find = false;

                    for (; xMaskStart < xMaskStop; xMaskStart++)
                    {
                        for (; yMaskStart < yMaskStop; yMaskStart++)
                        {
                            if (mask[xMaskStart + 1][yMaskStart + 1] == true && pixels[GetIndex(x + xMaskStart, y + yMaskStart)] == false)
                            {
                                find = false;
                                break;
                            }
                        }
                        if (find == false)
                            break;
                    }
                    if (find == true)
                        pixelsTmp[GetIndex(x, y)] = true;
                    else
                        pixelsTmp[GetIndex(x, y)] = false;
                }
            }

            CreateImage();
        }

        private void StrangeEdgeDetection_Click(object sender, RoutedEventArgs e)
        {
            //it's just an accident - unintentional

            mask00_cb.IsChecked = false;
            mask01_cb.IsChecked = false;
            mask02_cb.IsChecked = false;
            mask10_cb.IsChecked = false;
            mask11_cb.IsChecked = false;
            mask12_cb.IsChecked = false;
            mask20_cb.IsChecked = false;
            mask21_cb.IsChecked = false;
            mask22_cb.IsChecked = false;
            LoadMask();

            for (int x = 0; x < photo.Width; x++)
            {
                for (int y = 0; y < photo.Height; y++)
                {
                    int xMaskStart = -1, yMaskStart = -1;
                    int xMaskStop = 2, yMaskStop = 2;

                    if (x == 0)
                        xMaskStart = 0;
                    else if (x == (photo.Width - 1))
                        xMaskStop = 1;

                    if (y == 0)
                        yMaskStart = 0;
                    else if (y == (photo.Height - 1))
                        yMaskStop = 1;

                    bool find = true;
                    if (xMaskStart != -1 || yMaskStart != -1 || xMaskStop != 2 || yMaskStop != 2)
                        find = false;

                    for (; xMaskStart < xMaskStop; xMaskStart++)
                    {
                        for (; yMaskStart < yMaskStop; yMaskStart++)
                        {
                            if (pixels[GetIndex(x + xMaskStart, y + yMaskStart)] == true && mask[xMaskStart + 1][yMaskStart + 1] == false)
                            {
                                find = false;
                                break;
                            }
                        }
                        if (find == false)
                            break;
                    }
                    if (find == true)
                        pixelsTmp[GetIndex(x, y)] = true;
                }
            }

            CreateImage();
        }
    }
}
