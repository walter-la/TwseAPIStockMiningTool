using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Core.Domain.Interfaces;
using Stock.Core.Domain.Models.Extension;
namespace Stock.Core.Domain
{
    public class StockTradeDetail
    {
        /// <summary>
        /// 股票代號
        /// </summary>
        public int c { get; set; }
        /// <summary>
        /// 股票名稱
        /// </summary>
        public string n { get; set; }
        /// <summary>
        /// 最高價
        /// </summary>
        public float h { get; set; }
        /// <summary>
        /// 最低價
        /// </summary>
        public float l { get; set; }
        /// <summary>
        /// 揭示時間
        /// </summary>
        public DateTime t { get; set; }
        /// <summary>
        /// 暫停交易時間
        /// </summary>
        public DateTime st { get; set; }
        /// <summary>
        /// 恢復交易時間
        /// </summary>
        public DateTime rt { get; set; }
        /// <summary>
        /// 3 暫緩收盤, 2 趨漲↑, 1 趨跌↓, 0 平盤
        /// </summary>
        public int ip { get; set; }
        /// <summary>
        /// 開盤
        /// </summary>
        public float o { get; set; }
        /// <summary>
        /// 昨收, '-' 沒有
        /// </summary>
        public string y { get; set; }
        /// <summary>
        /// 累積成交量
        /// </summary>
        public int v { get; set; }
        /// <summary>
        /// 最近成交價, '-' 沒有
        /// </summary>
        public string z { get; set; }
        /// <summary>
        /// 當盤成交量, 不一定有
        /// </summary>
        public string tv { get; set; }
        /// <summary>
        /// 揭示買價 用'_'來做分割字元一次有1~5個
        /// </summary>
        public string b { get; set; }
        /// <summary>
        /// 揭示買量 用'_'來做分割字元一次有1~5個
        /// </summary>
        public string g { get; set; }
        /// <summary>
        /// 揭示賣價 用'_'來做分割字元一次有1~5個
        /// </summary>
        public string a { get; set; }
        /// <summary>
        /// 揭示賣量 用'_'來做分割字元一次有1~5個
        /// </summary>
        public string f { get; set; }
        /// <summary>
        /// 漲停價
        /// </summary>
        public float u { get; set; }
        /// <summary>
        /// 跌停價
        /// </summary>
        public float w { get; set; }
        /// <summary>
        /// 盤後揭示時間
        /// </summary>
        public DateTime ot { get; set; }
        /// <summary>
        /// 盤後零股累積成交量
        /// </summary>
        public string ov { get; set; }
        /// <summary>
        /// 盤後委買價
        /// </summary>
        public string oa { get; set; }
        /// <summary>
        /// 盤後委賣價
        /// </summary>
        public string ob { get; set; }

        public float PrevClosePrice
        {
            get
            {
                float price;
                if (float.TryParse(y, out price))
                    return price;

                return 0f;
            }
        }

        public int OddLotAccTradeVolume
        {
            get
            {
                int volume;
                if (int.TryParse(ov, out volume))
                    return volume;

                return 0;
            }
        }

        public float OddLotBestBidPrice
        {
            get
            {
                float price;
                if (float.TryParse(oa, out price))
                    return price;

                return 0f;
            }
        }

        public float OddLotBestAskPrice
        {
            get
            {
                float price;
                if (float.TryParse(ob, out price))
                    return price;

                return 0f;
            }
        }

        /// <summary>
        /// 取得最佳五檔委買價格，預設已排序由大到小。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<float> GetBestBidPrices()
        {
            return ConvertToPrices(b);
        }

        /// <summary>
        /// 取得最佳五檔委買，預設已排序與最佳五檔委買價格對應。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetBestBidVolumes()
        {
            return ConvertToVolumes(g);
        }

        /// <summary>
        /// 取得最佳五檔委賣價格，預設已排序由小到大。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<float> GetBestAskPrices()
        {
            return ConvertToPrices(a);
        }

        /// <summary>
        /// 取得最佳五檔委賣，預設已排序與最佳五檔委賣價格對應。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetBestAskVolumes()
        {
            return ConvertToVolumes(f);
        }

        private static IEnumerable<float> ConvertToPrices(string s)
        {
            return SplitBaseline(s).Select(item => float.Parse(item));
        }

        private static IEnumerable<int> ConvertToVolumes(string s)
        {
            return SplitBaseline(s).Select(item => int.Parse(item));
        }

