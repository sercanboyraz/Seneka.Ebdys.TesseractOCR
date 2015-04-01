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
            log4net.Config.XmlConfigurator.Configure();
            ConnectionString = Properties.Settings.Default.ConnectionString.ToString();
            Start();
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
                    using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("SELECT TOP 10 Id, DocumentDataTypeEnum, DataType, DocumentId,Data FROM DocumentData WITH (NOLOCK) WHERE Id NOT IN (SELECT DocumentDataId FROM DocumentOcr WITH (NOLOCK)) AND DataType = 'pdf' ORDER BY Id desc", Conn))
                    {
                        using (System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(command))
                        {
                            adapter.Fill(dsDocumentDatas);
                            //şimdilik data çekilip yazılıyor düzeltilecek.
                            for (int i = 0; i < 100; i++)
                            {
                                DataRow rowToProcess = dsDocumentDatas.Tables[0].Rows[i];
                                var path = Properties.Settings.Default.DocumentData;
                                var documentName = rowToProcess["DocumentId"].ToString() + ".pdf";
                                var fullOrjinalPath = path + documentName;
                                try
                                {
                                    System.IO.File.WriteAllBytes(path + documentName, (byte[])rowToProcess["Data"]);
                                    var newTifData = TiffConverter(fullOrjinalPath);
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
                var command = string.Concat("\"" + path + "\"" + " \"" + newPath.Replace(".tif", "") + "\"" + " pdf");
                var ocrResult = OCRExecute(command);
                if (ocrResult == false)
                {
                    log.Error(newPath);
                }
                else
                    log.Info(newPath);
            }
            catch (Exception ex)
            {
                log.Error("Ocr02" + Environment.NewLine + ex);
            }
        }

        private static string TiffConverter(string path)
        {
            var newPath = path;
            try
            {
                var command = string.Concat("-dNOPAUSE -q -r300 -sDEVICE=tiffgray -dBATCH -dFirstPage=1 -dLastPage=1 -sOutputFile=\"" + newPath.Replace(".pdf", ".tif") + "\" \"" + newPath + "\" -c quit");
                var ocrResult = OCRExecute(command, Properties.Settings.Default.HelperDatas+ "\\gs915\\bin\\gswin64c.exe");
                if (ocrResult == false)
                {
                    log.Error(path);
                    return "";
                }
                else
                    log.Info(path);
            }
            catch (Exception ex)
            {
                log.Error("TiffConverter01" + Environment.NewLine + ex);
                return "";
            }
            return newPath.Replace(".pdf", ".tif");
        }

        public static bool OCRExecute(string command, string processName = null)
        {
            bool result = false;
            try
            {
                if (processName == null)
                    CommandExecute(command);
                else
                    CommandExecute(command, processName);
                result = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                result = true;
            }
            return result;
        }
    }
}
