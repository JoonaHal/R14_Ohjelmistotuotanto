using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mökinvaraus.Models
{
    public class Varaus
    {
        public int VARAUS_ID { get; set; }
        public int MOKKI_ID { get; set; }
        public int ASIAKAS_ID { get; set; }
        public DateTime ALKUPVM { get; set; }
        public DateTime LOPPUPVM { get; set; }

    }
}
