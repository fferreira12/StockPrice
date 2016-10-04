using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{

    public enum MarketStringInfo
    {
        DATE, PAPERCODE, BIZNAME, CURRENCY
    }

    public enum MarketNumericInfo
    {
        MINPRICE, MAXPRICE, OPENPRICE, CLOSEPRICE, AVGPRICE,
        NEGOTIATIONSNUMBER, PAPERSNUMBER, VOLUME, MARKETTYPE
    }

    /*
        This Reader class is responsible for reading a text file in the format 
        provided by BM&FBovespa and turn it into a set of logic objects, 
        specially Stocks and MarketData

        It was designed to work with in-cash market
    */

    public class Reader
    {
        //just uses two other methods
        public static List<string> GetAllLinesFromPath(string fileFullPath)
        {
            return ReadLines(GetReadableStream(fileFullPath));
        }

        public static StreamReader GetReadableStream (string fileFullPath)
        {
            StreamReader sr = File.OpenText(fileFullPath);

            return sr;
        }

        public static List<string> ReadLines(StreamReader sr, long nOfLines = 0)
        {
            List<string> strs = new List<string>();

            long linesRead = 0;

            while (linesRead <= nOfLines && !sr.EndOfStream)
            {
                strs.Add(sr.ReadLine());

                //only increment if there's a line limit
                //otherwise go to the end of stream
                if (nOfLines > 0)
                {
                    linesRead++; 
                }
            }

            if (nOfLines > 0)
            {
                strs.RemoveAt(strs.Count - 1); 
            }

            return strs;
            
        }

        //IMPORTANT: POSITIONS SHOULD BE ENTERED AS ONE-BASED
        //FIRST CHAR IS 1, NOT 0
        public static string ReadSubstring(string str, int startChar, int endChar)
        {
            return str.Substring(startChar-1, endChar - startChar + 1);
        }

        public static string GetStringInfo(string line, MarketStringInfo infoToRetrieve)
        {
            int startPos = 0, finalPos = 0;

            switch (infoToRetrieve)
            {
                case MarketStringInfo.BIZNAME:
                    startPos = 28;
                    finalPos = 39;
                    break;
                case MarketStringInfo.CURRENCY:
                    startPos = 53;
                    finalPos = 56;
                    break;
                case MarketStringInfo.DATE:
                    startPos = 03;
                    finalPos = 10;
                    break;
                case MarketStringInfo.PAPERCODE:
                    startPos = 13;
                    finalPos = 24;
                    break;
            }

            return ReadSubstring(line, startPos, finalPos).Trim();

        }

        public static decimal GetNumericInfo(string line, MarketNumericInfo infoToRetrieve)
        {
            int startPos = 0, finalPos = 0;

            switch (infoToRetrieve)
            {
                case MarketNumericInfo.AVGPRICE:
                    startPos = 96;
                    finalPos = 108;
                    break;
                case MarketNumericInfo.CLOSEPRICE:
                    startPos = 109;
                    finalPos = 121;
                    break;
                case MarketNumericInfo.MAXPRICE:
                    startPos = 70;
                    finalPos = 82;
                    break;
                case MarketNumericInfo.MINPRICE:
                    startPos = 83;
                    finalPos = 95;
                    break;
                case MarketNumericInfo.OPENPRICE:
                    startPos = 57;
                    finalPos = 69;
                    break;
                case MarketNumericInfo.NEGOTIATIONSNUMBER:
                    startPos = 148;
                    finalPos = 152;
                    break;
                case MarketNumericInfo.PAPERSNUMBER:
                    startPos = 153;
                    finalPos = 170;
                    break;
                case MarketNumericInfo.VOLUME:
                    startPos = 171;
                    finalPos = 188;
                    break;
                case MarketNumericInfo.MARKETTYPE:
                    startPos = 25;
                    finalPos = 27;
                    break;
            }

            string str = ReadSubstring(line, startPos, finalPos).Trim();
            decimal value = 0;

            switch (infoToRetrieve)
            {
                case MarketNumericInfo.NEGOTIATIONSNUMBER:
                case MarketNumericInfo.PAPERSNUMBER:
                case MarketNumericInfo.MARKETTYPE:
                    value = decimal.Parse(str);
                    break;
                case MarketNumericInfo.VOLUME:
                    value = decimal.Parse(str.Substring(0,str.Length - 2) + "," + str.Substring(str.Length - 2));
                    break;
                default:
                    value = decimal.Parse(str.Substring(0, str.Length - 2) + "," + str.Substring(str.Length - 2));
                    break;

            }

            return value;
        }

        //the TKey of the dictionary is the date of the info
        //only mercado a vista (markettype = 010)
        public static Dictionary<string, MarketData> GetMarketDataFromPaper(string paperCode, List<string> allLines)
        {
            Stock stock = new Stock()
            {
                stockCode = paperCode
            };

            Dictionary<string, MarketData> allMarketData = new Dictionary<string, MarketData>();

            foreach(string line in allLines)
            {
                string codeOfLine = GetStringInfo(line, MarketStringInfo.PAPERCODE);
                decimal marketType = GetNumericInfo(line, MarketNumericInfo.MARKETTYPE);

                //only gets if code is equal and marketType = 10 (mercado a vista)
                if (codeOfLine == paperCode && marketType == 10m)
                {
                    string date = GetStringInfo(line, MarketStringInfo.DATE);
                    MarketData mData = GetMarketDataFromLine(line);
                    mData.stock = stock;
                    allMarketData.Add(date, mData);
                }
            }

            return allMarketData;
        }

        //does not add the stock object
        public static MarketData GetMarketDataFromLine(string line)
        {
            string date = GetStringInfo(line, MarketStringInfo.DATE);
            MarketData mData = new MarketData();
            
            //does not add the stock
            //mData.stock = stock; 
            mData.date = ConvertStringToDateTime(date);
            mData.avgPrice = GetNumericInfo(line, MarketNumericInfo.AVGPRICE);
            mData.closePrice = GetNumericInfo(line, MarketNumericInfo.CLOSEPRICE);
            mData.maxPrice = GetNumericInfo(line, MarketNumericInfo.MAXPRICE);
            mData.minPrice = GetNumericInfo(line, MarketNumericInfo.MINPRICE);
            mData.nOfNegotiations = GetNumericInfo(line, MarketNumericInfo.NEGOTIATIONSNUMBER);
            mData.nOfPapersTraded = GetNumericInfo(line, MarketNumericInfo.PAPERSNUMBER);
            mData.openPrice = GetNumericInfo(line, MarketNumericInfo.OPENPRICE);
            mData.volume = GetNumericInfo(line, MarketNumericInfo.VOLUME);
            mData.marketType = GetNumericInfo(line, MarketNumericInfo.MARKETTYPE);

            return mData;
        }

        public static DateTime ConvertStringToDateTime(string yyyymmdd)
        {
            if(yyyymmdd.Length != 8)
            {
                throw new Exception("A string de data deve conter exatamente 8 caracteres.");
            }
            DateTime d = new DateTime(int.Parse(yyyymmdd.Substring(0, 4)), int.Parse(yyyymmdd.Substring(4, 2)), int.Parse(yyyymmdd.Substring(6, 2)));
            return d;
        }

        public static Dictionary<string, Stock> GetAllStockData(string path)
        {
            return GetAllStockData(GetAllLinesFromPath(path));
        }

        public static Dictionary<string, Stock> GetAllStockData(List<string> allLines)
        {
            Dictionary<string, Stock> allStocks = new Dictionary<string, Stock>();

            string paperCode = string.Empty;

            //ignore first and last line
            for (int i = 1; i < allLines.Count-1 ; i++)
            {
                //get paper code of line
                paperCode = GetStringInfo(allLines[i], MarketStringInfo.PAPERCODE);

                //if allstocks has no elements or does not have a specific stock
                //then add it
                if(allStocks.Count == 0 || !allStocks.ContainsKey(paperCode))
                {
                    Stock stk = new Stock()
                    {
                        marketHistory = new Dictionary<string, MarketData>(),
                        stockCode = paperCode
                    };
                    allStocks.Add(paperCode, stk);
                }

                //get the market data
                //one stock, one day
                MarketData mData = GetMarketDataFromLine(allLines[i]);

                //get the date (key)
                string dateString = GetStringInfo(allLines[i], MarketStringInfo.DATE);

                //add market data to the right stock
                //only if market type is mercado a vista (010)
                if (mData.marketType == 10m)
                {
                    allStocks[paperCode].marketHistory.Add(dateString, mData); 
                }
            }

            return RemoveStocksWithoutMarketData(allStocks);
        }

        public static Dictionary<string, Stock> RemoveStocksWithoutMarketData(Dictionary<string, Stock> allStocks)
        {
            Dictionary<string, Stock> cleanedStocks = new Dictionary<string, Stock>();

            foreach(KeyValuePair<string, Stock> stkPair in allStocks)
            {
                if(stkPair.Value.marketHistory.Count > 0)
                {
                    cleanedStocks.Add(stkPair.Key, stkPair.Value);
                }
            }

            return cleanedStocks;
        }

    }

}