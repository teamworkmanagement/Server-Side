﻿using System;
using System.Collections.Generic;
using System.Text;
using TeamApp.Application.Filters;

namespace TeamApp.Application.DTOs.File
{
    public class FileRequestParameter: RequestParameter
    {
        public string BelongedId { get; set; }
    }
}
