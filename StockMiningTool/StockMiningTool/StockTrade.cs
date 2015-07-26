using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Stock
{
    public class StockTrade
    {
        /// <summary>
        /// 股票資訊的 API 網址。
        /// </summary>
        const string StockInfoUrl = "http://mis.twse.com.tw/stock/api/getStockInfo.jsp";
        /// <summary>
        /// 股票資訊 API 的參數格式字串。 股票代碼若更新，使用 GenerateGetAllStockInfoDataFormatString 來更新此格式字串。
        /// </summary>
        private string _allStockInfoFormatString = "delay=0&ex_ch=tse_1101.tw_{0}|tse_1102.tw_{0}|tse_1103.tw_{0}|tse_1104.tw_{0}|tse_1108.tw_{0}|tse_1109.tw_{0}|tse_1110.tw_{0}|tse_1201.tw_{0}|tse_1203.tw_{0}|tse_1210.tw_{0}|tse_1213.tw_{0}|tse_1215.tw_{0}|tse_1216.tw_{0}|tse_1217.tw_{0}|tse_1218.tw_{0}|tse_1219.tw_{0}|tse_1220.tw_{0}|tse_1225.tw_{0}|tse_1227.tw_{0}|tse_1229.tw_{0}|tse_1231.tw_{0}|tse_1232.tw_{0}|tse_1233.tw_{0}|tse_1234.tw_{0}|tse_1235.tw_{0}|tse_1236.tw_{0}|tse_1262.tw_{0}|tse_1301.tw_{0}|tse_1303.tw_{0}|tse_1304.tw_{0}|tse_1305.tw_{0}|tse_1307.tw_{0}|tse_1308.tw_{0}|tse_1309.tw_{0}|tse_1310.tw_{0}|tse_1312.tw_{0}|tse_1313.tw_{0}|tse_1314.tw_{0}|tse_1315.tw_{0}|tse_1316.tw_{0}|tse_1319.tw_{0}|tse_1321.tw_{0}|tse_1323.tw_{0}|tse_1324.tw_{0}|tse_1325.tw_{0}|tse_1326.tw_{0}|tse_1337.tw_{0}|tse_1338.tw_{0}|tse_1339.tw_{0}|tse_1340.tw_{0}|tse_1402.tw_{0}|tse_1409.tw_{0}|tse_1410.tw_{0}|tse_1413.tw_{0}|tse_1414.tw_{0}|tse_1416.tw_{0}|tse_1417.tw_{0}|tse_1418.tw_{0}|tse_1419.tw_{0}|tse_1423.tw_{0}|tse_1432.tw_{0}|tse_1434.tw_{0}|tse_1435.tw_{0}|tse_1436.tw_{0}|tse_1437.tw_{0}|tse_1438.tw_{0}|tse_1439.tw_{0}|tse_1440.tw_{0}|tse_1441.tw_{0}|tse_1442.tw_{0}|tse_1443.tw_{0}|tse_1444.tw_{0}|tse_1445.tw_{0}|tse_1446.tw_{0}|tse_1447.tw_{0}|tse_1449.tw_{0}|tse_1451.tw_{0}|tse_1452.tw_{0}|tse_1453.tw_{0}|tse_1454.tw_{0}|tse_1455.tw_{0}|tse_1456.tw_{0}|tse_1457.tw_{0}|tse_1459.tw_{0}|tse_1460.tw_{0}|tse_1463.tw_{0}|tse_1464.tw_{0}|tse_1465.tw_{0}|tse_1466.tw_{0}|tse_1467.tw_{0}|tse_1468.tw_{0}|tse_1469.tw_{0}|tse_1470.tw_{0}|tse_1471.tw_{0}|tse_1472.tw_{0}|tse_1473.tw_{0}|tse_1474.tw_{0}|tse_1475.tw_{0}|tse_1476.tw_{0}|tse_1477.tw_{0}|tse_1503.tw_{0}|tse_1504.tw_{0}|tse_1506.tw_{0}|tse_1507.tw_{0}|tse_1512.tw_{0}|tse_1513.tw_{0}|tse_1514.tw_{0}|tse_1515.tw_{0}|tse_1516.tw_{0}|tse_1517.tw_{0}|tse_1519.tw_{0}|tse_1521.tw_{0}|tse_1522.tw_{0}|tse_1524.tw_{0}|tse_1525.tw_{0}|tse_1526.tw_{0}|tse_1527.tw_{0}|tse_1528.tw_{0}|tse_1529.tw_{0}|tse_1530.tw_{0}|tse_1531.tw_{0}|tse_1532.tw_{0}|tse_1533.tw_{0}|tse_1535.tw_{0}|tse_1536.tw_{0}|tse_1537.tw_{0}|tse_1538.tw_{0}|tse_1539.tw_{0}|tse_1540.tw_{0}|tse_1541.tw_{0}|tse_1560.tw_{0}|tse_1568.tw_{0}|tse_1582.tw_{0}|tse_1583.tw_{0}|tse_1589.tw_{0}|tse_1590.tw_{0}|tse_1603.tw_{0}|tse_1604.tw_{0}|tse_1605.tw_{0}|tse_1608.tw_{0}|tse_1609.tw_{0}|tse_1611.tw_{0}|tse_1612.tw_{0}|tse_1613.tw_{0}|tse_1614.tw_{0}|tse_1615.tw_{0}|tse_1616.tw_{0}|tse_1617.tw_{0}|tse_1618.tw_{0}|tse_1626.tw_{0}|tse_1701.tw_{0}|tse_1702.tw_{0}|tse_1704.tw_{0}|tse_1707.tw_{0}|tse_1708.tw_{0}|tse_1709.tw_{0}|tse_1710.tw_{0}|tse_1711.tw_{0}|tse_1712.tw_{0}|tse_1713.tw_{0}|tse_1714.tw_{0}|tse_1715.tw_{0}|tse_1717.tw_{0}|tse_1718.tw_{0}|tse_1720.tw_{0}|tse_1721.tw_{0}|tse_1722.tw_{0}|tse_1723.tw_{0}|tse_1724.tw_{0}|tse_1725.tw_{0}|tse_1726.tw_{0}|tse_1727.tw_{0}|tse_1729.tw_{0}|tse_1730.tw_{0}|tse_1731.tw_{0}|tse_1732.tw_{0}|tse_1733.tw_{0}|tse_1734.tw_{0}|tse_1735.tw_{0}|tse_1736.tw_{0}|tse_1737.tw_{0}|tse_1762.tw_{0}|tse_1773.tw_{0}|tse_1783.tw_{0}|tse_1786.tw_{0}|tse_1789.tw_{0}|tse_1802.tw_{0}|tse_1805.tw_{0}|tse_1806.tw_{0}|tse_1808.tw_{0}|tse_1809.tw_{0}|tse_1810.tw_{0}|tse_1817.tw_{0}|tse_1902.tw_{0}|tse_1903.tw_{0}|tse_1904.tw_{0}|tse_1905.tw_{0}|tse_1906.tw_{0}|tse_1907.tw_{0}|tse_1909.tw_{0}|tse_2002.tw_{0}|tse_2006.tw_{0}|tse_2007.tw_{0}|tse_2008.tw_{0}|tse_2009.tw_{0}|tse_2010.tw_{0}|tse_2012.tw_{0}|tse_2013.tw_{0}|tse_2014.tw_{0}|tse_2015.tw_{0}|tse_2017.tw_{0}|tse_2020.tw_{0}|tse_2022.tw_{0}|tse_2023.tw_{0}|tse_2024.tw_{0}|tse_2025.tw_{0}|tse_2027.tw_{0}|tse_2028.tw_{0}|tse_2029.tw_{0}|tse_2030.tw_{0}|tse_2031.tw_{0}|tse_2032.tw_{0}|tse_2033.tw_{0}|tse_2034.tw_{0}|tse_2038.tw_{0}|tse_2049.tw_{0}|tse_2059.tw_{0}|tse_2062.tw_{0}|tse_2101.tw_{0}|tse_2102.tw_{0}|tse_2103.tw_{0}|tse_2104.tw_{0}|tse_2105.tw_{0}|tse_2106.tw_{0}|tse_2107.tw_{0}|tse_2108.tw_{0}|tse_2109.tw_{0}|tse_2114.tw_{0}|tse_2115.tw_{0}|tse_2201.tw_{0}|tse_2204.tw_{0}|tse_2206.tw_{0}|tse_2207.tw_{0}|tse_2208.tw_{0}|tse_2227.tw_{0}|tse_2228.tw_{0}|tse_2231.tw_{0}|tse_2301.tw_{0}|tse_2302.tw_{0}|tse_2303.tw_{0}|tse_2305.tw_{0}|tse_2308.tw_{0}|tse_2311.tw_{0}|tse_2312.tw_{0}|tse_2313.tw_{0}|tse_2314.tw_{0}|tse_2316.tw_{0}|tse_2317.tw_{0}|tse_2321.tw_{0}|tse_2323.tw_{0}|tse_2324.tw_{0}|tse_2325.tw_{0}|tse_2327.tw_{0}|tse_2328.tw_{0}|tse_2329.tw_{0}|tse_2330.tw_{0}|tse_2331.tw_{0}|tse_2332.tw_{0}|tse_2337.tw_{0}|tse_2338.tw_{0}|tse_2340.tw_{0}|tse_2342.tw_{0}|tse_2344.tw_{0}|tse_2345.tw_{0}|tse_2347.tw_{0}|tse_2348.tw_{0}|tse_2349.tw_{0}|tse_2351.tw_{0}|tse_2352.tw_{0}|tse_2353.tw_{0}|tse_2354.tw_{0}|tse_2355.tw_{0}|tse_2356.tw_{0}|tse_2357.tw_{0}|tse_2358.tw_{0}|tse_2359.tw_{0}|tse_2360.tw_{0}|tse_2361.tw_{0}|tse_2362.tw_{0}|tse_2363.tw_{0}|tse_2364.tw_{0}|tse_2365.tw_{0}|tse_2367.tw_{0}|tse_2368.tw_{0}|tse_2369.tw_{0}|tse_2371.tw_{0}|tse_2373.tw_{0}|tse_2374.tw_{0}|tse_2375.tw_{0}|tse_2376.tw_{0}|tse_2377.tw_{0}|tse_2379.tw_{0}|tse_2380.tw_{0}|tse_2382.tw_{0}|tse_2383.tw_{0}|tse_2384.tw_{0}|tse_2385.tw_{0}|tse_2387.tw_{0}|tse_2388.tw_{0}|tse_2390.tw_{0}|tse_2392.tw_{0}|tse_2393.tw_{0}|tse_2395.tw_{0}|tse_2397.tw_{0}|tse_2399.tw_{0}|tse_2401.tw_{0}|tse_2402.tw_{0}|tse_2404.tw_{0}|tse_2405.tw_{0}|tse_2406.tw_{0}|tse_2408.tw_{0}|tse_2409.tw_{0}|tse_2412.tw_{0}|tse_2413.tw_{0}|tse_2414.tw_{0}|tse_2415.tw_{0}|tse_2417.tw_{0}|tse_2419.tw_{0}|tse_2420.tw_{0}|tse_2421.tw_{0}|tse_2423.tw_{0}|tse_2424.tw_{0}|tse_2425.tw_{0}|tse_2426.tw_{0}|tse_2427.tw_{0}|tse_2428.tw_{0}|tse_2429.tw_{0}|tse_2430.tw_{0}|tse_2431.tw_{0}|tse_2433.tw_{0}|tse_2434.tw_{0}|tse_2436.tw_{0}|tse_2437.tw_{0}|tse_2438.tw_{0}|tse_2439.tw_{0}|tse_2440.tw_{0}|tse_2441.tw_{0}|tse_2442.tw_{0}|tse_2443.tw_{0}|tse_2444.tw_{0}|tse_2448.tw_{0}|tse_2449.tw_{0}|tse_2450.tw_{0}|tse_2451.tw_{0}|tse_2453.tw_{0}|tse_2454.tw_{0}|tse_2455.tw_{0}|tse_2456.tw_{0}|tse_2457.tw_{0}|tse_2458.tw_{0}|tse_2459.tw_{0}|tse_2460.tw_{0}|tse_2461.tw_{0}|tse_2462.tw_{0}|tse_2464.tw_{0}|tse_2465.tw_{0}|tse_2466.tw_{0}|tse_2467.tw_{0}|tse_2468.tw_{0}|tse_2471.tw_{0}|tse_2472.tw_{0}|tse_2474.tw_{0}|tse_2475.tw_{0}|tse_2476.tw_{0}|tse_2477.tw_{0}|tse_2478.tw_{0}|tse_2480.tw_{0}|tse_2481.tw_{0}|tse_2482.tw_{0}|tse_2483.tw_{0}|tse_2484.tw_{0}|tse_2485.tw_{0}|tse_2486.tw_{0}|tse_2488.tw_{0}|tse_2489.tw_{0}|tse_2491.tw_{0}|tse_2492.tw_{0}|tse_2493.tw_{0}|tse_2495.tw_{0}|tse_2496.tw_{0}|tse_2497.tw_{0}|tse_2498.tw_{0}|tse_2499.tw_{0}|tse_2501.tw_{0}|tse_2504.tw_{0}|tse_2505.tw_{0}|tse_2506.tw_{0}|tse_2509.tw_{0}|tse_2511.tw_{0}|tse_2514.tw_{0}|tse_2515.tw_{0}|tse_2516.tw_{0}|tse_2520.tw_{0}|tse_2524.tw_{0}|tse_2527.tw_{0}|tse_2528.tw_{0}|tse_2530.tw_{0}|tse_2534.tw_{0}|tse_2535.tw_{0}|tse_2536.tw_{0}|tse_2537.tw_{0}|tse_2538.tw_{0}|tse_2539.tw_{0}|tse_2540.tw_{0}|tse_2542.tw_{0}|tse_2543.tw_{0}|tse_2545.tw_{0}|tse_2546.tw_{0}|tse_2547.tw_{0}|tse_2548.tw_{0}|tse_2597.tw_{0}|tse_2601.tw_{0}|tse_2603.tw_{0}|tse_2605.tw_{0}|tse_2606.tw_{0}|tse_2607.tw_{0}|tse_2608.tw_{0}|tse_2609.tw_{0}|tse_2610.tw_{0}|tse_2611.tw_{0}|tse_2612.tw_{0}|tse_2613.tw_{0}|tse_2614.tw_{0}|tse_2615.tw_{0}|tse_2616.tw_{0}|tse_2617.tw_{0}|tse_2618.tw_{0}|tse_2634.tw_{0}|tse_2637.tw_{0}|tse_2642.tw_{0}|tse_2701.tw_{0}|tse_2702.tw_{0}|tse_2704.tw_{0}|tse_2705.tw_{0}|tse_2706.tw_{0}|tse_2707.tw_{0}|tse_2712.tw_{0}|tse_2722.tw_{0}|tse_2723.tw_{0}|tse_2727.tw_{0}|tse_2731.tw_{0}|tse_2801.tw_{0}|tse_2809.tw_{0}|tse_2812.tw_{0}|tse_2816.tw_{0}|tse_2820.tw_{0}|tse_2823.tw_{0}|tse_2832.tw_{0}|tse_2833.tw_{0}|tse_2834.tw_{0}|tse_2836.tw_{0}|tse_2837.tw_{0}|tse_2838.tw_{0}|tse_2841.tw_{0}|tse_2845.tw_{0}|tse_2847.tw_{0}|tse_2849.tw_{0}|tse_2850.tw_{0}|tse_2851.tw_{0}|tse_2852.tw_{0}|tse_2855.tw_{0}|tse_2856.tw_{0}|tse_2867.tw_{0}|tse_2880.tw_{0}|tse_2881.tw_{0}|tse_2882.tw_{0}|tse_2883.tw_{0}|tse_2884.tw_{0}|tse_2885.tw_{0}|tse_2886.tw_{0}|tse_2887.tw_{0}|tse_2888.tw_{0}|tse_2889.tw_{0}|tse_2890.tw_{0}|tse_2891.tw_{0}|tse_2892.tw_{0}|tse_2901.tw_{0}|tse_2903.tw_{0}|tse_2904.tw_{0}|tse_2905.tw_{0}|tse_2906.tw_{0}|tse_2908.tw_{0}|tse_2910.tw_{0}|tse_2911.tw_{0}|tse_2912.tw_{0}|tse_2913.tw_{0}|tse_2915.tw_{0}|tse_2923.tw_{0}|tse_2929.tw_{0}|tse_3002.tw_{0}|tse_3003.tw_{0}|tse_3004.tw_{0}|tse_3005.tw_{0}|tse_3006.tw_{0}|tse_3008.tw_{0}|tse_3010.tw_{0}|tse_3011.tw_{0}|tse_3013.tw_{0}|tse_3014.tw_{0}|tse_3015.tw_{0}|tse_3016.tw_{0}|tse_3017.tw_{0}|tse_3018.tw_{0}|tse_3019.tw_{0}|tse_3021.tw_{0}|tse_3022.tw_{0}|tse_3023.tw_{0}|tse_3024.tw_{0}|tse_3025.tw_{0}|tse_3026.tw_{0}|tse_3027.tw_{0}|tse_3028.tw_{0}|tse_3029.tw_{0}|tse_3030.tw_{0}|tse_3031.tw_{0}|tse_3032.tw_{0}|tse_3033.tw_{0}|tse_3034.tw_{0}|tse_3035.tw_{0}|tse_3036.tw_{0}|tse_3037.tw_{0}|tse_3038.tw_{0}|tse_3040.tw_{0}|tse_3041.tw_{0}|tse_3042.tw_{0}|tse_3043.tw_{0}|tse_3044.tw_{0}|tse_3045.tw_{0}|tse_3046.tw_{0}|tse_3047.tw_{0}|tse_3048.tw_{0}|tse_3049.tw_{0}|tse_3050.tw_{0}|tse_3051.tw_{0}|tse_3052.tw_{0}|tse_3054.tw_{0}|tse_3055.tw_{0}|tse_3056.tw_{0}|tse_3057.tw_{0}|tse_3058.tw_{0}|tse_3059.tw_{0}|tse_3060.tw_{0}|tse_3061.tw_{0}|tse_3062.tw_{0}|tse_3090.tw_{0}|tse_3094.tw_{0}|tse_3130.tw_{0}|tse_3149.tw_{0}|tse_3164.tw_{0}|tse_3167.tw_{0}|tse_3189.tw_{0}|tse_3209.tw_{0}|tse_3229.tw_{0}|tse_3231.tw_{0}|tse_3257.tw_{0}|tse_3296.tw_{0}|tse_3305.tw_{0}|tse_3308.tw_{0}|tse_3311.tw_{0}|tse_3312.tw_{0}|tse_3315.tw_{0}|tse_3338.tw_{0}|tse_3356.tw_{0}|tse_3376.tw_{0}|tse_3380.tw_{0}|tse_3383.tw_{0}|tse_3406.tw_{0}|tse_3419.tw_{0}|tse_3432.tw_{0}|tse_3437.tw_{0}|tse_3443.tw_{0}|tse_3450.tw_{0}|tse_3454.tw_{0}|tse_3474.tw_{0}|tse_3481.tw_{0}|tse_3494.tw_{0}|tse_3501.tw_{0}|tse_3504.tw_{0}|tse_3514.tw_{0}|tse_3515.tw_{0}|tse_3518.tw_{0}|tse_3519.tw_{0}|tse_3532.tw_{0}|tse_3533.tw_{0}|tse_3535.tw_{0}|tse_3536.tw_{0}|tse_3545.tw_{0}|tse_3550.tw_{0}|tse_3557.tw_{0}|tse_3559.tw_{0}|tse_3561.tw_{0}|tse_3573.tw_{0}|tse_3576.tw_{0}|tse_3579.tw_{0}|tse_3583.tw_{0}|tse_3584.tw_{0}|tse_3588.tw_{0}|tse_3591.tw_{0}|tse_3593.tw_{0}|tse_3596.tw_{0}|tse_3598.tw_{0}|tse_3605.tw_{0}|tse_3607.tw_{0}|tse_3617.tw_{0}|tse_3622.tw_{0}|tse_3638.tw_{0}|tse_3645.tw_{0}|tse_3653.tw_{0}|tse_3665.tw_{0}|tse_3669.tw_{0}|tse_3673.tw_{0}|tse_3679.tw_{0}|tse_3682.tw_{0}|tse_3686.tw_{0}|tse_3694.tw_{0}|tse_3698.tw_{0}|tse_3701.tw_{0}|tse_3702.tw_{0}|tse_3703.tw_{0}|tse_3704.tw_{0}|tse_3705.tw_{0}|tse_3706.tw_{0}|tse_4104.tw_{0}|tse_4106.tw_{0}|tse_4108.tw_{0}|tse_4119.tw_{0}|tse_4133.tw_{0}|tse_4137.tw_{0}|tse_4141.tw_{0}|tse_4142.tw_{0}|tse_4144.tw_{0}|tse_4164.tw_{0}|tse_4306.tw_{0}|tse_4414.tw_{0}|tse_4426.tw_{0}|tse_4526.tw_{0}|tse_4532.tw_{0}|tse_4536.tw_{0}|tse_4722.tw_{0}|tse_4725.tw_{0}|tse_4733.tw_{0}|tse_4737.tw_{0}|tse_4746.tw_{0}|tse_4755.tw_{0}|tse_4904.tw_{0}|tse_4906.tw_{0}|tse_4915.tw_{0}|tse_4916.tw_{0}|tse_4919.tw_{0}|tse_4930.tw_{0}|tse_4934.tw_{0}|tse_4935.tw_{0}|tse_4938.tw_{0}|tse_4942.tw_{0}|tse_4952.tw_{0}|tse_4956.tw_{0}|tse_4958.tw_{0}|tse_4960.tw_{0}|tse_4976.tw_{0}|tse_4977.tw_{0}|tse_4984.tw_{0}|tse_4994.tw_{0}|tse_4999.tw_{0}|tse_5007.tw_{0}|tse_5203.tw_{0}|tse_5215.tw_{0}|tse_5225.tw_{0}|tse_5234.tw_{0}|tse_5243.tw_{0}|tse_5259.tw_{0}|tse_5264.tw_{0}|tse_5269.tw_{0}|tse_5280.tw_{0}|tse_5285.tw_{0}|tse_5305.tw_{0}|tse_5388.tw_{0}|tse_5434.tw_{0}|tse_5469.tw_{0}|tse_5471.tw_{0}|tse_5484.tw_{0}|tse_5515.tw_{0}|tse_5519.tw_{0}|tse_5521.tw_{0}|tse_5522.tw_{0}|tse_5525.tw_{0}|tse_5531.tw_{0}|tse_5533.tw_{0}|tse_5534.tw_{0}|tse_5538.tw_{0}|tse_5607.tw_{0}|tse_5608.tw_{0}|tse_5706.tw_{0}|tse_5871.tw_{0}|tse_5880.tw_{0}|tse_5906.tw_{0}|tse_5907.tw_{0}|tse_6005.tw_{0}|tse_6108.tw_{0}|tse_6112.tw_{0}|tse_6115.tw_{0}|tse_6116.tw_{0}|tse_6117.tw_{0}|tse_6120.tw_{0}|tse_6128.tw_{0}|tse_6131.tw_{0}|tse_6133.tw_{0}|tse_6136.tw_{0}|tse_6139.tw_{0}|tse_6141.tw_{0}|tse_6142.tw_{0}|tse_6145.tw_{0}|tse_6152.tw_{0}|tse_6153.tw_{0}|tse_6155.tw_{0}|tse_6164.tw_{0}|tse_6165.tw_{0}|tse_6166.tw_{0}|tse_6168.tw_{0}|tse_6172.tw_{0}|tse_6176.tw_{0}|tse_6177.tw_{0}|tse_6183.tw_{0}|tse_6184.tw_{0}|tse_6189.tw_{0}|tse_6191.tw_{0}|tse_6192.tw_{0}|tse_6196.tw_{0}|tse_6197.tw_{0}|tse_6201.tw_{0}|tse_6202.tw_{0}|tse_6205.tw_{0}|tse_6206.tw_{0}|tse_6209.tw_{0}|tse_6213.tw_{0}|tse_6214.tw_{0}|tse_6215.tw_{0}|tse_6216.tw_{0}|tse_6224.tw_{0}|tse_6225.tw_{0}|tse_6226.tw_{0}|tse_6230.tw_{0}|tse_6235.tw_{0}|tse_6239.tw_{0}|tse_6243.tw_{0}|tse_6251.tw_{0}|tse_6257.tw_{0}|tse_6269.tw_{0}|tse_6271.tw_{0}|tse_6277.tw_{0}|tse_6278.tw_{0}|tse_6281.tw_{0}|tse_6282.tw_{0}|tse_6283.tw_{0}|tse_6285.tw_{0}|tse_6286.tw_{0}|tse_6289.tw_{0}|tse_6405.tw_{0}|tse_6409.tw_{0}|tse_6412.tw_{0}|tse_6414.tw_{0}|tse_6415.tw_{0}|tse_6504.tw_{0}|tse_6505.tw_{0}|tse_6605.tw_{0}|tse_6702.tw_{0}|tse_8011.tw_{0}|tse_8016.tw_{0}|tse_8021.tw_{0}|tse_8033.tw_{0}|tse_8039.tw_{0}|tse_8046.tw_{0}|tse_8070.tw_{0}|tse_8072.tw_{0}|tse_8081.tw_{0}|tse_8101.tw_{0}|tse_8103.tw_{0}|tse_8105.tw_{0}|tse_8110.tw_{0}|tse_8112.tw_{0}|tse_8114.tw_{0}|tse_8131.tw_{0}|tse_8150.tw_{0}|tse_8163.tw_{0}|tse_8201.tw_{0}|tse_8210.tw_{0}|tse_8213.tw_{0}|tse_8215.tw_{0}|tse_8249.tw_{0}|tse_8261.tw_{0}|tse_8271.tw_{0}|tse_8374.tw_{0}|tse_8404.tw_{0}|tse_8411.tw_{0}|tse_8422.tw_{0}|tse_8427.tw_{0}|tse_8429.tw_{0}|tse_8926.tw_{0}|tse_8940.tw_{0}|tse_8996.tw_{0}|tse_9802.tw_{0}|tse_9902.tw_{0}|tse_9904.tw_{0}|tse_9905.tw_{0}|tse_9906.tw_{0}|tse_9907.tw_{0}|tse_9908.tw_{0}|tse_9910.tw_{0}|tse_9911.tw_{0}|tse_9912.tw_{0}|tse_9914.tw_{0}|tse_9917.tw_{0}|tse_9918.tw_{0}|tse_9919.tw_{0}|tse_9921.tw_{0}|tse_9924.tw_{0}|tse_9925.tw_{0}|tse_9926.tw_{0}|tse_9927.tw_{0}|tse_9928.tw_{0}|tse_9929.tw_{0}|tse_9930.tw_{0}|tse_9931.tw_{0}|tse_9933.tw_{0}|tse_9934.tw_{0}|tse_9935.tw_{0}|tse_9937.tw_{0}|tse_9938.tw_{0}|tse_9939.tw_{0}|tse_9940.tw_{0}|tse_9941.tw_{0}|tse_9942.tw_{0}|tse_9943.tw_{0}|tse_9944.tw_{0}|tse_9945.tw_{0}|tse_9946.tw_{0}|tse_9955.tw_{0}|tse_9958.tw_{0}|";

        private UrlByIPWrapper _urlByIPWrapper = new UrlByIPWrapper(StockInfoUrl, IPAddress.Parse("210.71.239.179"));

        private string GetStockInfoData(DateTime date)
        {
            return string.Format(this._allStockInfoFormatString, date.ToString("yyyyMMdd"));
            //return string.Format(this._allStockInfoFormatString, "20140718");
        }

        public IEnumerable<int> GetAllStockId()
        {
            return _allStockInfoFormatString.Replace("delay=0&ex_ch=", "")
                                            .Replace(".tw_{0}|", "")
                                            .Split(new string[] { "tse_" }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(id => int.Parse(id));
        }

        public StockTradeObject GetStockInfoByDate(DateTime date)
        {
            var req = HttpWebRequest.Create(_urlByIPWrapper.Url) as HttpWebRequest;
            req.Method = WebRequestMethods.Http.Post;
            req.UserAgent = "Chrome/36.0";
            req.ContentType = "application/x-www-form-urlencoded";
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            req.Proxy = null;
            req.Timeout = 9000;
            using (var writer = new StreamWriter(req.GetRequestStream()))
            {
                writer.Write(GetStockInfoData(date));
            }

            using (var res = req.GetResponse() as HttpWebResponse)
            {
                var jsonContent = new StreamReader(res.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<StockTradeObject>(jsonContent);
            }
        }

        /// <summary>
        /// 取得股票資料
        /// </summary>
        /// <returns></returns>
        public StockTradeObject GetTodayStockInfo()
        {
            return GetStockInfoByDate(DateTime.Now);
        }

        public void GenerateGetAllStockInfoData(IStockInfoDataGenerator stockInfoDataGenerator)
        {
            if (stockInfoDataGenerator == null)
                return;

            this._allStockInfoFormatString = stockInfoDataGenerator.GenerateGetAllStockInfoData();
        }

        public void UpdateUrl()
        {
            _urlByIPWrapper.Update();
        }

        public static string CombinStockInfoDataFormatString<T>(params T[] stockIdList)
        {
            return "delay=0&ex_ch=" + string.Concat(stockIdList.Select(stockId => string.Format("tse_{0}.tw_{{0}}|", stockId.ToString())));
        }


    }
}
