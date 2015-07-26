using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Services.Interfaces
{
    /// <summary>
    /// 節點解析器，用於解析能表示為結點的表格資料。
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public interface INodeParser<TNode>
    {
        /// <summary>
        /// 描述取得表格節點的方法。
        /// </summary>
        /// <param name="documentNode"></param>
        /// <returns></returns>
        TNode GetTableNode(TNode documentNode);
        /// <summary>
        /// 描述取得表格所有欄位名稱的方法。
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IEnumerable<string> GetTableColumns(TNode table);
        /// <summary>
        /// 描述取得表格中的每一行節點的方法。
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        IEnumerable<TNode> GetTableRows(TNode table);
        /// <summary>
        /// 描述取得每一列節點中的每一列文字內容方法。
        /// </summary>
        /// <param name="tableRow"></param>
        /// <returns></returns>
        IEnumerable<string> GetTableCells(TNode tableRow);
    }
}
