using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.Utils
{
    public class Extensions
    {
        public static DateTime? UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            //System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            DateTimeOffset dto = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp);
            //dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dto.UtcDateTime;
        }
    }
}
