using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Application.Filters;

namespace TeamApp.Application.DTOs.AppSearch
{
    public class AppSearchRequest: RequestParameter
    {
        public string UserId { get; set; }
    }
}
