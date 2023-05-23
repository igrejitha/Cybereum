<<<<<<< Updated upstream
﻿using Cybereum.Filters;
using Cybereum.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;

namespace Cybereum.Controllers
{
    public class HomeController : Controller
    {

        cybereumEntities entities = new cybereumEntities();

        [Authorize]
        [SessionTimeout]
        public ActionResult Index()
        {
            long projectcount = 0;
            var countapproved = (from p in entities.tbl_user
                                 where p.emailverification == true && p.isactive == 1 && p.roleid != 1
                                 select p.userid).Count();

            var countpending = (from p in entities.tbl_user
                                where p.emailverification == true && p.isactive == 0 && p.roleid != 1
                                select p.userid).Count();

            //var countproject = (from p in entities.tbl_project
            //                    where p.isactive == 1
            //                    select p.projectid).Count();

            var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + HttpUtility.UrlEncode(gremlinvariables.database) + "/colls/" + HttpUtility.UrlEncode(gremlinvariables.collection), password: gremlinvariables.authKey);
            using (var gremlinClient = new GremlinClient(
                gremlinServer,
                new GraphSON2Reader(),
                new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType,
                gremlinvariables.connectionPoolSettings))
            {
                var gremlinScript = "g.V().hasLabel('project').count()";
                try
                {
                    //var results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    task.Wait();
                    var results = task.Result;
                    projectcount = results.ToList()[0];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            ViewBag.countApproved = countapproved;
            ViewBag.countPending = countpending;
            ViewBag.countProject = projectcount;

            return View();
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult Dashboard()
        {
            int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
            cybereumEntities entities = new cybereumEntities();
            var countUser = (from p in entities.tbl_user
                             where p.emailverification == true && p.isactive == 1 && p.pmuserid == pmuserid && p.roleid == 3
                             select p.userid).Count();

            //var countProject = (from p in entities.tbl_project
            //                    where p.isactive == 1 && p.createdby == pmuserid
            //                    select p.projectid).Count();
            long projectcount = 0;
            long taskcount = 0;
            var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + HttpUtility.UrlEncode(gremlinvariables.database) + "/colls/" + HttpUtility.UrlEncode(gremlinvariables.collection), password: gremlinvariables.authKey);
            using (var gremlinClient = new GremlinClient(
                gremlinServer,
                new GraphSON2Reader(),
                new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType,
                gremlinvariables.connectionPoolSettings))
            {
                var gremlinScript = "g.V().has('project','createdby','" + pmuserid + "').or().has('project','projectmembers','" + pmuserid + "')";
                try
                {
                    //var results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    task.Wait();
                    var results = task.Result;
                    //projectcount = results.ToList()[0];
                    projectcount = results.Count();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                var gremlinScript1 = "g.V().has('task','createdby','" + pmuserid + "').count()";
                if (Convert.ToInt16(Session["RoleId"]) == (int)Role.User)
                {
                    gremlinScript1 = "g.V().has('task','assignedto','" + pmuserid + "').count()";
                }

                try
                {
                    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript1);
                    task.Wait();
                    var results = task.Result;
                    taskcount = results.ToList()[0];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            //var countTask = (from p in entities.tbl_task
            //                 join a in entities.tbl_milestone on p.milestoneid equals a.milestoneid
            //                 join b in entities.tbl_project on a.projectid equals b.projectid
            //                 where p.isactive == 1 && b.createdby == pmuserid
            //                 select p.taskid).Count();

            //if (Convert.ToInt16(Session["RoleId"]) == (int)Role.User)
            //{
            //    countTask = (from p in entities.tbl_task
            //                 join a in entities.tbl_milestone on p.milestoneid equals a.milestoneid
            //                 join b in entities.tbl_project on a.projectid equals b.projectid
            //                 where p.isactive == 1 && p.assignedto == pmuserid
            //                 select p.taskid).Count();
            //}

            ViewBag.countUser = countUser;
            ViewBag.countProject = projectcount;
            ViewBag.countTask = taskcount;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [Authorize]
        [SessionTimeout]
        // GET: Project
        public ActionResult GANTT()
        {
            return View();
        }


        [Authorize]
        [SessionTimeout]
        public ActionResult GANTTChart(string projectid, ProjectActivity Activity)
        {
            if (projectid == null)
            {
                ViewBag.projectid = Convert.ToInt32(Session["ProjectId"]);
                Session["ProjectId"] = ViewBag.projectid;
            }
            else
            {
                ViewBag.projectid = projectid;
                Session["ProjectId"] = projectid;
            }


            //var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').order().by('enddate',decr).limit(1).project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
            //var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + gremlinvariables.database + "/colls/" + gremlinvariables.collection, password: gremlinvariables.authKey);
            //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
            //{
            //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
            //    task.Wait();
            //    var result = task.Result;
            //    if (result.Count > 0)
            //    {
            //        foreach (var item in result)
            //        {
            //            Activity.startdate = Convert.ToDateTime(item["enddate"]);
            //            Activity.enddate = Convert.ToDateTime(item["enddate"]);
            //        }
            //    }
            //    else
            //    {
            //        Activity.startdate = DateTime.Today;
            //        Activity.enddate = DateTime.Today;
            //    }
            //}

            //return View(Activity);
            return View();
        }


        [Authorize]
        [SessionTimeout]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GANTTChart([Bind(Include = "id,activityname,ismilestone,startdate,enddate,durations,predecessors,createdby,createddate,modifiedby,modifieddate,projectid")] ProjectActivity tbl_activity)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                long duration = 0;
                if (DateTime.Now.Date < tbl_activity.startdate.Date)
                {
                    duration = 0;
                }
                else if (DateTime.Now.Date > tbl_activity.enddate.Date)
                {
                    duration = 100;
                }
                else
                {
                    double dt1 = (DateTime.Now.Date - tbl_activity.startdate.Date).TotalDays;
                    double dt2 = (tbl_activity.enddate.Date - tbl_activity.startdate.Date).TotalDays;
                    if (dt2 != 0)
                        duration = Convert.ToInt64((dt1 / dt2) * 100);
                }


                var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + gremlinvariables.database + "/colls/" + gremlinvariables.collection, password: gremlinvariables.authKey);
                long count = 0;
                if (tbl_activity.id == null)
                {
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').has('activity','projectid','" + tbl_activity.projectid + "').count()";
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var objList = task.Result;
                        count = objList.ToList()[0];
                    }
                    if (count > 0)
                    {
                        message = "Activity name already exists.";
                        goto endloop;
                    }
                }
                

                if (tbl_activity.id == null)
                {
                    if (tbl_activity.createdby == null)
                    {
                        tbl_activity.createdby = Session["LoggedInUserId"].ToString();
                        tbl_activity.createdusername = Session["Username"].ToString();
                    }

                    string gremlinScript = $"g.addV('activity').property('pk', '{tbl_activity.activityname}')" +
                            $".property('activityname', '{tbl_activity.activityname}')" +
                            $".property('startdate', '{tbl_activity.startdate.ToString("yyyy-MM-dd")}')" +
                            $".property('enddate', '{tbl_activity.enddate.ToString("yyyy-MM-dd")}')" +
                            $".property('projectid', '{tbl_activity.projectid}')" +
                            $".property('durations', '{duration}')" +
                            $".property('ismilestone', '{tbl_activity.ismilestone}')" +
                            $".property('createdby', '{Convert.ToInt32(tbl_activity.createdby)}')" +
                            $".property('createdusername', '{tbl_activity.createdusername}')" +
                            $".property('createdon', '{DateTime.Now}')" +
                            $".property('type', 'activity')";

                    // Execute the Gremlin script                    
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Added Successfully";
                    }


                    gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').project('id').by(values('id'))";
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        foreach (var result1 in result)
                        {
                            tbl_activity.id = Convert.ToString(result1["id"]);
                        }
                    }


                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('contains').to(g.V('{tbl_activity.id}'))";
                    //gremlinScript = $"\ng.V().has('project','projectid','{tbl_activity.projectid}').as('a').V().has('activity','activityname','{tbl_activity.activityname}').addE('projectactivity').to('a')";
                    // Execute the Gremlin script
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Gremlin script executed successfully";
                    }

