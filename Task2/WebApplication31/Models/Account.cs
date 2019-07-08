using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication31.Models
{
    public class Account
    {
        public int id { get; set; }
        public int number { get; set; }
        public ICollection<InBal> inBals { get; set; }
        public ICollection<Rev> revs { get; set; }
        public ICollection<OutBal> outBals { get; set; }
        public Account()
        {
            inBals = new List<InBal>();
            revs = new List<Rev>();
            outBals = new List<OutBal>();
        }
    }
}