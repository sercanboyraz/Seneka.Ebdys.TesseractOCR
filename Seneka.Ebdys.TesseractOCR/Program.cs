using log4net;
using log4net.Config;
using Seneka.Ebdys.TesseractOCR.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Seneka.Ebdys.TesseractOCR
{
    class Program : ProcessCommand
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static string ConnectionString = "";

        static void Main(string[] args)
        {
            //////////////////////////////////////////////////////
            ///set TESSDATA_PREFIC="C:\Tesseract\tessdata"  ----------->bunu unutma client da önce bu satır çalışacak.
            ///////////////////////////////////////////////////////
            log4net.Config.XmlConfigurator.Configure();
            ConnectionString = Properties.Settings.Default.ConnectionString.ToString();
            MyProperty();
            Start();
        }

        public static void MyProperty()
        {
            var gggg = new string[] { "1.pdf" };
            foreach (var item in gggg)
            {
                var newTifData = GhostScriptTiffConverter.GhostScriptTiffConverterProcess(@"E:\" + item);
                if (!string.IsNullOrWhiteSpace(newTifData))
                    Ocr(newTifData);
                else
                    log.Error(newTifData + " no tiff data");
            }
        }


        public static void Start()
        {
            while (true)
            {
                var processDocument = new ProcessDocument();
                using (SqlConnection Conn = new SqlConnection(ConnectionString))
                {
                    Conn.Open();
                    var dsDocumentDatas = new DataSet();
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT TOP 5 Id, DocumentDataTypeEnum, DataType, DocumentId,Data FROM DocumentData WITH (NOLOCK) WHERE Id NOT IN (SELECT DocumentDataId FROM DocumentOcr WITH (NOLOCK)) AND DataType = 'pdf' ORDER BY Id asc", Conn))
                    {
                        using (System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                        {
                            adapter.Fill(dsDocumentDatas);
                            //şimdilik data çekilip yazılıyor düzeltilecek.
                            for (int i = 0; i < 5; i++)
                            {
                                DataRow rowToProcess = dsDocumentDatas.Tables[0].Rows[i];
                                var path = Properties.Settings.Default.DocumentData;
                                var documentName = rowToProcess["DocumentId"].ToString() + ".pdf";
                                var fullOrjinalPath = path + documentName;
                                try
                                {
                                    System.IO.File.WriteAllBytes(fullOrjinalPath, (byte[])rowToProcess["Data"]);
                                    var newTifData = GhostScriptTiffConverter.GhostScriptTiffConverterProcess(fullOrjinalPath);
                                    if (!string.IsNullOrWhiteSpace(newTifData))
                                        Ocr(newTifData);
                                    else
                                        log.Error(newTifData + " no tiff data");
                                }
                                catch (Exception ex)
                                {
                                    log.Error("OCR01" + Environment.NewLine + ex);
                                }
                            }
                        }
                    }
                    Conn.Close();
                }
            }
        }

        private static void Ocr(string path)
        {
            var newPath = path;
            try
            {
                var command = string.Concat("-l \"tur\" \"" + path + "\"" + " \"" + newPath.Replace(".tif", "") + "\"" + " pdf");
                var ocrResult = OCRExecute(command);
                if (ocrResult == false)
                    log.Error(newPath);
                else
                    log.Info(newPath);
            }
            catch (Exception ex)
            {
                log.Error("Ocr02" + Environment.NewLine + ex);
            }
        }

        private static bool OCRExecute(string command)
        {
            bool result = false;
            try
            {
                var ocrPdf = CommandExecute(command);
                if (ocrPdf == true)
                    result = true;
                else
                    result = false;
            }
            catch (Exception ex)
            {
                log.Error("OCRExecute" + Environment.NewLine + ex);
                result = true;
            }
            return result;
        }
    }
}
