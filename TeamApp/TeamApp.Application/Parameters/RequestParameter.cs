using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.Filters
{
    public class RequestParameter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int SkipItems { get; set; }
        public string KeyWord { get; set; }
        public RequestParameter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
            this.SkipItems = 0;
            this.KeyWord = string.Empty;
        }
        public RequestParameter(int pageNumber, int pageSize, int skipItems, string keyWord)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
            this.SkipItems = skipItems;
            this.KeyWord = keyWord;
        }
    }
}