                    //Connect the predeccesors to succesors
                    //foreach (var predecessor in tbl_activity.Predecessors)
                    //{
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('precedes').property('duration', '{tbl_activity.durations}').to(g.V('{tbl_activity.activityname}'))";
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Gremlin script executed successfully";
                    }
                    //}
                }
                //ViewBag.projectid = tbl_activity.projectid;
                return RedirectToAction("GANTTChart", new { projectid = tbl_activity.projectid });
            }
            endloop:
            ViewBag.Message = message;
            //ViewBag.projectid =tbl_activity.projectid;
            //return View(tbl_activity);
            return RedirectToAction("GANTTChart", new { projectid = tbl_activity.projectid });
        }

        public JsonResult getTestvalue()
        {
            try
            {
                //var projectdata = entities.sp_get_project(1, 1, 0).ToList();

                var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: gremlinvariables.containerLink, password: gremlinvariables.authKey);
                var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType, gremlinvariables.connectionPoolSettings);
                var gremlinScript = "g.V().has('project','id','3ce54f64-879c-40c1-a268-9f60f43ccde9').project('id','projectname','startdate','enddate').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate'))";
                var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                task.Wait();
                var projectdata = task.Result;

                List<TestModel> ganttchartmodellist = new List<TestModel>();
                foreach (var project in projectdata)
                {
                    TestModel ganttchart = new TestModel();
                    ganttchart.task = project.name.ToString();
                    ganttchart.type = project.name.ToString();
                    ganttchart.startTime = project.startdate.ToString("yyyy-MM-dd");
                    ganttchart.endTime = project.enddate.ToString("yyyy-MM-dd");
                    ganttchartmodellist.Add(ganttchart);

                    var milestonedata = entities.sp_get_milestone(1, 1, project.id, 0).ToList();
                    foreach (var itemmilestone in milestonedata)
                    {
                        TestModel ganttchart1 = new TestModel();
                        ganttchart1.task = itemmilestone.name.ToString();
                        ganttchart1.type = project.name.ToString();
                        ganttchart1.startTime = itemmilestone.startdate.ToString("yyyy-MM-dd");
                        ganttchart1.endTime = itemmilestone.enddate.ToString("yyyy-MM-dd");
                        ganttchartmodellist.Add(ganttchart1);

                        var taskdata = entities.sp_get_task(1, 1, itemmilestone.id, 0).ToList();
                        foreach (var itemtask in taskdata)
                        {
                            TestModel ganttchart2 = new TestModel();
                            ganttchart2.task = itemtask.name.ToString();
                            ganttchart2.type = itemmilestone.name.ToString();
                            ganttchart2.startTime = itemtask.startdate.ToString("yyyy-MM-dd");
                            ganttchart2.endTime = itemtask.enddate.ToString("yyyy-MM-dd");
                            ganttchartmodellist.Add(ganttchart2);

                            var subtaskdata = entities.sp_get_subtask(1, 1, itemtask.id, 0).ToList();
                            foreach (var itemsubtask in subtaskdata)
                            {
                                TestModel ganttchart3 = new TestModel();
                                ganttchart3.task = itemsubtask.name.ToString();
                                ganttchart3.type = itemtask.name.ToString();
                                ganttchart3.startTime = itemsubtask.startdate.ToString("yyyy-MM-dd");
                                ganttchart3.endTime = itemsubtask.enddate.ToString("yyyy-MM-dd");
                                ganttchartmodellist.Add(ganttchart3);
                            }
                        }
                    }

                }

                var x = JsonConvert.SerializeObject(ganttchartmodellist, Formatting.Indented);
                return Json(x, JsonRequestBehavior.AllowGet);
                //return Json(ganttchartmodellist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
        }

        public JsonResult getgannchart(string projectid)
        {
            try
            {
                DateTime startdate;
                DateTime enddate;

                var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: gremlinvariables.containerLink, password: gremlinvariables.authKey);
                var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType, gremlinvariables.connectionPoolSettings);
                var gremlinScript = "g.V().has('project','id','" + projectid + "').project('id','projectname','startdate','enddate').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate'))";
                var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                task.Wait();
                var projectdata = task.Result;

                List<ganttchartmodel> ganttchartmodellist = new List<ganttchartmodel>();
                int i = 1;
                foreach (var project in projectdata)
                {
                    ganttchartmodel ganttchart = new ganttchartmodel();
                    ganttchart.id = i.ToString();
                    i++;
                    ganttchart.taskid = project["id"].ToString();
                    ganttchart.name = project["projectname"].ToString();


                    startdate = Convert.ToDateTime(project["startdate"].ToString());
                    enddate = Convert.ToDateTime(project["enddate"].ToString());

                    long duration = 0;
                    if (DateTime.Now.Date < startdate.Date)
                    {
                        duration = 0;
                    }
                    else if (DateTime.Now.Date > enddate.Date)
                    {
                        duration = 100;
                    }
                    else
                    {
                        double dt1 = (DateTime.Now.Date - startdate.Date).TotalDays;
                        double dt2 = (enddate.Date - startdate.Date).TotalDays;
                        if (dt2 != 0)
                            duration = Convert.ToInt64((dt1 / dt2) * 100);
                    }
                    //int duration = IGUtilities.CalculateDays(startdate.Date, enddate.Date) * 8;

                    //ganttchart.progressValue = duration + "%";
                    ganttchart.actualStart = startdate.ToString("yyyy-MM-dd");
                    ganttchart.actualEnd = enddate.ToString("yyyy-MM-dd");
                    //ganttchart.connectTo = i.ToString();
                    //ganttchart.connecterType = "finish - start";

                    List<Children> Activity = new List<Children>();
                    gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations'))";
                    //gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').project('id','activityname','startdate','enddate','durations').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations'))";
                    var taskactivity = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    taskactivity.Wait();
                    var activitydata = taskactivity.Result;
                    
                    foreach (var itemactivity in activitydata)
                    {
                        Children objActivity = new Children();
                        objActivity.id = i.ToString();
                        i++;
                        objActivity.taskid = itemactivity["id"].ToString();
                        objActivity.name = itemactivity["activityname"].ToString();

                        startdate = Convert.ToDateTime(itemactivity["startdate"].ToString());
                        enddate = Convert.ToDateTime(itemactivity["enddate"].ToString());

                        //duration = itemactivity["durations"].ToString();
                        if (DateTime.Now.Date < startdate.Date)
                        {
                            duration = 0;
                        }
                        else if (DateTime.Now.Date > enddate.Date)
                        {
                            duration = 100;
                        }
                        else
                        {
                            double dt1 = (DateTime.Now.Date - startdate.Date).TotalDays;
                            double dt2 = (enddate.Date - startdate.Date).TotalDays;
                            if (dt2 != 0)
                                duration = Convert.ToInt64((dt1 / dt2) * 100);
                        }
                        //duration = IGUtilities.CalculateDays(startdate.Date, enddate.Date) * 8;
                        //objActivity.progressValue = duration + "%";

                        objActivity.actualStart = startdate.ToString("yyyy-MM-dd");
                        objActivity.actualEnd = enddate.ToString("yyyy-MM-dd");
                        objActivity.connectTo = i.ToString();
                        objActivity.connecterType = "finish - start";

                        //if (ganttchart.name != "FPSO-3")
                        //{
                        //    List<Children> Task = new List<Children>();
                        //    gremlinScript = "g.V().has('task','activityid','" + itemactivity["id"] + "').project('id','taskname','startdate','enddate','durations','activityid').by(id()).by(values('taskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('activityid'))";
                        //    var tasktask = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        //    tasktask.Wait();
                        //    var taskdata = tasktask.Result;
                        //    foreach (var itemtask in taskdata)
                        //    {
                        //        Children objTask = new Children();
                        //        objTask.id = i.ToString();
                        //        i++;
                        //        objTask.taskid = itemtask["id"].ToString();
                        //        objTask.name = itemtask["taskname"].ToString();
                        //        //objTask.progressValue = itemtask["durations"].ToString() + "%";                            
                        //        startdate = Convert.ToDateTime(itemtask["startdate"].ToString());
                        //        enddate = Convert.ToDateTime(itemtask["enddate"].ToString());

                        //        duration = 0;
                        //        if (DateTime.Now.Date < startdate.Date)
                        //        {
                        //            duration = 0;
                        //        }
                        //        else if (DateTime.Now.Date > enddate.Date)
                        //        {
                        //            duration = 100;
                        //        }
                        //        else
                        //        {
                        //            double dt1 = (DateTime.Now.Date - startdate.Date).TotalDays;
                        //            double dt2 = (enddate.Date - startdate.Date).TotalDays;
                        //            if (dt2 != 0)
                        //                duration = Convert.ToInt64((dt1 / dt2) * 100);
                        //        }

                        //        objTask.progressValue = duration + "%";

                        //        objTask.actualStart = startdate.ToString("yyyy-MM-dd");
                        //        objTask.actualEnd = enddate.ToString("yyyy-MM-dd");
                        //        objTask.connectTo = i.ToString();
                        //        objTask.connecterType = "finish - start";

                        //        List<Children> SubTask = new List<Children>();
                        //        gremlinScript = "g.V().has('subtask','taskid','" + itemtask["id"] + "').project('id','subtaskname','startdate','enddate','durations').by(id()).by(values('subtaskname')).by(values('startdate')).by(values('enddate')).by(values('durations'))";
                        //        var subtask = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        //        subtask.Wait();
                        //        var subtaskdata = subtask.Result;
                        //        int subtaskrow = 0;
                        //        foreach (var itemsubtask in subtaskdata)
                        //        {
                        //            subtaskrow++;
                        //            Children objSubTask = new Children();
                        //            objSubTask.id = i.ToString();
                        //            i++;
                        //            objSubTask.taskid = itemsubtask["id"].ToString();
                        //            objSubTask.name = itemsubtask["subtaskname"].ToString();
                        //            //objSubTask.progressValue = itemsubtask["durations"].ToString() + "%";
                        //            startdate = Convert.ToDateTime(itemsubtask["startdate"].ToString());
                        //            enddate = Convert.ToDateTime(itemsubtask["enddate"].ToString());
                        //            duration = 0;
                        //            if (DateTime.Now.Date < startdate.Date)
                        //            {
                        //                duration = 0;
                        //            }
                        //            else if (DateTime.Now.Date > enddate.Date)
                        //            {
                        //                duration = 100;
                        //            }
                        //            else
                        //            {
                        //                double dt1 = (DateTime.Now.Date - startdate.Date).TotalDays;
                        //                double dt2 = (enddate.Date - startdate.Date).TotalDays;
                        //                if (dt2 != 0)
                        //                    duration = Convert.ToInt64((dt1 / dt2) * 100);
                        //            }

                        //            objSubTask.progressValue = duration + "%";

                        //            objSubTask.actualStart = startdate.ToString("yyyy-MM-dd");
                        //            objSubTask.actualEnd = enddate.ToString("yyyy-MM-dd");
                        //            if (subtaskrow == subtaskdata.Count)
                        //            {
                        //                objSubTask.connectTo = i.ToString();
                        //                objSubTask.connecterType = "finish - finish";
                        //            }


                        //            SubTask.Add(objSubTask);
                        //        }

                        //        if (subtaskdata.Count > 0)
                        //        {
                        //            objTask.children = SubTask;
                        //        }
                        //        Task.Add(objTask);
                        //    }

                        //    if (taskdata.Count > 0)
                        //    {
                        //        objActivity.children = Task;
                        //    }
                        //}
                        Activity.Add(objActivity);
                    }

                    if (activitydata.Count > 0)
                    {
                        ganttchart.children = Activity;
                    }
                    ganttchartmodellist.Add(ganttchart);
                }

                var x = JsonConvert.SerializeObject(ganttchartmodellist, Formatting.Indented);
                return Json(x, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
        }

        public JsonResult getgannchartvalue(int? projectid)
        {
            try
            {
                var projectdata = entities.sp_get_project(1, 1, projectid).ToList();

                List<ganttchartmodel> ganttchartmodellist = new List<ganttchartmodel>();
                int i = 1;
                foreach (var project in projectdata)
                {
                    ganttchartmodel ganttchart = new ganttchartmodel();
                    ganttchart.id = i.ToString();
                    i++;
                    ganttchart.name = project.name.ToString();
                    ganttchart.progressValue = project.progressValue.ToString() + "%";
                    ganttchart.actualStart = project.startdate.ToString("yyyy-MM-dd");
                    ganttchart.actualEnd = project.enddate.ToString("yyyy-MM-dd");
                    ganttchart.connectTo = i.ToString();
                    ganttchart.connecterType = "finish - start";

                    List<Children> Milestone = new List<Children>();
                    var milestonedata = entities.sp_get_milestone(1, 1, project.id, 0).ToList();
                    foreach (var itemmilestone in milestonedata)
                    {
                        Children objMilestone = new Children();
                        objMilestone.id = i.ToString();
                        i++;
                        objMilestone.name = itemmilestone.name.ToString();
                        objMilestone.progressValue = itemmilestone.progressValue.ToString() + "%";
                        //objMilestone.actualStart = "Date.UTC(" + itemmilestone.startdate.ToString("yyyy,MM,dd") + ")";
                        objMilestone.actualStart = itemmilestone.startdate.ToString("yyyy-MM-dd");
                        objMilestone.actualEnd = itemmilestone.enddate.ToString("yyyy-MM-dd");
                        objMilestone.connectTo = i.ToString();
                        objMilestone.connecterType = "finish - start";

                        List<Children> Task = new List<Children>();
                        var taskdata = entities.sp_get_task(1, 1, itemmilestone.id, 0).ToList();
                        foreach (var itemtask in taskdata)
                        {
                            Children objTask = new Children();
                            objTask.id = i.ToString();
                            i++;
                            objTask.name = itemtask.name.ToString();
                            objTask.progressValue = itemtask.progressValue.ToString() + "%";
                            objTask.actualStart = itemtask.startdate.ToString("yyyy-MM-dd");
                            objTask.actualEnd = itemtask.enddate.ToString("yyyy-MM-dd");
                            objTask.connectTo = i.ToString();
                            objTask.connecterType = "finish - start";

                            List<Children> SubTask = new List<Children>();
                            var subtaskdata = entities.sp_get_subtask(1, 1, itemtask.id, 0).ToList();
                            int subtaskrow = 0;
                            foreach (var itemsubtask in subtaskdata)
                            {
                                subtaskrow++;
                                Children objSubTask = new Children();
                                objSubTask.id = i.ToString();
                                i++;
                                objSubTask.name = itemsubtask.name.ToString();
                                objSubTask.progressValue = itemsubtask.progressValue.ToString() + "%";
                                objSubTask.actualStart = itemsubtask.startdate.ToString("yyyy-MM-dd");
                                objSubTask.actualEnd = itemsubtask.enddate.ToString("yyyy-MM-dd");
                                if (subtaskrow != subtaskdata.Count)
                                {
                                    objSubTask.connectTo = i.ToString();
                                    objSubTask.connecterType = "finish - finish";
                                }


                                SubTask.Add(objSubTask);
                            }

                            if (subtaskdata.Count > 0)
                            {
                                objTask.children = SubTask;
                                Task.Add(objTask);
                            }
                        }

                        if (taskdata.Count > 0)
                        {
                            objMilestone.children = Task;
                            Milestone.Add(objMilestone);
                        }
                    }

                    if (milestonedata.Count > 0)
                    {
                        ganttchart.children = Milestone;
                        ganttchartmodellist.Add(ganttchart);
                    }
                }

                var x = JsonConvert.SerializeObject(ganttchartmodellist, Formatting.Indented);
                return Json(x, JsonRequestBehavior.AllowGet);
                //return Json(ganttchartmodellist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
        }

        //protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        //{
        //    return new JsonResult()
        //    {
        //        Data = data,
        //        ContentType = contentType,
        //        ContentEncoding = contentEncoding,
        //        JsonRequestBehavior = behavior,
        //        MaxJsonLength = Int32.MaxValue
        //    };
        //}
    }
