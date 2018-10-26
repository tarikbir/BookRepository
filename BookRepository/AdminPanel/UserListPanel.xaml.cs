﻿using System;
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
    /// <summary>
    /// UserListPanel.xaml etkileşim mantığı
    /// </summary>
    public partial class UserListPanel : Window
    {
        public UserListPanel()
        {
            InitializeComponent();
        }

        private void AddUserButton(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtAddUser.Text))
            {
                MessageBox.Show("Please enter a Username");
            }
            else
            {
                try
                {
                    ListBoxUser.Items.Add(txtAddUser.Text);
                    ListBoxUser.Items.Add(Int32.Parse(txtAddAge.Text));
                    ListBoxUser.Items.Add(txtAddCountry.Text);
                    ListBoxUser.Items.Add(txtAddState.Text);
                    ListBoxUser.Items.Add(txtAddCity.Text);
                    ListBoxUser.Items.Add(txtAddPass.Password);
                    var adduser = SqlHandler.AddUser(txtAddUser.Text, Int32.Parse(txtAddAge.Text), txtAddCountry.Text, txtAddState.Text, txtAddCity.Text, txtAddPass.Password);
                }
                catch(Exception error)
                {
                    MessageBox.Show("An Error has occured while adding a User", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                }

        }

        private void RemoveUserButton_Click(object sender, RoutedEventArgs e)
        {
            ListBoxUser.Items.Remove(ListBoxUser.SelectedItem);
        }

        private void UserListButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
