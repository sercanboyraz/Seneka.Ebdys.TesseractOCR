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
        public static bool CommandExecute(string Arguments)
        {
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = Properties.Settings.Default.HelperDatas;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.Arguments = Arguments;
            p.StartInfo = info;
            p.Start();
            try
            {
                p.Start();
                p.WaitForExit();
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
                return false;
            }
            return true;
        }
    }
}
