using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{

    [Serializable]
    public class Stock
    {
        public string stockCode;
        public AnalyticInfo indicators;
        public MarketHistory MarketHistory;
        public Dictionary<string, MarketData> _marketData;

        public Dictionary<string, MarketData> MarketDatas
        {
            get
            {
                return _marketData;
            }
            set
            {
                _marketData = value;
                MarketHistory = new MarketHistory(_marketData);
            }
        }

        public Stock()
        {
            MarketDatas = new Dictionary<string, MarketData>();
            indicators = new AnalyticInfo(this);
        }

    }

    



    
}
