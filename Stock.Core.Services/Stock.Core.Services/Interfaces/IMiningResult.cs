using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Services.Interfaces
{
    public interface IMiningResult
    {
        /// <summary>
        /// 表示挖礦的狀態。
        /// </summary>
        MiningStatus Status { get; }
        /// <summary>
        /// 表示挖到的總數。
        /// </summary>
        int Count { get; }
    }
}
