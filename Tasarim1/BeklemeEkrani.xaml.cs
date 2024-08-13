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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tasarim1
{
    /// <summary>
    /// BeklemeEkrani.xaml etkileşim mantığı
    /// </summary>
    public partial class BeklemeEkrani : Window
    {
        public BeklemeEkrani()
        {
            InitializeComponent();
            
        }
        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("RotateStoryboard");
            storyboard.Begin(MyEllipse, true); // MyEllipse ile animasyonu başlatıyoruz
        }
    }
}
