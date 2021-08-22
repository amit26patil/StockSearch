using Search.Core;
using Search.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.Search
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
            Console.ReadLine();
        }

        private static async Task MainAsync(string[] args)
        {
            SetRunnerSettings(args);
            var indexService = new ElasticSearchIndexService();
            await indexService.IndexDocuments();
            Console.WriteLine("************** Indexing Complete **************");
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter searchkeyword ex.(Internet of Things)");
                var searchKeyword = Console.ReadLine();
                await indexService.Search(searchKeyword);
            }
        }
        private static void SetRunnerSettings(string[] args)
        {
            var loadFromLocal = "n";
            var strCodes = "";
            if (args?.Length == 0)
            {
                Console.WriteLine("Load Pdfs From Local (Y|y)/(N|n)");
                loadFromLocal = Console.ReadLine();

                Console.WriteLine("Enter comma separated bse codes (532783, 532782)");
                strCodes = Console.ReadLine();
            }
            else
            {
                var map = args.Select(a => a.Split('='))
                              .Where(a => a.Length > 1)
                              .ToDictionary(a => a[0], b => b[1], StringComparer.OrdinalIgnoreCase);

                if (map.ContainsKey("loadFromLocal"))
                {
                    loadFromLocal = map["loadFromLocal"];
                }
                if (map.ContainsKey("codes"))
                {
                    strCodes = map["codes"];
                    
                }
            }
            RunnerSettings.IsLoadPdfFromLocal = "y".Equals(loadFromLocal, StringComparison.OrdinalIgnoreCase);

            var codes = strCodes.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim());
            RunnerSettings.EquityCodes = new HashSet<string>(codes);
        }
    }
}
