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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AMSnake
{
    /// <summary>
    /// Interaction logic for GameSettings1.xaml
    /// </summary>

    public partial class GameSettings1 : Window
    {
        MainWindow mainWindow;
        private bool loaded;
        public int Boost { get; set; }

        public int Delay {  get; set; }

        public GameSettings1(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            this.Visibility = Visibility.Hidden;
            mainWindow.Show();
        }

        public void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (loaded == true)
            {
                SpeedValue.Text = SpeedSlider.Value.ToString();
                Delay = (int)SpeedSlider.Value;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loaded= true;
        }


        public void BoostSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (loaded == true)
            {
                BoostValue.Text = BoostSlider.Value.ToString();
                Boost = (int)BoostSlider.Value;
            }
        }
    }
}
