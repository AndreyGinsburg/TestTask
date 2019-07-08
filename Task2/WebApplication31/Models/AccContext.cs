using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WebApplication31.Models
{
    public class AccContext : DbContext
    {
        public AccContext() : base("AccContext") { }
        public DbSet<Account> accounts { get; set; }
        public DbSet<Class> classes { get; set; }
        public DbSet<InBal> inBals { get; set; }
        public DbSet<Rev> revs { get; set; }
        public DbSet<OutBal> outBals { get; set; }
    }
}