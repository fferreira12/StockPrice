using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{

    /*
     This class is not flexible enough
         */

    [Serializable]
    public class AnalyticInfo
    {

        #region fields
        //TKey: string of date
        public Dictionary<string, decimal> SMAShort, SMALong, EMAShort, EMALong;
        public Dictionary<string, decimal> RSI, ROC, MACD, AccDist, AroonOsc, AroonUp, AroonDown;
        public int SMAPeriodShort, SMAPeriodLong, EMAPeriodShort, EMAPeriodLong;
        public int RSIPeriod, ROCPeriod, AccDistStartValue, AroonOscPeriod;
        public int MACDPeriodShort, MACDPeriodLong, MACDPeriodSignal;

        private Stock stk;
        #endregion

        #region properties
        //counts que quantity of indicators stores in a given instance
        public int QuantityOfIndicators
        {
            get
            {

                int sum = 0;
                FieldInfo[] fields = GetType().GetFields();
                foreach (FieldInfo f in fields)
                {
                    if (f.FieldType == typeof(Dictionary<string, decimal>))
                    {
                        sum += ((Dictionary<string, decimal>)f.GetValue(this)).Count();
                    }
                }
                return sum;
            }
        }
        public decimal Punctuation
        {
            get
            {
                return Punctuate();
            }
        } 
        #endregion

        #region Constructors
        //main constructor
        public AnalyticInfo(Stock stk, int SMAPeriodShort, int SMAPeriodLong, int EMAPeriodShort, int EMAPeriodLong,
                            int RSIPeriod, int ROCPeriod, int AccDistStartValue, int AroonOscPeriod,
                            int MACDPeriodShort, int MACDPeriodLong, int MACDPeriodSignal)
        {
            this.SMAPeriodShort = SMAPeriodShort;
            this.SMAPeriodLong = SMAPeriodLong;
            this.EMAPeriodShort = EMAPeriodShort;
            this.EMAPeriodLong = EMAPeriodLong;
            this.RSIPeriod = RSIPeriod;
            this.ROCPeriod = ROCPeriod;
            this.AccDistStartValue = AccDistStartValue;
            this.AroonOscPeriod = AroonOscPeriod;
            this.MACDPeriodLong = MACDPeriodLong;
            this.MACDPeriodShort = MACDPeriodShort;
            this.MACDPeriodSignal = MACDPeriodSignal;

            SMAShort = new Dictionary<string, decimal>();
            SMALong = new Dictionary<string, decimal>();
            EMAShort = new Dictionary<string, decimal>();
            EMALong = new Dictionary<string, decimal>();
            RSI = new Dictionary<string, decimal>();
            ROC = new Dictionary<string, decimal>();
            MACD = new Dictionary<string, decimal>();
            AccDist = new Dictionary<string, decimal>();
            AroonOsc = new Dictionary<string, decimal>();
            AroonUp = new Dictionary<string, decimal>();
            AroonDown = new Dictionary<string, decimal>();

            this.stk = stk;

        }

        //void constructor (set the periods to their default values
        public AnalyticInfo(Stock stk) : this(stk, 15, 50, 12, 26, 14, 12, 0, 25, 12, 26, 9)
        {

        }

        #endregion

        #region methods

        public bool Recalculate()
        {
            return MarketHistoryAnalyzer.FillAllWithDefaults(this.stk);
        }

        private decimal Punctuate(string refDate = "")
        {
            //preliminar tests
            if(SMALong.Count == 0 || SMAShort.Count == 0 || RSI.Count == 0 || ROC.Count == 0 || MACD.Count == 0 || AroonOsc.Count == 0)
            {
                return 0m;
            }

            decimal SMAPoints = 100m * SMAShort.Last().Value / SMALong.Last().Value;
            decimal EMAPoints = 100m * EMAShort.Last().Value / EMALong.Last().Value;
            decimal RSIPoints = RSI.Last().Value;
            decimal ROCPoints = 100m * ROC.Last().Value;
            decimal MACDPoints = 100m * MACD.Last().Value;
            decimal AOSCPoints = AroonOsc.Last().Value;

            decimal totalPoints = 0;

            totalPoints += SMAPoints + EMAPoints + RSIPoints + ROCPoints + MACDPoints + AOSCPoints;

            return totalPoints;
        }

        #endregion

    }
}
