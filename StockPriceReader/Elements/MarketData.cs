using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{

     /*
     *   MarketData refers to market info about a given stock
     *
     *   One day produces one MarketData
     */

    public class MarketData
    {
        public Stock stock;
        public DateTime date; //make it into a property: when you set it, refresh dateStr
        public string dateStr;
        public decimal marketType;
        public decimal minPrice, maxPrice;
        public decimal openPrice, closePrice;
        public decimal avgPrice;
        public decimal nOfNegotiations, nOfPapersTraded, volume;

    }
}
