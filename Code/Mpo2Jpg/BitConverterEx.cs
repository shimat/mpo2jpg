using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Mpo2Jpg
{
    /// <summary>
    /// 指定したエンディアンでバイト列と基本データ型の変換ができるクラス
    /// </summary>
    static class BitConverterEx
    {
        #region GetBytes
        /// <summary>
        /// 指定したブール値をバイト配列として返します。
        /// </summary>
        /// <param name="value">変換する数値</param>
        /// <param name="endian">エンディアン</param>
        /// <returns>長さ 1 のバイト配列。</returns>
        public static byte[] GetBytes(bool value, Endian endian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 指定した Unicode 文字値をバイト配列として返します。
        /// </summary>
        /// <param name="value">変換する数値</param>
        /// <param name="endian">エンディアン</param>
        /// <returns>長さ 2 のバイト配列。</returns>
        public static byte[] GetBytes(char value, Endian endian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 指定した 16 ビットの符号付き整数値をバイト配列として返します。
        /// </summary>
        /// <param name="value">変換する数値</param>
        /// <param name="endian">エンディアン</param>
        /// <returns>長さ 2 のバイト配列。</returns>
        public static byte[] GetBytes(short value, Endian endian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 指定した 32 ビットの符号付き整数値をバイト配列として返します。
        /// </summary>
        /// <param name="value">変換する数値</param>
        /// <param name="endian">エンディアン</param>
        /// <returns>長さ 4 のバイト配列。</returns>
        public static byte[] GetBytes(int value, Endian endian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 指定した 64 ビットの符号付き整数値をバイト配列として返します。
        /// </summary>
        /// <param name="value">変換する数値</param>
        /// <param name="endian">エンディアン</param>
        /// <returns>長さ 8 のバイト配列。</returns>
        public static byte[] GetBytes(long value, Endian endian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 指定した 16 ビットの符号なし整数値をバイト配列として返します。
        /// </summary>
        /// <param name="value">変換する数値</param>
        /// <param name="endian">エンディアン</param>
        /// <returns>長さ 2 のバイト配列。</returns>
        public static byte[] GetBytes(ushort value, Endian endian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 指定した 32 ビットの符号なし整数値をバイト配列として返します。
        /// </summary>
        /// <param name="value">変換する数値</param>
        /// <param name="endian">エンディアン</param>
        /// <returns>長さ 4 のバイト配列。</returns>
        public static byte[] GetBytes(uint value, Endian endian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 指定した 64 ビットの符号なし整数値をバイト配列として返します。
        /// </summary>
        /// <param name="value">変換する数値</param>
        /// <param name="endian">エンディアン</param>
        /// <returns>長さ 8 のバイト配列。</returns>
        public static byte[] GetBytes(ulong value, Endian endian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 指定した 32 ビットの符号なし整数値をバイト配列として返します。
        /// </summary>
        /// <param name="value">変換する数値</param>
        /// <param name="endian">エンディアン</param>
        /// <returns>長さ 4 のバイト配列。</returns>
        public static byte[] GetBytes(float value, Endian endian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(bytes);
            return bytes;
        }
        /// <summary>
        /// 指定した単精度浮動小数点値をバイト配列として返します。
        /// </summary>
        /// <param name="value">変換する数値</param>
        /// <param name="endian">エンディアン</param>
        /// <returns>長さ 8 のバイト配列。</returns>
        public static byte[] GetBytes(double value, Endian endian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
                Array.Reverse(bytes);
            return bytes;
        }
        #endregion

        #region ToXXX
        /// <summary>
        /// バイト配列内の指定位置にある1バイトから変換されたBooleanを返す
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static bool ToBoolean(byte[] value, int startIndex, Endian endian)
        {
            byte[] sub = GetSubArray(value, startIndex, sizeof(bool));
            return BitConverter.ToBoolean(Reverse(sub, endian), 0);
        }
        /// <summary>
        /// バイト配列内の指定位置にある2バイトから変換されたUnicode文字を返す
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static char ToChar(byte[] value, int startIndex, Endian endian)
        {
            byte[] sub = GetSubArray(value, startIndex, sizeof(char));
            return BitConverter.ToChar(Reverse(sub, endian), 0);
        }
        /// <summary>
        /// バイト配列内の指定位置にある2バイトから変換された16ビット符号付き整数を返す
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static short ToInt16(byte[] value, int startIndex, Endian endian)
        {
            byte[] sub = GetSubArray(value, startIndex, sizeof(short));
            return BitConverter.ToInt16(Reverse(sub, endian), 0);
        }
        /// <summary>
        /// バイト配列内の指定位置にある8バイトから変換された16ビット符号無し整数を返す
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static ushort ToUInt16(byte[] value, int startIndex, Endian endian)
        {
            byte[] sub = GetSubArray(value, startIndex, sizeof(ushort));
            return BitConverter.ToUInt16(Reverse(sub, endian), 0);
        }
        /// <summary>
        /// バイト配列内の指定位置にある4バイトから変換された32ビット符号付き整数を返す
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static int ToInt32(byte[] value, int startIndex, Endian endian)
        {
            byte[] sub = GetSubArray(value, startIndex, sizeof(int));
            return BitConverter.ToInt32(Reverse(sub, endian), 0);
        }
        /// <summary>
        /// バイト配列内の指定位置にある4バイトから変換された32ビット符号無し整数を返す
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static uint ToUInt32(byte[] value, int startIndex, Endian endian)
        {
            byte[] sub = GetSubArray(value, startIndex, sizeof(uint));
            return BitConverter.ToUInt32(Reverse(sub, endian), 0);
        }
        /// <summary>
        /// バイト配列内の指定位置にある8バイトから変換された64ビット符号付き整数を返す
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static long ToInt64(byte[] value, int startIndex, Endian endian)
        {
            byte[] sub = GetSubArray(value, startIndex, sizeof(long));
            return BitConverter.ToInt64(Reverse(sub, endian), 0);
        }
        /// <summary>
        /// バイト配列内の指定位置にある8バイトから変換された64ビット符号無し整数を返す
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static ulong ToUInt64(byte[] value, int startIndex, Endian endian)
        {
            byte[] sub = GetSubArray(value, startIndex, sizeof(ulong));
            return BitConverter.ToUInt64(Reverse(sub, endian), 0);
        }
        /// <summary>
        /// バイト配列内の指定位置にある4バイトから変換された32ビット浮動小数点数を返す
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static float ToSingle(byte[] value, int startIndex, Endian endian)
        {
            byte[] sub = GetSubArray(value, startIndex, sizeof(float));
            return BitConverter.ToSingle(Reverse(sub, endian), 0);
        }
        /// <summary>
        /// バイト配列内の指定位置にある8バイトから変換された64ビット浮動小数点数を返す
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static double ToDouble(byte[] value, int startIndex, Endian endian)
        {
            byte[] sub = GetSubArray(value, startIndex, sizeof(double));
            return BitConverter.ToDouble(Reverse(sub, endian), 0);
        }

        /// <summary>
        /// バイト配列内の指定位置にあるバイト列から構造体に変換する
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static T ToStruct<T>(byte[] value, int startIndex, Endian endian)
            where T : struct
        {
            return (T)ToStruct(value, startIndex, endian, typeof(T));
        }
        /// <summary>
        /// バイト配列内の指定位置にあるバイト列から構造体に変換する
        /// </summary>
        /// <param name="value">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        public static object ToStruct(byte[] value, int startIndex, Endian endian, Type type)
        {
            if (!type.IsValueType)
                throw new ArgumentException();

            // プリミティブ型は専用メソッドへ飛ばす
            TypeCode code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Boolean:
                    return ToBoolean(value, startIndex, endian);
                case TypeCode.Byte:
                    return value[startIndex];
                case TypeCode.Char:
                    return ToChar(value, startIndex, endian);
                case TypeCode.Double:
                    return ToDouble(value, startIndex, endian);
                case TypeCode.Int16:
                    return ToInt16(value, startIndex, endian);
                case TypeCode.Int32:
                    return ToInt32(value, startIndex, endian);
                case TypeCode.Int64:
                    return ToInt64(value, startIndex, endian);
                case TypeCode.SByte:
                    return value[startIndex];
                case TypeCode.Single:
                    return ToSingle(value, startIndex, endian);
                case TypeCode.UInt16:
                    return ToUInt16(value, startIndex, endian);
                case TypeCode.UInt32:
                    return ToUInt32(value, startIndex, endian);
                case TypeCode.UInt64:
                    return ToUInt64(value, startIndex, endian);
                default:
                    break; // 多分その他のstructなので以下処理する
            }

            // 構造体の全フィールドを取得
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            // 型情報から新規インスタンスを生成 (返却値)
            object obj = Activator.CreateInstance(type);
            int offset = 0;
            foreach (FieldInfo info in fields)
            {
                // フィールドの値をバイト列から1つ取得し、objの同じフィールドに設定
                Type fieldType = info.FieldType;
                if (!fieldType.IsValueType)
                    throw new InvalidOperationException();
                object fieldValue = BitConverterEx.ToStruct(value, startIndex + offset, endian, fieldType);
                info.SetValue(obj, fieldValue);
                // 次のフィールド値を見るためフィールドのバイトサイズ分進める
                offset += Marshal.SizeOf(fieldType);
            }

            return obj;
        }
        #endregion

        #region private
        /// <summary>
        /// バイト配列から一部分を抜き出す
        /// </summary>
        /// <param name="src">バイト配列</param>
        /// <param name="startIndex">value 内の開始位置</param>
        /// <param name="count">切り出すバイト数</param>
        /// <returns></returns>
        private static byte[] GetSubArray(byte[] src, int startIndex, int count)
        {
            byte[] dst = new byte[count];
            Array.Copy(src, startIndex, dst, 0, count);
            return dst;
        }

        /// <summary>
        /// エンディアンに従い適切なようにbyte[]を変換
        /// </summary>
        /// <param name="bytes">バイト配列</param>
        /// <param name="endian">エンディアン</param>
        /// <returns></returns>
        private static byte[] Reverse(byte[] bytes, Endian endian)
        {
            if (BitConverter.IsLittleEndian ^ endian == Endian.Little)
            {
                Array.Reverse(bytes);
                return bytes;
            }
            return bytes;
        }

        #endregion
    }
}
