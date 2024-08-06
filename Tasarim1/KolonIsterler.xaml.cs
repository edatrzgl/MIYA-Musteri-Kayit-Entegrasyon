using System.Threading.Tasks;
using System.Windows;
using Tasarim1;
using WPF_LoginForm; // KolonIsterlerData sınıfını içerir

namespace WPF_LoginForm.View
{
    public partial class KolonIsterler : Window
    {
        public KolonIsterler()
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

        private async void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
            KolonIsterlerData.Durum = txtDurum.Text;
            KolonIsterlerData.MusteriKodu = txtMusteriKodu.Text;
            KolonIsterlerData.Unvan = txtUnvan.Text;
            KolonIsterlerData.IlgiliKisi = txtIlgiliKisi.Text;
            KolonIsterlerData.MusteriGrubu = txtMüsteriGrubu.Text;
            KolonIsterlerData.MusteriEkGrubu = txtMusteriEkgrup.Text;
            KolonIsterlerData.OdemeTipi = txtOdemeTipi.Text;
            KolonIsterlerData.KisaAdi = txtKisaAdi.Text;
            KolonIsterlerData.VergiTipi = txtVergiTipi.Text;

            var mesaj = new Tasarim1.BildirimMesaji("Bilgiler kaydediliyor..!");
            mesaj.Show();
            await Task.Delay(500);
            this.Close();
        }
    }
}
