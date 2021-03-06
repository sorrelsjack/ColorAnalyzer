﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
            spinnerPanel.Visibility = Visibility.Visible;
            spinner.IsActive = true;

            int index = 0;
            int stride = image.PixelWidth * 4;
            int size = image.PixelHeight * stride;
            byte[] imageBytes = new byte[size];
            image.CopyPixels(imageBytes, stride, 0);

            byte red;
            byte green;
            byte blue;
            byte alpha;

            for (int y = 0; y < image.PixelHeight; y++)
            {
                for (int x = 0; x < image.PixelWidth; x++)
                {
                    index = y * stride + 4 * x;

                    blue = imageBytes[index++];
                    green = imageBytes[index++];
                    red = imageBytes[index++];
                    alpha = imageBytes[index];

                    if (alpha == 255)
                        colorList.Add(Color.FromRgb(red, green, blue));
                }
            }

            colorList = colorList.Distinct().ToList();

            foreach (var color in colorList)
            {
                Debug.WriteLine($"R:{color.R}, G:{color.G} B:{color.B} A:{color.A}");
            }

            PopulatePalette();

            spinnerPanel.Visibility = Visibility.Hidden;
            spinner.IsActive = false;
        }

        private void PopulatePalette()
        {
            foreach (var color in colorList)
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = color;
                Shape currentShape = new Ellipse() { Height = 30, Width = 30 };
                currentShape.MouseDown += (s, e) => { OnPaletteCircleClicked(s, e); };

                currentShape.Fill = brush;
                currentShape.Margin = new Thickness(0.0, 0.0, 5.0, 5.0);
                colorPalettePanel.Children.Add(currentShape);
            }
        }

        private void OnPaletteCircleClicked(object sender, MouseEventArgs e)
        {
            foreach (Shape shape in colorPalettePanel.Children)
            {
                shape.Stroke = null;
            }

            var colorCircle = sender as Shape;
            colorCircle.StrokeThickness = 2.0;
            colorCircle.Stroke = Brushes.Black;

            Color color = (colorCircle.Fill as SolidColorBrush).Color;
            rgbTextbox.Text = $"{color.R}, {color.G}, {color.B}";
            hexTextbox.Text = CalculateHex(color);
            hslTextbox.Text = CalculateHsl(color);
        }

        private string CalculateHex(Color color)
            => $"#{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";

        private string CalculateHsl(Color color)
        {
            return "";
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
                    Debug.WriteLine(ex);
                    MessageBox.Show("Error uploading image.");
                }

                ClearAll();
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

        private void ClearTextBoxes()
        {
            rgbTextbox.Text = string.Empty;
            hexTextbox.Text = string.Empty;
            hslTextbox.Text = string.Empty;
        }

        private void ClearColorPalette()
        {
            colorList.Clear();
            colorPalettePanel.Children.Clear();
        }

        private void ClearAll()
        {
            ClearTextBoxes();
            ClearColorPalette();
        }
    }
}