using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stock.Core.Services.Test
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            var minerFactory = new Miners.MinerFactory();
            var miner = minerFactory.Create<Domain.Models.DailyQuotes>();
            miner.MineCar.MiningDate = new DateTime(2014, 8, 1);
            var result = miner.Mining();
            Assert.IsTrue(result.Status == Interfaces.MiningStatus.Succeed);
        }

        [TestMethod]
        public void TestTradeDetailsMineCar()
        {
            //var t= Domain.Models.Extension.DateTimeExtension.CombineStockID(new DateTime(2014, 8, 12, 14, 30, 0), 0);
            var d = Domain.Models.Extension.DateTimeExtension.GetTradeDateTime(24741407714600, 2474);
            //var miner = minerFactory.Create<Domain.Models.PChomeTradeDetails>();

            //miner.MineCar.StockID = 3481;
            //var countResult = miner.MineCar.Clear();
            //var result = miner.Mining();
            //Assert.IsTrue(result.Status == Interfaces.MiningStatus.Succeed);

        }
    }
}
