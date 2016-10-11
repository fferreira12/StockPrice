using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    
    //creates integer and string indexer for market data elements
    public interface IMarketDataIndexer
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
        
    }

    //provides a way to get last n elements of market data or closing values
    public interface IGetLaster
    {

        //get the last n market data in an ordered fashion
        IEnumerable<MarketData> GetLastNMarKetDatas(int n, string refDate);

        //closing prices is used a lot, so having a direct way to get it will be fine
        IEnumerable<decimal> GetLastNClosingPrices(int n, string refDate);

    }

    public interface IGetLastInfo : IGetLaster
    {
        IEnumerable<decimal> GetLastNumericData(int n, string refDate, MarketNumericInfo infoToRetrieve);
    }

}
