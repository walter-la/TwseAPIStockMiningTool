using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityFramework.BulkInsert.Extensions;
using Stock.Core.Domain.Models;

namespace StockMiningTool
{
    public class SaveStockInfo : Stock.IStockInfoDbService
    {
        DateTime _lastSavedTime;

        public bool IsBusy
        {
            get { return false; }
        }

        public DateTime LastSavedTime
        {
            get { return _lastSavedTime; }
        }

        public void Save(List<Stock.StockTradeInfo> tradeInfoList)
        {
            using (var db = new Entities())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;

                db.BulkInsert<TradeDetails>(GetTradeList(tradeInfoList));
                db.BulkInsert<UnOrders>(GetBestTradeList(tradeInfoList));

                _lastSavedTime = DateTime.Now;

            }
        }

        private static IEnumerable<UnOrders> CreateBestTradeList(Stock.StockTradeInfo item)
        {
            // 依照買價、買量、賣價、賣量順序，將這四個列舉物件組成一個為四種屬性的臨時類別
            var bidPairs = item.GetBestBidPrices().Zip(item.GetBestBidVolumes(), (bidPrice, bidVolume) => new Tuple<float, int>(bidPrice, bidVolume));
            var askPairs = item.GetBestAskPrices().Zip(item.GetBestAskVolumes(), (askPrice, askVolume) => new Tuple<float, int>(askPrice, askVolume));
            var bidAskParis = bidPairs.Zip(askPairs, (bid, ask) => new Tuple<float, int, float, int>(bid.Item1, bid.Item2, ask.Item1, ask.Item2));

            // 將最佳五檔附加於該成交明細上
            return bidAskParis.Select(bidAsk => new UnOrders()
                {
                    TradeID = item.TradeID,
                    BestBidPrice = bidAsk.Item1,
                    BestBidVolume = bidAsk.Item2,
                    BestAskPrice = bidAsk.Item3,
                    BestAskVolume = bidAsk.Item4
                });
        }

        private static IEnumerable<UnOrders> GetBestTradeList(List<Stock.StockTradeInfo> tradeInfoList)
        {
            foreach (var bestTradeList in tradeInfoList.Select(item => CreateBestTradeList(item)))
                foreach (var bestTrade in bestTradeList)
                    yield return bestTrade;
        }

        private static TradeDetails CreateTrade(Stock.Core.Domain.Interfaces.IStockTradeInfo item)
        {
            var trade = new TradeDetails()
            {
                TradeID = item.TradeID,
                AccTradeVolume = item.AccTradeVolume,
                BestAskPrice = item.BestAskPrice,
                BestAskVolume = item.BestAskVolume,
                BestBidPrice = item.BestBidPrice,
                BestBidVolume = item.BestBidVolume,
                HighestPrice = item.HighestPrice,
                LatestTradePrice = item.LatestTradePrice,
                LowestPrice = item.LowestPrice,
                StockID = item.StockID,
                StockStatus = (byte)item.StockStatus,
                TradeStatus = (byte)item.TradeStatus,
                TradeTime = item.TradeTime,
                TradeVolume = item.TradeVolume,
                Change = item.Change,
                ChangeExtent = item.ChangeExtent
            };

            return trade;
        }

        private static IEnumerable<TradeDetails> GetTradeList(List<Stock.StockTradeInfo> tradeInfoList)
        {
            return tradeInfoList.Select(item => CreateTrade(item));
        }

        public void SaveAsync(List<Stock.StockTradeInfo> tradeInfoList)
        {
            Save(tradeInfoList);
        }

        public void Dispose()
        {

        }
    }
}
