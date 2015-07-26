using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Services.Interfaces
{
    /// <summary>
    /// 實體讀取器，可以將礦物對應實體欄位後，將礦物轉換為實體資料。
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityReader<TEntity>
    {
        void AddColumnMappings();

        IEnumerable<TEntity> ReadAll(HtmlAgilityPack.HtmlDocument htmlDocument);

        IEnumerable<TEntity> GetEntitySet(IEnumerable<TEntity> entities);

    }
}
