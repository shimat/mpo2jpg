using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mpo2Jpg
{
    public class MPIndexIFD
    {
        public ushort Count { get; set; }
        public IFD MPFVersion { get; set; }
        public IFD NumberOfImages { get; set; }
        public IFD MPEntry { get; set; }
        public IFD ImageUIDList { get; set; }
        public IFD TotalFrames { get; set; }
        public uint NextIFDOffset { get; set; }
        public MPEntryValue[] MPEntryValues { get; set; }
        public UniqueID[] UniqueIDs { get; set; }
    }
}
