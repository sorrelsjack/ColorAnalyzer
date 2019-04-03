using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ColorAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Color> colorList = new List<Color>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AnalyzeColor(BitmapImage image)
        {
            int index = 0;
            int stride = image.PixelWidth * 4;
            int size = image.PixelHeight * stride;
            byte[] imageBytes = new byte[size];
            image.CopyPixels(imageBytes, stride, 0);

            byte red;
            byte green;
            byte blue;
            byte alpha;

            while (index < size && index + 3 < size)
            {
                blue = imageBytes[index++];
                green = imageBytes[index++];
                red = imageBytes[index++];
                alpha = imageBytes[index];

                if (alpha == 255)
                    colorList.Add(Color.FromRgb(red, green, blue));
            }

            colorList = colorList.Distinct().ToList();

            foreach (var color in colorList)
            {
                Debug.WriteLine($"R:{color.R}, G:{color.G} B:{color.B} A:{color.A}");
            }

            PopulatePalette();
        }

        private void PopulatePalette()
        {
            foreach (var color in colorList)
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = color;
                Shape currentShape = new Ellipse() { Height = 20, Width = 20 };

                currentShape.Fill = brush;
                colorPalettePanel.Children.Add(currentShape);
            }
        }

        private void OnImageUpload(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            BitmapImage downloadedImage = new BitmapImage();

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    Stream stream = File.Open(openFileDialog.FileName, FileMode.Open);

                    downloadedImage.BeginInit();
                    downloadedImage.StreamSource = stream;
                    downloadedImage.EndInit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error uploading image.");
                }

                AnalyzeColor(downloadedImage);
            }
        }

        private void DownloadImage()
        {
        }

        private void OnEyedropper()
        {
        }

        private void OnSelection(object sender, RoutedEventArgs e)
        {
            BitmapImage capture = new BitmapImage();
        }
    }
}