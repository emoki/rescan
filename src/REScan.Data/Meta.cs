using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace REScan.Data {
    public class Meta {
        public Meta(string fileName, bool isHighBand = false)
        {
            Filename = Path.GetFileName(fileName);
            this.isHighBand = isHighBand;
        }
        public string Filename;
        public bool isHighBand;
    }
}
