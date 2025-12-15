using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    internal class CarOption
    {
        public string Name { get; private set; }
        public float Cost { get; private set; }

        public CarOption(string name, float cost)
        {
            Name = name;
            Cost = cost;
        }
    }
}
