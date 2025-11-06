using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace BluePrintOne
{
    public partial class CadWindow : Window
    {
        private string currentTool = "Line";
        private Point startPoint;
        private Shape? currentShape;
        private bool isDrawing = false;
        private string? currentFilePath = null;

        public CadWindow()
        {
            InitializeComponent();
        }

        private void Line_Click(object sender, RoutedEventArgs e) => currentTool = "Line";
        private void Rectangle_Click(object sender, RoutedEventArgs e) => currentTool = "Rectangle";
        private void Circle_Click(object sender, RoutedEventArgs e) => currentTool = "Circle";
        private void Clear_Click(object sender, RoutedEventArgs e) => DrawCanvas.Children.Clear();
        private void Undo_Click(object sender, RoutedEventArgs e) { if (DrawCanvas.Children.Count > 0) DrawCanvas.Children.RemoveAt(DrawCanvas.Children.Count - 1); }
        private void New_Click(object sender, RoutedEventArgs e) { DrawCanvas.Children.Clear(); currentFilePath = null; }
        private void Exit_Click(object sender, RoutedEventArgs e) => Close();
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (currentFilePath == null) SaveAs_Click(sender, e);
            else SaveToFile(currentFilePath);
        }
        
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg",
                DefaultExt = "png",
                FileName = $"Drawing_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            
            if (dialog.ShowDialog() == true)
            {
                currentFilePath = dialog.FileName;
                SaveToFile(currentFilePath);
            }
        }
        
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg"
            };
            
            if (dialog.ShowDialog() == true)
            {
                currentFilePath = dialog.FileName;
                MessageBox.Show("Open feature coming soon!", "BluePrintOne");
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(DrawCanvas);
            isDrawing = true;
            var brush = new SolidColorBrush(Color.FromRgb(107, 70, 193));

            if (currentTool == "Line")
            {
                currentShape = new Line { Stroke = brush, StrokeThickness = 3, X1 = startPoint.X, Y1 = startPoint.Y };
                DrawCanvas.Children.Add(currentShape);
            }
            else if (currentTool == "Rectangle")
            {
                currentShape = new Rectangle { Stroke = brush, StrokeThickness = 3, Fill = Brushes.Transparent };
                DrawCanvas.Children.Add(currentShape);
            }
            else if (currentTool == "Circle")
            {
                currentShape = new Ellipse { Stroke = brush, StrokeThickness = 3, Fill = Brushes.Transparent };
                DrawCanvas.Children.Add(currentShape);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return;
            var pos = e.GetPosition(DrawCanvas);

            if (currentShape is Line line)
            {
                line.X2 = pos.X;
                line.Y2 = pos.Y;
            }
            else if (currentShape is Rectangle rect)
            {
                var x = System.Math.Min(pos.X, startPoint.X);
                var y = System.Math.Min(pos.Y, startPoint.Y);
                var w = System.Math.Abs(pos.X - startPoint.X);
                var h = System.Math.Abs(pos.Y - startPoint.Y);
                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                rect.Width = w;
                rect.Height = h;
            }
            else if (currentShape is Ellipse ellipse)
            {
                var x = System.Math.Min(pos.X, startPoint.X);
                var y = System.Math.Min(pos.Y, startPoint.Y);
                var w = System.Math.Abs(pos.X - startPoint.X);
                var h = System.Math.Abs(pos.Y - startPoint.Y);
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
                ellipse.Width = w;
                ellipse.Height = h;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            currentShape = null;
        }

        private void SaveToFile(string filePath)
        {
            var renderBitmap = new RenderTargetBitmap((int)DrawCanvas.ActualWidth, (int)DrawCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(DrawCanvas);
            
            BitmapEncoder encoder = filePath.EndsWith(".jpg") ? new JpegBitmapEncoder() : new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            
            using (var file = File.Create(filePath))
            {
                encoder.Save(file);
            }
            
            MessageBox.Show($"Saved to: {filePath}", "Saved Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
