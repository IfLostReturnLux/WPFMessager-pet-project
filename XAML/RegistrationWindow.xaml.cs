using System.Windows;

namespace CrystalMessagerWPF
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void goToLoginWindow(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            App.Current.MainWindow = loginWindow;
            loginWindow.Show();
            this.Close();
        }
    }
}
