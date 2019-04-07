using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
using System.IO;
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
        private string _icon;
        private string _omiljeni;
        private DateTime _lastUpdate;
        private string _lastupdatestring;
        private bool ipLokacija;
        private double latitude;
        private double longitude;
        public DateTime LastUpdateDate {
            get {
                return _lastUpdate;
            }
            set {
                if (value != _lastUpdate)
                {
                    _lastUpdate = value;
                    OnPropertyChanged("LastUpdate");
                }
            }
        }
        public string LastUpdateString {
            get {
                return _lastupdatestring;
            }
            set {
                if (value != _lastupdatestring)
                {
                   _lastupdatestring = value;
                    OnPropertyChanged("LastUpdateString");
                }
            }
        }

        public ObservableCollection<Lokacija> Lokacije { get; set; }
        public HourlyForecast tempPoSatima;


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

        public string Icon_
        {
            get
            {
                return _icon;
            }
            set
            {
                if (value != _icon)
                {
                    _icon = value;
                    OnPropertyChanged("Icon_");
                }
            }
        }
        public String Omiljeni {
            get {
                return _omiljeni;
            }
            set {
                if (value != _omiljeni)
                {
                    _omiljeni = value;
                    OnPropertyChanged("Omiljeni");
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
            Lokacije = new ObservableCollection<Lokacija>();
            using (StreamReader reader = new StreamReader("Lokacije.json"))
            {
                string text = reader.ReadToEnd();
                Lokacije = JsonConvert.DeserializeObject<ObservableCollection<Lokacija>>(text);
                
            }
            foreach(Lokacija l in Lokacije)
            {
                if (l.Omiljena)
                {
                    Omiljeni = "Omiljena lokacija je: " + l.Naziv;
                    TrenutnaLokacija = l.Naziv;
                    if (TrenutnaLokacija == "Trenutna Lokacija")
                    {
                        ipLokacija = true;
                    }
                    else { ipLokacija = false; }
                    LoadCurrent();
                    LoadHourly();
                    Load_NFD();
                    break;
                }
            }
            LastUpdateDate = DateTime.Now;
            this.LokacijeListaEl.ItemsSource = Lokacije;
            //Thread auto = new Thread(AutoRefresh);
            Task t = new Task(doWork);
            t.Start();
            //tempPoSatima = new HourlyForecast();
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
        public void LoadCurrent()
        {
            using (WebClient webClient = new WebClient())
            {
                string url;
                if (ipLokacija)
                {
                    string ipJson = webClient.DownloadString("http://ip-api.com/json/");
                    var ipResult = JsonConvert.DeserializeObject<IPLoc>(ipJson);
                    url = "http://api.openweathermap.org/data/2.5/weather?lat="+ ipResult.lat +"&lon=" + ipResult.lon +"&APPID=8e17202912490c577a70504fd76979f3";
                }
                else
                {
                    url = "http://api.openweathermap.org/data/2.5/weather?q=" + TrenutnaLokacija + "&APPID=8e17202912490c577a70504fd76979f3";
                }
                string json = webClient.DownloadString(url);
                var result = JsonConvert.DeserializeObject<WeatherInfo.root>(json);

                WeatherInfo.root output = result;

                var cTemp = output.main.temp;
                var min = output.main.temp_min;
                var max = output.main.temp_max;
                result.weather[0].icon = "http://openweathermap.org/img/w/" + result.weather[0].icon + ".png";


                int temp = Convert.ToInt32(Convert.ToDouble(cTemp) - 273.15);
                int min_temp = Convert.ToInt32(Convert.ToDouble(min) - 273.15);
                int max_temp = Convert.ToInt32(Convert.ToDouble(max) - 273.15);

                Temp = temp;
                MinTemp = min_temp;
                MaxTemp = max_temp;
                Icon_ = result.weather[0].icon;
            }
            LastUpdateString = DateTime.Now.ToString("MM/dd/yyyy H:mm");

        }
        private void Load_CurrentWeather(object sender, RoutedEventArgs e)
        {

            LoadCurrent();

        }

        private void Load_HourlyForecast(object sender, RoutedEventArgs e)
        {
            LoadHourly();

        }
        private void Load_NextFiveDays(object sender, RoutedEventArgs e)
        {
            Load_NFD();
        }
            public void LoadHourly()
        {
            using (WebClient webClient = new WebClient())
            {
                string url;
                if (ipLokacija)
                {
                    string ipJson = webClient.DownloadString("http://ip-api.com/json/");
                    var ipResult = JsonConvert.DeserializeObject<IPLoc>(ipJson);
                    url = "http://api.openweathermap.org/data/2.5/forecast/hourly?lat=" + ipResult.lat + "&lon=" + ipResult.lon + "&units=metric&mode=xml,uk&APPID=8e17202912490c577a70504fd76979f3";
                }
                else
                {
                    url = "http://api.openweathermap.org/data/2.5/forecast/hourly?q=" + TrenutnaLokacija + "&units=metric&mode=xml,uk&APPID=8e17202912490c577a70504fd76979f3";

                }
                string json = webClient.DownloadString(url);
                var result = JsonConvert.DeserializeObject<HourlyForecast>(json);

                tempPoSatima = result;


                foreach (var w in tempPoSatima.list)
                {
                    string[] dateAndTime = w.dt_txt.Split(' ');
                    string time = dateAndTime[1].Substring(0, 5);
                    w.dt_txt = time;

                    string[] temp = w.main.temp.Split('.');
                    double tempToDouble = Double.Parse(w.main.temp);
                    w.main.temp = temp[0] + "˚C";
                    w.weather[0].icon = "http://openweathermap.org/img/w/" + w.weather[0].icon + ".png";
                }
                danas.ItemsSource = tempPoSatima.list.Take(12);
            }
            LastUpdateString = DateTime.Now.ToString("MM/dd/yyyy H:mm");

        }

        public void Load_NFD()
        {
            using (WebClient webClient = new WebClient())
            {
                string url;
                if (ipLokacija)
                {
                    string ipJson = webClient.DownloadString("http://ip-api.com/json/");
                    var ipResult = JsonConvert.DeserializeObject<IPLoc>(ipJson);
                    url = "http://api.openweathermap.org/data/2.5/forecast?lat=" + ipResult.lat + "&lon=" + ipResult.lon + "&units=metric&mode=xml,uk&APPID=8e17202912490c577a70504fd76979f3";
                }
                else
                {
                    url = "http://api.openweathermap.org/data/2.5/forecast?q=" + TrenutnaLokacija + "&units=metric&mode=xml,uk&APPID=8e17202912490c577a70504fd76979f3";

                }
                string json = webClient.DownloadString(url);
                var result = JsonConvert.DeserializeObject<NextFiveDays.weatherForecast>(json);

                NextFiveDays.weatherForecast output = result;
                NextFiveDays.weatherForecast nfd = new NextFiveDays.weatherForecast();
                // Algoritam za pronalazenje narednih 5 dana i njihove min, maks temperature.

                nfd.list = new ObservableCollection<NextFiveDays.list>();
                var today = (new DateTime(1970, 1, 1)).AddSeconds(output.list[0].dt);
                double nextDayMin = -1000;
                double nextDayMax = -1000;
                string optimalWeather = "";
                string dayOfWeek = "";

                // Trazimo narednih 5 dana, za to koristimo uporedjivanje sa danasnjim danom + i.
                for (int i = 1; i < 6; i++)
                {

                    // Prolazimo kroz sve dane trazeci prvu instancu sledeceg dana.
                    foreach (NextFiveDays.list l in output.list)
                    {
                        var nextDate = (new DateTime(1970, 1, 1)).AddSeconds(l.dt);
                        if (nextDate.Day == today.Day + i)
                        {
                            nextDayMin = l.main.temp_min;
                            Console.WriteLine(nextDate.DayOfWeek);
                            nextDayMax = l.main.temp_max;
                            dayOfWeek = TranslateDayOfTheWeek(nextDate.DayOfWeek.ToString());
                            break;
                        }
                    }

                    // Kada smo nasli sledeci dan, prolazimo ponovo kroz listu proveravajuci min, max tog dana.
                    foreach (NextFiveDays.list l in output.list)
                    {
                        var nextDate = (new DateTime(1970, 1, 1)).AddSeconds(l.dt);
                        if (nextDate.Day == today.Day + i)
                        {
                            if (l.main.temp_min < nextDayMin)
                            {
                                nextDayMin = l.main.temp_min;
                            }
                            if (l.main.temp_max > nextDayMax)
                            {
                                nextDayMax = l.main.temp_max;
                                optimalWeather = l.weather[0].icon;
                            }
                        }
                        else if (nextDate.Day > today.Day + i)
                        {
                            break;
                        }
                    }

                    // Dodavanje novog dana u listu.
                    NextFiveDays.list li = new NextFiveDays.list();
                    NextFiveDays.main main = new NextFiveDays.main();
                    NextFiveDays.weather w = new NextFiveDays.weather();

                    main.temp_minStr = nextDayMin.ToString().Split(',')[0] + "˚C";
                    main.temp_maxStr = nextDayMax.ToString().Split(',')[0] + "˚C";
                    main.dayOfWeek = dayOfWeek;
                    w.icon = "http://openweathermap.org/img/w/" + optimalWeather + ".png";
                    li.main = main;
                    li.weather = new List<NextFiveDays.weather>();
                    li.weather.Add(w);


                    nfd.list.Add(li);
                }
                nextFD.ItemsSource = nfd.list;
            }

        }

        private string TranslateDayOfTheWeek(string eng)
        {
            switch (eng.ToString())
            {
                case "Monday":
                    return "Ponedeljak";
                case "Tuesday":
                    return "Utorak";
                case "Wednesday":
                    return "Sreda";
                case "Thursday":
                    return "Četvrtak";
                case "Friday":
                    return "Petak";
                case "Saturday":
                    return "Subota";
                case "Sunday":
                    return "Nedelja";
            }
            return "Greška!";
        }

        private void WriteLokacije()
        {
            using (StreamWriter writer = new StreamWriter("Lokacije.json"))
            {
                string lokString = JsonConvert.SerializeObject(Lokacije);
                writer.Write(lokString);

            }
        }
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TrenutnaLokacija = ((TextBlock)sender).Text;
            if (TrenutnaLokacija == "Trenutna Lokacija")
            {
                ipLokacija = true;
            }
            else { ipLokacija = false; }
            LoadCurrent();
            LoadHourly();
            Load_NFD();
            LastUpdateDate = DateTime.Now;
            LastUpdateString = DateTime.Now.ToString("MM/dd/yyyy H:mm");

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Lokacija l = new Lokacija();
            l.Naziv = this.TrenutnaLokacijaUnos.Text;
            l.Omiljena = false;
            Lokacije.Add(l);
            WriteLokacije();

        }
        private void Button_Click_Omiljeno(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (Lokacija l in Lokacije)
            {
                if (l.Omiljena)
                {
                    l.Omiljena = false;
                    break;
                }
                i++;
            }
            i = 0;
            foreach (Lokacija l in Lokacije)
            {
                if (l.Naziv == TrenutnaLokacija)
                {
                    l.Omiljena = true;
                    Lokacija lok = (Lokacija)LokacijeListaEl.Items.GetItemAt(i);
                    Omiljeni = "Omiljena lokacija je: " + lok.Naziv;
                    break;
                }
                i++;
            }
            WriteLokacije();
        }

        private void Button_Click_Obrisi(object sender, RoutedEventArgs e)
        {
            foreach (Lokacija l in Lokacije)
            {
                if (l.Naziv == TrenutnaLokacija)
                {
                    Lokacije.Remove(l);
                    break;
                }
            }
            if (Lokacije.Count > 0)
            {
                TrenutnaLokacija = Lokacije.First().Naziv;
                if (TrenutnaLokacija == "Trenutna Lokacija")
                {
                    ipLokacija = true;
                }
                else { ipLokacija = false; }
                LoadCurrent();
                LoadHourly();
                Load_NFD();
                LastUpdateDate = DateTime.Now;
                LastUpdateString = DateTime.Now.ToString("MM/dd/yyyy H:mm");

            }
            else
            {
                TrenutnaLokacija = "";
            }
            WriteLokacije();

        }
        public void doWork()
        {
            while (true)
            {
                DateTime last = LastUpdateDate;
                
                    LastUpdateString = DateTime.Now.ToString("MM/dd/yyyy H:mm");
                if (last.AddMinutes(10) >= DateTime.Now)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        LoadCurrent();
                        LoadHourly();
                        Load_NFD();
                    });
                }
                                
                Thread.Sleep(600000);
            }

        }
    }
}
