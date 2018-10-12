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

namespace BookRepository
{
    public partial class LoginWindow : Window, IDisposable
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //sql check
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            CreateAccountWindow createAccountWindow = new CreateAccountWindow();
            createAccountWindow.ShowDialog();
        }

        private void btnRecover_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Yok ki.", "Yoo", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void Dispose()
        {
            ((MainWindow)Owner).Close();
        }
    }
}
