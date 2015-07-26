using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using log4net;
using RestSharp;
using Stock.Core.Services.Interfaces;

namespace Stock.Core.Services.Miners
{
    /// <summary>
    /// 簡易礦工，只會挖礦物到礦車上。
    /// </summary>
    public class Miner : IMiner
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MiningStatus Status { get; internal set; }

        public IMineCar MineCar { get; internal set; }

        public virtual IMiningResult Mining()
        {
            MineCar.Validate();

            Status = MiningStatus.GetResponding;
            var response = MineCar.GetResponse();

            if (!CheckException(response))
                return MiningResultFactory.Create(Status = MiningStatus.ResponseException);

            var htmlDocument = ConverToHtmlDocument(response);
            var miningResult = MineCar.Save(htmlDocument);

            Status = miningResult.Status;
            return miningResult;
        }

        internal bool CheckException(RestSharp.IRestResponse response)
        {
            if (response.ErrorException != null)
            {
                _log.Error(string.Format("Response error. StockID: {0}, Date: {1}", MineCar.StockID, MineCar.MiningDate), response.ErrorException);
                return false;
            }
            else if (!response.RawBytes.Any())
            {
                _log.ErrorFormat("RawBytes is empty. StockID: {0}, Date: {1}", MineCar.StockID, MineCar.MiningDate);
                return false;
            }

            return true;
        }

        internal HtmlAgilityPack.HtmlDocument ConverToHtmlDocument(RestSharp.IRestResponse response)
        {
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(MineCar.ContentEncoding.GetString(response.RawBytes));
            return htmlDocument;
        }


        public void Dispose()
        {
            using (MineCar) { }
        }
    }
}
