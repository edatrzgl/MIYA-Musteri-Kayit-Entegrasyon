using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
using Tasarim1;
using WPF_LoginForm.View;

namespace ExcelToPanorama
{
    public partial class SecimEkrani : System.Windows.Window
    {
        public SecimEkrani()
        {
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void BtnMusteri_Click(object sender, RoutedEventArgs e)
        {
            LoginView ekran=new LoginView();
            ekran.Show(); 
        }
        private void BtnUrun_Click(object sender,RoutedEventArgs e)
        {
            var mesaj = new BildirimMesaji("");
            mesaj.Show();
        }
    }
}
