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

        }

        private void btnUserAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtAddUser.Text))
            {
                MessageBox.Show("Please enter a Username");
            }
            else
            {
                lbxUser.Items.Add(txtAddUser.Text);
                lbxUser.Items.Add(Int32.Parse(txtAddAge.Text));
                lbxUser.Items.Add(txtAddCountry.Text);
                lbxUser.Items.Add(txtAddState.Text);
                lbxUser.Items.Add(txtAddCity.Text);
                lbxUser.Items.Add(txtAddPass.Password);
                var adduser = SqlHandler.AddUser(txtAddUser.Text, Int32.Parse(txtAddAge.Text), txtAddCountry.Text, txtAddState.Text, txtAddCity.Text, txtAddPass.Password);
            }
        }

        private void btnUserRemove_Click(object sender, RoutedEventArgs e)
        {
            lbxUser.Items.Remove(lbxUser.SelectedItem);
        }
    }
}
