using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Services.Interfaces
{
    /// <summary>
    /// 礦車原型，礦車可調節挖礦日、編碼、設計讓礦工挖礦的機器(REST)，並具有儲存礦物的能力。
    /// </summary>
    public interface IMineCar : ICompaniesInfo, IDisposable
    {
        /// <summary>
        /// 編碼。
        /// </summary>
        Encoding ContentEncoding { get; set; }
        /// <summary>
        /// 挖礦日。
        /// </summary>
        DateTime MiningDate { get; set; }
        /// <summary>
        /// 股票代號參數。
        /// </summary>
        int StockID { get; set; }
        /// <summary>
        /// 挖礦工具。
        /// </summary>
        RestSharp.IRestClient Client { get; set; }
        /// <summary>
        /// 設計挖礦流程。
        /// </summary>
        /// <returns></returns>
        RestSharp.IRestResponse GetResponse();
        /// <summary>
        /// 驗證礦車參數。
        /// </summary>
        void Validate();
        /// <summary>
        /// 存放礦物。
        /// </summary>
        /// <param name="htmlDocument"></param>
        IMiningResult Save(HtmlAgilityPack.HtmlDocument htmlDocument);
        /// <summary>
        /// 清除礦物。
        /// </summary>
        int Clear();

#if DEBUG
        /// <summary>
        /// 產生對應欄位的程式碼，該方法在未來需改成 T4
        /// </summary>
        /// <param name="file"></param>
        void ColumnMappingsCodeGen(string file);
#endif
    }
}
