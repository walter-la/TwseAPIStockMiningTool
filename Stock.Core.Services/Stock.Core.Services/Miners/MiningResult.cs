using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Core.Services.Interfaces;

namespace Stock.Core.Services.Miners
{
    public class MiningResult : IMiningResult
    {
        public MiningStatus Status { get; internal set; }

        public int Count { get; internal set; }
    }
}
