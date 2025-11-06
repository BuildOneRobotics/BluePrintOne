using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BluePrintOne
{
    public partial class CadWindow : Window
    {
        private string currentTool = "Line";
        private Point startPoint;
        private Shape? currentShape;
        private bool isDrawing = false;

        public CadWindow()
        {
            InitializeComponent();
        }

        private void Line_Click(object sender, RoutedEventArgs e) => currentTool = "Line";
        private void Rectangle_Click(object sender, RoutedEventArgs e) => currentTool = "Rectangle";
        private void Circle_Click(object sender, RoutedEventArgs e) => currentTool = "Circle";
        private void Clear_Click(object sender, RoutedEventArgs e) => DrawCanvas.Children.Clear();
        private void Save_Click(object sender, RoutedEventArgs e) => SaveToCloud();

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

        private void SaveToCloud()
        {
            var oneDrivePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\OneDrive";
            var savePath = Directory.Exists(oneDrivePath) ? oneDrivePath : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            savePath = Path.Combine(savePath, "BluePrintOne");
            
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            
            var fileName = $"Drawing_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var fullPath = Path.Combine(savePath, fileName);
            
            var renderBitmap = new RenderTargetBitmap((int)DrawCanvas.ActualWidth, (int)DrawCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(DrawCanvas);
            
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            
            using (var file = File.Create(fullPath))
            {
                encoder.Save(file);
            }
            
            MessageBox.Show($"Saved to: {fullPath}", "Saved Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
