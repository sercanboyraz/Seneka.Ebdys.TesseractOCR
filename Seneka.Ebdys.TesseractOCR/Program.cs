using Seneka.Ebdys.TesseractOCR.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seneka.Ebdys.TesseractOCR
{
    class Program:ProcessCommand
    {
        static void Main(string[] args)
        {

            var command = string.Concat("\"D:/1.tif"+"\""+" "+ "\"D:/2.pdf"+"\"" + " pdf");
            var ocrResult = OCRExecute(command);
            if (ocrResult)
            {

            }
        }

        public static bool OCRExecute(string command)
        {
            bool result = false;
            try
            {
                CommandExecute(command);
                result = true;
            }
            catch (Exception ex)
            {
                result = true;
            }
            return result;
        }
        
    }
}
