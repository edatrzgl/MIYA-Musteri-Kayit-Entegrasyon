using System.Windows;

namespace Tasarim1
{
    public partial class BildirimMesaji : Window
    {
        
        public BildirimMesaji()
        {
            InitializeComponent();
        }

        public BildirimMesaji(string mesaj)
        {
            InitializeComponent();
            MessageText.Text = mesaj;
        }

        private void BtnTamam_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
