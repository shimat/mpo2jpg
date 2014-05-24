using System;
using System.Collections.Generic;
using System.Text;

namespace Mpo2Jpg
{
    public class IFD
    {
        /// <summary>
        /// データ識別コード
        /// </summary>
        public ushort Tag { get; set; }
        /// <summary>
        /// データの書式
        /// </summary>
        public IFDType Type { get; set; }
        /// <summary>
        /// 書かれるデータの個数。Typeで示される1単位のバイト数 * Count がデータ長になる。
        /// </summary>
        public uint Count { get; set; }
        /// <summary>
        /// データの長さが4バイトまでなら、ここにデータが書かれる。
        /// 5バイト以上なら別の場所にデータが書かれ、そこまでのオフセットを示す。
        /// </summary>
        public byte[] Offset { get; set; }
        /// <summary>
        /// Offsetをuintに変換したもの
        /// </summary>
        public uint OffsetUInt32
        {
            get { return BitConverter.ToUInt32(Offset, 0); }
        }
        /// <summary>
        /// データの長さが5バイト以上のときの、Offsetによって示された先にあるデータ
        /// </summary>
        public byte[] Value { get; set; }


        public IFD()
        {
            Tag = 0;
            Type = IFDType.Undefined;
            Count = 0;
            Offset = null;
            Value = null;
        }
        public IFD(byte[] bytes, int o, int offsetStart, Endian endian)
        {
            Tag = (ushort)BitConverterEx.ToInt16(bytes, o + 0, endian);
            Type = (IFDType)BitConverterEx.ToInt16(bytes, o + 2, endian);
            Count = (uint)BitConverterEx.ToInt32(bytes, o + 4, endian);
           
            Offset = new byte[4];
            Array.Copy(bytes, o + 8, Offset, 0, Offset.Length);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(Offset);

            // 長さが4バイトを超えるときは、Valueをオフセットとして扱う
            int typeSize = GetSizeOf(Type);
            if (typeSize * Count > 4)
            {
                Value = new byte[typeSize * Count];
                Array.Copy(bytes, offsetStart + OffsetUInt32, Value, 0, Value.Length);
            }

            //Value = (uint)BitConverterEx.ToInt32(bytes, startIndex + 8, endian);
        }

        /// <summary>
        /// データの型式が何バイトのデータ長かを返す
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private int GetSizeOf(IFDType type)
        {
            switch (type)
            {
                case IFDType.Byte:
                case IFDType.SByte:
                case IFDType.Ascii:
                case IFDType.Undefined:
                    return 1;
                case IFDType.Short:
                case IFDType.SShort:
                    return 2;
                case IFDType.Long:
                case IFDType.SLong:
                case IFDType.Float:
                    return 4;
                case IFDType.Rational:
                case IFDType.SRational:
                case IFDType.DFloat:
                    return 8;
                default:
                    throw new Exception();
            }
        }
    }
}
