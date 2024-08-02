using System;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Newtonsoft.Json;

namespace CrystalMessagerWPF
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        readonly string filePath = "PersonData.json";
        private string loginText;
        private string passwordText;
        private bool checkMarkState;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void loginButton(object sender, RoutedEventArgs e)
        {
            loginText = loginTextBox.Text;
            passwordText = passwordTextBox.Text;
            checkMarkState = rememberCheckBox.IsChecked.Value;

            DbUtils.ConnectionDB();
            int id = Authorization.Authorize(loginText, passwordText);
            if (id != 0)
            {
                DbUtils.ConnectionDB();
                DbUtils.command.CommandText = "SELECT name FROM userdata WHERE id=@id";
                DbUtils.command.Parameters.AddWithValue("@id", id);
                var reader = DbUtils.command.ExecuteReader();
                reader.Read();
                string name = reader.GetString("name");

                PersonData loginData = new PersonData(loginText, passwordText, id); 
                loginData.Name = name;
                string json = JsonConvert.SerializeObject(loginData);
                File.WriteAllText(filePath, json);


                MainWindow mainWindow = new MainWindow(loginData);
                App.Current.MainWindow = mainWindow;
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Вы ввели неправильный логин или пароль");
            }
            DbUtils.CloseDB();
        }

        private void goToRegWindow(object sender, RoutedEventArgs e)
        {
            RegistrationWindow regWindow = new RegistrationWindow();
            App.Current.MainWindow = regWindow;
            regWindow.Show();
            this.Close();
        }
    }
}
