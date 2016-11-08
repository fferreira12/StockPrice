using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{

    /*
     Should be generic
     not relating to an specific Stock or a given date
     should only provide a way to calculate the indicator for a given stock and date

         */

    public class Indicator
    {

        #region fields

        #endregion

        #region properties

        public string Name { get; set; }
        #endregion

        #region constructors

        public Indicator(string indicatorName, decimal value, DateTime date, Stock stock)
        {
            Name = indicatorName;
        }

        #endregion
    }
}
