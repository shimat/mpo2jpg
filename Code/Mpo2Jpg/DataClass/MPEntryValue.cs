using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mpo2Jpg
{
    [StructLayout(LayoutKind.Sequential)]
    public class MPEntryValue
    {
        /// <summary>
        /// 個別画像種別管理情報
        /// </summary>
        public uint ImageAttr { get; set; }
        /// <summary>
        /// 個別画像サイズ
        /// </summary>
        public uint ImageSize { get; set; }
        /// <summary>
        /// 個別画像データオフセット
        /// </summary>
        public uint ImageDataOffset { get; set; }
        /// <summary>
        /// 従属画像1エントリNo.
        /// </summary>
        public ushort DependentImage1 { get; set; }
        /// <summary>
        /// 従属画像2エントリNo.
        /// </summary>
        public ushort DependentImage2 { get; set; }


        public MPEntryValue()
            : this(0, 0, 0, 0, 0)
        {
        }
        public MPEntryValue(uint imageAttr, uint imageSize, uint imageDataOffset, ushort dependentImage1, ushort dependentImage2)
        {
            ImageAttr = imageAttr;
            ImageSize = imageSize;
            ImageDataOffset = imageDataOffset;
            DependentImage1 = dependentImage1;
            DependentImage2 = dependentImage1;
        }
        public MPEntryValue(byte[] bytes, int startIndex, Endian endian)
        {
            ImageAttr = (uint)BitConverterEx.ToInt32(bytes, startIndex + 0, endian);
            ImageSize = (uint)BitConverterEx.ToInt32(bytes, startIndex + 4, endian);
            ImageDataOffset = (uint)BitConverterEx.ToInt32(bytes, startIndex + 8, endian);
            DependentImage1 = (ushort)BitConverterEx.ToInt16(bytes, startIndex + 12, endian);
            DependentImage2 = (ushort)BitConverterEx.ToInt16(bytes, startIndex + 14, endian);
        }

    }
}
