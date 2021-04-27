using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.Wrappers
{
    public class PagedResponse<T>
    {
        public int PageNumber { get; set; }
        public int SkipRows { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public List<T> Items { set; get; }

        public PagedResponse(List<T> items, int pageSize, int totalRecords, int pageNumber = -1, int skipRows = -1)
        {
            this.SkipRows = skipRows;
            this.PageSize = pageSize;
            this.Items = items;
            this.TotalRecords = totalRecords;
            this.PageNumber = pageNumber;
        }
    }
}
