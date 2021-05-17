using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamApp.Application.Utils
{
    public static class Extensions
    {
        static readonly string[] VietNamChar =
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
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
        public static string UnsignUnicode(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            for (var i = 1; i < VietNamChar.Length; i++)
                for (var j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            return str.ToLower();
        }

        public class RadomString
        {

            private static Random random = new Random();
            public static string RandomString(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }
    }
}
