using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stock
{
    public class StockMiner : IRequestObject
    {
        private TimeSpan ServerTime;

        private Stock.StockTrade _stockTrade = null;

        private IStockInfoDbService _stockInfoDbService = null;

        public Dictionary<int, StockTradeInfo> StockInfoDictionary { get; private set; }

        public List<Tuple<DateTime, Exception>> ExceptionList { get; private set; }

        public DateTime LastGetStockInfoTime { get; private set; }

        public DateTime LastSaveStockInfoTime
        {
            get
            {
                if (_stockInfoDbService == null)
                    return default(DateTime);

                return _stockInfoDbService.LastSavedTime;
            }
        }

        /// <summary>
        /// 是否正在取得成交明細或儲存資料中
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return (_stockInfoDbService != null && _stockInfoDbService.IsBusy); //_isBusy ||
            }
        }


        public bool CanCreateRequst
        {
            get { return IsTradeTime(DateTime.Now); }
        }

        public StockMiner()
        {
            this.StockInfoDictionary = new Dictionary<int, StockTradeInfo>();
            this.ExceptionList = new List<Tuple<DateTime, Exception>>();
        }

        public void Initialize(Stock.StockTrade stockTrade, IStockInfoDbService stockInfoDbService)
        {
            if (stockTrade == null)
                throw new ArgumentNullException("stockTrade");

            if (stockInfoDbService == null)
                throw new ArgumentNullException("stockInfoDbService");

            this._stockTrade = stockTrade;
            this._stockInfoDbService = stockInfoDbService;

            // 確保不會判斷昨天的資料
            if (LastGetStockInfoTime.Date != DateTime.Now.Date)
            {
                foreach (var id in stockTrade.GetAllStockId())
                    StockInfoDictionary[id] = null;
            }
        }

        private StockTradeObject GetLatestTradeObject(Stock.StockTrade stockTrade)
        {
            try
            {
                return stockTrade.GetTodayStockInfo();
                //return stockTrade.GetStockInfoByDate(new DateTime(2014, 07, 18));
            }
            catch (System.Net.WebException webEx)
            {
                stockTrade.UpdateUrl();

                if (webEx.Status != System.Net.WebExceptionStatus.Timeout)
                {
                    // NameResolutionFailure
                }

                // 尚未實作例外錯誤紀錄
                ExceptionList.Add(Tuple.Create(DateTime.Now, webEx as Exception));
            }
            catch (Exception ex)
            {
                // 尚未實作例外錯誤紀錄
                ExceptionList.Add(Tuple.Create(DateTime.Now, ex));
            }
            finally
            {
                LastGetStockInfoTime = DateTime.Now;
            }

            return null;
        }

        private void UpadteStockInfoDictionary(List<StockTradeInfo> tradeInfoList)
        {
            foreach (var tradeInfo in tradeInfoList)
            {
                var stockId = tradeInfo.c;

                // 設定成交漲幅
                tradeInfo.SetTradeStatus(StockInfoDictionary[stockId]);

                // 紀錄最後一次的最新成交資訊
                StockInfoDictionary[stockId] = tradeInfo;
            }
        }

        private List<StockTradeInfo> GetChangedTradeInfoList(Stock.StockTradeObject stockTradeObject)
        {
            // 已累積成交量判斷是否有最新的成交資訊
            var tradeInfoList = stockTradeObject.msgArray.Where(info => (StockInfoDictionary[info.c] == null || info.v > StockInfoDictionary[info.c].v)
                                                                && IsTradeTime(info.TradeTime.TimeOfDay))
                                          .AsParallel()
                                          .ToList();
            return tradeInfoList;
        }

        public void Handle()
        {

#if DEBUG
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            sw.Restart();
#endif
            // 取得所有最新的股票資訊
            var stockTradeObject = GetLatestTradeObject(this._stockTrade);

#if DEBUG
            var elapsedMilliseconds = sw.ElapsedMilliseconds;
            System.Diagnostics.Debug.WriteLine("{0} total download time {1}", DateTime.Now.TimeOfDay, elapsedMilliseconds);
            sw.Stop();
#endif
            if (stockTradeObject == null)
            {
                // 網路例外錯誤
                return;
            }

            if (stockTradeObject.HasError)
            {
                // 伺服器錯誤訊息
                return;
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine("{0} system:{1}", DateTime.Now, stockTradeObject.queryTime.sysTime);
#endif

            if (stockTradeObject.queryTime.sysTime < ServerTime)
            {
                // 此次請求延遲很久造成小於上次取得的系統時間
                return;
            }

            // 最後一次取得的系統時間
            this.ServerTime = stockTradeObject.queryTime.sysTime;

            if (!IsTradeTime(this.ServerTime))
            {
                // 過濾07:50:00的重置資料
                return;
            }

            lock (StockInfoDictionary)
            {
                // 取得新的成交明細清單
                var tradeInfoList = GetChangedTradeInfoList(stockTradeObject);

                if (!tradeInfoList.Any())
                {
                    // 沒有新的成交項目
                    return;
                }

                // 更新股票資訊快取
                UpadteStockInfoDictionary(tradeInfoList);

                try
                {
                    // 進行資料儲存
                    this._stockInfoDbService.SaveAsync(tradeInfoList);
                }
                catch (Exception ex)
                {
                    ExceptionList.Add(Tuple.Create(DateTime.Now, ex));
                }
            }
        }

        /// <summary>
        /// 只在星期一到五且早上8:59到下午2:31交易
        /// </summary>
        public bool IsTradeTime(DateTime datetime)
        {
            var timeOfDay = datetime.TimeOfDay;
            var dayOfWeek = datetime.DayOfWeek;
            return IsTradeTime(timeOfDay) && DayOfWeek.Sunday < dayOfWeek && dayOfWeek < DayOfWeek.Saturday;
        }

        public bool IsTradeTime(TimeSpan timeOfDay)
        {
            return new TimeSpan(8, 59, 0) <= timeOfDay && timeOfDay <= new TimeSpan(14, 31, 0);
        }

    }
}
