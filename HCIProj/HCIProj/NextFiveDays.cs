using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCIProj
{
    class NextFiveDays
    {
        public class main
        {
            public string dayOfWeek { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
            public string temp_minStr { get; set; }
            public string temp_maxStr { get; set; }
        }

        public class weather
        {
            public string icon { get; set; }
        }

        public class weatherForecast
        {
            public ObservableCollection<list> list { get; set; }
        }

        public class list
        {
            public double dt { get; set; }
            public main main { get; set; }
            public List<weather> weather { get; set; }
        }
    }
}
