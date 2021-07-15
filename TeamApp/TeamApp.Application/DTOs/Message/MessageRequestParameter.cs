using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Application.Filters;

namespace TeamApp.Application.DTOs.Message
{
    public class MessageRequestParameter : RequestParameter
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }
    }
}
