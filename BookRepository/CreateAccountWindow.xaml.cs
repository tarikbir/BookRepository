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
        public CreateAccountWindow()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var register = SqlHandler.Register(txtUser.Text,Int32.Parse(txtAge.Text),txtCountry.Text,txtState.Text,txtCity.Text,txtPass.Password,chkIsAdmin.IsChecked ?? false);
            if (register.Success)
            {
                MessageBox.Show("User has been added successfully","Success",MessageBoxButton.OK,MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Oops! A problem has occured!\n\n"+register.ErrorText,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private void txtAge_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void txtCountry_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^A-z]+").IsMatch(e.Text);
        }

        private void txtState_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^A-z]+").IsMatch(e.Text);
        }

        private void txtCity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^A-z]+").IsMatch(e.Text);
        }
    }
}
