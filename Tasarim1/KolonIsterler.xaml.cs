using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Tasarim1;

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
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KolonIsterlerData.txt");
            var lines = new List<string>
    {
        $"Durum={txtDurum.Text}",
        //$"MusteriKodu={txtMusteriKodu.Text}",
        //$"Unvan={txtUnvan.Text}",
        //$"IlgiliKisi={txtIlgiliKisi.Text}",
        $"MusteriGrubu={txtMüsteriGrubu.Text}",
        $"MusteriEkGrubu={txtMusteriEkgrup.Text}",
        $"OdemeTipi={txtOdemeTipi.Text}",
        $"KisaAdi={txtKisaAdi.Text}",
        //$"VergiTipi={txtVergiTipi.Text}"
    };

            try
            {
                // Write data to the text file
                File.WriteAllLines(filePath, lines);

                // Log or debug the file path to ensure it's correct
                //MessageBox.Show($"Dosya başarıyla kaydedildi: {filePath}", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                var mesaj1 = new Tasarim1.BildirimMesaji($"Dosya başarıyla kaydedildi: {filePath}");
                mesaj1.Show();



                var mesaj = new Tasarim1.BildirimMesaji("Bilgiler kaydediliyor..!");
                mesaj.Show();
                await Task.Delay(500); // Wait for a short period to simulate saving
                mesaj.Close();
                this.Close();
            }
            catch (Exception ex)
            {
                // MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                var mesaj = new Tasarim1.BildirimMesaji($"Bir hata oluştu: {ex.Message}");
                mesaj.Show();
            }
        }

    }
}
