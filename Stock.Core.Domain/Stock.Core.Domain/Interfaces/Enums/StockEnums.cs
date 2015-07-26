using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Domain.Interfaces
{
    /// <summary>
    /// 當前股價與最後一次不同的成交價的漲幅關係。
    /// </summary>
    public enum TradeStatus : byte
    {
        /// <summary>
        /// 開盤
        /// </summary>
        Opening = 0,
        /// <summary>
        /// 漲
        /// </summary>
        Rise = 1,
        /// <summary>
        /// 跌
        /// </summary>
        Fall = 2
    }

    /// <summary>
    /// 當前股價與昨收價格的漲幅關係。
    /// </summary>
    public enum StockStatus : byte
    {
        /// <summary>
        /// 平 0
        /// </summary>
        Unch = 0,
        /// <summary>
        /// 漲 1
        /// </summary>
        Rise = 1,
        /// <summary>
        /// 跌 2
        /// </summary>
        Fall = 2,
        ///// <summary>
        ///// 暫緩收盤股票
        ///// </summary>
        //Postpone = 3
        /// <summary>
        /// 無比價(含當日除權、除息、新上市、恢復交易者) 4
        /// </summary>
        NA = 4
    }

}
