using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    public class Stock
    {
        public string stockCode;
        public Dictionary<string, MarketData> marketHistory;
        public AnalyticInfo indicators;

        public Stock()
        {
            marketHistory = new Dictionary<string, MarketData>();
            indicators = new AnalyticInfo();
        }

    }

    



    
}
