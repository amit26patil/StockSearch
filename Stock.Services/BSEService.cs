using Search.Core;
using Search.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Search.Services
{
    public class BSEService
    {
        private readonly string AnnualReportListApi = @"https://api.bseindia.com/BseIndiaAPI/api/AnnualReport/w?scripcode={0}";
        private readonly PdfService PdfService;

        public BSEService()
        {
            PdfService = new PdfService();
        }
        public List<EquityModel> GetEquities()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "Equity.csv");
            var csv = new List<string[]>();
            var lines = System.IO.File.ReadAllLines(filePath);
            foreach (string line in lines)
                csv.Add(line.Split(','));
            var properties = csv[0];
            List<EquityModel> equityModels = new List<EquityModel>();

            for (int i = 1; i < lines.Length; i++)
            {
                var objResult = new Dictionary<string, string>();
                for (int j = 0; j < properties.Length; j++)
                    objResult.Add(properties[j], csv[i][j]);

                equityModels.Add(new EquityModel(objResult));
            }
            return equityModels;
        }
        public async Task<AnnualReportList> GetReportList(EquityModel equity)
        {
            var url = string.Format(AnnualReportListApi, equity.EquityCode);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json";

            using (var webResponse = await httpWebRequest.GetResponseAsync() as HttpWebResponse)
            {
                string result;
                using (var responseStream = webResponse.GetResponseStream())
                using (var streamReader = new StreamReader(responseStream))
                {
                    result = await streamReader.ReadToEndAsync();
                }
                var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<AnnualReportList>(result);
                foreach (var item in lst.Table)
                {
                    item.Code = equity.EquityCode;
                    item.Group = equity.Group;
                    item.Industry = equity.Industry;
                    item.Instrument = equity.Instrument;
                    item.IssuerName = equity.IssuerName;
                    item.SecurityId = equity.SecurityId;
                    item.SecurityName = equity.SecurityName;
                    item.FaceValue = equity.FaceValue;
                }
                return lst;
            }
        }
        public async Task<List<PdfReportDetails>> GetPdfReports(ReportDetails reportDetails)
        {
            if (RunnerSettings.IsLoadPdfFromLocal)
            {
                var result = ReadPdfFromLocal(reportDetails);
                if (result != null)
                    return result;
            }
            var reportUrl = $"https://www.bseindia.com/bseplus/AnnualReport/{reportDetails.Code}/{reportDetails.FileName}";
            using (var wc = new System.Net.WebClient())
            {
                if (RunnerSettings.IsLoadPdfFromLocal)
                {
                    string path = GetFilePath(reportDetails);
                    await wc.DownloadFileTaskAsync(reportUrl, path);
                    return ReadPdfFromLocal(reportDetails);
                }
                var byts = await wc.DownloadDataTaskAsync(reportUrl);
                var pdfReportDetails = PdfService.GetPdfReportDetails(byts, reportDetails.FileName, reportDetails.Code);
                MapReportDetailsToPdfDetails(reportDetails, pdfReportDetails);
                return pdfReportDetails;
            }

        }

        private static void MapReportDetailsToPdfDetails(ReportDetails reportDetails, List<PdfReportDetails> pdfReportDetails)
        {
            foreach (var pdfReport in pdfReportDetails)
            {
                pdfReport.Group = reportDetails.Group;
                pdfReport.Industry = reportDetails.Industry;
                pdfReport.Instrument = reportDetails.Instrument;
                pdfReport.IssuerName = reportDetails.IssuerName;
                pdfReport.SecurityId = reportDetails.SecurityId;
                pdfReport.SecurityName = reportDetails.SecurityName;
                pdfReport.FaceValue = reportDetails.FaceValue;
            }
        }

        private List<PdfReportDetails> ReadPdfFromLocal(ReportDetails reportDetails)
        {
            string path = GetFilePath(reportDetails);
            if (File.Exists(path))
            {
                var pdfReportDetails = PdfService.GetPdfReportDetails(path);
                MapReportDetailsToPdfDetails(reportDetails, pdfReportDetails);
                return pdfReportDetails;
            }
            return null;
            //500002_68385500002.pdf
        }

        private static string GetFilePath(ReportDetails reportDetails)
        {
            var fileLocation = @"C:\temp\reports";
            var fileName = $"{reportDetails.Code}_{reportDetails.FileName}";
            var p = Path.Combine(fileLocation, fileName);
            return p;
        }
    }
}
