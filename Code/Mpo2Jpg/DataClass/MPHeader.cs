using System;
using System.Collections.Generic;
using System.Text;

namespace Mpo2Jpg
{
    public class MPHeader
    {
        public Endian Endian { get; set; }
        public uint FirstIFDOffset { get; set; }

        public MPHeader()
            : this(Endian.Little, 0)
        {
        }
        public MPHeader(Endian endian, uint firstIFDOffset)
        {
            Endian = endian;
            FirstIFDOffset = firstIFDOffset;
        }
    }
}
