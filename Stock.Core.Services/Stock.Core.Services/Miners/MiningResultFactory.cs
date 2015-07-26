using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock.Core.Services.Interfaces;

namespace Stock.Core.Services.Miners
{
    public class MiningResultFactory
    {
        public static IMiningResult Create(MiningStatus status, int count = 0)
        {
            return new MiningResult() { Status = status, Count = count };
        }

        public static IMiningResult Create(int count)
        {
            return Create(MiningStatus.Succeed, count);
        }
    }
}
