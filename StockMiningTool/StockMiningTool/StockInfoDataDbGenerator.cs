using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Core.Domain.Models;
namespace StockMiningTool
{
    public class StockInfoDataDbGenerator : Stock.IStockInfoDataGenerator
    {
        public string GenerateGetAllStockInfoData()
        {
            using (var db = new Entities())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var lines = db.Companies.Select(item => item.StockID).ToArray();
                return Stock.StockTrade.CombinStockInfoDataFormatString<int>(lines);
            }
        }
    }
}
