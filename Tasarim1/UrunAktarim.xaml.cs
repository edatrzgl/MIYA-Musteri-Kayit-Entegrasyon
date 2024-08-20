using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Xml;
using Flurl.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Documents;
using System.Xml.XPath;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Text;
using WPF_LoginForm;
using Tasarim1;
using System.Reflection;
using System.Windows.Controls.Primitives;


namespace ExcelToPanorama
{
    public partial class UrunAktarim : Window

    {

        private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KolonIsterlerDataUrun.txt");
        private DataTable dataTable;
        public UrunAktarim()
        {
            InitializeComponent();
            VersionRun.Text = GetVersionNumber();//version numarası yazıldı
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)//ekran küçültme
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        public string GetVersionNumber()//version numarasını aldık 
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            SecimEkrani secimEkrani = new SecimEkrani();
            secimEkrani.Show();
            this.Close();
        }

        private void btnBilgileriAktar_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private string NormalizeSpaces(string input)//boşlukları kaldıran fonk
        {
            // Birden fazla ardışık boşluğu tek bir boşluk ile değiştirir
            return System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");
        }
        private string ReplaceTurkishCharacters(string text)
        {
            return text
                .Replace("ı", "i")
                .Replace("İ", "I")
                .Replace("ş", "s")
                .Replace("Ş", "S")
                .Replace("ç", "c")
                .Replace("Ç", "C")
                .Replace("ü", "u")
                .Replace("Ü", "U")
                .Replace("ö", "o")
                .Replace("Ö", "O")
                .Replace("ğ", "g")
                .Replace("Ğ", "G");
        }
        private string RemoveAllSpaces(string input)
        {
            // Tüm boşlukları kaldırır
            return input.Replace(" ", string.Empty);
        }
        private void btnExcelYükle_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Dosyaları|*.xls;*.xlsx;*.xlsm"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string dosyaAdı = openFileDialog.FileName;

                // Bekleme ekranını oluştur ve göster (en başta)
                var beklemeEkrani = new BeklemeEkrani();
                beklemeEkrani.Topmost = true;
                beklemeEkrani.Show();
                Task.Delay(3000);

                Excel.Application excelUygulama = null;
                Excel.Workbook çalışmaKitabı = null;
                Excel.Worksheet çalışmaSayfası = null;

                try
                {
                    // Text dosyasından verileri oku
                    var kolonIsterlerData = File.ReadAllLines("KolonIsterlerDataUrun.txt")
                        .Select(line => line.Split('='))
                        .ToDictionary(parts => parts[0], parts => parts.Length > 1 ? parts[1] : string.Empty);

                    excelUygulama = new Excel.Application();
                    çalışmaKitabı = excelUygulama.Workbooks.Open(dosyaAdı);
                    çalışmaSayfası = çalışmaKitabı.Worksheets[1];

                    dataTable?.Clear();
                    dataTable = new DataTable();

                    int sütunSayısı = çalışmaSayfası.UsedRange.Columns.Count;
                    int satırSayısı = çalışmaSayfası.UsedRange.Rows.Count;

                    // Sütun isimlerini tek seferde al
                    var sütunAdları = new string[sütunSayısı];
                    for (int sütun = 1; sütun <= sütunSayısı; sütun++)
                    {
                        Excel.Range başlıkHücresi = çalışmaSayfası.Cells[1, sütun];
                        string sütunAdı = başlıkHücresi.Value2?.ToString().Replace(" ", "") ?? "";
                        sütunAdı = ReplaceTurkishCharacters(sütunAdı);
                        sütunAdları[sütun - 1] = sütunAdı;
                        dataTable.Columns.Add(sütunAdı);
                    }

                    // Satırları ve hücreleri işleyerek dataTable'ı doldur
                    object[,] hücreVerileri = çalışmaSayfası.UsedRange.Value2;
                    for (int satır = 2; satır <= satırSayısı; satır++)
                    {
                        DataRow yeniSatır = dataTable.NewRow();
                        for (int sütun = 1; sütun <= sütunSayısı; sütun++)
                        {
                            string hücreVerisi = hücreVerileri[satır, sütun]?.ToString() ?? "";

                            if (sütunAdları[sütun - 1] == "Adres")
                            {
                                hücreVerisi = hücreVerisi.Replace("-", "").Replace(".", "");
                                hücreVerisi = NormalizeSpaces(hücreVerisi);
                            }
                            else if (sütunAdları[sütun - 1] == "OdemeTipi")
                            {
                                hücreVerisi = RemoveAllSpaces(hücreVerisi);
                            }
                            else if (sütunAdları[sütun - 1] == "KisaAdi" && hücreVerisi.Length > 30)
                            {
                                hücreVerisi = hücreVerisi.Substring(0, 30);
                                hücreVerileri[satır, sütun] = hücreVerisi; // Değişikliği Excel'e kaydet
                            }

                            yeniSatır[sütun - 1] = hücreVerisi;
                        }
                        dataTable.Rows.Add(yeniSatır);
                    }

                    // Boş hücreleri doldur
                    foreach (DataRow row in dataTable.Rows)
                    {
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            if (string.IsNullOrWhiteSpace(row[column].ToString()) && kolonIsterlerData.TryGetValue(column.ColumnName, out var value))
                            {
                                row[column] = value;
                            }
                        }
                    }

                    çalışmaKitabı.Save();
                    dataGrid.ItemsSource = dataTable.DefaultView;
                    dataGrid.Items.Refresh();
                    foreach (var column in dataGrid.Columns)
                    {
                        if (new[] { "UrunKodu", "UrunAdi", "UrunGrupKodu", "UrunEkGrupKodu", "SeviyeliGrup1", "UreticiKodu", "Birim1", "SatisKDVOrani", "ALISKDVORANI" }
                            .Contains(column.Header.ToString()))
                        {
                            var headerStyle = new Style(typeof(DataGridColumnHeader));
                            headerStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, Brushes.Red));
                            column.HeaderStyle = headerStyle;
                        }
                    }

                    var mesaj1 = new Tasarim1.BildirimMesaji("Excel Dosyası Başarıyla Yüklendi!");
                    mesaj1.Show();
                }
                catch (Exception ex)
                {
                    var mesaj = new Tasarim1.BildirimMesaji($"Bir hata oluştu: {ex.Message}");
                    mesaj.Show();
                }
                finally
                {
                    // Bekleme ekranını kapat
                    beklemeEkrani.Close();

                    if (çalışmaKitabı != null)
                    {
                        çalışmaKitabı.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(çalışmaKitabı);
                    }
                    if (çalışmaSayfası != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(çalışmaSayfası);
                    }
                    if (excelUygulama != null)
                    {
                        excelUygulama.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelUygulama);
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
        }
        private void btnKolonSabitleriniDegistir_Click(object sender, RoutedEventArgs e)
        {
            KolonIsterlerUrun ekran = new KolonIsterlerUrun();
            ekran.Show();
        }
    }
}
