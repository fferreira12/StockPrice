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

    public interface IIndicator
    {
        //#region properties

        //string Name { get; set; }

        //#endregion

        #region methods

        Dictionary<string, decimal> CalculateIndicator(MarketHistory mHistory);

        #endregion

    }
}
