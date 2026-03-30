using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    public class Core
    {
        public static DB_pr14Entities Context = new DB_pr14Entities();
        public static int UserID { get; set; } = -1;
    }
}
