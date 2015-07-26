using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;

namespace Stock
{
    public class StockTradeObject
    {

        public List<StockTradeInfo> msgArray { get; set; }

        public int userDelay {get;set;}

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string rtmessage { get; set; }

        public string referer { get; set; }

        public StockTradeQueryTime queryTime { get; set; }

        /// <summary>
        /// 0000 表示正常
        /// </summary>
        public string rtcode { get; set; }

        public bool HasError
        {
            get { return rtcode != "0000"; }
        }
    
    }
}
