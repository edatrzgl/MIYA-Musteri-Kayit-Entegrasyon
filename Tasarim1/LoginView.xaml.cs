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

namespace WPF_LoginForm.View
{

    public partial class LoginView : Window
    {
      

        
        public string VersionNumber { get; set; }




        private CancellationTokenSource cancellationTokenSource;

        private DataTable dataTable;



        public LoginView()
        {
            InitializeComponent();
            VersionRun.Text = GetVersionNumber();//version numarası yazıldı

        }


        private List<DataRow> GetCheckedRows()
        {
            var checkedRows = new List<DataRow>();

            // DataGrid içerisindeki tüm satırlara erişin
            foreach (var item in dataGrid.Items)
            {
                var row = item as DataRowView;
                if (row != null)
                {
                    // CheckBox'ın işaretli olup olmadığını kontrol edin
                    var cell = GetDataGridCell(dataGrid, row);
                    if (cell != null)
                    {
                        var checkBox = GetVisualChild<CheckBox>(cell);
                        if (checkBox != null && checkBox.IsChecked == true)
                        {
                            checkedRows.Add(row.Row);
                        }
                    }
                }
            }

            return checkedRows;
        }
        public string GetVersionNumber()//version numarasını aldık 
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        // Helper method to get DataGridCell from DataGrid and DataRowView
        private DataGridCell GetDataGridCell(DataGrid dataGrid, DataRowView row)
        {
            var container = dataGrid.ItemContainerGenerator.ContainerFromItem(row) as DataGridRow;
            if (container != null)
            {
                var column = dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Seç");
                if (column != null)
                {
                    var cellContent = column.GetCellContent(container);
                    return GetDataGridCell(cellContent);
                }
            }
            return null;
        }

