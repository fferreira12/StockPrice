using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    
    public interface IMarketDataRetriever
    {

        //indexer for getting mData by integer
        MarketData this[int index]
        {
            get;
            set;
        }

        //indexer for getting mData by integer
        MarketData this[string date]
        {
            get;
            set;
        }

        //get the last n market data in an ordered fashion
        IEnumerable<MarketData> GetLastNMarKetDatas(int n, string refDate);

        //closing prices is used a lot, so having a direct way to get it will be fine
        IEnumerable<decimal> GetLastNClosingPrices(int n, string refDate);
        
    }

}
