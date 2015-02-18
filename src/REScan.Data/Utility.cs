using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REScan.Data {
    public static class Utility {
        public static bool Uninitialized(double t) {
            return t.Equals(double.NaN);
        }
        public static bool Uninitialized(int t) {
            return t.Equals(int.MinValue);
        }
        public static bool Uninitialized(long t) {
            return t.Equals(long.MinValue);
        }
        public static bool Uninitialized(string t) {
            return t.Equals("");
        }
    }
}
