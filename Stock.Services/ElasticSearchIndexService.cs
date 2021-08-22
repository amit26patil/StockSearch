using Nest;
using Search.Core;
using Search.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.Services
{
    public class ElasticSearchIndexService
    {
        private ElasticClient ElasticSearchClient;

        public ElasticSearchIndexService()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("stock_reports_search");

            this.ElasticSearchClient = new ElasticClient(settings);
        }
        public async Task IndexDocuments()
        {
            var bseService = new BSEService();
            var equityModels = bseService.GetEquities();
            var hashMap = equityModels.Where(a => !RunnerSettings.EquityCodes.Any() || RunnerSettings.EquityCodes.Contains(a.EquityCode))
                .Where(a => a.IsActive)
                .GroupBy(a => a.Group)
                .ToDictionary(k => k.Key, v => v.ToList());
            foreach (var item in hashMap.OrderBy(a => a.Key))
            {
                var equities = item.Value;//.Where(a => a.EquityCode == "532783") ;
                var batches = equities.CreateBatches(50);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Console.WriteLine($"{batches.Count()} Batches Found for Group '{item.Key}'");
                foreach (var batch in batches)
                {
                    await IndexCodes(batch);
                }
                sw.Stop();
                Console.WriteLine($"Indexing complete for Group {item.Key} in {sw.ElapsedMilliseconds * 1000} seconds");
            }
        }
        public async Task Search(string searchKeyword)
        {
            Func<SearchDescriptor<PdfReportDetails>, ISearchRequest> searchDesc =
                (SearchDescriptor<PdfReportDetails> x) => x.Index("stock_reports_search")
                                                            .Query(q => q
                                                                        .Bool(b =>
                                                                                b.Must(m =>
                                                                                    m.MatchPhrase(mp => mp.Field(f => f.PdfText).Query(searchKeyword)))));
            var result = await ElasticSearchClient.SearchAsync(searchDesc);
        }
        private async Task IndexCodes(IEnumerable<EquityModel> equityModels)
        {
            var bseService = new BSEService();
            var tasks = new List<Task<AnnualReportList>>();
            foreach (var equity in equityModels)
            {
                tasks.Add(bseService.GetReportList(equity));
            }
            await Task.WhenAll(tasks);
            var reports = tasks.SelectMany(a => a.Result.Table.Take(1)).ToList();
            var pdfTasks = new List<Task<List<PdfReportDetails>>>();
            foreach (var report in reports)
            {
                var pdfReportDetailsTask = bseService.GetPdfReports(report);
                pdfTasks.Add(pdfReportDetailsTask);
            }
            await Task.WhenAll(pdfTasks);
            var data = pdfTasks.SelectMany(a => a.Result).ToList();

            foreach (var item in data.CreateBatches(300))
            {
                var bulkResponse = await ElasticSearchClient.BulkAsync((bulk) => bulk.IndexMany(item));
            }
            
        }
    }
}
