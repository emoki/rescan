using REScan.Common;
using REScan.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;


// Use the start of das file to be the same as the start of pctel file.
// SecCode = p_CW_Channel_CenterFreq
// CPICH RSCP = CW RSSI (db)
// CarrierSL = -999
// CenterFreq = try to output from pctel -- 
// UARFCN = dummy value 111 
// BroadcastCode = CW Channel CW
// 100kz  bandwith on cw


namespace REScan.IO
{
    public class PctelIO : DataIO<Pctel>
    {
        public PctelIO()
        {
            Headers = new Dictionary<Version, string>();
            Headers.Add(Version.V1PlainText, "Time,Longitude,Latitude,CW Channel,CW RSSI (dB)");
            CurrentVersion = Version.Unknown;
        }
        public override string DataType()
        {
            return "Pctel";
        }
        public override string Extension()
        {
            return FileUtility.PctelExtension();
        }
        public override string REAnalysisExtension(bool useSecondaryExtension = false)
        {
            if (!useSecondaryExtension)
                return "csv" + "." + FileUtility.REAnalysisExtension();
            else
                return Extension() + "." + FileUtility.REAnalysisExtension();
        }

        protected override string Header()
        {
            if (CurrentVersion.Equals(Version.Unknown))
                throw new FormatException("PctelIO version is unknown.");
            return Headers[CurrentVersion];
        }
        protected override bool IsText()
        {
            return true;
        }
        public override List<Pctel> ReadFile(string fileName)
        {
            var list = base.ReadFile(fileName);
            return list;
        }
        protected override void DetermineHeader(string header)
        {
            if (header.Contains(Headers[Version.V1PlainText]))
                CurrentVersion = Version.V1PlainText;
            else
            {
                CurrentVersion = Version.Unknown;
                throw new FormatException("Unable to determine PctelIO version.");
            }
        }
        protected override void SkipTextHeader(TextReader reader)
        {
            for (int i = 0; i < 7; i++)
            {
                reader.ReadLine();
            }
        }
        protected override Pctel Parse(string txt)
        {
            switch (CurrentVersion)
            {
                case Version.V1PlainText:
                    return ParseV1(txt);
                default:
                    throw new InvalidOperationException("PctelIO version is unknown.");
            }
        }
        private Pctel ParseV1(string txt)
        {
            var row = txt.Split(',');
            if (row.Count() < 5)
                ThrowParseException(Version.V1PlainText);

            var index = 0;
            Pctel pt = new Pctel();
            pt.Time = ((DateTimeOffset)DateTime.Parse(
                    row[index++],
                    CultureInfo.CreateSpecificCulture("en-US"),
                    DateTimeStyles.AssumeUniversal
                )).ToUnixTimeSeconds();
            if (row[index] != "")
                pt.Lon = double.Parse(row[index++]);
            else
                ++index;
            if (row[index] != "")
                pt.Lat = double.Parse(row[index++]);
            else
                ++index;
            pt.Channel = int.Parse(row[index++]);
            pt.CarrierSignalLevel = double.Parse(row[index++]);
            pt.Frequency = -1;
            pt.CarrierBandwidth = 100000;
            if (Utility.Uninitialized(pt.Lat) || Utility.Uninitialized(pt.Lon))
                pt.IsGpsLocked = false;
            else
                pt.IsGpsLocked = true;
            return pt;
        }
        private void ThrowParseException(Version version)
        {
            throw new FormatException("Unable to parse " + System.Enum.GetName(typeof(Version), version) + " file.  Not enough columns.");
        }
        protected override Pctel Parse(byte[] binary)
        {
            throw new NotSupportedException("Binary IO for Pctel is not supported.");
        }
        protected override int BinaryStructByteSize()
        {
            throw new NotSupportedException("Binary IO for Pctel is not supported.");
        }
        protected override void outputRedEyeAnalysisFormat(TextWriter writer, Pctel meas, Meta meta)
        {
            base.outputRedEyeAnalysisFormat(writer, meas, meta);
            var freq = 850;
            if(meta.isHighBand) {
                freq = 2150;
            }
            string s = "";
            s += RedeyeDelimiter;
            s += meas.CollectionRound; s += RedeyeDelimiter;
            s += "111"; s += RedeyeDelimiter; // Dummy channel
            s += freq; s += RedeyeDelimiter;
            s += meas.CarrierSignalLevel; s += RedeyeDelimiter; 
            s += meas.Channel; s += RedeyeDelimiter;
            s += meas.CarrierSignalLevel; s += RedeyeDelimiter;
            s += "0"; s += RedeyeDelimiter;
            s += "p_"; s += meas.Channel; s += "_"; s += (freq*10).ToString("00000"); s = s.Insert(s.Length - 1, "_"); 
            //s += RedeyeDelimiter;
            //s += meas.PixelX; s += RedeyeDelimiter;
            //s += meas.PixelY;
            writer.WriteLine(s);
        }
        protected enum Version
        {
            Unknown,
            V1PlainText,
        };
        private Dictionary<Version, string> Headers;
        Version CurrentVersion;
    }
}