=======
﻿using Cybereum.Filters;
using Cybereum.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Newtonsoft.Json.Linq;

namespace Cybereum.Controllers
{
    public class HomeController : Controller
    {

        cybereumEntities entities = new cybereumEntities();

        [Authorize]
        [SessionTimeout]
        public ActionResult Index()
        {
            long projectcount = 0;
            var countapproved = (from p in entities.tbl_user
                                 where p.emailverification == true && p.isactive == 1 && p.roleid != 1
                                 select p.userid).Count();

            var countpending = (from p in entities.tbl_user
                                where p.emailverification == true && p.isactive == 0 && p.roleid != 1
                                select p.userid).Count();

<<<<<<< Updated upstream
            //var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + HttpUtility.UrlEncode(gremlinvariables.database) + "/colls/" + HttpUtility.UrlEncode(gremlinvariables.collection), password: gremlinvariables.authKey);
            //using (var gremlinClient = new GremlinClient(
            //    gremlinServer,
            //    new GraphSON2Reader(),
            //    new GraphSON2Writer(),
            //    GremlinClient.GraphSON2MimeType,
            //    gremlinvariables.connectionPoolSettings))
            //{
            var gremlinScript = "g.V().hasLabel('project').count()";
            try
            {
                //var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                //task.Wait();
                //var results = task.Result;
=======
            
            var gremlinScript = "g.V().hasLabel('project').count()";
            try
            {
>>>>>>> Stashed changes
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                projectcount = results.ToList()[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
<<<<<<< Updated upstream
            //}
=======
            
>>>>>>> Stashed changes
            ViewBag.countApproved = countapproved;
            ViewBag.countPending = countpending;
            ViewBag.countProject = projectcount;

            return View();
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult Dashboard()
        {
            int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
            cybereumEntities entities = new cybereumEntities();
            var countUser = (from p in entities.tbl_user
                             where p.emailverification == true && p.isactive == 1 && p.pmuserid == pmuserid && p.roleid == 3
                             select p.userid).Count();

            
            long projectcount = 0;
            long taskcount = 0;
<<<<<<< Updated upstream
            //var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + HttpUtility.UrlEncode(gremlinvariables.database) + "/colls/" + HttpUtility.UrlEncode(gremlinvariables.collection), password: gremlinvariables.authKey);
            //using (var gremlinClient = new GremlinClient(
            //    gremlinServer,
            //    new GraphSON2Reader(),
            //    new GraphSON2Writer(),
            //    GremlinClient.GraphSON2MimeType,
            //    gremlinvariables.connectionPoolSettings))
            //{
            var gremlinScript = "g.V().has('project','createdby','" + pmuserid + "').or().has('project','projectmembers','" + pmuserid + "')";
            try
            {
                //var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                //task.Wait();
                //var results = task.Result;
=======
            var gremlinScript = "g.V().has('project','createdby','" + pmuserid + "').or().has('project','projectmembers','" + pmuserid + "')";
            try
            {
>>>>>>> Stashed changes
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                projectcount = results.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var gremlinScript1 = "g.V().has('task','createdby','" + pmuserid + "').count()";
            if (Convert.ToInt16(Session["RoleId"]) == (int)Role.User)
            {
                gremlinScript1 = "g.V().has('task','assignedto','" + pmuserid + "').count()";
            }

            try
            {
                //var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript1);
                //task.Wait();
                //var results = task.Result;
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                taskcount = results.ToList()[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            ViewBag.countUser = countUser;
            ViewBag.countProject = projectcount;
            ViewBag.countTask = taskcount;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [Authorize]
        [SessionTimeout]
        // GET: Project
        public ActionResult GANTT()
        {
            return View();
        }


        [Authorize]
        [SessionTimeout]
        public ActionResult GANTTChart(string projectid, ProjectActivity Activity)
        {
            if (projectid == null)
            {
                ViewBag.projectid = Convert.ToInt32(Session["ProjectId"]);
                Session["ProjectId"] = ViewBag.projectid;
            }
            else
            {
                ViewBag.projectid = projectid;
                Session["ProjectId"] = projectid;
            }
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream


            //var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').order().by('enddate',decr).limit(1).project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
            //var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + gremlinvariables.database + "/colls/" + gremlinvariables.collection, password: gremlinvariables.authKey);
            //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
            //{
            //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
            //    task.Wait();
            //    var result = task.Result;
            //    if (result.Count > 0)
            //    {
            //        foreach (var item in result)
            //        {
            //            Activity.startdate = Convert.ToDateTime(item["enddate"]);
            //            Activity.enddate = Convert.ToDateTime(item["enddate"]);
            //        }
            //    }
            //    else
            //    {
            //        Activity.startdate = DateTime.Today;
            //        Activity.enddate = DateTime.Today;
            //    }
            //}

            return View(Activity);
            //List<GanttChartItem> items = getgannchartNew(projectid);
            //return View(model:items);
=======
            return View(Activity);
>>>>>>> Stashed changes
=======
            return View(Activity);
>>>>>>> Stashed changes
        }


        [Authorize]
        [SessionTimeout]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GANTTChart([Bind(Include = "id,activityname,ismilestone,startdate,enddate,durations,predecessors,createdby,createddate,modifiedby,modifieddate,projectid")] ProjectActivity tbl_activity)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                long duration = 0;
                if (DateTime.Now.Date < tbl_activity.startdate.Date)
                {
                    duration = 0;
                }
                else if (DateTime.Now.Date > tbl_activity.enddate.Date)
                {
                    duration = 100;
                }
                else
                {
                    double dt1 = (DateTime.Now.Date - tbl_activity.startdate.Date).TotalDays;
                    double dt2 = (tbl_activity.enddate.Date - tbl_activity.startdate.Date).TotalDays;
                    if (dt2 != 0)
                        duration = Convert.ToInt64((dt1 / dt2) * 100);
                }


                var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + gremlinvariables.database + "/colls/" + gremlinvariables.collection, password: gremlinvariables.authKey);
                long count = 0;
                if (tbl_activity.id == null)
                {
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').has('activity','projectid','" + tbl_activity.projectid + "').count()";
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var objList = task.Result;
                        count = objList.ToList()[0];
                    }
                    if (count > 0)
                    {
                        message = "Activity name already exists.";
                        goto endloop;
                    }
                }


                if (tbl_activity.id == null)
                {
                    if (tbl_activity.createdby == null)
                    {
                        tbl_activity.createdby = Session["LoggedInUserId"].ToString();
                        tbl_activity.createdusername = Session["Username"].ToString();
                    }

                    string gremlinScript = $"g.addV('activity').property('pk', '{tbl_activity.activityname}')" +
                            $".property('activityname', '{tbl_activity.activityname}')" +
                            $".property('startdate', '{tbl_activity.startdate.ToString("yyyy-MM-dd")}')" +
                            $".property('enddate', '{tbl_activity.enddate.ToString("yyyy-MM-dd")}')" +
                            $".property('projectid', '{tbl_activity.projectid}')" +
                            $".property('durations', '{duration}')" +
                            $".property('ismilestone', '{tbl_activity.ismilestone}')" +
                            $".property('createdby', '{Convert.ToInt32(tbl_activity.createdby)}')" +
                            $".property('createdusername', '{tbl_activity.createdusername}')" +
                            $".property('createdon', '{DateTime.Now}')" +
                            $".property('type', 'activity')";

                    // Execute the Gremlin script                    
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Added Successfully";
                    }


                    gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').project('id').by(values('id'))";
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        foreach (var result1 in result)
                        {
                            tbl_activity.id = Convert.ToString(result1["id"]);
                        }
                    }


                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('contains').to(g.V('{tbl_activity.id}'))";
                    //gremlinScript = $"\ng.V().has('project','projectid','{tbl_activity.projectid}').as('a').V().has('activity','activityname','{tbl_activity.activityname}').addE('projectactivity').to('a')";
                    // Execute the Gremlin script
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Gremlin script executed successfully";
                    }

                    //Connect the predeccesors to succesors
                    //foreach (var predecessor in tbl_activity.Predecessors)
                    //{
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('precedes').property('duration', '{tbl_activity.durations}').to(g.V('{tbl_activity.activityname}'))";
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Gremlin script executed successfully";
                    }
                    //}
                }
                //ViewBag.projectid = tbl_activity.projectid;
                return RedirectToAction("GANTTChart", new { projectid = tbl_activity.projectid });
            }
            endloop:
            ViewBag.Message = message;
            //ViewBag.projectid =tbl_activity.projectid;
            //return View(tbl_activity);
            return RedirectToAction("GANTTChart", new { projectid = tbl_activity.projectid });
        }

        public JsonResult getTestvalue()
        {
            try
            {
                //var projectdata = entities.sp_get_project(1, 1, 0).ToList();

                var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: gremlinvariables.containerLink, password: gremlinvariables.authKey);
                var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType, gremlinvariables.connectionPoolSettings);
                var gremlinScript = "g.V().has('project','id','3ce54f64-879c-40c1-a268-9f60f43ccde9').project('id','projectname','startdate','enddate').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate'))";
                var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                task.Wait();
                var projectdata = task.Result;

                List<TestModel> ganttchartmodellist = new List<TestModel>();
                foreach (var project in projectdata)
                {
                    TestModel ganttchart = new TestModel();
                    ganttchart.task = project.name.ToString();
                    ganttchart.type = project.name.ToString();
                    ganttchart.startTime = project.startdate.ToString("yyyy-MM-dd");
                    ganttchart.endTime = project.enddate.ToString("yyyy-MM-dd");
                    ganttchartmodellist.Add(ganttchart);

                    var milestonedata = entities.sp_get_milestone(1, 1, project.id, 0).ToList();
                    foreach (var itemmilestone in milestonedata)
                    {
                        TestModel ganttchart1 = new TestModel();
                        ganttchart1.task = itemmilestone.name.ToString();
                        ganttchart1.type = project.name.ToString();
                        ganttchart1.startTime = itemmilestone.startdate.ToString("yyyy-MM-dd");
                        ganttchart1.endTime = itemmilestone.enddate.ToString("yyyy-MM-dd");
                        ganttchartmodellist.Add(ganttchart1);

                        var taskdata = entities.sp_get_task(1, 1, itemmilestone.id, 0).ToList();
                        foreach (var itemtask in taskdata)
                        {
                            TestModel ganttchart2 = new TestModel();
                            ganttchart2.task = itemtask.name.ToString();
                            ganttchart2.type = itemmilestone.name.ToString();
                            ganttchart2.startTime = itemtask.startdate.ToString("yyyy-MM-dd");
                            ganttchart2.endTime = itemtask.enddate.ToString("yyyy-MM-dd");
                            ganttchartmodellist.Add(ganttchart2);

                            var subtaskdata = entities.sp_get_subtask(1, 1, itemtask.id, 0).ToList();
                            foreach (var itemsubtask in subtaskdata)
                            {
                                TestModel ganttchart3 = new TestModel();
                                ganttchart3.task = itemsubtask.name.ToString();
                                ganttchart3.type = itemtask.name.ToString();
                                ganttchart3.startTime = itemsubtask.startdate.ToString("yyyy-MM-dd");
                                ganttchart3.endTime = itemsubtask.enddate.ToString("yyyy-MM-dd");
                                ganttchartmodellist.Add(ganttchart3);
                            }
                        }
                    }

                }

                var x = JsonConvert.SerializeObject(ganttchartmodellist, Formatting.Indented);
                return Json(x, JsonRequestBehavior.AllowGet);
                //return Json(ganttchartmodellist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
=======
            return View(Activity);
>>>>>>> Stashed changes
        }
        

