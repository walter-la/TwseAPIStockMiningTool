using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock
{
    public interface IStockInfoDbService:IDisposable
    {
        bool IsBusy { get; }

        DateTime LastSavedTime { get; }

        void Save(List<StockTradeInfo> tradeInfoList);

        void SaveAsync(List<StockTradeInfo> tradeInfoList);

        void Dispose();
    }
}
