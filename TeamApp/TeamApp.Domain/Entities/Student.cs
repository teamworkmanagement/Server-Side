using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamApp.Domain.Entities
{
    public class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        [Timestamp]
        public DateTimeOffset Dob { get; set; }
    }
}
