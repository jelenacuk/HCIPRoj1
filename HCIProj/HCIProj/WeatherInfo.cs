using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HCIProj
{
    class WeatherInfo
    {



        public class main
        {
            public string temp { get; set; }
            public string temp_min { get; set; }
            public string temp_max { get; set; }
            public string humidity { get; set; }

        }

        public class clouds
        {
            public string all { get; set; }
        }

        public class rain
        {
            [Newtonsoft.Json.JsonProperty("3h")]
            public string _3h { get; set; }
            [Newtonsoft.Json.JsonProperty("1h")]
            public string _1h { get; set; }
        }

        public class wind
        {
            public string speed { get; set; }
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
            public clouds clouds { get; set; }
            public wind wind { get; set; }
            public rain rain { get; set; }
            public int cod { get; set; }

        }

    }
}
