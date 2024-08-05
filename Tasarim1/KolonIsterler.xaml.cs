using System.Windows;
using System.Windows.Navigation;

namespace WPF_LoginForm.View
{
    public partial class KolonIsterler : Window
    {
        public KolonIsterler()
        {
            InitializeComponent();
            MainFrame.Navigate(new KolonIsterler());
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
            var loginView = new LoginView
            {
                Durum = txtDurum.Text,
                MusteriKodu = txtMusteriKodu.Text,
                Unvan = txtUnvan.Text,
                IlgiliKisi = txtIlgiliKisi.Text,
                MusteriGrubu = txtMüsteriGrubu.Text,
                MusteriEkGrubu = txtMusteriEkgrup.Text,
                OdemeTipi = txtOdemeTipi.Text,
                KisaAdi = txtKisaAdi.Text,
                VergiTipi = txtVergiTipi.Text
            };

            loginView.Show();
        }
    }
}
