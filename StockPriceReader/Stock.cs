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

        public Stock()
        {
            marketHistory = new Dictionary<string, MarketData>();
        }

    }

    public class MarketData
    {
        public Stock stock;
        public DateTime date;
        public decimal marketType;
        public decimal minPrice, maxPrice;
        public decimal openPrice, closePrice;
        public decimal avgPrice;
        public decimal nOfNegotiations, nOfPapersTraded, volume;

    }

    //create an extension method to override the ToString method of the DateTimeClass
    //the format should be yyyymmdd
    public static class DateTimeExt
    {
        public static string ToStringF(this DateTime val)
        {
            string year, month, day;

            year = val.Year.ToString();

            if(val.Month < 10)
            {
                month = "0" + val.Month.ToString();
            }
            else
            {
                month = val.Month.ToString();
            }

            if (val.Day < 10)
            {
                day = "0" + val.Day.ToString();
            }
            else
            {
                day = val.Day.ToString();
            }

            return year + month + day;
        }
    }
}
