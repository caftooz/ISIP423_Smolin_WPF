using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    public class SeatViewModel
    {
        public int Id { get; set; }      
        public int Number { get; set; }
        public int Row { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsSelected { get; set; }
    }
}
