using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockPrice;

namespace ReaderTesting
{
    [TestClass]
    public class HistoryFileSelectorTests
    {
        [TestMethod]
        public void TestGetMostRecentFileFullPath()
        {

            HistoryFileSelector hfs = new HistoryFileSelector(@"C:\Users\Cliente\Downloads");

            string fullPath = hfs.GetMostRecentFileFullPath();

        }
    }
}
