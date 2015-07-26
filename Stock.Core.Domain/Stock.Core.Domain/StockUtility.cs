using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stock.Core.Domain.Interfaces;

namespace Stock.Core.Domain
{
    public static class StockUtility
    {
        /// <summary>
        /// 是否為台灣的上市公司股票代號。
        /// </summary>
        /// <param name="stockID"></param>
        /// <returns></returns>
        public static bool IsTaiwanListedCompanies(this int stockID)
        {
            return 1000 < stockID && stockID < 9999;
        }
        /// <summary>
        /// 計算漲幅價格。
        /// </summary>
        /// <param name="latestTradePrice">最新成交價</param>
        /// <param name="prevClosePrice">昨收</param>
        /// <returns></returns>
        public static float CalculateChange(float latestTradePrice, float prevClosePrice)
        {
            return (float)((decimal)latestTradePrice - (decimal)prevClosePrice);
        }

        /// <summary>
        /// 計算昨收價格。
        /// </summary>
        /// <param name="latestTradePrice">最新成交價</param>
        /// <param name="change">漲幅價格</param>
        /// <returns></returns>
        public static float CalculatePrevClosePrice(float latestTradePrice, float change)
        {
            return (float)((decimal)latestTradePrice - (decimal)change);
        }

        /// <summary>
        /// 計算漲跌幅百分比值(0~100%)。
        /// </summary>
        /// <param name="change">漲幅價格</param>
        /// <param name="prevClosePrice">成交價</param>
        /// <returns></returns>
        public static float ChangeExtentByTradePrice(this float change, float tradePrice)
        {
            return ChangeExtentByPrevClosePrice(change, (decimal)tradePrice - (decimal)change);
        }

        /// <summary>
        /// 計算漲跌幅百分比值(0~100%)。
        /// </summary>
        /// <param name="change">漲幅價格</param>
        /// <param name="prevClosePrice">昨收</param>
        /// <returns></returns>
        public static float ChangeExtentByPrevClosePrice(this float change, float prevClosePrice)
        {
            return ChangeExtentByPrevClosePrice(change, prevClosePrice);
        }

        /// <summary>
        /// 計算漲跌幅百分比值(0~100%)。
        /// </summary>
        /// <param name="prevClosePrice">昨收</param>
        /// <param name="change">漲幅價格</param>
        /// <returns></returns>
        public static float ChangeExtentByPrevClosePrice(this  float change, decimal prevClosePrice)
        {
            return (float)Math.Round((1M / prevClosePrice) * (decimal)(change * 100), 2);
        }

        /// <summary>
        /// 取得昨收後的漲跌狀態。
        /// </summary>
        /// <param name="change">漲幅價格</param>
        /// <returns></returns>
        public static StockStatus GetStockStatus(this float change)
        {
            if (change > 0)
                return StockStatus.Rise;
            else if (change < 0)
                return StockStatus.Fall;
            else
                return StockStatus.Unch;
        }

        /// <summary>
        /// 取得成交後的漲幅狀態。
        /// </summary>
        /// <param name="latestTradePrice">最新成交價</param>
        /// <param name="lastTradePrice">上一個成交價</param>
        /// <param name="lastTradeStatus">上一個成交後的漲幅狀態</param>
        /// <returns></returns>
        public static TradeStatus GetTradeStatus(float latestTradePrice, float lastTradePrice, TradeStatus lastTradeStatus)
        {
            if (latestTradePrice == lastTradePrice)
                return lastTradeStatus;
            else if (latestTradePrice > lastTradePrice)
                return TradeStatus.Rise;
            else if (latestTradePrice < lastTradePrice)
                return TradeStatus.Fall;
            else
                return TradeStatus.Opening;
        }
    }
}
