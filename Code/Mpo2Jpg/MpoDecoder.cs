using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO; 

namespace Mpo2Jpg
{
    /// <summary>
    /// 
    /// </summary>
    public class MpoDecoder
    {
        #region フィールド
        /// <summary>
        /// APP2マーカー長
        /// </summary>
        private const int MarkerLength = 2 + 2 + 4;

        /// <summary>
        /// MPOファイルのバイト列
        /// </summary>
        private readonly byte[] buffer;
        /// <summary>
        /// APP2マーカのあるオフセット
        /// </summary>
        private int[] app2Offsets;
        /// <summary>
        /// SOI(JPEGの最初のマーカ)があるオフセット
        /// </summary>
        private int[] sois;
        /// <summary>
        /// 読み取ったMPヘッダ情報
        /// </summary>
        private MPExtensions[] mpExtensions;

        /// <summary>
        /// MPヘッダ情報
        /// </summary>
        public MPExtensions[] MPExtensions 
        {
            get
            {
                return mpExtensions;
            }
        }
        #endregion

        #region 初期化
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="fileName"></param>
        public MpoDecoder(string fileName)
        {
            app2Offsets = null;
            sois = null;
            mpExtensions = null;

            // ファイルからバイト列として読み込む
            buffer = ReadFile(fileName);

            // バイト列を解釈
            Decode(buffer); 
        }

        /// <summary>
        /// ファイルからバイト列として読み込む
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private byte[] ReadFile(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                return buffer;
            }            
        }
        #endregion

        #region 出力
        /// <summary>
        /// MPフォーマットを解析した結果に従い、含まれるJPEGをそれぞれMemoryStreamとして返す
        /// </summary>
        /// <returns></returns>
        public MemoryStream[] GetJpegStreams()
        {
            if (app2Offsets == null || app2Offsets.Length == 0 || mpExtensions == null || mpExtensions.Length == 0)
                throw new InvalidOperationException();

            MPEntryValue[] mpEntries = mpExtensions[0].Index.MPEntryValues;
            MemoryStream[] streams = new MemoryStream[mpEntries.Length];

            int offsetStart1 = app2Offsets[0] + MarkerLength;
            for (int i = 0; i < mpEntries.Length; i++)
            {
                int size = (int)mpEntries[i].ImageSize;
                int offset = (i == 0) ? 0 : (offsetStart1 + (int)mpEntries[i].ImageDataOffset);
                streams[i] = new MemoryStream(buffer, offset, size);
            }

            return streams;
        }
        /// <summary>
        /// MPフォーマットを解析した結果に従い、含まれるJPEGをBitmapとして取得する
        /// </summary>
        /// <returns></returns>
        public Bitmap[] GetJpegBitmaps()
        {
            MemoryStream[] streams = GetJpegStreams();
            Bitmap[] bitmaps = new Bitmap[streams.Length];

            for (int i = 0; i < streams.Length; i++)
            {
                bitmaps[i] = new Bitmap(streams[i]);
            }

            return bitmaps;
        }
        #region Brute-Force
        /// <summary>
        /// ファイルの内容をすべて探索し、JPEGらしい箇所を見つけてそのメモリ領域をMemoryStreamとして返す
        /// </summary>
        /// <returns></returns>
        public MemoryStream[] GetJpegStreamsBF()
        {
            int[] offsetList = GetJpegOffsets(buffer);
            List<MemoryStream> result = new List<MemoryStream>();

            for (int i = 0; i < offsetList.Length; i++)
            {
                int nextOffset = (i < offsetList.Length - 1) ? offsetList[i + 1] : buffer.Length;
                int count = nextOffset - offsetList[i];
                result.Add(new MemoryStream(buffer, offsetList[i], count));
            }

            return result.ToArray();
        }
        /// <summary>
        /// ファイルの内容をすべて探索し、JPEGらしい箇所をみつけてBitmapに変換する
        /// </summary>
        /// <returns></returns>
        public Bitmap[] GetJpegBitmapsBF()
        {
            MemoryStream[] streams = GetJpegStreamsBF();
            Bitmap[] bitmaps = new Bitmap[streams.Length];

            for (int i = 0; i < streams.Length; i++)
            {
                bitmaps[i] = new Bitmap(streams[i]);
            }

            return bitmaps;
        }
        #endregion
        #endregion

