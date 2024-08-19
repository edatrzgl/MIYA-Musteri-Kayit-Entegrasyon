using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tasarim1;

namespace WPF_LoginForm.View
{
    public partial class KolonIsterler : Window
    {
        private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KolonIsterlerData.txt");

        public KolonIsterler()
        {
            InitializeComponent();
            LoadDataFromFile();
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
          var lines = new List<string>
            {
                $"Durum={txtDurum.Text}",
                //$"MusteriKodu={txtMusteriKodu.Text}",
                //$"Unvan={txtUnvan.Text}",
                $"IlgiliKisi={txtIlgiliKisi.Text}",
                $"MusteriGrubu={txtMüsteriGrubu.Text}",
                $"MusteriEkGrubu={txtMusteriEkgrup.Text}",
                $"OdemeTipi={txtOdemeTipi.Text}",
                $"KisaAdi={txtKisaAdi.Text}",
                //$"VergiTipi={txtVergiTipi.Text}"
            };

            try
            {
                // Verileri text dosyasına yaz
                File.WriteAllLines(filePath, lines);

                // İlk mesajı göster
                var mesaj1 = new Tasarim1.BildirimMesaji($"Dosya başarıyla kaydedildi: {filePath}");
                mesaj1.Show();

                // Mesajı belirli bir süre sonra kapat
                await Task.Delay(2000); // 2 saniye bekle
                mesaj1.Close();

                // İkinci mesajı göster
                var mesaj2 = new Tasarim1.BildirimMesaji("Bilgiler kaydediliyor..!");
                mesaj2.Show();

                // Kaydetme işlemi için kısa bir süre bekle
                await Task.Delay(500); // 0.5 saniye bekle
                mesaj2.Close();

                this.Close();
            }
            catch (Exception ex)
            {
                // Hata mesajını göster
                var mesajHata = new Tasarim1.BildirimMesaji($"Bir hata oluştu: {ex.Message}");
                mesajHata.Show();

                // Hata mesajını belirli bir süre sonra kapat
                await Task.Delay(2000); // 2 saniye bekle
                mesajHata.Close();
            }
        }


        private void LoadDataFromFile()
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    var keyValue = line.Split('=');
                    if (keyValue.Length == 2)
                    {
                        var key = keyValue[0].Trim();
                        var value = keyValue[1].Trim();

                        switch (key)
                        {
                            case "Durum":
                                txtDurum.Text = value;
                                break;
                            // case "MusteriKodu":
                            //     txtMusteriKodu.Text = value;
                            //     break;
                            // case "Unvan":
                            //     txtUnvan.Text = value;
                            //     break;
                             case "IlgiliKisi":
                                txtIlgiliKisi.Text = value;
                                break;
                            case "MusteriGrubu":
                                txtMüsteriGrubu.Text = value;
                                break;
                            case "MusteriEkGrubu":
                                txtMusteriEkgrup.Text = value;
                                break;
                            case "OdemeTipi":
                                txtOdemeTipi.Text = value;
                                break;
                            case "KisaAdi":
                                txtKisaAdi.Text = value;
                                break;
                                // case "VergiTipi":
                                //     txtVergiTipi.Text = value;
                                //     break;
                        }
                    }
                }
            }
        }
    }
}
