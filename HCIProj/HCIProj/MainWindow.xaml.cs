﻿using System;
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
using System.ComponentModel;
using System.Collections.ObjectModel;
namespace HCIProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _trenutnaLokacija;
        private int _minTemp;
        private int _maxTemp;
        private int _temp;
        public ObservableCollection<string> Lokacije { get; set; }
        public String TrenutnaLokacija {
            get {
                return _trenutnaLokacija;
            }
            set {
                if (value != _trenutnaLokacija)
                {
                    _trenutnaLokacija = value;
                    OnPropertyChanged("TrenutnaLokacija");
                }
            }
        }
        public int MinTemp {
            get {
                return _minTemp;
            }
            set {
                if (value != _minTemp)
                {
                    _minTemp = value;
                    OnPropertyChanged("MinTemp");
                }
            }
        }
        public int MaxTemp {
            get {
                return _maxTemp;
            }
            set {
                if (value != _maxTemp)
                {
                    _maxTemp = value;
                    OnPropertyChanged("MaxTemp");
                }
            }
        }
        public int Temp {
            get {
                return _temp;
            }
            set {
                if (value != _temp)
                {
                    _temp = value;
                    OnPropertyChanged("Temp");
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Temp = 0;
            MinTemp = 0;
            MaxTemp = 0;
            Lokacije = new ObservableCollection<string>();
            Lokacija l1 = new Lokacija();
            l1.Naziv = "Novi Sad";
            l1.Neomiljena = false;
            Lokacija l2 = new Lokacija();
            l2.Naziv = "Beograd";
            l2.Neomiljena = true;
            Lokacije.Add("Novi Sad");
            Lokacije.Add("Beograd");
        }

        #region PropertyChangedNotifier
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private void Load_CurrentWeather(object sender, RoutedEventArgs e)
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

                Temp = temp;
                MinTemp = min_temp;
                MaxTemp = max_temp;
            }
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
