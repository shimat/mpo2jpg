using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Mpo2Jpg
{
    class Program
    {
        const string MpoFile = @"mpo\HNI_0008.MPO";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            string[] files;
#if DEBUG
            files = new string[1]{ MpoFile };
#else
            if (args.Length == 0)
            {
                using (OpenFileDialog dialog = new OpenFileDialog
                {
                    DefaultExt = ".mpo",
                    Filter = "MPO files(*.mpo)|*.mpo|All files(*.*)|*.*",
                    RestoreDirectory = true,
                    CheckFileExists = true,
                    AutoUpgradeEnabled = false,
                    Multiselect = true,
                })
                {
                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;
                    files = dialog.FileNames;
                }
            }
            else
            {
                files = args;
            }
#endif

            foreach (string f in files)
            {
                try
                {
                    MpoDecoder mpo = new MpoDecoder(f);

                    //foreach (MPExtensions ext in mpo.MPExtensions)
                    //{                
                    //}

                    Bitmap[] bitmaps = mpo.GetJpegBitmaps();
                    //Bitmap[] bitmaps = mpo.GetJpegBitmapsBF();  

                    string dstDir = Path.GetDirectoryName(f);
                    string fileBaseName = Path.GetFileNameWithoutExtension(f);
                    for (int i = 0; i < bitmaps.Length; i++)
                    {
                        bitmaps[i].Save(Path.Combine(dstDir, string.Format("{0}_{1}.jpg", fileBaseName, i)), ImageFormat.Jpeg);
                        bitmaps[i].Dispose();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), Path.GetFileName(f), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }   
            }


            MessageBox.Show("Conversion to JPEG succeeded.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
