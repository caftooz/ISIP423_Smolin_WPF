using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace WpfApp
{
    internal class CarConfiguretion
    {
        public CarModel CarModel { get; set; }
        public EngineType EngineType { get; set; }
        public CarColor CarColor { get; set; }
        public CarOption CarOption { get; set; }
        public UserData UserData { get; set; }
        public CreaditData CreaditData { get; set; }
        public float TotalCost { get; set; }
    }
}
