using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    public class CarModel
    {
        public string Name { get; private set; }
        public float Cost { get; private set; }

        public CarModel(string name, float cost)
        {
            Name = name;
            Cost = cost;
        }
    }
}
