using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace StockPrice.Tests
{
    [TestClass()]
    public class ReaderTests
    {
        [TestMethod()]
        public void GettAllLinesFromTextTest()
        {
            string t = ReaderTesting.Properties.Resources.COTAHIST_A2016;
            List<string> l = Reader.GettAllLinesFromText(t);

        }
    }
}