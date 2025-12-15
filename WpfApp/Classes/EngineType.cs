using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    internal class EngineType
    {
        public string Name { get; private set; }
        public float Cost { get; private set; }

        public EngineType(string name, float cost)
        {
            Name = name;
            Cost = cost;
        }
    }
}
