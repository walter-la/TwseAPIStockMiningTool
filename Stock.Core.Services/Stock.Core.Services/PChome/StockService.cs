using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using RestSharp;
using HtmlAgilityPack;
using Stock.Core.Domain;
using Stock.Core.Domain.Models;
using Stock.Core.Domain.Models.Extension;
using Stock.Core.Domain.Interfaces;
using EntityFramework.BulkInsert.Extensions;

namespace Stock.Core.Services.PChome
{
    /// <summary>
    /// PChome Stock 取得交易資料服務。
    /// </summary>
    public class StockService : ServiceBase
    {
        /// <summary>
        /// 剩餘要抓取的股票代號，由小到大排序。
        /// </summary>
        public int[] AllStockID { get; private set; }
        /// <summary>
        /// 交易日期。
        /// </summary>
        public DateTime TradeDate { get; private set; }

        public StockService(DateTime tradeDate)
        {
            TradeDate = tradeDate.Date;
            InitMiningAllStockID();
        }

        /// <summary>
        /// 初始化要抓取的股票代號。
        /// </summary>
        private void InitMiningAllStockID()
        {
            var lastMiningStockID = GetLastMiningStockID();

            AllStockID = _context.Companies.Where(c => c.StockID > lastMiningStockID).Select(c => c.StockID).OrderBy(id => id).ToArray();
        }

        /// <summary>
        /// 取得最後一次抓取的股票代號。
        /// </summary>
        /// <returns></returns>
        private int GetLastMiningStockID()
        {
            var lastMiningStockID = (from td in _context.PChomeTradeDetails
                                     where td.TradeTime.Year == TradeDate.Year
                                     where td.TradeTime.Month == TradeDate.Month
                                     where td.TradeTime.Day == TradeDate.Day
                                     orderby td.StockID descending
                                     select td.StockID).FirstOrDefault();
            return lastMiningStockID;
        }

        public int Mining(int stockID)
        {
            // 取得個股 Html
            var response = GetRestResponse(stockID);

            // 驗證 Html
            var tradeDetailsHtmlNode = GetTradeDetailsHtmlNode(response);
            if (tradeDetailsHtmlNode == null)
                return -1;

            // 將 Html 轉換為明細
            var tradeDetails = GetMachiningTradeDetails(stockID, tradeDetailsHtmlNode);

            // 儲存明細
            SaveTradeDetails(tradeDetails);

            // 回傳搓合次數總數
            return tradeDetails.Count;
        }

        /// <summary>
        /// 取得PChome股市個股成交明細頁面。
        /// </summary>
        /// <param name="stockID"></param>
        /// <returns></returns>
        private IRestResponse GetRestResponse(int stockID)
        {
            //"http://stock.pchome.com.tw/stock/sto0/ock3/sid3481.html"
            var url = string.Format("http://stock.pchome.com.tw/stock/sto0/ock3/sid{0}.html", stockID);

            var clinet = new RestClient(url)
            {
                CookieContainer = new CookieContainer()
            };

            var req = new RestRequest().AddParameter("is_check", "1");

            clinet.Get(req);
            return clinet.Post(req);
        }

        /// <summary>
        /// 取得明細節點，若無法取得，則傳回 null。
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private HtmlNode GetTradeDetailsHtmlNode(IRestResponse response)
        {
            if (string.IsNullOrEmpty(response.Content) || response.ErrorException != null)
                return null;

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response.Content);
            return htmlDocument.DocumentNode.Descendants("table").FirstOrDefault(n => n.GetAttributeValue("id", "") == "tb_chart");
        }

        /// <summary>
        /// 取得完整成交明細資料。
        /// </summary>
        /// <param name="stockID"></param>
        /// <param name="tradeDetailsHtmlNode"></param>
        /// <returns></returns>
        private List<PChomeTradeDetails> GetMachiningTradeDetails(int stockID, HtmlNode tradeDetailsHtmlNode)
        {
            var tradeDetails = GetRawTradeDetails(tradeDetailsHtmlNode).ToList().OrderBy(t => t.TradeTime).ToList();
            MachiningTradeDetails(tradeDetails, stockID);
            return tradeDetails;
        }

        /// <summary>
        /// 取得原生成交明細資料，不做計算。
        /// </summary>
        /// <param name="tradeDetailsHtmlNode"></param>
        /// <returns></returns>
        private IEnumerable<PChomeTradeDetails> GetRawTradeDetails(HtmlNode tradeDetailsHtmlNode)
        {
            foreach (var trNode in tradeDetailsHtmlNode.Descendants("tr").Skip(1))
            {
                // [時間, 買價, 賣價, 成交價, 漲跌, 分量(張), 累計量(張)]
                var tdQuery = trNode.Descendants("td");
                var tradeTime = TradeDate.Add(TimeSpan.Parse(tdQuery.First().InnerText));
                float bidPrice;
                float.TryParse(tdQuery.Skip(1).First().InnerText, out bidPrice);
                float askPrice;
                float.TryParse(tdQuery.Skip(2).First().InnerText, out askPrice);
                float latestTradePrice;
                float.TryParse(tdQuery.Skip(3).First().InnerText, out latestTradePrice);
                float change;
                float.TryParse(tdQuery.Skip(4).First().InnerText, out change);
                int tradeVolume;
                int.TryParse(tdQuery.Skip(5).First().InnerText, out tradeVolume);
                int accTradeVolume;
                int.TryParse(tdQuery.Skip(6).First().InnerText, out accTradeVolume);

                yield return new PChomeTradeDetails()
                {
                    BestBidPrice = bidPrice,
                    BestAskPrice = askPrice,
                    Change = change,
                    AccTradeVolume = accTradeVolume,
                    TradeVolume = tradeVolume,
                    TradeTime = tradeTime,
                    LatestTradePrice = latestTradePrice
                };
            }
        }

        /// <summary>
        /// 將原生成交明細資料計算出其它相關明細欄位。
        /// </summary>
        /// <param name="tradeDetails"></param>
        /// <param name="stockID"></param>
        private void MachiningTradeDetails(IList<PChomeTradeDetails> tradeDetails, int stockID)
        {
            float highestPrice = 0f;
            float lowestPrice = float.MaxValue;
            float lastTradePrice = 0f;
            TradeStatus lastTradeStatus = TradeStatus.Opening;

            foreach (var tradeDetail in tradeDetails)
            {
                lastTradeStatus = StockUtility.GetTradeStatus(tradeDetail.LatestTradePrice, lastTradePrice, lastTradeStatus);
                lastTradePrice = tradeDetail.LatestTradePrice;

                if (tradeDetail.LatestTradePrice > highestPrice)
                    highestPrice = tradeDetail.LatestTradePrice;

                if (tradeDetail.LatestTradePrice < lowestPrice)
                    lowestPrice = tradeDetail.LatestTradePrice;

                tradeDetail.TradeID = tradeDetail.TradeTime.CombineStockID(stockID);
                tradeDetail.StockID = stockID;
                tradeDetail.StockStatus = (byte)tradeDetail.Change.GetStockStatus();
                tradeDetail.TradeStatus = (byte)lastTradeStatus;
                tradeDetail.HighestPrice = highestPrice;
                tradeDetail.LowestPrice = lowestPrice;
                tradeDetail.ChangeExtent = tradeDetail.Change.ChangeExtentByTradePrice(tradeDetail.LatestTradePrice);
            }
        }

        private void SaveTradeDetails(List<PChomeTradeDetails> tradeDetails)
        {
            _context.BulkInsert<PChomeTradeDetails>(tradeDetails);
        }

    }
}
