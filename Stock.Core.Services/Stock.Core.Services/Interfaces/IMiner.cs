using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Core.Services.Interfaces
{
    /// <summary>
    /// 礦工，設計挖礦流程，並實際挖礦放到配附的礦車中存放。
    /// </summary>
    public interface IMiner : IDisposable
    {
        MiningStatus Status { get; }
        /// <summary>
        /// 不同礦種的礦車。
        /// </summary>
        IMineCar MineCar { get; }
        /// <summary>
        /// 開始挖礦，這年頭挖礦傷身體，努力挖礦吧。
        /// </summary>
        IMiningResult Mining();
    }
}
