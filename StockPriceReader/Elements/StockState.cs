using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    [Serializable]
    public class StockState
    {

        #region fields
        private Dictionary<string, Stock> stocks;
        #endregion

        #region constructors
        public StockState(Dictionary<string, Stock> stocks)
        {
            this.stocks = stocks;
        }

        //void constructor for when you want to deserialize
        public StockState()
        {

        }
        #endregion

        #region methods
        public bool Serialize(string fileName)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, this);
                stream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Dictionary<string, Stock> Deserialize(string fileName)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                StockState st = (StockState) formatter.Deserialize(stream);
                stream.Close();
                return st.stocks;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

    }
}
