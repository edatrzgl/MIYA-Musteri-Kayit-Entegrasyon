using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

namespace WPF_LoginForm.View
{
    public partial class LoginView : Window
    {
        public LoginView()
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

        private void btnExcelYükle_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Open(fileName);
                Excel.Worksheet worksheet = workbook.Worksheets[1];

                DataTable dt = new DataTable();
    //mustafa test
                // Sütun başlıklarını DataTable'a ekleme
                int colCount = worksheet.UsedRange.Columns.Count;
                for (int col = 1; col <= colCount; col++)
                {
                    Excel.Range headerCell = worksheet.Cells[1, col];
                    dt.Columns.Add(headerCell.Value2 != null ? headerCell.Value2.ToString() : "");
                }

                // Veri satırlarını DataTable'a ekleme
                int rowCount = worksheet.UsedRange.Rows.Count;
                for (int row = 2; row <= rowCount; row++)
                {
                    DataRow newRow = dt.NewRow();
                    for (int col = 1; col <= colCount; col++)
                    {
                        Excel.Range cell = worksheet.Cells[row, col];
                        newRow[col - 1] = cell.Value2 != null ? cell.Value2.ToString() : ".";
                    }
                    dt.Rows.Add(newRow);
                }

                // DataTable'ı DataGrid'e bağlama
                dataGrid.ItemsSource = dt.DefaultView;

                // Excel işlemlerini kapatma
                workbook.Close();
                excelApp.Quit();

                // Kullanıcıya bilgi vermek için mesaj kutusu göster
                MessageBox.Show("Excel dosyası başarıyla yüklendi ", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // DataGrid'de seçim değiştiğinde yapılacak işlemler buraya eklenebilir
        }

        private void btnBilgileriAktar_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
