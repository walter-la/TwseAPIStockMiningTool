using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Stock.Core.Domain.Interfaces;

namespace Stock.Core.Services.TWSE
{
    public class Transformation
    {
        /// <summary>
        /// 將股轉換成千股。
        /// </summary>
        /// <param name="shares"></param>
        /// <returns></returns>
        public static object ConverToThousandShares(string shares)
        {
            int value;
            int.TryParse(shares, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
            return value / 1000;
        }

        /// <summary>
        /// 漲跌(+/-)欄位符號說明:+/-/X表示漲/跌/不比價。
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static object ConvertToStockStatus(string s)
        {
            if (s == "＋")
            {
                return (byte)StockStatus.Rise;
            }
            else if (s == "－")
            {
                return (byte)StockStatus.Fall;
            }
            else if (s == "X")
            {
                return (byte)StockStatus.NA;
            }
            else
            {
                return (byte)StockStatus.Unch;
            }
        }
    }
}
