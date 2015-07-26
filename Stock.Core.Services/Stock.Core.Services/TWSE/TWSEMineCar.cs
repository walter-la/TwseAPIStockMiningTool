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
    /// <summary>
    /// TWSE網站存取的基底礦車，定義不變的操作流層或描述節點表達式。
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class TWSEMineCar<TEntity> : MineCar<TEntity>
    {
        /// <summary>
        /// 請求網址。從TWSE網站的網頁取得觀察中，只需改變網址參數可導向不同日期的報表，故在基底層加上自訂日期。
        /// </summary>
        /// <returns></returns>
        public abstract string RequestUrl { get; }

        //public abstract string Xpath { get; }
        public override RestSharp.IRestResponse GetResponse()
        {
            /* 在此實作取得資料的部份，只需專注於REST */
            var response = Client.Execute(new RestSharp.RestRequest(RequestUrl));
            return response;
        }

        public override HtmlAgilityPack.HtmlNode GetTableNode(HtmlAgilityPack.HtmlNode documentNode)
        {
            /* 在此實作 XPath 或 Lambda 來描述資料節點位置 */
            //div[1]/div/center/table
            //return documentNode.SelectSingleNode("//div[1]/div/center/table");//給每日報表2004-2005使用
            return documentNode.SelectSingleNode("//div[@id='tbl-containerx']/table");
            //return documentNode.SelectSingleNode(Xpath);
            //var div = documentNode.Descendants("div").Where(n => n.Id == "tbl-containerx").FirstOrDefault();
            //if (div == null)
            //    return null;

            //return div.ChildNodes.Where(t => t.Name == "table").FirstOrDefault();
        }

        public override IEnumerable<string> GetTableColumns(HtmlAgilityPack.HtmlNode table)
        {
            var tr = table.Descendants("tr").Skip(1).Take(1).FirstOrDefault();
            //return tr.Descendants("td").Select(n => n.InnerText);//給每日報表2004-2005使用
            return tr.Descendants("th").Select(n => n.InnerText);
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
