using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Stock.Core.Services.Interfaces;

namespace Stock.Core.Services.Miners
{
    /// <summary>
    /// 礦工工廠，簡易礦工只會挖掘礦物，每一個礦工會配一台礦車。可建立不同工廠於不同網站，做為該網站的初始化資料。
    /// </summary>
    public class MinerFactory : IMinerFactory
    {
        public CookieContainer Cookies { get; private set; }

        public MinerFactory()
        {
            Cookies = new CookieContainer();
        }

        /// <summary>
        /// 產生礦工，並配一台可挖掘該實體的礦車。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IMiner Create<TEntity>()
        {
            var mineCar = MineCarFactory.Create<TEntity>();

            mineCar.MiningDate = DateTime.Now.Date;
            mineCar.Client = new RestSharp.RestClient()
            {
                CookieContainer = Cookies,
                UserAgent = "Mozilla/5.0",
            };

            return new Miner()
            {
                MineCar = mineCar
            };
        }
    }
}
