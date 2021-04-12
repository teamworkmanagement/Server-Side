using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.Utils
{
    public static class Extensions
    {
        public static DateTime? UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTimeOffset dto = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp);
            return dto.UtcDateTime;
        }

        public static DateTime? FormatTime(this DateTime? dt)
        {
            if (dt != null)
                dt = ((DateTime)dt).ToLocalTime();
            return dt;
        }
    }
}
