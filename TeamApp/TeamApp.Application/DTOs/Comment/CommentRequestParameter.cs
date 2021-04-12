using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Application.Filters;

namespace TeamApp.Application.DTOs.Comment
{
    public class CommentRequestParameter: RequestParameter
    {
        public string PostId { get; set; }
    }
}
