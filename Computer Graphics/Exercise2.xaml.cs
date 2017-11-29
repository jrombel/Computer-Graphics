using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Computer_Graphics
{
    public partial class Exercise2 : Page
    {
        public Exercise2()
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(image_i, BitmapScalingMode.NearestNeighbor);
            quality_s.Value = 80;
        }

        private void loadImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|PPM Files (*.ppm)|*.ppm";

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    string filename = dlg.FileName;

                    String[] substrings = filename.Split('.');

                    if (substrings[substrings.Length - 1] == "jpeg" || substrings[substrings.Length - 1] == "png")
                    {
                        BitmapImage b = new BitmapImage();
                        b.BeginInit();
                        b.UriSource = new Uri(filename);
                        b.EndInit();

                        image_i.Source = b;
                    }
                    else if (substrings[substrings.Length - 1] == "ppm")
                    {
                        StreamReader sr = new StreamReader(filename);

                        String format = "";
                        int rows = 0;
                        int columns = 0;
                        int max = 0;
                        int startPosition = 0;

                        byte[] buffer = new byte[1];
                        int color = 0;
                        String line;
                        String[] data;
                        int counter = 0;
                        List<byte> tmp = new List<byte>();

                        bool escape = false;
                        while ((line = sr.ReadLine()) != null)
                        {
                            startPosition += (line.Length + 1);
                            if (line == "" || line[0] == '#')
                                continue;
                            line = Regex.Replace(line, @"\s+", " ");
                            data = line.Split(' ');
                            foreach (var information in data)
                            {
                                if (information == "")
                                    continue;
                                if (information[0] == '#')
                                    break;

                                if (counter > 3)
                                {
                                    color = Int32.Parse(information);
                                    if (max != 255)
                                        color = color * 255 / max;
                                    tmp.Add((byte)color);
                                }
                                else if (counter == 0)
                                    format = information;
                                else if (counter == 1)
                                    rows = Int32.Parse(information);
                                else if (counter == 2)
                                    columns = Int32.Parse(information);
                                else if (counter == 3)
                                {
                                    max = Int32.Parse(information);
                                    if (format == "P6")
                                        escape = true;
                                }
                                counter++;
                            }
                            if (escape == true)
                                break;
                        }
                        sr.Close();

                        if (format == "P6")
                        {
                            BinaryReader br = new BinaryReader(new FileStream(dlg.FileName, FileMode.Open));
                            br.BaseStream.Position = startPosition;
                            buffer = br.ReadBytes(rows * columns * 3);
                            br.Close();
                        }
                        if (format != "P6")
                            buffer = tmp.ToArray();

                        var pixelFormat = PixelFormats.Rgb24;
                        var stride = 3 * rows;

                        var bitmap = BitmapSource.Create(rows, columns, 0d, 0d, pixelFormat, null, buffer, stride);

                        image_i.Source = bitmap;
                    }
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("File error!");
            }
        }

        private void saveImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();

            saveDialog.FileName = "file";
            saveDialog.DefaultExt = "jpg";
            saveDialog.Filter = "JPG images (*.jpg)|*.jpg";

            Nullable<bool> result = saveDialog.ShowDialog();

            if (result == true)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                Guid photoID = System.Guid.NewGuid();
                String photolocation = saveDialog.FileName + ".jpg";

                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image_i.Source));
                encoder.QualityLevel = (int)quality_s.Value;

                using (var filestream = new FileStream(photolocation, FileMode.Create))
                    encoder.Save(filestream);
            }
        }
    }
}
