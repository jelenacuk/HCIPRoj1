using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCIProj
{
    public class HourlyForecast
    {
        public ObservableCollection<list> list { get; set; }
    }

    public class main
    {
        public string temp { get; set; }
    }

    public class weather
    {
        public string icon { get; set; }
    }

    public class list
    {
        public string dt_txt { get; set; }
        public main main { get; set; }
        public List<weather> weather { get; set; }
    }

    
}
