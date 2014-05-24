using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mpo2Jpg
{
    public class UniqueID
    {
        public byte[] Bytes;
        public int StartIndex;

        public UniqueID()
        {
            Bytes = null;
            StartIndex = 0;
        }
        public UniqueID(byte[] bytes, int startIndex, Endian endian)
        {
            Bytes = bytes;
            StartIndex = startIndex;
        }
    }
}
