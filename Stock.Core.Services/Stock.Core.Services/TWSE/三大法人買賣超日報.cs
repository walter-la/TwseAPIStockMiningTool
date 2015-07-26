using Stock.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Core.Services.Miners;
using Stock.Core.Domain;
using Stock.Core.Domain.Models.Extension;
namespace Stock.Core.Services.TWSE
{
    class 三大法人買賣超日報 : MineCar<ForeignDailyTradeDetails>
    {
        public virtual string RequestUrl
        {
            get
            {
                string url = string.Format("http://www.twse.com.tw/ch/trading/fund/T86/print.php?edition=ch&filename=genpage/{0}/{1}_2by_issue.dat&type=html&select2=ALLBUT0999", this.MiningDate.ToString("yyyyMM"), this.MiningDate.ToString("yyyyMMdd"));
                return url;
            }
        }

        public override void AddColumnMappings()
        {
            AddMapping(item => item.StockID, "證券代號", (s) =>
            {
                if (s.Length == 4)
                {
                    int value;
                    int.TryParse(s, out value);
                    return value;
                }
                return 0;
            });
            AddMapping(item => item.ForeignBuyShares, "外資買進股數");
            AddMapping(item => item.ForeignSellShares, "外資賣出股數");
            AddMapping(item => item.SecuritiesBuyShares, "投信買進股數");
            AddMapping(item => item.SecuritiesSellShares, "投信賣出股數");
            AddMapping(item => item.DealersBuyShares, "自營商買進股數");
            AddMapping(item => item.DealersSellShares, "自營商賣出股數");
            AddMapping(item => item.DifferenceShares, "三大法人買賣超股數");
        }

        public override IEnumerable<ForeignDailyTradeDetails> GetEntitySet(IEnumerable<ForeignDailyTradeDetails> entitySet)
        {
            foreach (var entity in entitySet)
            {
                entity.TradeID = MiningDate.CombineStockID(entity.StockID);
                entity.TradeTime = MiningDate;
            }
            return entitySet.Where(item => item.StockID.IsTaiwanListedCompanies());
        }

        public override RestSharp.IRestResponse GetResponse()
        {
            /* 在此實作取得資料的部份，只需專注於REST */
            var response = Client.Execute(new RestSharp.RestRequest(RequestUrl));
            return response;
        }

        public override HtmlAgilityPack.HtmlNode GetTableNode(HtmlAgilityPack.HtmlNode documentNode)
        {
            return documentNode.SelectSingleNode("//table");
        }

        public override IEnumerable<string> GetTableColumns(HtmlAgilityPack.HtmlNode table)
        {
            var tr = table.Descendants("tr").Skip(1).Take(1).FirstOrDefault();
            return tr.Descendants("td").Select(n => n.InnerText);
        }

        public override IEnumerable<HtmlAgilityPack.HtmlNode> GetTableRows(HtmlAgilityPack.HtmlNode table)
        {
            return table.Descendants("tr").Skip(2);
        }

        public override IEnumerable<string> GetTableCells(HtmlAgilityPack.HtmlNode tableRow)
        {
            return tableRow.ChildNodes.Where(n => n.Name == "td").Select(n => n.InnerText);
        }
    }
}
