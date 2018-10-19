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
        public Response.LoginEntryResponse login;

        public LoginWindow()
        {
            InitializeComponent();
            txtUser.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            login = SqlHandler.UserEntry(txtUser.Text, txtPass.Password);
            if(login.Success)
            {
                MessageBox.Show("You have successfully logged in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("An error occured.\n\n" + login.ErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            CreateAccountWindow createAccountWindow = new CreateAccountWindow();
            createAccountWindow.ShowDialog();
        }

        private void btnRecover_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
