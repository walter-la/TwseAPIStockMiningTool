using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.BulkInsert.Extensions;
using Stock.Core.Domain.Models;

namespace StockMiningTool
{
    public class StockInfoDbService : Stock.IStockInfoDbService
    {
        /// <summary>
        /// 是否正在儲存中。
        /// </summary>
        private bool _isBusy = false;

        private DateTime _lastSavedTime;

        /// <summary>
        /// 準備寫入的成交資訊緩衝佇列。
        /// </summary>
        private Queue<List<Stock.StockTradeInfo>> _tradeInfoListBufferQueue = new Queue<List<Stock.StockTradeInfo>>();

        public bool IsBusy
        {
            get { return _isBusy; }
        }

        public DateTime LastSavedTime
        {
            get { return _lastSavedTime; }
        }

        private void Save()
        {
            _isBusy = true;

            using (var db = new Entities())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;

                while (true)
                {
                    // 依成交順序，採用批次寫入到資料庫
                    var tradeInfoList = DequeueTradeInfoList();
                    if (tradeInfoList == null)
                        break;

#if DEBUG
                    var sw = new System.Diagnostics.Stopwatch();
                    sw.Restart();
                    sw.Restart();
#endif

                    db.BulkInsert<TradeDetails>(GetTradeList(tradeInfoList));
                    db.BulkInsert<UnOrders>(GetBestTradeList(tradeInfoList));
#if DEBUG
                    var elapsedMilliseconds = sw.ElapsedMilliseconds;
                    System.Diagnostics.Debug.WriteLine("{0} total save time {1}", DateTime.Now.TimeOfDay, elapsedMilliseconds);
                    sw.Stop();
#endif
                    _lastSavedTime = DateTime.Now;

                }
            }

            _isBusy = false;
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

        /// <summary>
        /// 取得最多為 Maximun(預設100) 數量的成交明細，用於分批寫入方式。
        /// </summary>
        /// <returns></returns>
        private List<Stock.StockTradeInfo> DequeueTradeInfoList()
        {
            if (_tradeInfoListBufferQueue.Any())
                return _tradeInfoListBufferQueue.Dequeue();

            return null;
        }

        public void Save(List<Stock.StockTradeInfo> tradeInfoList)
        {
            lock (_tradeInfoListBufferQueue)
            {
                _tradeInfoListBufferQueue.Enqueue(tradeInfoList);
            }

            if (!_isBusy)
            {
                Save();
            }
        }

        public void SaveAsync(List<Stock.StockTradeInfo> tradeInfoList)
        {
            lock (_tradeInfoListBufferQueue)
            {
                _tradeInfoListBufferQueue.Enqueue(tradeInfoList);
            }

            if (!_isBusy)
            {
                System.Threading.Tasks.Task.Run(new Action(Save));
            }
        }



        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

}
