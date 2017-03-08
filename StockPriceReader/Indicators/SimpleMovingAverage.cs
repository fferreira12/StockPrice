using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice.Indicators
{
    class SimpleMovingAverage : IIndicator
    {

        #region static fields
        static readonly int DEFAULT_PERIOD = 15;
        #endregion

        #region fields
        int period;
        #endregion

        #region constructors
        public SimpleMovingAverage(int period)
        {
            this.period = period > 0 ? period : DEFAULT_PERIOD;
        }
        #endregion


        public Dictionary<string, decimal> CalculateIndicator(MarketHistory mHistory)
        {
            Dictionary<string, decimal> temp = new Dictionary<string, decimal>();

            foreach (MarketData m in mHistory)
            {
                var lastCloses = mHistory.GetLastNClosingPrices(period, m.dateStr);

                decimal avg = lastCloses.Average();
                
                temp.Add(m.dateStr, avg);
                
            }

            return temp;

        }

    }

}
