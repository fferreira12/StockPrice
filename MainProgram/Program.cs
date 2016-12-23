using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockPrice;
using System.IO;

namespace MainProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating stock dictionary");
            Dictionary<string, Stock> stocks = new Dictionary<string, Stock>();

            Console.WriteLine("Checking to see if there is already an data analysis for today");
            if (File.Exists("stocksWithIndicators.bin") && new FileInfo("stocksWithIndicators.bin").CreationTime.Date == DateTime.Now.Date)
            {
                
                Console.WriteLine("There is. Recovering file");
                StockState ss = new StockState();
                stocks = ss.Deserialize("stocksWithIndicators.bin");
                
            }
            else
            {
                Console.WriteLine("There is not. Procceding with Analysis.");
                Console.WriteLine("Getting most recent stock market file");
                HistoryFileSelector hfs = new HistoryFileSelector(@"C:\Users\Cliente\Downloads");
                string mostRecent = hfs.GetMostRecentFileFullPath();

                Console.WriteLine("Reading data from file");
                stocks = Reader.GetAllStockData(mostRecent);

                Console.WriteLine("Analyzing data");
                MarketHistoryAnalyzer.FillAllWithDefaults(stocks);

                Console.WriteLine("Saving to disk");
                StockState sc1 = new StockState(stocks);
                sc1.Serialize("stocksWithIndicators.bin");
            }

            

            Console.WriteLine("Removing stocks not traded every day");
            List<Stock> tradedEveryday = StockComparer.RemoveStocksNotTradedEveryday(stocks);

            Console.WriteLine("Creating Stock Comparator");
            StockComparer sc = new StockComparer(tradedEveryday);

            Console.WriteLine("Running comparison");
            //List<Stock> rankedStocks = StockComparer.RankOfBestStocks(tradedEveryday);
            sc.RankStocksByCompare();

            Console.WriteLine("Saving to disk");
            StockState sc2 = new StockState(sc.RankedStocks);
            sc2.Serialize("rankedStocks.bin");

            Console.WriteLine("Emailing results");
            EmailNotifier en = new EmailNotifier("ff12sender", "33914047");
            en.Send(sc.RankedStocks);

            Console.WriteLine("All done. Check your email");
            Console.WriteLine("Press any key to exit");

            Console.ReadKey();

        }
    }
}
