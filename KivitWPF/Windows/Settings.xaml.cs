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

namespace PriceListCash.Windows
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        Properties.Settings App = new Properties.Settings();
        public Settings()
        {
            InitializeComponent();
        }

        private void cbxNamePriceList_Loaded(object sender, RoutedEventArgs e)
        {
            txtNamePrinter.Text = App.NamePrinter;
            cbxNamePriceList.SelectedIndex = App.ВидЦенника;
            txtNamePrinter.IsEnabled = false;
            cbxNamePriceList.IsEnabled = false;
            btnSave.IsEnabled = false;
        }

        private void checkActiveChange_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)checkActiveChange.IsChecked)
            {
                txtNamePrinter.IsEnabled = true;
                cbxNamePriceList.IsEnabled = true;
                btnSave.IsEnabled = true;
            }
        }

        private void checkActiveChange_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)checkActiveChange.IsChecked)
            {
                txtNamePrinter.IsEnabled = false;
                cbxNamePriceList.IsEnabled = false;
                btnSave.IsEnabled = false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)checkActiveChange.IsChecked)
            {
                App.NamePrinter = txtNamePrinter.Text;
                App.ВидЦенника = (byte)cbxNamePriceList.SelectedIndex;
                App.Save();
                Close();
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
