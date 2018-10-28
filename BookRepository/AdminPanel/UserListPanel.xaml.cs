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

namespace BookRepository.AdminPanel
{
    public partial class UserListPanel : Window
    {
        public UserListPanel()
        {
            InitializeComponent();
        }

        private void btnUserList_Click(object sender, RoutedEventArgs e)
        {
            var allUserResponse = SqlHandler.GetAllUsers();
            lbxUser.Items.Clear();
            Dispatcher.Invoke(() =>
            {
                foreach (var item in allUserResponse.Content)
                {
                    lbxUser.Items.Add(item);
                }
            });
        }

        private void btnUserAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtAddUsername.Text))
            {
                MessageBox.Show("Please enter a valid username!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                User user = new User()
                {
                    Username = txtAddUsername.Text,
                    Age = UInt32.Parse(txtAddAge.Text),
                    Location = String.Join(",", txtAddCountry.Text, txtAddState.Text, txtAddCity.Text),
                    IsAdmin = chkIsAdmin.IsChecked ?? false
                };
                var addUserResponse = SqlHandler.AddUser(user, txtAddPass.Password);
                if (addUserResponse.Success)
                {
                    MessageBox.Show("Successfully added " + user.Username + ".", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    lbxUser.Items.Add(user);
                }
                else
                    MessageBox.Show("Error adding " + user.Username + ".\n\n" + addUserResponse.ErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUserRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbxUser.SelectedIndex > -1)
            {
                User user = (User)lbxUser.SelectedItem;
                if (user.UserID == CommonLibrary.LoggedInUser.UserID)
                {
                    MessageBox.Show("You can't remove yourself from the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }
                var removeUserResponse = SqlHandler.RemoveUser(user);
                if (removeUserResponse.Success)
                {
                    MessageBox.Show("Successfully removed " + user.Username + ".", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    lbxUser.Items.Remove(lbxUser.SelectedItem);
                }
                else
                    MessageBox.Show("Error removing " + user.Username + ".\n\n" + removeUserResponse.ErrorText, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lbxUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxUser.SelectedIndex > -1)
            {
                User user = (User)lbxUser.SelectedItem;
                txtAddUsername.Text = user.Username;
                txtAddAge.Text = user.Age.ToString();
                string[] location = user.Location.Split(',');
                txtAddCountry.Text = location[0];
                txtAddState.Text = location[1];
                txtAddCity.Text = location[2];
                chkIsAdmin.IsChecked = user.IsAdmin;
            }
        }
    }
}
