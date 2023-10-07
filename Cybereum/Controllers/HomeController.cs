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

            var countorganization = (from p in entities.tbl_user
                                where p.emailverification == true && p.isactive == 1 && p.roleid != 1
                                select p.organization).Distinct().Count();

            string org= Session["Organization"].ToString();
            int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
            var usercount = (from p in entities.tbl_user
                             where p.emailverification == true && p.isactive == 1 && p.roleid != 1 && p.roleid != 4
                             && p.organization == org && p.userid!= pmuserid
                             select p.userid).Count();

            string projectmember = IGUtilities.getprojectmembers(pmuserid);
            if (projectmember == "") projectmember = "''";
            var gremlinScript = $"g.V().has('project','projectmanager',within({projectmember})).count()";
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
            ViewBag.countUser = usercount;
            ViewBag.countOrganization = countorganization;

            return View();
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult Dashboard()
        {
            int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
            cybereumEntities entities = new cybereumEntities();
            
            long projectcount = 0;
            long taskcount = 0;
            long subtaskcount = 0;
            //var gremlinScript = "g.V().has('project','projectmanager','" + pmuserid + "').or().has('project','projectmembers','" + pmuserid + "').count()";
            var gremlinScript = $"g.V().or(has('project','projectmanager','{ pmuserid }'),has('project','projectmembers','" + pmuserid + "')).count()";
            //var gremlinScript = "g.V().has('project','projectmanager','" + pmuserid + "').count()";
            try
            {
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                projectcount = results.ToList()[0];
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
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                taskcount = results.ToList()[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (Convert.ToInt16(Session["RoleId"]) == (int)Role.User)
            {
                gremlinScript1 = "g.V().has('subtask','assignedto','" + pmuserid + "').count()";
            }

            try
            {
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                subtaskcount = results.ToList()[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //ViewBag.countUser = countUser;
            ViewBag.countProject = projectcount;
            ViewBag.countTask = taskcount;
            ViewBag.countSubTask = subtaskcount;

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
                        objActivity.name = itemactivity.activityname;

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