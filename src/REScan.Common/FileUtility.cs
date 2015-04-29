using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace REScan.Common {
    public class FileUtility {
        public static string WaypointExtension() { return "wpt"; }
        public static string DasExtension() { return "dmm"; }
        public static string GsmExtension() { return "wnd"; }
        public static string WcdmaExtension() { return "wnu"; }
        public static string LteExtension() { return "wnl"; }
        public static string REAnalysisExtension() { return "wna"; }
        public static List<string> FindValidFiles(List<string> fileNames, string extension) {
            var newFileNames = new List<string>();
            foreach(var fileName in fileNames) {
                var newFileName = FindValidFile(fileName, extension);
                if(!String.IsNullOrEmpty(newFileName)) {
                    newFileNames.Add(newFileName);
                }
            }
            return newFileNames;
        }
        public static string FindValidFile(string fileName, string extension) {
            var newFileName = Path.ChangeExtension(fileName, extension);
            if(File.Exists(newFileName)) {
                return newFileName;
            }
            return null;
        }
        public static bool DoesExtensionMatch(string fileName, string extension) {
            return Path.GetExtension(fileName) == extension;
        }
    }
}
