using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Cybereum.Models;

namespace Cybereum.DAL
{
    public class GanttContext : DbContext
    {
        public GanttContext() : base("GanttContext") { }
        public DbSet<GanttTask> GanttTasks { get; set; }
        public DbSet<GanttLink> GanttLinks { get; set; }
    }
}