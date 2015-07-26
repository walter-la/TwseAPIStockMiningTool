using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Domain.Models.Extension
{
    public static class DateTimeExtension
    {

        static readonly DateTime UnixTime = new DateTime(1970, 1, 1);

        public static decimal CombineStockID(this DateTime datetime, int stockID)
        {
            return (stockID * 10000000000) + (long)(datetime.ToUniversalTime() - UnixTime).TotalSeconds;
        }

        public static DateTime GetTradeDateTime(this decimal stockIdDateTime, int stockID)
        {
            return UnixTime.AddSeconds( (double)(stockIdDateTime - (stockID * 10000000000)));
        }
    }
}
