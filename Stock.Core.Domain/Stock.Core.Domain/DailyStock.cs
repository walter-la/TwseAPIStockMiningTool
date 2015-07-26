using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Core.Domain.Models;
using Stock.Core.Domain.Models.Extension;
using Stock.Core.Domain.Interfaces;
namespace Stock.Core.Domain
{
    public class DailyStock
    {
        /// <summary>
        /// 產生每日的明細
        /// </summary>
        /// <param name="CurDateTime">指定特定時間重建(只抓年/月/日)</param>
        public static void GenDailyQuotes(DateTime? CurDate)
        {
            DateTime curDate = new DateTime();
            if (CurDate.HasValue)
                curDate = CurDate.Value.Date;
            else
                curDate = DateTime.Now.Date;

            using (Entities db = new Entities())
            {
                //db.Configuration.AutoDetectChangesEnabled = false;
                //db.Configuration.ValidateOnSaveEnabled = false;

                var tradeList = from trade in db.TradeDetails
                                where trade.TradeTime.Year == curDate.Year &&
                                      trade.TradeTime.Month == curDate.Month &&
                                      trade.TradeTime.Day == curDate.Day
                                select trade;
                db.DailyQuotes.AddRange(CreateDailyQuotes(tradeList, curDate));
                db.SaveChanges();
            }
        }

        public static IEnumerable<DailyQuotes> CreateDailyQuotes(IEnumerable<TradeDetails> tradeList,DateTime CurDate)
        {
            IEnumerable<DailyQuotes> dailyQuotes = from item in tradeList
                                                   group item by item.StockID into g
                                                   let PrevClosePrice=CalPrevClosePrice(g.Key,CurDate) //收盤價
                                                   let change = (float)((decimal)g.Select(p => p.LatestTradePrice).Max() - (decimal)PrevClosePrice)//漲跌幅
                                                   let AccTradeVolume=g.Select(p=>p.AccTradeVolume).Max() 
                                                   let BuyVolume=g.Where(p=>p.StockStatus==(byte)TradeStatus.Rise).Sum(p=>p.TradeVolume)//外盤
                                                   let SellVolume=g.Where(p=>p.StockStatus==(byte)TradeStatus.Fall).Sum(p=>p.TradeVolume)//內盤
                                                   let OpeningPrice=g.Where(p=>p.HighestPrice>0)
                                                   select new DailyQuotes 
                                                   {
                                                       DailyQuotesID=CurDate.CombineStockID(g.Key),//流水號為StockID+DateTime(年月日)去計算秒數
                                                       HighestPrice = g.Select(p => p.HighestPrice).Max(),//最高價
                                                       LowestPrice = g.Select(p => p.LowestPrice).Min(),//最低價
                                                       OpeningPrice = OpeningPrice.Any()?OpeningPrice.FirstOrDefault().HighestPrice:0,//找出第一筆HighestPrice(最高價)非0的值作為開盤價
                                                       PrevClosePrice = PrevClosePrice,
                                                       AccTradeShares = AccTradeVolume * 1000,  // 轉成股
                                                       LatestTradePrice = g.Select(p => p.LatestTradePrice).Last(),//收盤成交價
                                                       TradeVolume = g.Select(p => p.TradeVolume).Last(),//收盤最後一筆成交量
                                                       Change = change,//漲跌價
                                                       ChangeExtent =(change==0||PrevClosePrice==0)?0: (float)Math.Round((1M / (decimal)PrevClosePrice) * (decimal)(change * 100), 2),//漲跌幅
                                                       AveragePrice = (AccTradeVolume == 0) ? 0 : (float)Math.Round(g.Sum(p => p.LatestTradePrice) / AccTradeVolume, 2),//均價  AveragePrice=SUM(LatestTradePrice)/AccTradeVolume
                                                       TradeValue = (decimal)Math.Round(g.Select(p => p.TradeVolume * p.LatestTradePrice).Sum(), 2),//總量
                                                       BuyVolume = BuyVolume,//外盤
                                                       SellVolume = SellVolume,//內盤
                                                       BuyVolumeRatio = BuyVolume + SellVolume == 0 ? 0 : BuyVolume / (BuyVolume + SellVolume) * 100,//外盤比
                                                       SellVolumeRatio = BuyVolume + SellVolume == 0 ? 0 : SellVolume / (BuyVolume + SellVolume) * 100//內盤比
                                                   };
            return dailyQuotes;
        }
        /// <summary>
        /// 計算昨日收盤
        /// </summary>
        /// <returns></returns>
        private static float CalPrevClosePrice(int StockID, DateTime CurDate)
        {
            using (Entities db = new Entities())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;
                var TradePrevClosePrice = (from item in db.TradeDetails
                                           where item.TradeTime.Year == CurDate.Year &&
                                                 item.TradeTime.Month == CurDate.Month &&
                                                 item.TradeTime.Day == CurDate.Day &&
                                                item.StockID == StockID
                                           select new {
                                               TradeTime =item.TradeTime,
                                               LatestTradePrice = item.LatestTradePrice
                                           }).ToList();
                if (TradePrevClosePrice.Any())
                {
                    return TradePrevClosePrice.First(p => p.TradeTime == TradePrevClosePrice.Select(tradeTime => tradeTime.TradeTime).Max()).LatestTradePrice;
                }
                else
                    return 0f;
            }
            
        }
    }
}
