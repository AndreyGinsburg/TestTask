using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication31.Models
{
    public class OutBal
    {
        public int id { get; set; }
        public double act { get; set; }
        public double pass { get; set; }
        public int? classId { get; set; }
        public int? accountId { get; set; }
        public int number { get; set; }
        public Class _class { get; set; }
        public Account _account { get; set; }
    }
}