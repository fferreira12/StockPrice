using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{

    [Serializable]
    public class Stock : ICloneable
    {
        public string stockCode;
        public AnalyticInfo indicators;
        public MarketHistory MarketHistory;
        public Dictionary<string, MarketData> _marketData;

        public Dictionary<string, MarketData> MarketDatas
        {
            get
            {
                return _marketData;
            }
            set
            {
                _marketData = value;
                MarketHistory = new MarketHistory(_marketData);
            }
        }

        public Stock()
        {
            MarketDatas = new Dictionary<string, MarketData>();
            indicators = new AnalyticInfo(this);
        }

        public object Clone()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return formatter.Deserialize(ms);
            }
        }
    }

    



    
}
