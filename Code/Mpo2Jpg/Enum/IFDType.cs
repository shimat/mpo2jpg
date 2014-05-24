using System;
using System.Collections.Generic;
using System.Text;

namespace Mpo2Jpg
{
    public enum IFDType : ushort
    {
        Byte = 0x0001,
        Ascii = 0x0002,
        Short = 0x0003,
        Long = 0x0004,
        Rational = 0x0005,
        SByte = 0x0006,
        Undefined = 0x0007,
        SShort = 0x0008,
        SLong = 0x0009,
        SRational = 0x000A,
        Float = 0x000B,
        DFloat = 0x000C,
    }
}