        public JsonResult getgannchart(string projectid)
        {
            try
            {
                DateTime startdate;
                DateTime enddate;
                string connection = string.Empty;

<<<<<<< Updated upstream
                //var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: gremlinvariables.containerLink, password: gremlinvariables.authKey);
                //var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType, gremlinvariables.connectionPoolSettings);
                var gremlinScript = "g.V().has('project','id','" + projectid + "').project('id','projectname','startdate','enddate').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate'))";
                //var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                //task.Wait();
                //var projectdata = task.Result;
=======
                var gremlinScript = "g.V().has('project','id','" + projectid + "').project('id','projectname','startdate','enddate').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate'))";
>>>>>>> Stashed changes
                var projectdata = IGUtilities.ExecuteGremlinScript(gremlinScript);

                List<ganttchartmodel> ganttchartmodellist = new List<ganttchartmodel>();
                int i = 1;
                foreach (var project in projectdata)
                {
                    ganttchartmodel ganttchart = new ganttchartmodel();
                    ganttchart.id = i.ToString();
                    i++;
                    ganttchart.taskid = project["id"].ToString();
                    ganttchart.name = project["projectname"].ToString();


                    startdate = Convert.ToDateTime(project["startdate"].ToString());
                    enddate = Convert.ToDateTime(project["enddate"].ToString());

                    long duration = 0;
                    if (DateTime.Now.Date < startdate.Date)
                    {
                        duration = 0;
                    }
                    else if (DateTime.Now.Date > enddate.Date)
                    {
                        duration = 100;
                    }
                    else
                    {
                        double dt1 = (DateTime.Now.Date - startdate.Date).TotalDays;
                        double dt2 = (enddate.Date - startdate.Date).TotalDays;
                        if (dt2 != 0)
                            duration = Convert.ToInt64((dt1 / dt2) * 100);
                    }
                    //int duration = IGUtilities.CalculateDays(startdate.Date, enddate.Date) * 8;

                    //ganttchart.progressValue = duration + "%";
                    ganttchart.actualStart = startdate.ToString("yyyy-MM-dd");
                    ganttchart.actualEnd = enddate.ToString("yyyy-MM-dd");
                    
                    List<Children> Activity = new List<Children>();
                    List<chartconnector> chartconnector = new List<chartconnector>();
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream

                    gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations','predecessors').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('predecessors').fold())";
                    //gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').project('id','activityname','startdate','enddate','durations').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations'))";
                    var taskactivity = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    taskactivity.Wait();
                    var activitydata = taskactivity.Result;

=======

                    gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations','predecessors').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('predecessors').fold())";
                    //gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').project('id','activityname','startdate','enddate','durations').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations'))";
                    //var taskactivity = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //taskactivity.Wait();
                    //var activitydata = taskactivity.Result;
                    var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);

>>>>>>> Stashed changes
=======

                    gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations','predecessors').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('predecessors').fold())";
                    //gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').project('id','activityname','startdate','enddate','durations').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations'))";
                    //var taskactivity = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //taskactivity.Wait();
                    //var activitydata = taskactivity.Result;
                    var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    int activityindex = 0;
>>>>>>> Stashed changes
                    foreach (var itemactivity in activitydata)
=======

                    gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations','predecessors').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('predecessors').fold())";
                    var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    string pList = JsonConvert.SerializeObject(activitydata);
                    List<ProjectActivity> Activitylist = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);
                    Activitylist = Activitylist.OrderBy(a => a.startdate).ThenBy(a => a.enddate).ToList();
                    int activityindex = 0;
                    foreach (var itemactivity in Activitylist)
>>>>>>> Stashed changes
                    {
                        activityindex++;
                        Children objActivity = new Children();
                        objActivity.id = i.ToString();
                        i++;
                        objActivity.taskid = itemactivity.id.ToString();
                        objActivity.name = itemactivity.activityname;//itemactivity["activityname"].ToString();

                        startdate = itemactivity.startdate;
                        enddate = itemactivity.enddate;

<<<<<<< Updated upstream
                        //duration = itemactivity["durations"].ToString();
=======
>>>>>>> Stashed changes
                        if (DateTime.Now.Date < startdate.Date)
                        {
                            duration = 0;
                        }
                        else if (DateTime.Now.Date > enddate.Date)
                        {
                            duration = 100;
                        }
                        else
                        {
                            double dt1 = (DateTime.Now.Date - startdate.Date).TotalDays;
                            double dt2 = (enddate.Date - startdate.Date).TotalDays;
                            if (dt2 != 0)
                                duration = Convert.ToInt64((dt1 / dt2) * 100);
                        }
<<<<<<< Updated upstream
                        //duration = IGUtilities.CalculateDays(startdate.Date, enddate.Date) * 8;
=======
>>>>>>> Stashed changes
                        //objActivity.progressValue = duration + "%";

                        objActivity.actualStart = startdate.ToString("yyyy-MM-dd");
                        objActivity.actualEnd = enddate.ToString("yyyy-MM-dd");
<<<<<<< Updated upstream
                        //objActivity.connectTo = i.ToString();
                        //objActivity.connecterType = "start - finish";

                        var predecessors = itemactivity["predecessors"];
=======
                        
                        var predecessors = itemactivity.Predecessors;
>>>>>>> Stashed changes
                        var stringlist = JsonConvert.SerializeObject(predecessors);
                        var jArray = JArray.Parse(stringlist);
                        string tasks = string.Empty;
                        foreach (string item in jArray)
                        {
                            tasks = tasks + item + ",";
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
                        }
                        if (tasks != "") tasks = tasks.Remove(tasks.LastIndexOf(",")).ToString();
                        if (tasks.ToString() != string.Empty)
                        {
                            string[] ints = tasks.Split(',').ToArray();
                            connection = string.Empty;
                            if (ints.Count() > 1)
                            {
                                for (int j = 0; j <= ints.Count() - 1; j++)
                                {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                                    //if (j == 0)
                                    //{
                                    //    var connector = Activity.Find(a => a.taskid == ints[j]);
                                    //    if (connector != null)
                                    //    {
                                    //        objActivity.connectTo = connector.id;
                                    //        objActivity.connecterType = "start - finish";
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    var connector = Activity.Find(a => a.taskid == ints[j]);
                                    //    if (connector != null)
                                    //    {
                                    //        var student = connector;
                                    //        if (student != null)
                                    //        {
                                    //            if (student.connectTo == null)
                                    //            {
                                    //                student.connectTo = objActivity.id;
                                    //                student.connecterType = "Finish - Start";
                                    //            }
                                    //        }
                                    //    }
                                    //}

=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
                                    var connector = Activity.Find(a => a.taskid == ints[j]);
                                    chartconnector conn = new chartconnector();
                                    if (connector != null)
                                    {
                                        conn.connectTo = connector.id;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                                        conn.connectorType = "finish - start";
=======
                                        conn.connectorType = "start-finish";
>>>>>>> Stashed changes
                                    }
                                    //  connector: [
                                    //  { connectTo: "1_5", connectorType: "start-finish"},
                                    //  { connectTo: "1_1", connectorType: "finish-finish"},
                                    //  { connectTo: "1_3", connectorType: "finish-finish"},
                                    //  { connectTo: "1_4", connectorType: "start-start"}
<<<<<<< Updated upstream
                                    //],  
=======
                                    //],
>>>>>>> Stashed changes
=======
                                        conn.connectorType = "start-finish";
                                    }                                    
>>>>>>> Stashed changes
                                    chartconnector.Add(conn);
                                }
                                objActivity.connector = chartconnector;
                            }
                            else
                            {
                                var connector = Activity.Find(a => a.taskid == ints[0]);
                                if (connector != null)
                                {
                                    objActivity.connectTo = connector.id;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                                    objActivity.connecterType = "start - finish";
                                }
                            }
                            //objActivity.connectTo = tasks;
                        }
=======
                        }
                        if (tasks != "") tasks = tasks.Remove(tasks.LastIndexOf(",")).ToString();
                        if (tasks.ToString() != string.Empty)
                        {
                            string[] ints = tasks.Split(',').ToArray();
                            connection = string.Empty;
                            if (ints.Count() > 1)
                            {
                                for (int j = 0; j <= ints.Count() - 1; j++)
                                {
                                    //if (j == 0)
                                    //{
                                    //    var connector = Activity.Find(a => a.taskid == ints[j]);
                                    //    if (connector != null)
                                    //    {
                                    //        objActivity.connectTo = connector.id;
                                    //        objActivity.connecterType = "start - finish";
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    var connector = Activity.Find(a => a.taskid == ints[j]);
                                    //    if (connector != null)
                                    //    {
                                    //        var student = connector;
                                    //        if (student != null)
                                    //        {
                                    //            if (student.connectTo == null)
                                    //            {
                                    //                student.connectTo = objActivity.id;
                                    //                student.connecterType = "Finish - Start";
                                    //            }
                                    //        }
                                    //    }
                                    //}

                                    var connector = Activity.Find(a => a.taskid == ints[j]);
                                    chartconnector conn = new chartconnector();
                                    if (connector != null)
                                    {
                                        conn.connectTo = connector.id;
                                        conn.connectorType = "finish - start";
                                    }
                                    //  connector: [
                                    //  { connectTo: "1_5", connectorType: "start-finish"},
                                    //  { connectTo: "1_1", connectorType: "finish-finish"},
                                    //  { connectTo: "1_3", connectorType: "finish-finish"},
                                    //  { connectTo: "1_4", connectorType: "start-start"}
                                    //],
                                    chartconnector.Add(conn);
                                }
                                objActivity.connector = chartconnector;
                            }
                            else
                            {
                                var connector = Activity.Find(a => a.taskid == ints[0]);
                                if (connector != null)
                                {
                                    objActivity.connectTo = connector.id;
                                    objActivity.connecterType = "start - finish";
                                }
                            }
                        }
>>>>>>> Stashed changes
=======
                                    objActivity.connecterType = "start-finish";
                                }
                            }
                        }
