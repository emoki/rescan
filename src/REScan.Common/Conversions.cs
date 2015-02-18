using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REScan.Common
{
    public class Conversions
    {
        static public double DbmToWatts(double dbm) {
            return Math.Pow(10, (dbm - 30) / 10);
        }
        static public double WattsToDbm(double watts) {
            return 10 * Math.Log10(watts / 1) + 30;
        }
    }
}
