using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    public class AnalyticInfo
    {

        //TKey: string of date
        public Dictionary<string, decimal> SMAShort, SMALong, EMAShort, EMALong;
        public Dictionary<string, decimal> RSI, ROC, MACD, AccDist, AroonOsc, AroonUp, AroonDown;
        public int SMAPeriodShort, SMAPeriodLong, EMAPeriodShort, EMAPeriodLong;
        public int RSIPeriod, ROCPeriod, AccDistStartValue, AroonOscPeriod;
        public int MACDPeriodShort, MACDPeriodLong, MACDPeriodSignal;
        //counts que quantity of indicators stores in a given instance
        public int QuantityOfIndicators 
        {
            get
            {
                int sum = 0;
                FieldInfo[] fields = GetType().GetFields(); 
                foreach(FieldInfo f in fields)
                {
                    if(f.FieldType == typeof(Dictionary<string, decimal>))
                    {
                        sum += ((Dictionary<string, decimal>) f.GetValue(this)).Count();
                    }
                }
                return sum;
            }
        }

        #region Constructors
        //main constructor
        public AnalyticInfo(int SMAPeriodShort, int SMAPeriodLong, int EMAPeriodShort, int EMAPeriodLong,
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

        }

        //void constructor (set the periods to their default values
        public AnalyticInfo() : this(15, 50, 12, 26, 14, 12, 0, 25, 12, 26, 9)
        {

        }

        #endregion

    }
}