>>>>>>> Stashed changes
                        else
                        {
                            //objActivity.connectTo = i.ToString();
                            //objActivity.connecterType = "start - finish";
<<<<<<< Updated upstream
                        }

=======
                        }                                                
                        
>>>>>>> Stashed changes
=======
                                    objActivity.connecterType = "start-finish";
                                }
                            }
                        }
                        else
                        {
                            //objActivity.connectTo = (i-2).ToString();
                            //objActivity.connecterType = "start - finish";
                        }                                                
                        
>>>>>>> Stashed changes
                        //if (ganttchart.name != "FPSO-3")
                        //{
                        //    List<Children> Task = new List<Children>();
                        //    gremlinScript = "g.V().has('task','activityid','" + itemactivity["id"] + "').project('id','taskname','startdate','enddate','durations','activityid').by(id()).by(values('taskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('activityid'))";
                        //    var tasktask = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        //    tasktask.Wait();
                        //    var taskdata = tasktask.Result;
                        //    foreach (var itemtask in taskdata)
                        //    {
                        //        Children objTask = new Children();
                        //        objTask.id = i.ToString();
                        //        i++;
                        //        objTask.taskid = itemtask["id"].ToString();
                        //        objTask.name = itemtask["taskname"].ToString();
                        //        //objTask.progressValue = itemtask["durations"].ToString() + "%";                            
                        //        startdate = Convert.ToDateTime(itemtask["startdate"].ToString());
                        //        enddate = Convert.ToDateTime(itemtask["enddate"].ToString());

                        //        duration = 0;
                        //        if (DateTime.Now.Date < startdate.Date)
                        //        {
                        //            duration = 0;
                        //        }
                        //        else if (DateTime.Now.Date > enddate.Date)
                        //        {
                        //            duration = 100;
                        //        }
                        //        else
                        //        {
                        //            double dt1 = (DateTime.Now.Date - startdate.Date).TotalDays;
                        //            double dt2 = (enddate.Date - startdate.Date).TotalDays;
                        //            if (dt2 != 0)
                        //                duration = Convert.ToInt64((dt1 / dt2) * 100);
                        //        }

                        //        objTask.progressValue = duration + "%";

                        //        objTask.actualStart = startdate.ToString("yyyy-MM-dd");
                        //        objTask.actualEnd = enddate.ToString("yyyy-MM-dd");
                        //        objTask.connectTo = i.ToString();
                        //        objTask.connecterType = "finish - start";

                        //        List<Children> SubTask = new List<Children>();
                        //        gremlinScript = "g.V().has('subtask','taskid','" + itemtask["id"] + "').project('id','subtaskname','startdate','enddate','durations').by(id()).by(values('subtaskname')).by(values('startdate')).by(values('enddate')).by(values('durations'))";
                        //        var subtask = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        //        subtask.Wait();
                        //        var subtaskdata = subtask.Result;
                        //        int subtaskrow = 0;
                        //        foreach (var itemsubtask in subtaskdata)
                        //        {
                        //            subtaskrow++;
                        //            Children objSubTask = new Children();
                        //            objSubTask.id = i.ToString();
                        //            i++;
                        //            objSubTask.taskid = itemsubtask["id"].ToString();
                        //            objSubTask.name = itemsubtask["subtaskname"].ToString();
                        //            //objSubTask.progressValue = itemsubtask["durations"].ToString() + "%";
                        //            startdate = Convert.ToDateTime(itemsubtask["startdate"].ToString());
                        //            enddate = Convert.ToDateTime(itemsubtask["enddate"].ToString());
                        //            duration = 0;
                        //            if (DateTime.Now.Date < startdate.Date)
                        //            {
                        //                duration = 0;
                        //            }
                        //            else if (DateTime.Now.Date > enddate.Date)
                        //            {
                        //                duration = 100;
                        //            }
                        //            else
                        //            {
                        //                double dt1 = (DateTime.Now.Date - startdate.Date).TotalDays;
                        //                double dt2 = (enddate.Date - startdate.Date).TotalDays;
                        //                if (dt2 != 0)
                        //                    duration = Convert.ToInt64((dt1 / dt2) * 100);
                        //            }

                        //            objSubTask.progressValue = duration + "%";

                        //            objSubTask.actualStart = startdate.ToString("yyyy-MM-dd");
                        //            objSubTask.actualEnd = enddate.ToString("yyyy-MM-dd");
                        //            if (subtaskrow == subtaskdata.Count)
                        //            {
                        //                objSubTask.connectTo = i.ToString();
                        //                objSubTask.connecterType = "finish - finish";
                        //            }


                        //            SubTask.Add(objSubTask);
                        //        }

                        //        if (subtaskdata.Count > 0)
                        //        {
                        //            objTask.children = SubTask;
                        //        }
                        //        Task.Add(objTask);
                        //    }

                        //    if (taskdata.Count > 0)
                        //    {
                        //        objActivity.children = Task;
                        //    }
                        //}
                        Activity.Add(objActivity);
                    }
                    //foreach (Children objactivity in Activity)
                    //{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                    //    if (objactivity.connectTo == null )
                    //    {

                    //    }
                    //}
