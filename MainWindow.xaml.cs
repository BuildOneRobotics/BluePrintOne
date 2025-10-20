using System.Windows;

namespace BluePrintOne
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _ = Updater.CheckForUpdates();
        }

        private void CadDesign_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Opening CAD Design...", "BluePrintOne");
        }

        private void Planning_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Opening Planning...", "BluePrintOne");
        }

        private void ProjectManagement_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Opening Project Management...", "BluePrintOne");
        }
    }
}
