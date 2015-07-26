using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Stock.Core.Services.Interfaces
{
    /// <summary>
    /// 欄位對應原型。
    /// </summary>
    public interface IColumnMapping
    {
        /// <summary>
        /// 實體屬性。
        /// </summary>
        PropertyInfo Property { get; }
        /// <summary>
        /// 礦物轉換方法。
        /// </summary>
        Func<string, object> Transformation { get; }
    }
}
