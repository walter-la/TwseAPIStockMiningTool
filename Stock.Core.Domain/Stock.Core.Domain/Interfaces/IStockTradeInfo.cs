using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stock.Core.Domain.Interfaces
{
    public interface IStockTradeInfo
    {
        /// <summary>
        /// PK(股票代號+Unix成交總秒數)
        /// </summary>
        decimal TradeID { get; }
        /// <summary>
        /// 股票代號。
        /// </summary>
        int StockID { get; }
        /// <summary>
        /// 最高價
        /// </summary>
        float HighestPrice { get; }
        /// <summary>
        /// 最低價
        /// </summary>
        float LowestPrice { get; }
        /// <summary>
        /// 成交時間
        /// </summary>
        DateTime TradeTime { get; }
        /// <summary>
        /// 成交總量
        /// </summary>
        int AccTradeVolume { get; }
        /// <summary>
        /// 當前股價與最後一次不同的成交價的漲幅關係。
        /// </summary>
        TradeStatus TradeStatus { get; }
        /// <summary>
        /// 當前股價與昨收價格的漲幅關係。
        /// </summary>
        StockStatus StockStatus { get; }
        /// <summary>
        /// 最新成交價。
        /// </summary>
        float LatestTradePrice { get; }
        /// <summary>
        /// 成交量。
        /// </summary>
        int TradeVolume { get; }
        /// <summary>
        /// 揭示買價。
        /// </summary>
        float BestBidPrice { get; }
        /// <summary>
        /// 揭示買量。
        /// </summary>
        int BestBidVolume { get; }
        /// <summary>
        /// 揭示賣價。
        /// </summary>
        float BestAskPrice { get; }
        /// <summary>
        /// 揭示賣量。
        /// </summary>
        int BestAskVolume { get; }
        /// <summary>
        /// 漲跌價差
        /// </summary>
        float Change { get; }
        /// <summary>
        /// 漲跌幅度，例如 3.87%
        /// </summary>
        float ChangeExtent { get; }
    }
}
