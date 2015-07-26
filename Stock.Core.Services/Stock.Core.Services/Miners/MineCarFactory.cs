using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Core.Domain.Models;
using Stock.Core.Services.Interfaces;
using Stock.Core.Services.TWSE;
using Stock.Core.Services.PChome;

namespace Stock.Core.Services.Miners
{
    /// <summary>
    /// 礦車工廠，給予礦工裝載礦坑路線(REST)、礦種取得(Parser)、裝載礦物到礦車(Save Entity Set)。
    /// </summary>
    public class MineCarFactory
    {
        /// <summary>
        /// 建立該實體可以用的擴車，每當新增礦車時，在這邊告知工廠可引進新礦車。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static IMineCar Create<TEntity>()
        {
            var entityType = typeof(TEntity);

            if (entityType == typeof(DailyQuotes))
            {
                return new DailyQuotesMineCar();
            }
            else if (entityType == typeof(PChomeTradeDetails))
            {
                return new TradeDetailsMineCar() { ContentEncoding = Encoding.UTF8 };
            }
            else if (entityType == typeof(ForeignDailyTradeDetails))
            {
                return new 三大法人買賣超日報();
            }
            
            return default(IMineCar);
        }
    }
}
