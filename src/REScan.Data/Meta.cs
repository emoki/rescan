using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace REScan.Data {
    public class Meta {
        public Meta(string fileName) {
            Filename = Path.GetFileName(fileName);
        }
        public string Filename;
    }
}
