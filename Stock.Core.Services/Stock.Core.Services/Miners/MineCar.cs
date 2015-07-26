using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq.Expressions;
using Stock.Core.Services.Interfaces;
using EntityFramework.BulkInsert.Extensions;
using System.Reflection;
using log4net;

namespace Stock.Core.Services.Miners
{
    /// <summary>
    /// 抽象礦車，具有解析礦物的能力，並且能將礦物裝載儲存。
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class MineCar<TEntity> : ServiceBase, IMineCar, IEntityReader<TEntity>, INodeParser<HtmlAgilityPack.HtmlNode>
    {
        protected readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 儲存來源資料欄位對應實體欄位與轉換方法
        /// </summary>
        protected readonly Dictionary<string, IColumnMapping> _columnMappings = new Dictionary<string, IColumnMapping>();

        /// <summary>
        /// 以表格欄位索引來做實體屬性欄位對應
        /// </summary>
        protected readonly Dictionary<int, IColumnMapping> _indexColumnMappings = new Dictionary<int, IColumnMapping>();

        public Encoding ContentEncoding { get; set; }

        public DateTime MiningDate { get; set; }

        public int StockID { get; set; }

        public RestSharp.IRestClient Client { get; set; }

        public abstract RestSharp.IRestResponse GetResponse();

        /// <summary>
        /// 轉換以索引來做欄位對應，解析資料時依照該索引取得屬性寫入資料。
        /// </summary>
        /// <param name="tableColumns"></param>
        /// <returns></returns>
        public virtual void ConvertToIndexColumnMappings(IEnumerable<string> tableColumns)
        {
            var columnNames = tableColumns.ToArray();
            _indexColumnMappings.Clear();

            for (int i = 0; i < columnNames.Length; i++)
            {
                var columnName = columnNames[i];
                if (_columnMappings.ContainsKey(columnName))
                {
                    _indexColumnMappings.Add(i, _columnMappings[columnName]);
                }
            }
        }

        public abstract HtmlAgilityPack.HtmlNode GetTableNode(HtmlAgilityPack.HtmlNode documentNode);

        public abstract IEnumerable<string> GetTableColumns(HtmlAgilityPack.HtmlNode table);

        public abstract IEnumerable<HtmlAgilityPack.HtmlNode> GetTableRows(HtmlAgilityPack.HtmlNode table);

        public abstract IEnumerable<string> GetTableCells(HtmlAgilityPack.HtmlNode tableRow);

        /// <summary>
        /// 增加實體欄位與來源資料欄位名稱對應與轉換方法。
        /// </summary>
        /// <param name="property"></param>
        /// <param name="column"></param>
        /// <param name="transformation"></param>
        internal virtual void AddMapping(Expression<Func<TEntity, object>> property, string column, Func<string, object> transformation = null)
        {
            if (!_columnMappings.ContainsKey(column))
            {
                _columnMappings.Add(column, ColumnMapping.Create<TEntity>(property, transformation));
            }
        }

        public abstract void AddColumnMappings();

        public virtual IEnumerable<TEntity> ReadAll(HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            var tableNode = GetTableNode(htmlDocument.DocumentNode);
            if (tableNode == null)
            {
                _log.ErrorFormat("Html or Table node expression error. StockID: {0}, Date: {1}", StockID, MiningDate);
                yield break;
            }

            var tableColumns = GetTableColumns(tableNode);
            if (tableColumns == null)
            {
                _log.ErrorFormat("Table columns expression error. StockID: {0}, Date: {1}", StockID, MiningDate);
                yield break;
            }

            ConvertToIndexColumnMappings(tableColumns);
            if (!_indexColumnMappings.Any())
            {
                _log.ErrorFormat("Table columns mapppings fialed. StockID: {0}, Date: {1}", StockID, MiningDate);
                yield break;
            }

            foreach (var rowNode in GetTableRows(tableNode))
            {
                var entity = Activator.CreateInstance<TEntity>();

                foreach (var cell in GetTableCells(rowNode).Select((text, index) => new { text, index }))
                {
                    if (_indexColumnMappings.ContainsKey(cell.index))
                    {
                        var columnMapping = _indexColumnMappings[cell.index];
                        columnMapping.Property.SetValue(entity, columnMapping.Transformation(cell.text), null);
                    }
                }

                yield return entity;
            }
        }

        public abstract IEnumerable<TEntity> GetEntitySet(IEnumerable<TEntity> entitySet);

        public virtual void Validate()
        {
            // 預設以TWSE 臺灣證券交易所的編碼，不同網站可實作不同工廠來做編碼初始化
            if (ContentEncoding == null)
                ContentEncoding = Encoding.Default;

            // 預設為今日的資料
            if (MiningDate == default(DateTime))
                MiningDate = DateTime.Now.Date;
        }

        public virtual IMiningResult Save(HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            // Step 1. Html Table Columns Mapping Entity Fields
            AddColumnMappings();

            // Step 2. Covnert Html Table To Entity Set
            var entitySet = ReadAll(htmlDocument).ToList();
            if (!_indexColumnMappings.Any())
            {
                return MiningResultFactory.Create(MiningStatus.ContentParseError);
            }

            // Step 3. Handle PK, Status etc...
            entitySet = GetEntitySet(entitySet).ToList();
            if (!entitySet.Any())
            {
                return MiningResultFactory.Create(MiningStatus.EntitySetIsEmpty);
            }

            // Step 4. Bulk Insert Entity Set
            try
            {
                _context.BulkInsert<TEntity>(entitySet);
                return MiningResultFactory.Create(entitySet.Count);
            }
            catch(Exception ex)
            {
                _log.Error(string.Format("SaveEntitySetException. StockID: {0}, Date: {1}", StockID, MiningDate), ex);
                return MiningResultFactory.Create(MiningStatus.SaveEntitySetException);
            }
        }

        public virtual int Clear()
        {
            return 0;
        }

#if DEBUG
        public virtual void ColumnMappingsCodeGen(string file)
        {
            // TODO [T4]: ColumnMappingsCodeGen

            Validate();
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.Load(file, ContentEncoding);

            var tableNode = GetTableNode(htmlDocument.DocumentNode);
            var tableColumns = GetTableColumns(tableNode);

            foreach (var colName in tableColumns)
            {
                System.Diagnostics.Debug.WriteLine(string.Format(@"AddMapping(item => item, ""{0}"");", colName));
            }
        }
#endif


    }
}