        private static string[] SplitBaseline(string s)
        {
            // '-' 表示漲停或跌停，不在有委買賣價格的出現。
            return (s ?? "").Replace("-", "0").Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 設定新的成交價與上個成交價的漲幅狀態。
        /// </summary>
        /// <param name="newInfo"></param>
        /// <param name="lastInfo"></param>
        /// <returns></returns>
        public void SetTradeStatus(StockTradeDetail lastInfo)
        {
            if (lastInfo == null)
            {
                this.TradeStatus = TradeStatus.Opening;
            }
            else if (LatestTradePrice > lastInfo.LatestTradePrice)
            {
                this.TradeStatus = TradeStatus.Rise;
            }
            else if (LatestTradePrice < lastInfo.LatestTradePrice)
            {
                this.TradeStatus = TradeStatus.Fall;
            }
            else
            {
                this.TradeStatus = lastInfo.TradeStatus;
            }
        }

        /// <summary>
        /// 股票代碼(4) + Unix Time 總秒數(10)，共 14 碼
        /// </summary>
        public decimal TradeID
        {
            get { return t.CombineStockID(c); }
        }

        public int StockID
        {
            get { return c; }
        }

        public float HighestPrice
        {
            get { return h; }
        }

        public float LowestPrice
        {
            get { return l; }
        }

        public DateTime TradeTime
        {
            get { return t; }
        }

        public int AccTradeVolume
        {
            get { return v; }
        }

        /// <summary>
        /// 當前股價與最後一次不同的成交價的漲幅關係。
        /// </summary>
        public TradeStatus TradeStatus { get; private set; }

        /// <summary>
        /// 當前股價與昨收價格的漲幅關係。
        /// </summary>
        public StockStatus StockStatus
        {
            get
            {
                var latestTradePrice = LatestTradePrice;
                var prevClosePrice = PrevClosePrice;

                // 若無昨收，則以開盤價替代
                if (prevClosePrice == 0)
                {
                    prevClosePrice = o;
                }

                // 以昨收判斷當前股價漲跌狀態
                if (latestTradePrice > prevClosePrice)
                {
                    return StockStatus.Rise;
                }
                else if (latestTradePrice < prevClosePrice)
                {
                    return StockStatus.Fall;
                }
                else
                {
                    return StockStatus.Unch;
                }
            }
        }

        /// <summary>
        /// 漲跌價差
        /// </summary>
        public float Change
        {
            get
            {
                return (float)((decimal)LatestTradePrice - (decimal)PrevClosePrice);
            }
        }

        /// <summary>
        /// 漲跌幅度，例如 3.87%
        /// </summary>
        public float ChangeExtent
        {
            get
            {
                return (float)Math.Round((1M / (decimal)PrevClosePrice) * (decimal)(Change * 100), 2);
            }
        }

        /// <summary>
        /// 揭示買價。
        /// </summary>
        public float BestBidPrice
        {
            get
            {
                return GetBestBidPrices().FirstOrDefault();
            }
        }

        /// <summary>
        /// 揭示買量。
        /// </summary>
        public int BestBidVolume
        {
            get
            {
                return GetBestBidVolumes().FirstOrDefault();
            }
        }

        /// <summary>
        /// 揭示賣價。
        /// </summary>
        public float BestAskPrice
        {
            get
            {
                return GetBestAskPrices().FirstOrDefault();
            }
        }

        /// <summary>
        /// 揭示賣量。
        /// </summary>
        public int BestAskVolume
        {
            get
            {
                return GetBestAskVolumes().FirstOrDefault();
            }
        }

        public float LatestTradePrice
        {
            get
            {
                float price;
                if (float.TryParse(z, out price))
                    return price;

                return 0f;
            }
        }

        public int TradeVolume
        {
            get
            {
                int volume;
                if (int.TryParse(tv, out volume))
                    return volume;

                return 0;
            }
        }

        public IEnumerable<Stock.Core.Domain.Models.UnOrders> UnOrders()
        {
            // 依照買價、買量、賣價、賣量順序，將這四個列舉物件組成一個為四種屬性的臨時類別
            var bidPairs = GetBestBidPrices().Zip(GetBestBidVolumes(), (bidPrice, bidVolume) => new Tuple<float, int>(bidPrice, bidVolume));
            var askPairs = GetBestAskPrices().Zip(GetBestAskVolumes(), (askPrice, askVolume) => new Tuple<float, int>(askPrice, askVolume));
            var bidAskParis = bidPairs.Zip(askPairs, (bid, ask) => new Tuple<float, int, float, int>(bid.Item1, bid.Item2, ask.Item1, ask.Item2));

            // 將最佳五檔附加於該成交明細上
            return bidAskParis.Select(bidAsk => new Stock.Core.Domain.Models.UnOrders()
            {
                TradeID = TradeID,
                BestBidPrice = bidAsk.Item1,
                BestBidVolume = bidAsk.Item2,
                BestAskPrice = bidAsk.Item3,
                BestAskVolume = bidAsk.Item4
            });
        }

        public Stock.Core.Domain.Models.TradeDetails ToTradeDetail()
        {
            return new Stock.Core.Domain.Models.TradeDetails()
                {
                    AccTradeVolume = AccTradeVolume,
                    BestAskPrice = BestAskPrice,
                    BestAskVolume = BestAskVolume,
                    BestBidPrice = BestBidPrice,
                    BestBidVolume = BestBidVolume,
                    Change = Change,
                    ChangeExtent = ChangeExtent,
                    HighestPrice = HighestPrice,
                    LatestTradePrice = LatestTradePrice,
                    LowestPrice = LowestPrice,
                    StockID = c,
                    StockStatus = (byte)StockStatus,
                    TradeTime = TradeTime,
                    TradeVolume = TradeVolume,
                    TradeID = TradeID,
                    UnOrders = UnOrders().ToList()
                };
        }
    }
}
