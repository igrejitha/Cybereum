using Cybereum.Filters;
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
    [Authorize]
    public class HomeController : Controller
    {

        cybereumEntities entities = new cybereumEntities();

        //[Authorize (Roles = "Default Access")]
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

            
            var gremlinScript = "g.V().hasLabel('project').count()";
            try
            {
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                projectcount = results.ToList()[0];
            }
            catch (Exception ex)
            {
                throw ex;
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

            
            long projectcount = 0;
            long taskcount = 0;
            var gremlinScript = "g.V().has('project','createdby','" + pmuserid + "').or().has('project','projectmembers','" + pmuserid + "')";
            try
            {
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
            return View(Activity);
        }
        

        public JsonResult getgannchart(string projectid)
        {
            try
            {
                DateTime startdate;
                DateTime enddate;
                string connection = string.Empty;

                var gremlinScript = "g.V().has('project','id','" + projectid + "').project('id','projectname','startdate','enddate').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate'))";
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

                    gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations','predecessors').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('predecessors').fold())";
                    var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    string pList = JsonConvert.SerializeObject(activitydata);
                    List<ProjectActivity> Activitylist = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);
                    Activitylist = Activitylist.OrderBy(a => a.startdate).ThenBy(a => a.enddate).ToList();
                    int activityindex = 0;
                    foreach (var itemactivity in Activitylist)
                    {
                        activityindex++;
                        Children objActivity = new Children();
                        objActivity.id = i.ToString();
                        i++;
                        objActivity.taskid = itemactivity.id.ToString();
                        objActivity.name = itemactivity.activityname;//itemactivity["activityname"].ToString();

                        startdate = itemactivity.startdate;
                        enddate = itemactivity.enddate;

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
                        //objActivity.progressValue = duration + "%";

                        objActivity.actualStart = startdate.ToString("yyyy-MM-dd");
                        objActivity.actualEnd = enddate.ToString("yyyy-MM-dd");
                        
                        var predecessors = itemactivity.Predecessors;
                        var stringlist = JsonConvert.SerializeObject(predecessors);
                        var jArray = JArray.Parse(stringlist);
                        string tasks = string.Empty;
                        foreach (string item in jArray)
                        {
                            tasks = tasks + item + ",";
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
                                    var connector = Activity.Find(a => a.taskid == ints[j]);
                                    chartconnector conn = new chartconnector();
                                    if (connector != null)
                                    {
                                        conn.connectTo = connector.id;
                                        conn.connectorType = "start-finish";
                                    }                                    
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
                                    objActivity.connecterType = "start-finish";
                                }
                            }
                        }
                        else
                        {
                            //objActivity.connectTo = (i-2).ToString();
                            //objActivity.connecterType = "start - finish";
                        }                                                
                        
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
                        var item = Activity[Activity.Count - 2];
                        Activity.LastOrDefault().connectTo = item.id.ToString();
                        Activity.LastOrDefault().connecterType = "finish-start";

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
    }
}