        #region デコード
        #region APPマーカー探索
        /// <summary>
        /// バイト列中からMPヘッダの始点を探索する
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private int FindApp2Offset(byte[] bytes, int startIndex)
        {
            byte[] marker = new byte[] { 0xff, 0xe2 };
            int length = bytes.Length - MarkerLength;

            for (int i = startIndex; i < length; )
            {
                // マーカを探す
                int j = SearchArray(bytes, marker, i);
                // マーカが見つかったら、MPフォーマット識別コードをチェック
                if (j != -1)
                {
                    // 'M', 'P', 'F', NULL
                    if (bytes[j + 4] == 0x4D && bytes[j + 5] == 0x50 && bytes[j + 6] == 0x46 && bytes[j + 7] == 0x00)
                        return j;
                    else
                        i = j + MarkerLength;
                }
                else
                    break;
            }
            return -1;
        }

        /// <summary>
        /// 指定されたSOI近傍から、バイト列中から2番目以降のMPヘッダの始点を探索する
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="soiOffsets"></param>
        /// <returns></returns>
        private int[] FindApp2Offsets(byte[] bytes, IList<int> soiOffsets)
        {
            if (soiOffsets == null)
                throw new ArgumentNullException("soiOffsets");

            int[] result = new int[soiOffsets.Count];
            for (int i = 0; i < soiOffsets.Count; i++)
            {
                result[i] = FindApp2Offset(bytes, soiOffsets[i]);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// バイト列からMPフォーマットを解釈する
        /// </summary>
        private void Decode(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException();

            // フォーマット識別コードを探し、最初のAPP2データ開始位置を見つける
            int o = FindApp2Offset(buffer, 0);
            if (o < 0)
                throw new Exception("マルチピクチャフォーマットじゃないかも");
            app2Offsets = new int[1]{ o };

            /*for (int i = o + 8; i < o + 28; i++)
            {
                Debug.Write(string.Format("{0:X2} ", buffer[i]));
            }*/

            // 最初の一つをデコード
            List<MPExtensions> extList = new List<Mpo2Jpg.MPExtensions>();
            extList.Add(DecodeOne(buffer, o, true));

            // 各JPEGのSOIの取得
            sois = GetSOIOffsets(app2Offsets[0], extList[0].Index.MPEntryValues);
            if (sois.Length == 0 || sois[0] != 0)
                throw new Exception("SOI取得エラー");
            // APP2マーカーの位置の取得 (最初のは除く)
            int[] app2Remaining = FindApp2Offsets(buffer, ArrayGetRange(sois, 1, sois.Length - 1));            

            // 2番目以降をデコード
            foreach (int item in app2Remaining)
            {
                extList.Add(DecodeOne(buffer, item, false));
            }

            // app2Offsetsにapp2Remainingを連結
            {
                List<int> app2OffsetsTemp = new List<int>(app2Offsets);
                app2OffsetsTemp.AddRange(app2Remaining);
                app2Offsets = app2OffsetsTemp.ToArray();
            }

            mpExtensions = extList.ToArray();
        }
        /// <summary>
        /// バイト列から個別画像1つに対するMPフォーマットを解釈する
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="app2Offset"></param>
        /// <param name="isFirst"></param>
        private MPExtensions DecodeOne(byte[] buffer, int app2Offset, bool isFirst)
        {
            // オフセットの基点            
            int offsetStart = app2Offset + MarkerLength;

            // MPフォーマット付属情報
            MPExtensions ext = new MPExtensions();

            // エンディアン情報と最初のIFDへのオフセット
            ext.Header = DecodeMPHeader(buffer, offsetStart + 0);
            Endian endian = ext.Header.Endian;
            int firstIfdIndex = offsetStart + (int)ext.Header.FirstIFDOffset;

            // MPインデックスIFD
            if (isFirst)
            {
                ext.Index = DecodeMPIndexIFD(buffer, firstIfdIndex, offsetStart, endian);
                // MPエントリの読み出し
                ext.Index.MPEntryValues = DecodeMPEntries(buffer, offsetStart, ext.Index, endian);
                // 個別画像ユニークIDの読み出し
                ext.Index.UniqueIDs = DecodeUniqueIDs(buffer, offsetStart, ext.Index, endian);
            }         

            // 個別情報IFDへのオフセット
            int indivisualOffset;
            if (isFirst)
                indivisualOffset = offsetStart + (int)ext.Index.NextIFDOffset;
            else
                indivisualOffset = firstIfdIndex;

            ext.IndAttr = DecodeMPIndAttrIFD(buffer, indivisualOffset, offsetStart, endian);
            return ext;
        }

        #region MPヘッダ
        /// <summary>
        /// MPヘッダを読みだす
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="o">オフセット</param>
        /// <returns></returns>
        private MPHeader DecodeMPHeader(byte[] buffer, int o)
        {
            Endian endian = GetEndian(buffer, o);
            uint firstIFDOffset = BitConverterEx.ToUInt32(buffer, o + 4, endian);
            return new MPHeader(endian, firstIFDOffset);
        }
        /// <summary>
        /// 対象ファイルにおけるエンディアンを判定する
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="o">オフセット</param>
        /// <returns></returns>
        private Endian GetEndian(byte[] bytes, int o)
        {
            byte[] b = bytes;
            if (b[o + 0] == 0x49 && b[o + 1] == 0x49 && b[o + 2] == 0x2A && b[o + 3] == 0x00)
                return Endian.Little;
            if (b[o + 0] == 0x4D && b[o + 1] == 0x4D && b[o + 2] == 0x00 && b[o + 3] == 0x2A)
                return Endian.Big;
            throw new Exception("予期しないエンディアンです");
        }
        #endregion
        #region MPインデックスIFD
        /// <summary>
        /// MPインデックスIFDの読み出し
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="o">オフセット</param>
        /// <param name="endian"></param>
        /// <returns></returns>
        private MPIndexIFD DecodeMPIndexIFD(byte[] buffer, int o, int offsetStart, Endian endian)
        {
            // バイト列から読み込み
            MPIndexIFD index = new MPIndexIFD
            {
                Count = BitConverterEx.ToUInt16(buffer, o, endian),
                MPFVersion = new IFD(buffer, o + 2, offsetStart, endian),
                NumberOfImages = new IFD(buffer, o + 14, offsetStart, endian),
                MPEntry = new IFD(buffer, o + 26, offsetStart, endian),
            };

            // タグ番号をチェック
            if (index.MPFVersion.Tag != 45056)
                throw new Exception("MPFVersionのデコードに失敗しました");
            if (index.NumberOfImages.Tag != 45057)
                throw new Exception("NumberOfImagesのデコードに失敗しました");
            if (index.MPEntry.Tag != 45058)
                throw new Exception("MPEntryのデコードに失敗しました");  
            
            // 個別画像ユニークIDリスト以下は省略されて存在しないかもしれない
            if (index.Count > 3)
            {
                index.ImageUIDList = new IFD(buffer, o + 38, offsetStart, endian);
                index.TotalFrames = new IFD(this.buffer, o + 50, offsetStart, endian);
                index.NextIFDOffset = BitConverterEx.ToUInt32(buffer, o + 62, endian);
            }
            else if(index.Count == 3)
            {                
                index.ImageUIDList = null;
                index.TotalFrames = null;
                index.NextIFDOffset = BitConverterEx.ToUInt32(buffer, o + 38, endian);
            }
            else
            {
                throw new InvalidOperationException("MPインデックスIFDの解析に失敗しました");
            }

            return index;
        }

        /// <summary>
        /// MPエントリの読み出し
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offsetStart">オフセット基準位置</param>
        /// <param name="index"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        private MPEntryValue[] DecodeMPEntries(byte[] buffer, int offsetStart, MPIndexIFD index, Endian endian)
        {
            int offset = offsetStart + (int)index.MPEntry.OffsetUInt32;
            MPEntryValue[] mpEntries = new MPEntryValue[index.NumberOfImages.OffsetUInt32];
            for (int i = 0; i < mpEntries.Length; i++)
                mpEntries[i] = new MPEntryValue(buffer, offset + (16 * i), endian);

            return mpEntries;
        }

        /// <summary>
        /// 個別画像ユニークIDリストの読み出し
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offsetStart">オフセット基準位置</param>
        /// <param name="index"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        private UniqueID[] DecodeUniqueIDs(byte[] buffer, int offsetStart, MPIndexIFD index, Endian endian)
        {
            UniqueID[] uids = null;
            if (index.ImageUIDList != null)
            {
                uids = new UniqueID[index.NumberOfImages.OffsetUInt32];
                int offset = offsetStart + (int)index.ImageUIDList.OffsetUInt32;
                for (int i = 0; i < uids.Length; i++)
                    uids[0] = new UniqueID(buffer, offset + (33 * i), endian);
            }
            return uids;
        }

        /// <summary>
        /// MPEntryから各JPEGのSOIのオフセットを得る
        /// </summary>
        /// <returns></returns>
        private int[] GetSOIOffsets(int app2Offset1, MPEntryValue[] mpEntries)
        {
            int offsetStart1 = app2Offset1 + MarkerLength;
            int[] sois = new int[mpEntries.Length];

            for (int i = 0; i < mpEntries.Length; i++)
            {
                sois[i] = (i == 0) ? 0 : (offsetStart1 + (int)mpEntries[i].ImageDataOffset);
            }
            return sois;
        }
        #endregion
        #region MP個別情報IFD
        /// <summary>
        /// MP個別情報IFDの読み出し
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="o">オフセット</param>
        /// <param name="offsetStart">オフセット基準位置</param>
        /// <param name="endian"></param>
        /// <returns></returns>
        private MPIndividualAttributesIFD DecodeMPIndAttrIFD(byte[] buffer, int o, int offsetStart, Endian endian)
        {
            // バイト列からIFDのフィールド個数を読み込み
            MPIndividualAttributesIFD indAttr = new MPIndividualAttributesIFD
            {
                Count = BitConverterEx.ToUInt16(buffer, o + 0, endian),
            };

            // count個分フィールドを読み込み、Tagの値に従って適当なプロパティに設定
            for (int i = 0; i < indAttr.Count; i++)
			{
			    int offset = o + 2 * (12 * i);
                IFD ifd = new IFD(buffer, o + 2 + (12 * i), offsetStart, endian);
                switch(ifd.Tag)
                {
                    case 45056: indAttr.MPFVersion = ifd; break;
                    case 45313: indAttr.MPIndividualNum = ifd; break;
                    case 45569: indAttr.PanOrientation = ifd; break;
                    case 45570: indAttr.PanOverlap_H = ifd; break;
                    case 45571: indAttr.PanOverlap_V = ifd; break;
                    case 45572: indAttr.BaseViewpointNum = ifd; break;
                    case 45573: indAttr.ConvergenceAngle = ifd; break;
                    case 45574: indAttr.BaselineLength = ifd; break;
                    case 45575: indAttr.VerticalDivergence = ifd; break;
                    case 45576: indAttr.AxisDistance_X = ifd; break;
                    case 45577: indAttr.AxisDistance_Y = ifd; break;
                    case 45578: indAttr.AxisDistance_Z = ifd; break;
                    case 45579: indAttr.YawAngle = ifd; break;
                    case 45580: indAttr.PitchAngle = ifd; break;
                    case 45581: indAttr.RollAngle = ifd; break;
                    default:
                        throw new Exception(string.Format("無効なMP個別情報IFDのTag({0})です", ifd.Tag));
                }
			}

            // 普通は0
            indAttr.NextIFDOffset = BitConverterEx.ToUInt32(this.buffer, o + 2 + (12 * indAttr.Count), endian);

            return indAttr;
        }
        #endregion
        #endregion

        #region 総当たりによるJPEG抽出
        /// <summary>
        /// JPEGっぽいバイト列の始点をすべて返す
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private int[] GetJpegOffsets(byte[] bytes)
        {
            byte[][] jpegHeaders = new byte[][]{
                new byte[] { 0xff, 0xd8, 0xff, 0xe1 },
                new byte[] { 0xff, 0xd8, 0xff, 0xe0 },
            };
            int offset = 0;
            List<int> result = new List<int>();

            while (true)
            {
                int currentOffset = -1;
                foreach (var header in jpegHeaders)
                {
                    currentOffset = SearchArray(bytes, header, offset);
                    if (currentOffset != -1)
                        break;
                }

                if (currentOffset != -1)
                {
                    offset = currentOffset;
                    result.Add(offset);
                    offset += jpegHeaders[0].Length; ;
                }
                else
                    break;
            }

            return result.ToArray();
        }
        #endregion

        #region 配列処理
        /// <summary>
        /// 配列の一部分を抜き出して返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private T[] ArrayGetRange<T>(T[] src, int startIndex, int count)
        {
            T[] dst = new T[count];
            Array.Copy(src, startIndex, dst, 0, count);
            return dst;
        }
        /// <summary>
        /// バイト配列中に指定したバイト配列が現れるインデックスを返す
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="pattern"></param>
        /// <param name="startIndex"></param>
        /// <returns>見つからなければ-1を返す</returns>
        private int SearchArray(byte[] bytes, byte[] pattern, int startIndex)
        {
            int length = bytes.Length - pattern.Length + 1;
            for (int i = startIndex; i < bytes.Length; i++)
            {
                bool matched = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (bytes[i + j] != pattern[j])
                    {
                        matched = false;
                        break;
                    }
                }
                if (matched)
                    return i;
            }
            return -1;
        }
        #endregion
    }
}
