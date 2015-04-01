using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seneka.Ebdys.TesseractOCR.Helper
{
    public class ProcessCommand
    {
        protected static void CommandExecute(string Arguments, string fileName = null)
        {
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = fileName != null ? fileName : "C:\\Tesseract\\tesseract.exe";
            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            //-t .75
            //info.WorkingDirectory = @"C:\\Tesseract\\";
            info.Arguments = Arguments;
            p.StartInfo = info;
            try
            {
                p.Start();
                p.WaitForExit();
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
            }
        }
    }
}
