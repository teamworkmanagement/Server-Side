using System;
using System.Collections.Generic;
using System.Text;

namespace TeamApp.Application.DTOs.Task
{
    public class DragTaskModel
    {
        //list source id
        public string SourceDroppableId { get; set; }
        public int SourceIndex { get; set; }

        //list destination id
        public string DestinationDroppableId { get; set; }
        public int DestinationIndex { get; set; }
    }
}
