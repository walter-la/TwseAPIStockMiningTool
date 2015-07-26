using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Services.Interfaces
{
    /// <summary>
    /// 上市公司的資訊。
    /// </summary>
    public interface ICompaniesInfo
    {
        /// <summary>
        /// 取得所有上市的股票代碼。
        /// </summary>
        /// <returns></returns>
        IList<int> GetAllStockID();
    }
}
