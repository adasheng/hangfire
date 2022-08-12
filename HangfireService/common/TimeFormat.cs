using System;

namespace HangfireService.common
{
    public class TimeFormat
    {
        /// <summary>
        /// 将DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="dateTime">DateTime时间</param>
        ///  <param name="format">精度：Seconds-秒，Milliseconds-毫秒</param>
        /// <returns></returns>
        public static long ToUnixTimeStamp(DateTime dateTime, string accuracy = "Seconds")
        {
            long intResult = 0;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            switch (accuracy)
            {
                case "Seconds":
                    intResult = (long)(dateTime - startTime).TotalSeconds;
                    break;
                case "Milliseconds":
                    intResult = (long)(dateTime - startTime).TotalMilliseconds;
                    break;
                default:
                    intResult = (long)(dateTime - startTime).TotalSeconds;
                    break;
            }

            return intResult;
        }



        public static DateTime TimeStampToDateTime(long timeStamp, bool inMilli = false)
        {
            DateTimeOffset dateTimeOffset = inMilli ? DateTimeOffset.FromUnixTimeMilliseconds(timeStamp) : DateTimeOffset.FromUnixTimeSeconds(timeStamp);
            return dateTimeOffset.LocalDateTime;
        }



    }
}
