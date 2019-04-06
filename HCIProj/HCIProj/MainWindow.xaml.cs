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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Net;

namespace HCIProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

     

        private  void Load_CurrentWeather(object sender, RoutedEventArgs e)
        {

            using (WebClient webClient = new WebClient())
            {
                string url = "http://api.openweathermap.org/data/2.5/weather?q=London,uk&APPID=8e17202912490c577a70504fd76979f3";
                string json = webClient.DownloadString(url);
                var result = JsonConvert.DeserializeObject<WeatherInfo.root>(json);

                WeatherInfo.root output = result;

                var cTemp = output.main.temp;
                var min = output.main.temp_min;
                var max = output.main.temp_max;

                int temp = Convert.ToInt32(Convert.ToDouble(cTemp) - 273.15);
                int min_temp = Convert.ToInt32(Convert.ToDouble(min) - 273.15);
                int max_temp = Convert.ToInt32(Convert.ToDouble(max) - 273.15);

            }
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
