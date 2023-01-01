using System.Windows;
using System.Windows.Input;
using WPF_AI.Hospital.windows;

namespace WPF_AI.Hospital
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CountiesButton_Click(object sender, RoutedEventArgs e)
        {
            var countiesWindow = new CountiesWindow();
            countiesWindow.Show();
            this.Close();
        }

        private void SymptomsButton_Click(object sender, RoutedEventArgs e)
        {
            var symptomsWindow = new SymptomsWindow();
            symptomsWindow.Show();
            this.Close();
        }

        private void DiseasesButton_Click(object sender, RoutedEventArgs e)
        {
            var diseasesWindow = new DiseasesWindow();
            diseasesWindow.Show();
            this.Close();
        }

        private void VirusesButton_Click(object sender, RoutedEventArgs e)
        {
            var virusesWindow = new VirusesWindow();
            virusesWindow.Show();
            this.Close();
        }

        private void PeopleButton_Click(object sender, RoutedEventArgs e)
        {
            var peopleWindow = new PeopleWindow();
            peopleWindow.Show();
            this.Close();
        }

        private void ClinicButton_Click(object sender, RoutedEventArgs e)
        {
            var clinicWindow = new ClinicWindow();
            clinicWindow.Show();
            this.Close();
        }
    }
}