        // Helper method to get VisualChild of a given type
        private T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    foundChild = (T)child;
                    break;
                }
                else
                {
                    foundChild = GetVisualChild<T>(child);
                    if (foundChild != null)
                    {
                        break;
                    }
                }
            }

            return foundChild;
        }

        // Helper method to get DataGridCell from cell content
        private DataGridCell GetDataGridCell(FrameworkElement cellContent)
        {
            var parent = VisualTreeHelper.GetParent(cellContent);

            while (parent != null && !(parent is DataGridCell))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as DataGridCell;
        }
        
        private void btnKolonSabitleriniDegistir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // KolonIsterler penceresinin zaten açık olup olmadığını kontrol et
                var existingWindow = Application.Current.Windows.OfType<KolonIsterler>().FirstOrDefault();
                if (existingWindow != null)
                {
                    // Pencere zaten açık, hata mesajı göster
                    var mesaj = new Tasarim1.BildirimMesaji("Pencere zaten açık.");
                    mesaj.Show();
                }
                else
                {
                    // Pencere açık değil, yeni bir pencere oluştur ve göster
                    KolonIsterler ekran = new KolonIsterler();
                    ekran.Show();
                }
            }
            catch (Exception ex)
            {
                var mesaj = new Tasarim1.BildirimMesaji($"Bilinmeyen bir hata oluştu: {ex.Message}");
                mesaj.Show();
            }
        }

        // Tüm satırları seç
        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (dataTable != null)
            {
                if (!dataTable.Columns.Contains("Seç"))
                {
                    dataTable.Columns.Add("Seç", typeof(bool));
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    row["Seç"] = true; // Seç kolonundaki tüm değerleri true yap
                }

                // DataGrid'in güncellenmesini sağlamak için
                dataGrid.ItemsSource = dataTable.DefaultView;
                dataGrid.Items.Refresh(); // DataGrid'i yenile
            }
        }

        // Tüm seçimleri kaldır
        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (dataTable != null)
            {
                if (!dataTable.Columns.Contains("Seç"))
                {
                    dataTable.Columns.Add("Seç", typeof(bool));
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    row["Seç"] = false; // Seç kolonundaki tüm değerleri false yap
                }

                // DataGrid'in güncellenmesini sağlamak için
                dataGrid.ItemsSource = dataTable.DefaultView;
                dataGrid.Items.Refresh(); // DataGrid'i yenile
            }
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

        private async void btnExcelYükle_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Dosyaları|*.xls;*.xlsx;*.xlsm"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string dosyaAdı = openFileDialog.FileName;

                Excel.Application excelUygulama = null;
                Excel.Workbook çalışmaKitabı = null;
                Excel.Worksheet çalışmaSayfası = null;

                // Bekleme ekranını oluştur ve göster
                var beklemeEkrani = new BeklemeEkrani();
                beklemeEkrani.Show();

                try
                {
                    // Read values from text file
                    var kolonIsterlerData = File.ReadAllLines("KolonIsterlerData.txt")
                        .Select(line => line.Split('='))
                        .ToDictionary(parts => parts[0], parts => parts.Length > 1 ? parts[1] : string.Empty);

                    excelUygulama = new Excel.Application();
                    çalışmaKitabı = excelUygulama.Workbooks.Open(dosyaAdı);
                    çalışmaSayfası = çalışmaKitabı.Worksheets[1];

                    dataTable?.Clear();
                    dataTable = new DataTable();

                    await Task.Delay(3000); // Önceki mesajı gösterme süresi
                    int sütunSayısı = çalışmaSayfası.UsedRange.Columns.Count;
                    for (int sütun = 1; sütun <= sütunSayısı; sütun++)
                    {
                        Excel.Range başlıkHücresi = çalışmaSayfası.Cells[1, sütun];
                        string sütunAdı = başlıkHücresi.Value2?.ToString().Replace(" ", "") ?? "";
                        sütunAdı = ReplaceTurkishCharacters(sütunAdı);
                        dataTable.Columns.Add(sütunAdı);
                    }

                    int satırSayısı = çalışmaSayfası.UsedRange.Rows.Count;
                    for (int satır = 2; satır <= satırSayısı; satır++)
                    {
                        DataRow yeniSatır = dataTable.NewRow();
                        for (int sütun = 1; sütun <= sütunSayısı; sütun++)
                        {
                            Excel.Range hücre = çalışmaSayfası.Cells[satır, sütun];
                            string cellValue = hücre.Value2?.ToString() ?? "";

                            if (dataTable.Columns[sütun - 1].ColumnName == "Adres")
                            {
                                cellValue = cellValue.Replace("-", "").Replace(".", "");
                                cellValue = NormalizeSpaces(cellValue);
                            }
                            else if (dataTable.Columns[sütun - 1].ColumnName == "OdemeTipi")
                            {
                                cellValue = RemoveAllSpaces(cellValue);
                            }

                            if (dataTable.Columns[sütun - 1].ColumnName == "KisaAdi" && cellValue.Length > 30)
                            {
                                cellValue = cellValue.Substring(0, 30);
                                hücre.Value2 = cellValue;
                            }

                            yeniSatır[sütun - 1] = cellValue;
                        }
                        dataTable.Rows.Add(yeniSatır);
                    }

                    // Fill empty cells with values from KolonIsterlerData
                    foreach (DataRow row in dataTable.Rows)
                    {
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            if (string.IsNullOrWhiteSpace(row[column].ToString()))
                            {
                                if (kolonIsterlerData.TryGetValue(column.ColumnName, out var value))
                                {
                                    row[column] = value;
                                }
                            }
                        }
                    }

                    çalışmaKitabı.Save();
                    dataGrid.ItemsSource = dataTable.DefaultView;
                    dataGrid.Items.Refresh();

                    // Apply styles to DataGrid columns
                    foreach (var column in dataGrid.Columns)
                    {
                        if (column.Header.ToString() == "DURUM" || column.Header.ToString() == "MusteriKodu" || column.Header.ToString() == "Unvan" || column.Header.ToString() == "IlgiliKisi" || column.Header.ToString() == "MusteriGrubu" || column.Header.ToString() == "MusteriEkGrubu" || column.Header.ToString() == "OdemeTipi" || column.Header.ToString() == "KisaAdi" || column.Header.ToString() == "VergiTipi")
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


        // Boşlukları normalleştiren yardımcı yöntem
        private string NormalizeSpaces(string input)
        {
            // Birden fazla ardışık boşluğu tek bir boşluk ile değiştirir
            return System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");
        }

        // Tüm boşlukları kaldıran yardımcı yöntem
        private string RemoveAllSpaces(string input)
        {
            // Tüm boşlukları kaldırır
            return input.Replace(" ", string.Empty);
        }


        // Tek harfli boşlukları kaldıran yardımcı yöntem
        private string RemoveSingleCharacterSpaces(string input)
        {
            // Tek harfli boşlukları kaldırmak için regex kullanabiliriz
            return System.Text.RegularExpressions.Regex.Replace(input, @"(?<=\S) (?=\S)", "");
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btnBilgileriAktar_Click(object sender, RoutedEventArgs e)
        {
            string panServisLinki = txtLink.Text;
            string panServisSifresi = txtSifre.Text;
            string dist = txtDist.Text;
            string firmaKodu = txtFirmaKodu.Text;
            string calismaYili = txtCalismaYili.Text;
            string UserName = txtKullaniciTipi.Text;

            if (dataTable == null)
            {
                var mesaj = new Tasarim1.BildirimMesaji("Lütfen Bir Excel Dosyası Yükleyin!");
                mesaj.Show();
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                CheckInvalidCharactersInExcel();

                if (!CheckRequiredColumns(dataTable))
                {
                    return;
                }

                // İşaretli CheckBox'lara sahip satırları al
                var rowsToProcess = GetCheckedRows();

                if (rowsToProcess.Count == 0)
                {
                    var mesaj = new Tasarim1.BildirimMesaji("Lütfen Gönderilecek Satırları Seçin!");
                    mesaj.Show();
                    return;
                }

                rtbErrorMessages.Document.Blocks.Clear(); // Önceki hata mesajlarını temizle

                foreach (var row in rowsToProcess)
                {
                    try
                    {
                        // CancellationToken'ın iptal edilip edilmediğini kontrol edin
                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        // Tüm hücrelerin boş olup olmadığını kontrol edin
                        bool allCellsEmpty = true;
                        foreach (var cell in row.ItemArray)
                        {
                            if (!string.IsNullOrEmpty(cell?.ToString()))
                            {
                                allCellsEmpty = false;
                                break;
                            }
                        }

                        if (allCellsEmpty)
                        {
                            var mesaj = new Tasarim1.BildirimMesaji("Seçili satırda tüm hücreler boş. Veri aktarımı durduruluyor.");
                            mesaj.Show();
                            return; // Veri aktarımını durdur
                        }

                        // Hücrelerin arka plan rengini temizleyin
                        ClearRowCellBackground(row);

                        var customers = new List<CustomerIntegration> { MapRowToCustomer(row) };
                        string xmlData = ConvertCustomersToXML(customers, UserName, panServisSifresi, firmaKodu, calismaYili, dist);

                        var response = await panServisLinki
                            .WithHeader("Authorization", $"Bearer {panServisSifresi}")
                            .WithHeader("Content-Type", "text/xml")
                            .PostStringAsync(xmlData);

                        string responseString = await response.GetStringAsync();
                        string errorMessage = ParseErrorMessageFromResponse(responseString);

                        // `MusteriKodu` değerini satırdan al
                        string musteriKodu = row["MusteriKodu"].ToString();

                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            HighlightInvalidCells(row, Colors.LightCoral);
                            AppendErrorMessage($"Hata: {errorMessage}", musteriKodu);
                        }
                        else
                        {
                            HighlightSuccessfulCells(row, Colors.LightGreen);
                            AppendErrorMessage("Başarılı bir şekilde aktarım gerçekleşti", musteriKodu);
                        }
                    }
                    catch (FlurlHttpException ex)
                    {
                        string errorResponse = await ex.GetResponseStringAsync();
                        string errorMessage = ParseErrorMessage(errorResponse);
                        string musteriKodu = row["MusteriKodu"].ToString();
                        HighlightInvalidCells(row, Colors.LightCoral);
                        AppendErrorMessage($"Hata: {ex.Message}\nYanıt: {errorMessage}", musteriKodu);
                    }
                    catch (System.Security.SecurityException ex)
                    {
                        var mesaj = new Tasarim1.BildirimMesaji("Gerekli izinlere sahip olmadığınız için işlemi tamamlayamadık. Lütfen uygulamayı yönetici olarak çalıştırmayı deneyin.");
                        mesaj.Show();
                        return;
                    }
                    catch (Exception ex)
                    {
                        string musteriKodu = row["MusteriKodu"].ToString();
                        HighlightInvalidCells(row, Colors.LightCoral);
                        AppendErrorMessage($"Hata: {ex.Message}", musteriKodu);
                    }

                    // Sunucuyu aşırı yüklememek için bekleme süresi
                    await Task.Delay(1000); // Gerekirse bekleme süresini ayarlayın
                }
            }
            catch (OperationCanceledException)
            {
                var mesaj = new Tasarim1.BildirimMesaji("Aktarım durduruldu.");
                mesaj.Show();
            }
            catch (Exception ex)
            {
                // Genel hataları işleme
                AppendErrorMessage($"İstek gönderilirken bir hata oluştu: {ex.Message}", "");
            }
        }







        private void SetAllCheckBoxes(bool isChecked)
        {
            // DataGrid'in Items koleksiyonunda gezinin
            foreach (var item in dataGrid.Items)
            {
                // DataGrid'in öğelerini DataRowView olarak tip değiştirin
                if (item is DataRowView rowView)
                {
                    // Seçim CheckBox'ını bulup işaretleyin veya temizleyin
                    var checkBox = GetCheckBoxForRow(rowView.Row);
                    if (checkBox != null)
                    {
                        checkBox.IsChecked = isChecked;
                    }
                }
            }
        }

        private CheckBox GetCheckBoxForRow(DataRow row)
        {
            int rowIndex = dataTable.Rows.IndexOf(row);

            if (rowIndex < 0 || rowIndex >= dataGrid.Items.Count)
                return null;

            var rowContainer = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;

            if (rowContainer == null)
            {
                // Eğer satır henüz oluşturulmadıysa, zorunlu olarak oluşturulmasını sağlar
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                rowContainer = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }

            if (rowContainer != null)
            {
                var cellContent = dataGrid.Columns[0].GetCellContent(rowContainer);
                var checkBox = cellContent as CheckBox;

                return checkBox;
            }

            return null;
        }




        private void ClearRowCellBackground(DataRow row)
        {
            int rowIndex = dataTable.Rows.IndexOf(row);

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                var cell = dataGrid.Columns[i].GetCellContent(dataGrid.Items[rowIndex]);
                if (cell != null)
                {
                    var dataGridCell = GetDataGridCell(cell);
                    if (dataGridCell != null)
                    {
                        dataGridCell.Background = Brushes.White; // Varsayılan arka plan rengi
                    }
                }
            }
        }
  
        //AKTARILAN HÜCRELERİ BOYAMA
        private void HighlightInvalidCells(DataRow row, Color color)
        {
            int rowIndex = dataTable.Rows.IndexOf(row);

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                var cell = dataGrid.Columns[i].GetCellContent(dataGrid.Items[rowIndex]);
                if (cell != null)
                {
                    var dataGridCell = GetDataGridCell(cell);
                    if (dataGridCell != null)
                    {
                        dataGridCell.Background = new SolidColorBrush(color);
                    }
                }
            }
        }



        private void HighlightSuccessfulCells(DataRow row, Color color)
        {
            int rowIndex = dataTable.Rows.IndexOf(row);

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                var cell = dataGrid.Columns[i].GetCellContent(dataGrid.Items[rowIndex]);
                if (cell != null)
                {
                    var dataGridCell = GetDataGridCell(cell);
                    if (dataGridCell != null)
                    {
                        dataGridCell.Background = new SolidColorBrush(color);
                    }
                }
            }
        }

        private void AppendErrorMessage(string message, string MusteriKodu)
        {
            string fullMessage = $"MusteriKodu: {MusteriKodu} - {message}";
            Paragraph paragraph = new Paragraph(new Run(fullMessage));
            rtbErrorMessages.Document.Blocks.Add(paragraph);
            rtbErrorMessages.ScrollToEnd();
        }




        private string ParseErrorMessage(string response)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);
            var errorNode = xmlDoc.SelectSingleNode("//error");
            return errorNode?.InnerText ?? "Bilinmeyen bir hata oluştu.";
        }



        private (bool hasExceptions, string exceptionMessages) ParseResponseForExceptions(string response)
        {
            var exceptionMessages = new List<string>();

            var startIndex = 0;
            while ((startIndex = response.IndexOf("@Message       :", startIndex)) != -1)
            {
                startIndex += "@Message       :".Length;
                var endIndex = response.IndexOf("@", startIndex);
                if (endIndex == -1) endIndex = response.Length;

                var message = response.Substring(startIndex, endIndex - startIndex).Trim();
                exceptionMessages.Add(message);

                startIndex = endIndex;
            }

            return (exceptionMessages.Count > 0, string.Join("\n", exceptionMessages));
        }


        private string ParseErrorMessageFromResponse(string responseString)
        {
            try
            {
                var xDoc = XDocument.Parse(responseString);
                var errorElements = xDoc.Descendants().Where(e => e.Name.LocalName == "Hata");
                List<string> errorMessages = new List<string>();
                foreach (var errorElement in errorElements)
                {
                    errorMessages.Add(errorElement.Value);
                }
                return string.Join("\n", errorMessages);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during XML parsing
                return $"XML Yanıtı çözümleme hatası: {ex.Message}";
            }
        }

        private void CheckInvalidCharactersInExcel()
        {
            if (dataTable == null) return;

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (row[col] != null && row[col] != DBNull.Value)
                    {
                        string cellValue = row[col].ToString();
                        bool containsInvalidChars = ContainsInvalidXmlChars(cellValue);

                        if (containsInvalidChars)
                        {
                            Console.WriteLine($"Geçersiz karakter içeren hücre: [{col.ColumnName}] - {cellValue}");
                        }
                    }
                }
            }
        }




        private bool ContainsInvalidXmlChars(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            string pattern = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.IsMatch(text, pattern);
        }

        private bool CheckRequiredColumns(DataTable dt)
        {
            List<string> missingColumns = new List<string>();

            foreach (RequiredColumns col in Enum.GetValues(typeof(RequiredColumns)))
            {
                if (!dt.Columns.Contains(col.ToString()))
                {
                    missingColumns.Add(col.ToString());
                }
            }

            if (missingColumns.Count > 0)
            {
                // Mevcut sütunlar eksik olduğunda gösterilecek mesaj
                string errorMessage = "Gerekli sütunlar eksik: " + string.Join(", ", missingColumns);
                // MessageBox.Show(errorMessage, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);

                // Bildirim mesajı oluşturuluyor
                var notificationMessage = new Tasarim1.BildirimMesaji(errorMessage);
                notificationMessage.Show();

                return false;

            }

            return true;
        }

        private CustomerIntegration MapRowToCustomer(DataRow row)
        {
            var vergiTip = ReplaceTurkishCharacters(RemoveAllSpaces(row["VergiTipi"].ToString()));


            // VergiTipi değerini dönüştürün


            // OdemeTipi değerini dönüştürün
            Enum.TryParse(row["OdemeTipi"].ToString(), true, out OdemeTipiEnum odemeTipi);
            var returned = new CustomerIntegration
            {
                Durum = Enum.TryParse(row["Durum"].ToString(), true, out DurumEnum durum) ? (int?)durum : (int?)null,
                ErpKod2 = row["MusteriKodu"].ToString(),
                Unvan = row["Unvan"].ToString(),
                IlgiliKisi = row["IlgiliKisi"].ToString(),
                Adres1 = row["Adres"].ToString().Replace("-", string.Empty),
                MerkezIlTextKod = row["Sehir"].ToString(),
                Ilce = row["Ilce"].ToString(),
                TCKimlikNo = row["TcNo"].ToString(),
                CepTelNo = row["Telefon"].ToString(),
                VD = row["VergiDairesi"].ToString(),
                VN = row["VergiNumarasi"].ToString(),
                MusteriGrupTextKod = row["MusteriGrubu"].ToString(),
                MusteriEkGrupTextKod = row["MusteriEkGrubu"].ToString(),
                OdemeTipi = Enum.TryParse(row["OdemeTipi"].ToString(), true, out OdemeTipiEnum odemeTipiEnum) ? (int?)odemeTipiEnum : (int?)null,
                KisaAd = row["KisaAdi"].ToString(),
                KdvMuaf = Enum.TryParse(vergiTip, true, out VergiTipiEnum vergiTipiEnum) ? (int?)vergiTipiEnum : (int?)null,
                KoordinatX = (row["KoordinatX"] != DBNull.Value && row["KoordinatX"].ToString() != "") ? Convert.ToDecimal(row["KoordinatX"]) : (decimal?)null,
                KoordinatY = (row["KoordinatY"] != DBNull.Value && row["KoordinatY"].ToString() != "") ? Convert.ToDecimal(row["KoordinatY"]) : (decimal?)null,
                VadeGun = row["VadeGunu"] != DBNull.Value ? Convert.ToInt32(row["VadeGunu"]) : (int?)null,
                IskontoOran = row["Iskonto"] != DBNull.Value ? Convert.ToDecimal(row["Iskonto"]) : (decimal?)null
            };
            return returned;
        }


        private string ConvertCustomersToXML(List<CustomerIntegration> customers, string UserName, string panServisSifresi, string firmaKodu, string calismaYili, string dist)
        {
            if (customers == null || customers.Count == 0)
                throw new InvalidOperationException("Customer list is empty or invalid.");

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
                    xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                    xmlWriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
                    xmlWriter.WriteStartElement("soap", "Body", null);

                    xmlWriter.WriteStartElement("IntegrationSendEntitySetWithLogin", "http://integration.univera.com.tr");

                    xmlWriter.WriteElementString("strUserName", UserName);
                    xmlWriter.WriteElementString("strPassWord", panServisSifresi);
                    xmlWriter.WriteElementString("bytFirmaKod", firmaKodu);
                    xmlWriter.WriteElementString("lngCalismaYili", calismaYili);
                    xmlWriter.WriteElementString("lngDistributorKod", dist);

                    xmlWriter.WriteStartElement("objPanIntEntityList"); // Start objPanIntEntityList

                    xmlWriter.WriteStartElement("Musteriler");

                    foreach (var customer in customers)
                    {
                        xmlWriter.WriteStartElement("clsMusteriIntegration");
                        // xmlWriter.WriteElementString("GrupKod", "99");
                        // xmlWriter.WriteElementString("EkGrupKod", "99");
                        xmlWriter.WriteElementString("Referans", $"{dist}-{customer.ErpKod2}");
                        xmlWriter.WriteElementString("DistKod", dist);

                        foreach (var prop in customer.GetType().GetProperties())
                        {
                            var value = prop.GetValue(customer);

                            if (value == null)
                            {
                                if (prop.PropertyType == typeof(decimal?) || prop.PropertyType == typeof(decimal))
                                {
                                    xmlWriter.WriteElementString(prop.Name, "0");
                                }
                                else if (prop.PropertyType == typeof(int?) || prop.PropertyType == typeof(int))
                                {
                                    xmlWriter.WriteElementString(prop.Name, "0");
                                }
                                else
                                {
                                    xmlWriter.WriteElementString(prop.Name, string.Empty);
                                }
                            }
                            else
                            {
                                string stringValue = value.ToString();

                                if (prop.PropertyType == typeof(decimal?) || prop.PropertyType == typeof(decimal))
                                {
                                    stringValue = ((decimal?)value).GetValueOrDefault().ToString("G", CultureInfo.InvariantCulture);
                                }
                                else if (prop.PropertyType == typeof(int?) || prop.PropertyType == typeof(int))
                                {
                                    stringValue = ((int?)value).GetValueOrDefault().ToString();
                                }

                                xmlWriter.WriteElementString(prop.Name, stringValue);
                            }
                        }

                        xmlWriter.WriteEndElement(); // Close clsMusteriIntegration
                    }

                    xmlWriter.WriteEndElement(); // Close Musteriler

                    xmlWriter.WriteElementString("SatirBazliTransaction", "true");
                    xmlWriter.WriteElementString("LogKategori", "0");

                    xmlWriter.WriteStartElement("IntegrationGorevSonucTip");
                    xmlWriter.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
                    xmlWriter.WriteEndElement(); // Close IntegrationGorevSonucTip

                    xmlWriter.WriteElementString("SCCall", "false");
                    xmlWriter.WriteElementString("ReturnLoglist", "true");

                    xmlWriter.WriteEndElement(); // Close objPanIntEntityList
                    xmlWriter.WriteEndElement(); // Close IntegrationSendEntitySetWithLogin
                    xmlWriter.WriteEndElement(); // Close soap:Body
                    xmlWriter.WriteEndElement(); // Close soap:Envelope

                    xmlWriter.WriteEndDocument();
                }

                // UTF-8 encoding
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }


    }

    public enum RequiredColumns
    {
        Durum,
        MusteriKodu,
        Unvan,
        IlgiliKisi,
        MusteriGrubu,
        MusteriEkGrubu,
        OdemeTipi,
        KisaAdi,
        VergiTipi
    }

    public class CustomerIntegration
    {
        public int? Durum { get; set; }
        public string ErpKod2 { get; set; }
        public string Unvan { get; set; }
        public string IlgiliKisi { get; set; }
        public string Adres1 { get; set; }
        public string MerkezIlTextKod { get; set; }
        public string Ilce { get; set; }
        public string TCKimlikNo { get; set; }
        public string CepTelNo { get; set; }
        public string VD { get; set; }
        public string VN { get; set; }
        public string MusteriGrupTextKod { get; set; }
        public string MusteriEkGrupTextKod { get; set; }
        public int? OdemeTipi { get; set; }
        public string KisaAd { get; set; }
        public int? KdvMuaf { get; set; }
        public decimal? KoordinatX { get; set; }
        public decimal? KoordinatY { get; set; }
        public int? VadeGun { get; set; }
        public decimal? IskontoOran { get; set; }

    }
    public enum VergiTipiEnum
    {
        KDVdenMuaf = 1,
        GercekKisi = 2,
        TuzelKisi = 3,
        YabanciUyruk = 4
    }


    public enum OdemeTipiEnum
    {
        Nakit = 0,
        Cek = 1,
        Senet = 2,
        KrediKarti = 3,
        AcikHesap = 4,
        TicariKart = 5,
        DBS = 6,
        HavaleEFT = 7
    }

    public enum DurumEnum
    {
        Aktif = 0,
        Pasif = 1,
        Iptal = 2,
        Silindi = 3,
        PotansiyelPasif = 4,
        PotansiyelAktif = 5
    }

}