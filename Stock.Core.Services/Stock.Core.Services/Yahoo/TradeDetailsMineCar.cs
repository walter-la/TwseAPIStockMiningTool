using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Core.Domain.Models.Extension;
using Stock.Core.Domain.Models;
using RestSharp;

namespace Stock.Core.Services.Yahoo
{
    public class TradeDetailsMineCar:Miners.MineCar<PChomeTradeDetails>
    {

        public override RestSharp.IRestResponse GetResponse()
        {
            return Client.Get(new RestRequest(string.Format("https://tw.stock.yahoo.com/q/ts?s={0}&t=50", StockID)));
        }

        public override HtmlAgilityPack.HtmlNode GetTableNode(HtmlAgilityPack.HtmlNode documentNode)
        {
            return documentNode.SelectSingleNode("//table[@cellpadding='4']");
        }

        public override IEnumerable<string> GetTableColumns(HtmlAgilityPack.HtmlNode table)
        {
            return table.Descendants("tr").FirstOrDefault().Descendants("td").Select(n => n.InnerText);
        }

        public override IEnumerable<HtmlAgilityPack.HtmlNode> GetTableRows(HtmlAgilityPack.HtmlNode table)
        {
            return table.Descendants("tr").Skip(1);
        }

        public override IEnumerable<string> GetTableCells(HtmlAgilityPack.HtmlNode tableRow)
        {
            return tableRow.Descendants("td").Select(n => n.InnerText);
        }

        public override void AddColumnMappings()
        {
            AddMapping(item => item.TradeTime, "時     間");
            AddMapping(item => item.BestBidPrice, "買     進");
            AddMapping(item => item.BestAskPrice, "賣     出");
            AddMapping(item => item.LatestTradePrice, "成   交   價");
            AddMapping(item => item.Change, "漲     跌");
            AddMapping(item => item.TradeVolume, "單     量(張)");
        }

        public override IEnumerable<PChomeTradeDetails> GetEntitySet(IEnumerable<PChomeTradeDetails> entitySet)
        {
            foreach (var item in entitySet)
            {
                item.TradeID = MiningDate.CombineStockID(StockID);
                item.StockID = StockID;
            }
            return entitySet;
        }

        public bool IsValid
        {
            get
            {
                return _columnMappings.Count == _indexColumnMappings.Count;
            }
        }
    }
}
