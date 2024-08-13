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

        private void Ellipse_Loaded(object sender, RoutedEventArgs e)
        {
            var rotateTransform = RotateTransform;
            var storyboard = new Storyboard();
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(animation, rotateTransform);
            Storyboard.SetTargetProperty(animation, new PropertyPath(RotateTransform.AngleProperty));
            storyboard.Children.Add(animation);

            storyboard.Begin();
        }

        public void ShowLoading()
        {
            this.Visibility = Visibility.Visible; // Yükleme ekranını görünür yapar
        }

        public void HideLoading()
        {
            this.Visibility = Visibility.Collapsed; // Yükleme ekranını gizler
        }
    }
}
