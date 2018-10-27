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
            foreach (var item in allUserResponse.Content)
            {
                lbxUser.Items.Add(item);
            }
        }

        private void btnUserAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtAddUsername.Text))
            {
                MessageBox.Show("Please enter a valid username!","Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                User user = new User()
                {
                    Username = txtAddUsername.Text,
                    Age = UInt32.Parse(txtAddAge.Text),
                    Location = String.Join(",", txtAddCountry.Text, txtAddState.Text, txtAddCity.Text),
                    IsAdmin = false
                };
                lbxUser.Items.Add(user);
                var AddUser = SqlHandler.AddUser(user, txtAddPass.Password);
            }
        }

        private void btnUserRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbxUser.SelectedIndex > -1)
            {
                User user = (User)lbxUser.SelectedItem;
                lbxUser.Items.Remove(lbxUser.SelectedItem);
                var RemoveUser = SqlHandler.RemoveUser(user);
            }
        }

        private void lbxUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxUser.SelectedIndex > -1)
            {
                User user = (User) lbxUser.SelectedItem;
                txtAddUsername.Text = user.Username;
                txtAddAge.Text = user.Age.ToString();
                string[] location = user.Location.Split(',');
                txtAddCountry.Text = location[0];
                txtAddState.Text = location[1];
                txtAddCity.Text = location[2];
            }
        }
    }
}
