using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCIProj
{
    class WeatherInfo
    {
        
       

        public class main
        {
            public double temp { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }

        }
        public class weather
        {
            public string icon { get; set; }
        }


        public class root
        {
            public string name { get; set; }
            public main main { get; set; }
            public List<weather> weather { get; set; }

        }

    }
}
