using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BCM.Common
{
    public static class Method
    {
        /// <summary>
        /// 获得32为GUID码
        /// </summary>
        /// <returns>string</returns>
        public static string GetGuid32()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Trim();
        }

        public static bool IsTimeRange(string start, string end, DateTime addat)
        {
            var nHour = addat.Hour;
            var nMin = addat.Minute;
            int sHour = Convert.ToInt32(start.Split(':')[0]);
            int sMin = Convert.ToInt32(start.Split(':')[1]);
            int eHour = Convert.ToInt32(end.Split(':')[0]);
            int eMin = Convert.ToInt32(end.Split(':')[1]);
            if (sHour > nHour || nHour > eHour)
            {
                return false;
            }
            if (sHour == nHour || nHour == eHour)
            {
                if (sMin > nMin || nMin > eMin)
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
          /// 判断一个时间是否位于指定的时间段内
          /// </summary>
          /// <param name="time_interval">时间区间字符串</param>
          /// <returns></returns>
        public static bool isLegalTime(DateTime dt, string time)
        {
            //当前时间
            int time_now = dt.Hour * 10000 + dt.Minute * 100 + dt.Second;
            //查看各个时间区间
            // string[] time_interval = time_intervals.Split(';');
            // foreach (string time in time_interval)
            // {
            //空数据直接跳过
            // if (string.IsNullOrWhiteSpace(time))
            // {
            //     continue;
            // }
            //一段时间格式：六个数字-六个数字
            if (!Regex.IsMatch(time, "^[0-9]{6}-[0-9]{6}$"))
            {
                Console.WriteLine("{0}： 错误的时间数据", time);
            }
            string timea = time.Substring(0, 6);
            string timeb = time.Substring(7, 6);
            int time_a, time_b;
            //尝试转化为整数
            if (!int.TryParse(timea, out time_a))
            {
                Console.WriteLine("{0}： 转化为整数失败", timea);
            }
            if (!int.TryParse(timeb, out time_b))
            {
                Console.WriteLine("{0}： 转化为整数失败", timeb);
            }
            //如果当前时间不小于初始时间，不大于结束时间，返回true
            if (time_a <= time_now && time_now <= time_b)
            {
                return true;
            }
            // }
            //不在任何一个区间范围内，返回false
            return false;
        }

        #region 转换时间为unix时间戳
        /// <summary>
        /// 转换时间为unix时间戳
        /// </summary>
        /// <param name="date">需要传递UTC时间,避免时区误差,例:DataTime.UTCNow</param>
        /// <returns></returns>
        public static double ConvertToUnixOfTime(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        #endregion

        #region 时间戳转换为时间

        public static DateTime StampToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + (timeStamp.Length == 13 ? "0000" : "0000000"));
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return dtStart.Add(toNow);
        }

        #endregion
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="sInputString">待加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5Encryption(this string sInputString)
        {
            return BitConverter.ToString(System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.Default.GetBytes(sInputString))).Replace("-", "");
        }
    }
}
