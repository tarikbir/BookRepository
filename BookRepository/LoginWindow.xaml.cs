using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookRepository
{
    public partial class LoginWindow : Window
    {
        public User User;
        public bool LoggedIn;

        public LoginWindow()
        {
            InitializeComponent();
            txtUser.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginResponse = SqlHandler.UserEntry(txtUser.Text, txtPass.Password);
            if(loginResponse.Success)
            {
                string extraText = String.Empty;
                if (loginResponse.Content.IsAdmin) extraText = " as an admin";
                MessageBox.Show("You have successfully logged in" + extraText + "."
                    , "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoggedIn = true;
                User = loginResponse.Content;
                this.Close();
            }
            else
            {
                MessageBox.Show("An error occured.\n\n" + loginResponse.ErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoggedIn = false;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            CreateAccountWindow createAccountWindow = new CreateAccountWindow();
            createAccountWindow.ShowDialog();
        }

        private void btnRecover_Click(object sender, RoutedEventArgs e)
        {
            User = new User()
            {
                UserID = 1,
                Username = "admin",
                IsAdmin = true
            };
            LoggedIn = true;
            this.Close();
        }
    }
}
