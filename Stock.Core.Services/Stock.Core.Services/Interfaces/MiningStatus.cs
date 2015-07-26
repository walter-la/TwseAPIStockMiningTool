using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Services.Interfaces
{
    public enum MiningStatus
    {
        None,
        /// <summary>
        /// 取得自訂回應中
        /// </summary>
        GetResponding,
        /// <summary>
        /// 網站回應非 OK 的狀態碼
        /// </summary>
        ResponseException,
        /// <summary>
        /// 內容解析錯誤，可能是網站內容錯誤，或者是描述節點或沒有對應欄位
        /// </summary>
        ContentParseError,
        /// <summary>
        /// 最後處理後的實體集合沒有任何項目
        /// </summary>
        EntitySetIsEmpty,
        /// <summary>
        /// 儲存實體集合時，發生例外錯誤
        /// </summary>
        SaveEntitySetException,
        /// <summary>
        /// 儲存完成
        /// </summary>
        Succeed
    }
}
