using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seneka.Ebdys.TesseractOCR.Helper
{
    public class ProcessDocument
    {
        public int DocumentId { get; set; }
        public int DocumentDataId { get; set; }
        public byte[] DocumentData { get; set; }
        public string DataType { get; set; }
        public string DocumentText { get; set; }
    }
}
