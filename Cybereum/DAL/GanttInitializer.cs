using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Cybereum.Models;

namespace Cybereum.DAL
{
    public class GanttInitializer : DropCreateDatabaseIfModelChanges<GanttContext>
    {
        private cybereumEntities db = new cybereumEntities();

        protected override void Seed(GanttContext context)
        {
         //   List<GanttTask> tasks = new List<GanttTask>()
         //   {
         //       new GanttTask() { GanttTaskId = 1, Text = "Project #2", StartDate = DateTime.Now.AddHours(-3),
         //           Duration = 18, SortOrder = 10, Progress = 0.4m, ParentId = null },
         //       new GanttTask() { GanttTaskId = 2, Text = "Task #1", StartDate = DateTime.Now.AddHours(-2),
         //           Duration = 8, SortOrder = 10, Progress = 0.6m, ParentId = 1 },
         //       new GanttTask() { GanttTaskId = 3, Text = "Task #2", StartDate = DateTime.Now.AddHours(-1),
         //           Duration = 8, SortOrder = 20, Progress = 0.6m, ParentId = 1 },
         //       new GanttTask() { GanttTaskId = 4, Text = "Task #3", StartDate = DateTime.Now.AddHours(-1),
         //           Duration = 8, SortOrder = 20, Progress = 0.6m, ParentId = 1 }
         //   };
         //   tasks.ForEach(s => context.GanttTasks.Add(s));
         //   context.SaveChanges();

         //   List<GanttLink> links = new List<GanttLink>()
         //{
         //    new GanttLink() { GanttLinkId = 1, SourceTaskId = 1, TargetTaskId = 2, Type = "1" },
         //    new GanttLink() { GanttLinkId = 2, SourceTaskId = 2, TargetTaskId = 3, Type = "0" },
         //    new GanttLink() { GanttLinkId = 3, SourceTaskId = 3, TargetTaskId = 4, Type = "0" },
         //    new GanttLink() { GanttLinkId = 4, SourceTaskId = 4, TargetTaskId = 1, Type = "0" }
         //};
         //   links.ForEach(s => context.GanttLinks.Add(s));
         //   context.SaveChanges();


            //var files = (from a in db.tbl_project
            //                 //from b in db.tbl_milestone.Where(b => b.projectid == a.projectid).DefaultIfEmpty()
            //                 //from c in db.tbl_task.Where(c => c.milestoneid == b.milestoneid).DefaultIfEmpty()
            //                 //from d in db.tbl_subtask.Where(d => d.taskid == c.taskid).DefaultIfEmpty()
            //             where a.isactive == 1
            //             select new
            //             {
            //                 projectid = a.projectid,
            //                 projectname = a.projectname,
            //                 startdate = a.startdate,
            //                 enddate = a.enddate
            //             }).ToList();


            //List<GanttTask> tasks1 = new List<GanttTask>();
            //List<GanttLink> links1 = new List<GanttLink>();
            //foreach (var project in files)
            //{
            //    GanttTask chartactivity = new GanttTask();
            //    chartactivity.GanttTaskId = project.projectid;
            //    chartactivity.Text = project.projectname;
            //    chartactivity.StartDate = project.startdate;

            //    //Storing input Dates  
            //    DateTime FromYear = Convert.ToDateTime(project.enddate);
            //    DateTime ToYear = Convert.ToDateTime(project.startdate);

            //    //Creating object of TimeSpan Class  
            //    TimeSpan objTimeSpan = ToYear - FromYear;
            //    //TotalDays  
            //    int Days = Convert.ToInt16(objTimeSpan.TotalDays);
            //    chartactivity.Duration = Days;
            //    chartactivity.SortOrder = 1;
            //    chartactivity.ParentId = null;
            //    tasks1.Add(chartactivity);

                

            //    var files1 = (from a in db.tbl_milestone
            //                  where a.isactive == 1 && a.projectid == project.projectid
            //                  select new
            //                  {
            //                      milestoneid = a.milestoneid,
            //                      milestonename = a.milestonename,
            //                      startdate = a.startdate,
            //                      enddate = a.enddate
            //                  }).ToList();
            //    foreach (var milestone in files1)
            //    {
            //        GanttLink ganttlink = new GanttLink();
            //        ganttlink.GanttLinkId = milestone.milestoneid;
            //        ganttlink.SourceTaskId = project.projectid;
            //        ganttlink.TargetTaskId = 1;
            //        ganttlink.Type = "1";
            //        links1.Add(ganttlink);
            //    }

            //}


            //tasks1.ForEach(s => context.GanttTasks.Add(s));
            //context.SaveChanges();
            //links1.ForEach(s => context.GanttLinks.Add(s));
            //context.SaveChanges();
        }
    }
}

