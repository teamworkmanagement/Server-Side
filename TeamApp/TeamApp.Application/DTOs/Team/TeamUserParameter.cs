using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Application.Filters;

namespace TeamApp.Application.DTOs.Team
{
    public class TeamUserParameter: RequestParameter
    {
        public string TeamId { get; set; }
    }
}
