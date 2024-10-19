using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using REScan.Common;
using REScan.Data;
using System.Runtime.Remoting.Messaging;

namespace REScan.IO
{
    public abstract class DataIO<T> where T : Coordinate
    {
        public abstract string DataType();
        public abstract string Extension();
        public virtual string REAnalysisExtension(bool useSecondaryExtension = false) {
            return Extension() + "." + FileUtility.REAnalysisExtension();
        }
        protected abstract string Header();
        protected abstract bool IsText();
        public virtual bool IsEmpty(string fileName) {
            return new System.IO.FileInfo(fileName).Length == 0;
        }
        public virtual List<T> ReadFile(string fileName) {
            SetAndValidateHeader(fileName);

            if(IsText()) {
                return ReadTextFile(fileName);
            }
            else {
                return ReadBinaryFile(fileName);
            }
        }
        protected virtual void SetAndValidateHeader(string fileName) {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(fileName))) {
                var byteHeader = reader.ReadBytes(MaxHeaderLength);
                var stringHeader = System.Text.Encoding.Default.GetString(byteHeader);
                DetermineHeader(stringHeader);
                if(!stringHeader.Contains(Header())) {
                    throw new FormatException("File header for '" + fileName + "' does not match" + DataType() + " header.  Possibly wrong file format.");
                }
            }
        }
        protected virtual void DetermineHeader(string header) {
            // Using the header this function determines which file version we are reading in.  Therefore
            // it only needs to be overriden by classes that have multiple versions.
        }
        protected virtual void SkipTextHeader(TextReader reader) {
            var stringHeader = reader.ReadLine();
        }
        protected virtual void SkipBinaryHeader(BinaryReader reader) {
            // Skip ahead to the beginning of the binary data.  Every header has a carriage return appended
            // so there will at least be one byte to read.
            while (reader.BaseStream.Position != reader.BaseStream.Length && reader.ReadByte() != 0x0A) { }
        }
        protected virtual List<T> ReadTextFile(string fileName) {
            List<T> records = new List<T>();

            using(TextReader reader = new StreamReader(fileName)) {

                SkipTextHeader(reader);

                // Begin populating records till end of file. 
                while (reader.Peek() != -1) {
                    try {
                       var record = Parse(reader.ReadLine());
                       records.Add(record);
                    }
                    catch(FormatException format) {
                        ErrorList.Add("Error in fileName: " + format.Message);
                    }
             }

                return records;
            }
        }
        protected virtual List<T> ReadBinaryFile(string fileName) {
            List<T> records = new List<T>();

            using(BinaryReader reader = new BinaryReader(File.OpenRead(fileName))) {

                SkipBinaryHeader(reader);

                // Begin populating records till end of file. 
                while(reader.BaseStream.Position != reader.BaseStream.Length) {
                    try {
                        var record = Parse(reader.ReadBytes(BinaryStructByteSize()));
                        records.Add(record);
                    }
                    catch(FormatException format) {
                        ErrorList.Add("Error in fileName: " + format.Message);
                    }
                }

                return records;
            }
        }
        protected virtual T Parse(string ascii) {
            throw new NotSupportedException("Text parse of " + DataType() + " is not supported.");
        }
        protected virtual T Parse(byte[] binary) {
            throw new NotSupportedException("Binary parse of " + DataType() + " is not supported.");
        }
        protected virtual int BinaryStructByteSize() {
            throw new NotSupportedException(DataType() + " does not have a binary struct size.");
        }
        public virtual void OutputRedeyeAnalysisFile(string outputFileName, List<T> data, Meta meta, bool append = true) {
            using(TextWriter writer = new StreamWriter(outputFileName, append)) {
                if(!append)
                    outputRedeyeAnalysisHeader(writer);
                
                foreach(var meas in data) {
                    outputRedEyeAnalysisFormat(writer, meas, meta);
                }
            }
        }
        protected virtual void outputRedeyeAnalysisHeader(TextWriter writer) {
            writer.Write("FileName\tUmtsAsnVersion1.0.0Latitude\tLongitude\tScannerID\tDate\tTime\tHGT_AGL\tGpsLock\tMeasCount\tUARFCN\tCenterFreq\tCarrierSL\tBroadcastCode\tCPICH RSCP\tInterference\tSecCode");
            outputDerivedRedeyeAnalysisHeader(writer);
            writer.WriteLine();
        }

        protected virtual void outputDerivedRedeyeAnalysisHeader(TextWriter writer) {
            return;
        }

        protected virtual void outputRedEyeAnalysisFormat(TextWriter writer, T meas, Meta meta) {
            string s = "";
            s += meta.Filename; s += RedeyeDelimiter;
            s += meas.Lat; s += RedeyeDelimiter;
            s += meas.Lon; s += RedeyeDelimiter;
            s += meas.ScannerID; s += RedeyeDelimiter;
            System.DateTime dt1970 = new System.DateTime(1970, 1, 1);
            dt1970 = dt1970.AddSeconds(meas.Time);
            s += dt1970.ToShortDateString(); s += RedeyeDelimiter;
            s += dt1970.ToString("HH:mm:ss"); s += RedeyeDelimiter;
            s += meas.Height; s += RedeyeDelimiter;
            s += meas.IsGpsLocked.ToString();
            writer.Write(s);
        }
        protected int MaxHeaderLength = 10240;
        protected string RedeyeDelimiter = "\t";

        public List<string> ErrorList = new List<string>();
    }
}
