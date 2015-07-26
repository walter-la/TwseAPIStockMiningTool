using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Stock
{
    public class StockInfoDataFileGenerator : IStockInfoDataGenerator
    {
        public string FullFilePath { get; private set; }

        public StockInfoDataFileGenerator(string fullFilePath)
        {
            this.FullFilePath = fullFilePath;
        }

        public string GenerateGetAllStockInfoData()
        {
            var lines = File.ReadAllLines(FullFilePath);
            return StockTrade.CombinStockInfoDataFormatString<string>(lines);
        }
    }
}
