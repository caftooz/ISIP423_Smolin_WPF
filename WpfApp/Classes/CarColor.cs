using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    internal class CarColor
    {
        public string Name { get; private set; }
        public float Cost { get; private set; }

        public CarColor(string name, float cost)
        {
            Name = name;
            Cost = cost;
        }
    }
}
