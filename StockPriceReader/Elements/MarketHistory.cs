using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    //this class' role is to simplify the access of items through the MarketData objects
    //it should implement some indexer methods to ease the access
    //it should implement some of interfaces (yet to be created)
    public class MarketHistory : IMarketDataIndexer, IGetLaster, IEnumerable<MarketData> //transform the non generic IEnumerable into a generic IEnumerable<MarketData>
    {

        #region fields
        //list of dates that are contained within the MarketDatas dictionary
        //it is supposed to be altered just when the marketDatas object is changed
        private List<string> dates;

        //internally keeps track of all market data
        //it is supposed to be filled by the reader class methods
        private Dictionary<string, MarketData> marketDatas;
        #endregion

        #region constructors
        //basic constructor
        //initializes the fields
        public MarketHistory()
        {
            dates = new List<string>();
            marketDatas = new Dictionary<string, MarketData>();
        }

        //Dictionary constructor
        //to ease the creation directly from a reader class method
        public MarketHistory(Dictionary<string,MarketData> markethistory) : this()
        {
            this.MarketDatas = markethistory;
        }
        #endregion

        #region indexer methods
        //access by date string
        public MarketData this[string date]
        {
            get
            {
                return marketDatas[date];
            }

            set
            {
                marketDatas[date] = value;
            }
        }

        //access by index
        public MarketData this[int index]
        {
            get
            {
                return marketDatas[dates[index]];
            }

            set
            {
                marketDatas[dates[index]] = value;
            }
        }
        #endregion

        #region properties
        //read only list of dates
        public List<string> Dates
        {
            //read only, just a change in marketDatas will refresh it
            get
            {
                return dates;
            }
        }

        //MarketDatas setter only, as it will be read by other methods
        public Dictionary<string, MarketData> MarketDatas
        {
            //write only, since this class will provide better ways to read its data
            set
            {
                marketDatas = value;
                //when setting the market data, refresh the dates
                dates =
                    (from d in marketDatas.Keys
                        orderby d
                        select d).ToList();
            }
        }
        #endregion

        #region methods
        //commonly used by the Analyzer class
        public IEnumerable<MarketData> GetLastNMarKetDatas(int n, string refDate)
        {
            IEnumerable<MarketData> last =
                (from d in dates
                    where dates.IndexOf(refDate) - dates.IndexOf(d) < n &&
                        dates.IndexOf(refDate) - dates.IndexOf(d) >= 0
                    orderby d
                    select marketDatas[d]);

            return last;
        }

        //commonly used by the Analyzer class
        public IEnumerable<decimal> GetLastNClosingPrices(int n, string refDate)
        {
            IEnumerable<decimal> last =
                (from d in dates
                    where dates.IndexOf(refDate) - dates.IndexOf(d) < n &&
                        dates.IndexOf(refDate) - dates.IndexOf(d) >= 0
                    orderby d
                    select marketDatas[d].closePrice);

            return last;
        }

        public IEnumerator<MarketData> GetEnumerator()
        {
            return new MarketHistoryEnumerator(this);
        }

        private IEnumerator GetEnumerator1()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }

        ////will iterate through the KeyValuePairs in the dictionary?
        ////I want only the marketData
        //public IEnumerator GetEnumerator()
        //{
        //    List<MarketData> listOfMD =
        //        (from d in marketDatas.Values
        //            orderby d.dateStr
        //            select d).ToList();

        //    return ((IEnumerable)listOfMD).GetEnumerator();
        //}

        #endregion
    }

    public class MarketHistoryEnumerator : IEnumerator<MarketData>
    {
        //holds the items that will be enumerated through
        private MarketHistory _mHistory;

        //constructor that takes an MarketHistory
        public MarketHistoryEnumerator(MarketHistory mHist)
        {
            _mHistory = mHist;
        }

        //fields controling the current item
        private int _index = -1;
        private MarketData _current = null;

        //Current function, returns a MarketData
        public MarketData Current
        {
            get
            {
                if(_mHistory == null ||_current == null)
                {
                    throw new InvalidOperationException();
                }
                return _current;
            }
        }

        //private current 1
        //just returns the public Current
        private MarketData Current1
        {
            get
            {
                return this.Current;
            }
        }

        //explicit Current of the enumerator, returns an general object
        object IEnumerator.Current
        {
            get { return Current1; }
        }

        //private bool disposedValue = false;
        public void Dispose()
        {
            //not implemented
        }

        //moves to the next item
        public bool MoveNext()
        {
            if (_mHistory == null)
            {
                throw new InvalidOperationException();
            }
            try
            {
                _index++;   
                _current = _mHistory[_index];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Reset()
        {
            _index = 0;
            _current = null;
        }
    }
}
