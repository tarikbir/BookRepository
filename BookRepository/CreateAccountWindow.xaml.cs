using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class CreateAccountWindow : Window
    {
        List<Vote> userVotes;

        public CreateAccountWindow()
        {
            InitializeComponent();
            userVotes = new List<Vote>();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtUser.Text) || String.IsNullOrWhiteSpace(txtState.Text) || String.IsNullOrWhiteSpace(txtCountry.Text)
                || String.IsNullOrWhiteSpace(txtCity.Text) || String.IsNullOrWhiteSpace(txtAge.Text) || String.IsNullOrWhiteSpace(txtPass.Password))
            {
                MessageBox.Show("Please enter valid info.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            User user = new User()
            {
                Username = txtUser.Text,
                Age = UInt32.Parse(txtAge.Text),
                Location = String.Join(",", txtCity.Text, txtState.Text, txtCountry.Text),
                IsAdmin = chkIsAdmin.IsChecked ?? false
            };

            Response.BaseResponse success = new Response.BaseResponse() { Success = true };

            FullBookVoteWindow fullBookVoteWindow = new FullBookVoteWindow(userVotes);
            if (userVotes.Count < 10)
            {
                fullBookVoteWindow.ShowDialog();
            }

            if (fullBookVoteWindow.DialogResult == true || userVotes.Count >= 10)
            {
                var register = SqlHandler.AddUser(user, txtPass.Password);
                if (register.Success)
                {
                    foreach (var item in userVotes)
                    {
                        SqlHandler.AddVote(register.Content.ToString(), item.Book.ISBN, item.Rating);
                    }
                }
                else
                {
                    success.Success = false;
                    success.ErrorText = register.ErrorText;
                }
            }
            else
            {
                success.Success = false;
                success.ErrorText = "Voting cancelled by user.";
            }

            if (success.Success)
            {
                MessageBox.Show("User has been added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("There was an error while registering!\n\n" + success.ErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtAge_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex(@"[^0-9]+").IsMatch(e.Text);
        }

        private void txtCountry_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex(@"[^\p{L}]+").IsMatch(e.Text);
        }

        private void txtState_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex(@"[^\p{L}]+").IsMatch(e.Text);
        }

        private void txtCity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex(@"[^\p{L}]+").IsMatch(e.Text);
        }
    }
}
