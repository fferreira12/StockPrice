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
    public class MarketHistory : IMarketDataIndexer, IGetLastInfo, IEnumerable<MarketData> //transform the non generic IEnumerable into a generic IEnumerable<MarketData>
    {

        #region fields

        //list of dates that are contained within the MarketDatas dictionary
        //it is supposed to be altered just when the marketDatas object is changed
        private List<string> dates = new List<string>();

        //internally keeps track of all market data
        //it is supposed to be filled by the reader class methods
        private Dictionary<string, MarketData> marketDatas = new Dictionary<string, MarketData>();

        #endregion

        #region constructors

        //basic constructor
        //initializes the fields
        private MarketHistory()
        {
            //dates = new List<string>();
            //marketDatas = new Dictionary<string, MarketData>();
        }

        //Dictionary constructor
        //to ease the creation directly from a reader class method
        public MarketHistory(Dictionary<string,MarketData> markethistory) : this()
        {
            MarketDatas = markethistory;
            RefreshDates();
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
                //if there is market data but the dates list has zero elements
                if(dates.Count == 0 && marketDatas.Count > 0)
                {
                    RefreshDates();
                }
                if (index >= 0 && marketDatas.Count > index)
                {
                    return marketDatas[dates[index]];
                }else
                {
                    return null;
                }
            }

            //different approach depending on wheter the item should be added or edited
            set
            {
                if (marketDatas.ContainsKey(dates[index]))
                {
                    marketDatas[dates[index]] = value;
                }
                else
                {
                    marketDatas.Add(value.dateStr, value);
                }
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
                if(dates.Count == 0 && marketDatas.Count > 0)
                {
                    RefreshDates();
                }
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
                RefreshDates();

                //testing purposes
                if(dates.Count != 0)
                {

                }
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

        private void RefreshDates()
        {
            //there is no point in refreshing if there is no market data
            if (marketDatas != null && marketDatas.Count > 0)
            {
                dates =
                            (from d in marketDatas.Keys
                             orderby d
                             select d).ToList(); 
            }
        }

        //get the index of a given date 
        public int GetIndexOfDate(string date)
        {
            if(dates.Count == 0 && marketDatas.Count > 0)
            {
                RefreshDates();
            }
            return dates.IndexOf(date);
        }

        public int GetIndexOfDate(MarketData m)
        {
            return GetIndexOfDate(m.dateStr);
        }

        public MarketData PreviousMarketData(string date)
        {
            return PreviousMarketData(GetIndexOfDate(date));
        }
        public MarketData PreviousMarketData(int index)
        {
            if (index > 0)
            {
                return this[index - 1]; 
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<decimal> GetLastNumericData(int n, string refDate, MarketNumericInfo infoToRetrieve)
        {
            //last market data, from which retrieve the numeric data
            IEnumerable<MarketData> lastMData =
                from d in Dates
                where Dates.IndexOf(refDate) - Dates.IndexOf(d) < n
                orderby d
                select this[d];
            IEnumerable<decimal> results = null;

            switch (infoToRetrieve)
            {
                case MarketNumericInfo.AVGPRICE:
                    results =
                        from m in lastMData
                        select m.avgPrice;
                    break;
                case MarketNumericInfo.CLOSEPRICE:
                    results =
                        from m in lastMData
                        select m.closePrice;
                    break;
                case MarketNumericInfo.MARKETTYPE:
                    results =
                        from m in lastMData
                        select m.marketType;
                    break;
                case MarketNumericInfo.MAXPRICE:
                    results =
                        from m in lastMData
                        select m.maxPrice;
                    break;
                case MarketNumericInfo.MINPRICE:
                    results =
                        from m in lastMData
                        select m.minPrice;
                    break;
                case MarketNumericInfo.NEGOTIATIONSNUMBER:
                    results =
                        from m in lastMData
                        select m.nOfNegotiations;
                    break;
                case MarketNumericInfo.OPENPRICE:
                    results =
                        from m in lastMData
                        select m.openPrice;
                    break;
                case MarketNumericInfo.PAPERSNUMBER:
                    results =
                        from m in lastMData
                        select m.nOfPapersTraded;
                    break;
                case MarketNumericInfo.VOLUME:
                    results =
                        from m in lastMData
                        select m.volume;
                    break;
            }

            return results;

        }


        #endregion

        #region IEnumerable methods

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
                    //throw new InvalidOperationException();
                    return null;
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
            //not sure if this will work
            this._mHistory = null;
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
