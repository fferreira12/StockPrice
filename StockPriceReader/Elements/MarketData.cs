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
        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                dateStr = _date.ToString("yyyyMMdd");
            }
        }
        public string dateStr;
        public decimal marketType;
        public decimal minPrice, maxPrice;
        public decimal openPrice, closePrice;
        public decimal avgPrice;
        public decimal nOfNegotiations, nOfPapersTraded, volume;

    }
}