=======
=======
>>>>>>> Stashed changes
                    //    //gremlinScript = $"g.V().has('activity', 'id', '{objactivity.taskid}').in('precedes').project('id').by(id)";
                    //    gremlinScript = $"g.V().has('activity', 'predecessors', '{objactivity.taskid}').project('id').by(id)";
                    //    var activitylist = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //    if (activitylist.Count > 0)
                    //    {
                    //        chartconnector = new List<chartconnector>();
                    //        foreach (var item in activitylist)
                    //        {
                    //            chartconnector conn = new chartconnector();
                    //            var connector = Activity.Find(a => a.taskid == item["id"]);
                    //            if (connector != null)
                    //            {
                    //                conn.connectTo = connector.id;
                    //                conn.connectorType = "finish - start";
                    //            }
                    //            chartconnector.Add(conn);
                    //        }
                    //        objactivity.connector = chartconnector;
                    //    }
                    //}

<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

                    if (activitydata.Count > 0)
                    {
<<<<<<< Updated upstream
                        Activity = Activity.OrderBy(a => a.actualStart).ThenBy(a => a.actualEnd).ToList();
<<<<<<< Updated upstream
=======

                        var item = Activity[Activity.Count - 2];
                        Activity.LastOrDefault().connectTo = item.id.ToString();
                        Activity.LastOrDefault().connecterType = "Finish-start";

>>>>>>> Stashed changes
=======
                        var item = Activity[Activity.Count - 2];
                        Activity.LastOrDefault().connectTo = item.id.ToString();
                        Activity.LastOrDefault().connecterType = "finish-start";

>>>>>>> Stashed changes
                        ganttchart.children = Activity;
                    }
                    ganttchartmodellist.Add(ganttchart);
                }

                var x = JsonConvert.SerializeObject(ganttchartmodellist, Formatting.Indented);
                return Json(x, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
<<<<<<< Updated upstream
        }

        public JsonResult getgannchartvalue(int? projectid)
        {
            try
            {
                var projectdata = entities.sp_get_project(1, 1, projectid).ToList();

                List<ganttchartmodel> ganttchartmodellist = new List<ganttchartmodel>();
                int i = 1;
                foreach (var project in projectdata)
                {
                    ganttchartmodel ganttchart = new ganttchartmodel();
                    ganttchart.id = i.ToString();
                    i++;
                    ganttchart.name = project.name.ToString();
                    ganttchart.progressValue = project.progressValue.ToString() + "%";
                    ganttchart.actualStart = project.startdate.ToString("yyyy-MM-dd");
                    ganttchart.actualEnd = project.enddate.ToString("yyyy-MM-dd");
                    ganttchart.connectTo = i.ToString();
                    ganttchart.connecterType = "finish - start";

                    List<Children> Milestone = new List<Children>();
                    var milestonedata = entities.sp_get_milestone(1, 1, project.id, 0).ToList();
                    foreach (var itemmilestone in milestonedata)
                    {
                        Children objMilestone = new Children();
                        objMilestone.id = i.ToString();
                        i++;
                        objMilestone.name = itemmilestone.name.ToString();
                        objMilestone.progressValue = itemmilestone.progressValue.ToString() + "%";
                        //objMilestone.actualStart = "Date.UTC(" + itemmilestone.startdate.ToString("yyyy,MM,dd") + ")";
                        objMilestone.actualStart = itemmilestone.startdate.ToString("yyyy-MM-dd");
                        objMilestone.actualEnd = itemmilestone.enddate.ToString("yyyy-MM-dd");
                        //objMilestone.connectTo = i.ToString();
                        objMilestone.connecterType = "finish - start";

                        List<Children> Task = new List<Children>();
                        var taskdata = entities.sp_get_task(1, 1, itemmilestone.id, 0).ToList();
                        foreach (var itemtask in taskdata)
                        {
                            Children objTask = new Children();
                            objTask.id = i.ToString();
                            i++;
                            objTask.name = itemtask.name.ToString();
                            objTask.progressValue = itemtask.progressValue.ToString() + "%";
                            objTask.actualStart = itemtask.startdate.ToString("yyyy-MM-dd");
                            objTask.actualEnd = itemtask.enddate.ToString("yyyy-MM-dd");
                            //objTask.connectTo = i.ToString();
                            objTask.connecterType = "finish - start";

                            List<Children> SubTask = new List<Children>();
                            var subtaskdata = entities.sp_get_subtask(1, 1, itemtask.id, 0).ToList();
                            int subtaskrow = 0;
                            foreach (var itemsubtask in subtaskdata)
                            {
                                subtaskrow++;
                                Children objSubTask = new Children();
                                objSubTask.id = i.ToString();
                                i++;
                                objSubTask.name = itemsubtask.name.ToString();
                                objSubTask.progressValue = itemsubtask.progressValue.ToString() + "%";
                                objSubTask.actualStart = itemsubtask.startdate.ToString("yyyy-MM-dd");
                                objSubTask.actualEnd = itemsubtask.enddate.ToString("yyyy-MM-dd");
                                if (subtaskrow != subtaskdata.Count)
                                {
                                    //objSubTask.connectTo = i.ToString();
                                    objSubTask.connecterType = "finish - finish";
                                }


                                SubTask.Add(objSubTask);
                            }

                            if (subtaskdata.Count > 0)
                            {
                                objTask.children = SubTask;
                                Task.Add(objTask);
                            }
                        }

                        if (taskdata.Count > 0)
                        {
                            objMilestone.children = Task;
                            Milestone.Add(objMilestone);
                        }
                    }

                    if (milestonedata.Count > 0)
                    {
                        ganttchart.children = Milestone;
                        ganttchartmodellist.Add(ganttchart);
                    }
                }

                var x = JsonConvert.SerializeObject(ganttchartmodellist, Formatting.Indented);
                return Json(x, JsonRequestBehavior.AllowGet);
                //return Json(ganttchartmodellist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
        }

        //protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        //{
        //    return new JsonResult()
        //    {
        //        Data = data,
        //        ContentType = contentType,
        //        ContentEncoding = contentEncoding,
        //        JsonRequestBehavior = behavior,
        //        MaxJsonLength = Int32.MaxValue
        //    };
        //}
=======
        }        
>>>>>>> Stashed changes
    }
>>>>>>> Stashed changes
}