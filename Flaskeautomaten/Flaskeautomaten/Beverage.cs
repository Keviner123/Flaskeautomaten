using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flaskeautomaten
{
    public class Beverage
    {
        public int SerialNo { get; set; }

        protected Beverage(int serialNo)
        {
            SerialNo = serialNo;
        }

    }
}
