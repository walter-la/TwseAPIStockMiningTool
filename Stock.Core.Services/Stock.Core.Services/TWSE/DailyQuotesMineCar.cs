using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Core.Domain.Models;
using Stock.Core.Services.Miners;
using Stock.Core.Domain;
using Stock.Core.Domain.Interfaces;
using Stock.Core.Domain.Models.Extension;

namespace Stock.Core.Services.TWSE
{
    public class DailyQuotesMineCar : TWSEMineCar<DailyQuotes>
    {
        public override string RequestUrl
        {
            get
            {
                // 每日收盤行情 http://www.twse.com.tw/ch/trading/exchange/MI_INDEX/MI_INDEX3_print.php?genpage=genpage/Report201407/A11220140724ALLBUT0999_1.php&type=html
                var url = string.Format("http://www.twse.com.tw/ch/trading/exchange/MI_INDEX/MI_INDEX3_print.php?genpage=genpage/Report{0}/A112{1}ALLBUT0999_1.php&type=html",
                                        MiningDate.ToString("yyyyMM"),
                                        MiningDate.ToString("yyyyMMdd"));
                return url;
            }
        }

        public override void AddColumnMappings()
        {
            /* 在此實作 [實體屬性] 對應 [資料欄位名稱] */

            AddMapping(item => item.StockID, "證券代號", (s) => 
            {
                if (s.Trim().Length == 4)
                {
                    int value;
                    int.TryParse(s, out value);
                    return value;
                }
                else
                    return 0;
            });
            AddMapping(item => item.AccTradeShares, "成交股數");
            AddMapping(item => item.TradeCount, "成交筆數");
            AddMapping(item => item.TradeValue, "成交金額");
            AddMapping(item => item.OpeningPrice, "開盤價");
            AddMapping(item => item.HighestPrice, "最高價");
            AddMapping(item => item.LowestPrice, "最低價");
            AddMapping(item => item.LatestTradePrice, "收盤價");
            AddMapping(item => item.Change, "漲跌價差");
            AddMapping(item => item.StockStatus, "漲跌(+/-)", TWSE.Transformation.ConvertToStockStatus);
        }

        public override IEnumerable<DailyQuotes> GetEntitySet(IEnumerable<DailyQuotes> entitySet)
        {
            /* 在此實作尚未含有資料的實體屬性，例如TradeID或者需要過濾的條件 */

            foreach (var entity in entitySet)
            {
                entity.DailyQuotesID = MiningDate.CombineStockID(entity.StockID);
                entity.DailyDate = MiningDate;

                // 漲跌價差只提供正數，轉換成漲幅價格需包含有號符號。
                if (entity.StockStatus == (byte)StockStatus.Fall)
                {
                    entity.Change *= -1;
                }
            }
            return entitySet.Where(item => item.StockID.IsTaiwanListedCompanies());
        }

    }
}
