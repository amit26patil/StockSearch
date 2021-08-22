using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Search.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Search.Services
{
    public class PdfService
    {
        public List<PdfReportDetails> GetPdfReportDetails(string filePath)
        {
            try
            {
                using (PdfReader pdfReader = new PdfReader(filePath))
                using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
                {
                    var fileName = Path.GetFileName(filePath);
                    var tmp = fileName.Split('_');
                    if (tmp.Length > 0)
                    {
                        return GetPdf(tmp[1], tmp[0], pdfDocument);
                    }

                    return GetPdf(tmp[0], "", pdfDocument);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception => {ex.Message}");
            }
            return new List<PdfReportDetails>();
        }
        public List<PdfReportDetails> GetPdfReportDetails(byte[] byts, string pdfFileName, string code)
        {
            try
            {
                using (PdfReader pdfReader = new PdfReader(new MemoryStream(byts)))
                using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
                {
                    return GetPdf(pdfFileName, code, pdfDocument);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception => ${ex}");
            }
            return new List<PdfReportDetails>();
        }

        private static List<PdfReportDetails> GetPdf(string pdfFileName, string code, PdfDocument pdfDocument)
        {
            var name = Path.GetFileNameWithoutExtension(pdfFileName);
            var num = pdfDocument.GetNumberOfPages();
            List<PdfReportDetails> pdfReportDetails = new List<PdfReportDetails>();
            //int chunkSize = 50;
            //int i = 0;
            //while (i < num)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    int chunkInd = 0;
            //    int sInd = i;
            //    while (chunkInd < chunkSize && i < num)
            //    {
            //        PdfPage pg = pdfDocument.GetPage(i + 1);
            //        var str = PdfTextExtractor.GetTextFromPage(pg);
            //        sb.AppendLine(str);
            //        chunkInd++;
            //        i++;
            //    }
            //    pdfReportDetails.Add(new PdfReportDetails()
            //    {
            //        ObjectID = $"{code}_{name}_{sInd + 1}_{i}",
            //        EquityCode = code,
            //        //PageNumber = i + 1,
            //        PdfFileName = pdfFileName,
            //        PdfText = sb.ToString()
            //    });
            //}
            for (int i = 0; i < num; i++)
            {
                PdfPage pg = pdfDocument.GetPage(i + 1);
                var str = PdfTextExtractor.GetTextFromPage(pg);
                //sb.AppendLine(str);
                pdfReportDetails.Add(new PdfReportDetails()
                {
                    ObjectID = $"{code}_{name}_{i + 1}",
                    EquityCode = code,
                    PageNumber = i + 1,
                    PdfFileName = pdfFileName,
                    PdfText = str
                });
            }
            //pdfReportDetails.Add(new PdfReportDetails()
            //{
            //    ObjectID = $"{code}_{name}",
            //    EquityCode = code,
            //    //PageNumber = i + 1,
            //    PdfFileName = pdfFileName,
            //    PdfText = sb.ToString()
            //});
            return pdfReportDetails;
        }
    }
}
