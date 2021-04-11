using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Application.Filters;

namespace TeamApp.Application.DTOs.Post
{
    public class BasicFilter
    {
        public const string Lastest = "lastest";
        public const string LastHour = "lasthour";
        public const string Today = "today";
        public const string ThisWeek = "thisweek";
        public const string ThisMonth = "thismonth";
    }
    public class PostRequestParameter : RequestParameter
    {
        public string UserId { get; set; }
        public string BasicFilter { get; set; }
        public long? FromDate { get; set; }
        public long? ToDate { get; set; }
        public string GroupId { get; set; }
        public string PostUser { get; set; }
    }
}
