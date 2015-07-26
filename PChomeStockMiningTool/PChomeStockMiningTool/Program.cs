using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PChomeStockMiningTool
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var failList = new List<int>();
            int counter = 0;
            Console.WriteLine("Begin Time: {0}", DateTime.Now);

            using (var miner = new Stock.Core.Services.Miners.MinerFactory().Create<Stock.Core.Domain.Models.PChomeTradeDetails>())
            {
                foreach (var stockID in miner.MineCar.GetAllStockID())
                {
                    miner.MineCar.StockID = stockID;
                    var clearCount = miner.MineCar.Clear();
                    var miningResult = miner.Mining();
                    Console.WriteLine("{0}: {1}", stockID, miningResult.Count);

                    switch (miningResult.Status)
                    {
                        case Stock.Core.Services.Interfaces.MiningStatus.Succeed:
                        case Stock.Core.Services.Interfaces.MiningStatus.EntitySetIsEmpty:
                            counter += miningResult.Count;
                            break;
                        default:
                            Console.Beep();
                            failList.Add(stockID);
                            break;
                    }
                }

            }

            foreach (var id in failList)
            {
                Console.WriteLine(id);
            }
            Console.WriteLine("End Time: {0}, Counter: {1}, Failed: {2}", DateTime.Now, counter, failList.Count);
            Console.WriteLine("Completed... Press Any Keys Exit...");
            Console.ReadKey();
        }

        public static void TestYahooTradeDetails()
        {
            var mineCar = new Stock.Core.Services.Yahoo.TradeDetailsMineCar();
            mineCar.Client = new RestSharp.RestClient()
            {
                CookieContainer = new System.Net.CookieContainer(),
                UserAgent = "Mozilla/5.0 Chrome",
            };
            mineCar.MiningDate = DateTime.Now.Date;
            var allStockID = mineCar.GetAllStockID();
            while (true)
            {
                int counter = 0;
                System.Diagnostics.Trace.WriteLine(string.Format("Start Time:{0}", DateTime.Now.TimeOfDay));
                foreach (var stockID in allStockID)
                {
                    mineCar.StockID = stockID;
                    var res = mineCar.GetResponse();
                    var hd = new HtmlAgilityPack.HtmlDocument();
                    hd.LoadHtml(System.Text.Encoding.Default.GetString(res.RawBytes));

                    var tableNode = mineCar.GetTableNode(hd.DocumentNode);
                    var tableColumns = mineCar.GetTableColumns(tableNode);
                    mineCar.ConvertToIndexColumnMappings(tableColumns);
                    if (!mineCar.IsValid)
                    {

                    }
                    System.Diagnostics.Trace.WriteLine(string.Format("{0}: {1}", DateTime.Now.TimeOfDay, stockID));
                    System.Threading.Thread.Sleep(100);
                }
                counter++;
                System.Diagnostics.Trace.WriteLine(string.Format("{0}: {1}", DateTime.Now.TimeOfDay, counter));

            }
            mineCar.Dispose();
        }

        public static void FixDateData()
        {
            var db = new Stock.Core.Domain.Models.Entities();
            var time = new DateTime(2014, 08, 11, 07, 50, 00);
            var stockIDs = db.Companies.Select(item => item.StockID).ToList();
            foreach (var stockID in stockIDs)
            {
                var id = Stock.Core.Domain.Models.Extension.DateTimeExtension.CombineStockID(time, stockID);
                System.Diagnostics.Debug.WriteLine(string.Format("delete FROM [Stock].[dbo].[TradeDetails] where TradeID = {0};", id));
                
            }



            //db.TradeDetails.Where()
            //var counter1 = 0;
            //var date = new DateTime(2014, 08, 11);
            //var query = db.PChomeTradeDetails.Where(item => item.TradeTime >= date).OrderBy(item => item.TradeID).Skip(0);

            //while (true)
            //{
            //    var list = query.Take(1000).ToList();
            //    if (!list.Any())
            //        break;


            //    var newList = list.Select(item => new Stock.Core.Domain.Models.PChomeTradeDetails()
            //        {
            //            AccTradeVolume = item.AccTradeVolume,
            //            BestAskPrice = item.BestAskPrice,
            //            BestBidPrice = item.BestBidPrice,
            //            Change = item.Change,
            //            ChangeExtent = item.ChangeExtent,
            //            HighestPrice = item.HighestPrice,
            //            LatestTradePrice = item.LatestTradePrice,
            //            LowestPrice = item.LowestPrice,
            //            StockID = item.StockID,
            //            StockStatus = item.StockStatus,
            //            TradeID = Stock.Core.Domain.Models.Extension.DateTimeExtension.CombineStockID(item.TradeTime, item.StockID),
            //            TradeStatus = item.TradeStatus,
            //            TradeTime = item.TradeTime,
            //            TradeVolume = item.TradeVolume
            //        }).ToList();

            //    db.PChomeTradeDetails.AddRange(newList);
            //    db.SaveChanges();

            //    query = query.Skip(1000);

            //    counter1 += 1000;
            //    Console.WriteLine(counter1);
            //}

            db.Dispose();
            //return;
        }

    }
}
