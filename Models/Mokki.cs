using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mökinvaraus.Models
{
    public class Mokki
    {
        public int MOKKI_ID { get; set; }
        public string NIMI { get; set; }
        public string SIJAINTI { get; set; }
        public int ASUKASMAARA { get; set; }
        public string KUVAUS { get; set; }
        public double YOHINTA { get; set; }
    }
}
