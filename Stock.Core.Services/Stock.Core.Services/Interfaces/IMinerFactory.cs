using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Stock.Core.Services.Interfaces
{
    /// <summary>
    /// 描述礦工工廠，每一個礦坑(網站等資料來源 etc...)所建立專屬的礦工工廠。
    /// </summary>
    public interface IMinerFactory
    {
        CookieContainer Cookies { get; }

        IMiner Create<TEntity>();
    }
}
