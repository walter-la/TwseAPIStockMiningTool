using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stock.Core.Domain.Models;

namespace Stock.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var minerFactory = new Stock.Core.Services.Miners.MinerFactory();
            var miner = minerFactory.Create<ForeignDailyTradeDetails>();
            miner.MineCar.MiningDate = new DateTime(2014, 8, 1);
            var result = miner.Mining();
        }
    }
}
