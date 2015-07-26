using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Core.Services.Miners;
using Stock.Core.Services.Interfaces;
using Stock.Core.Domain;
using Stock.Core.Domain.Models;
using Stock.Core.Domain.Interfaces;
using Stock.Core.Domain.Models.Extension;
using RestSharp;

namespace Stock.Core.Services.PChome
{
    public class TradeDetailsMineCar : MineCar<PChomeTradeDetails>
    {

        public override RestSharp.IRestResponse GetResponse()
        {
            var url = string.Format("http://stock.pchome.com.tw/stock/sto0/ock3/sid{0}.html", this.StockID);
            var req = new RestRequest(url).AddParameter("is_check", "1");
            
            Client.Get(req);
            return Client.Post(req);
        }

        public override HtmlAgilityPack.HtmlNode GetTableNode(HtmlAgilityPack.HtmlNode documentNode)
        {
            return documentNode.SelectSingleNode("//table[@id='tb_chart']");
        }

        public override IEnumerable<string> GetTableColumns(HtmlAgilityPack.HtmlNode table)
        {
            var tr = table.Descendants("tr").FirstOrDefault();
            return tr.Descendants("td").Select(n => n.InnerText);
        }

        public override IEnumerable<HtmlAgilityPack.HtmlNode> GetTableRows(HtmlAgilityPack.HtmlNode table)
        {
            return table.Descendants("tr").Skip(1);
        }

        public override IEnumerable<string> GetTableCells(HtmlAgilityPack.HtmlNode tableRow)
        {
            return tableRow.ChildNodes.Where(n => n.Name == "td").Select(n => n.InnerText);
        }

        public override void AddColumnMappings()
        {
            AddMapping(item => item.TradeTime, "時間");
            AddMapping(item => item.BestBidPrice, "買價");
            AddMapping(item => item.BestAskPrice, "賣價");
            AddMapping(item => item.LatestTradePrice, "成交價");
            AddMapping(item => item.Change, "漲跌");
            AddMapping(item => item.TradeVolume, "分量(張)");
            AddMapping(item => item.AccTradeVolume, "累計量(張)");
        }

        public override IEnumerable<PChomeTradeDetails> GetEntitySet(IEnumerable<PChomeTradeDetails> entitySet)
        {
            float highestPrice = 0f;
            float lowestPrice = float.MaxValue;
            float lastTradePrice = 0f;
            TradeStatus lastTradeStatus = TradeStatus.Opening;

            var tradeDetailsOrderybyTime = entitySet.OrderBy(item => item.TradeTime).ToList();
            foreach (var tradeDetail in tradeDetailsOrderybyTime)
            {
                lastTradeStatus = StockUtility.GetTradeStatus(tradeDetail.LatestTradePrice, lastTradePrice, lastTradeStatus);
                lastTradePrice = tradeDetail.LatestTradePrice;

                if (tradeDetail.LatestTradePrice > highestPrice)
                    highestPrice = tradeDetail.LatestTradePrice;

                if (tradeDetail.LatestTradePrice < lowestPrice)
                    lowestPrice = tradeDetail.LatestTradePrice;

                tradeDetail.StockID = StockID;
                tradeDetail.StockStatus = (byte)tradeDetail.Change.GetStockStatus();
                tradeDetail.TradeStatus = (byte)lastTradeStatus;
                tradeDetail.HighestPrice = highestPrice;
                tradeDetail.LowestPrice = lowestPrice;
                tradeDetail.ChangeExtent = tradeDetail.Change.ChangeExtentByTradePrice(tradeDetail.LatestTradePrice);
                tradeDetail.TradeTime = MiningDate.Add(tradeDetail.TradeTime.TimeOfDay);
                tradeDetail.TradeID = tradeDetail.TradeTime.CombineStockID(StockID);
            }
            return tradeDetailsOrderybyTime;
        }

        public override int Clear()
        {
            var endDate = MiningDate.AddDays(1);
            var items = from item in _context.PChomeTradeDetails
                        where item.StockID == StockID
                        where MiningDate <= item.TradeTime && item.TradeTime < endDate
                        select item;

            _context.PChomeTradeDetails.RemoveRange(items);
            return _context.SaveChanges();
        }
    }
}
