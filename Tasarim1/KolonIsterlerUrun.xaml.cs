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
using System.IO;


namespace ExcelToPanorama
{
  
    public partial class KolonIsterlerUrun : Window
    {
        
        private readonly string excelFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KolonIsterlerDataUrun.txt");

        public KolonIsterlerUrun()
        {
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }


        private void btnKapat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
        
        }
    }
}
