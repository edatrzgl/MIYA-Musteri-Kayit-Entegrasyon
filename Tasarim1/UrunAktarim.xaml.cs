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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExcelToPanorama
{
    public partial class UrunAktarim : Window
    {
        public UrunAktarim()
        {
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }


        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBilgileriAktar_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void btnExcelYükle_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnKolonSabitleriniDegistir_Click(object sender, RoutedEventArgs e)
        { 

        }
    }
}
