using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cybereum.Models;
using Cybereum.Filters;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;

namespace Cybereum.Controllers
{
    public class TaskController : Controller
    {
        private cybereumEntities db = new cybereumEntities();

        // GET: Task
        [Authorize]
        [SessionTimeout]
        public ActionResult Index(int? milestoneid)
        {
            //var tbl_task = db.tbl_task.Include(t => t.tbl_project).Include(t => t.tbl_user).Include(t => t.tbl_tasktype);
            //return View(tbl_task.ToList());

            TempData["MilestoneId"] = milestoneid;
            ViewBag.milestoneid = TempData["MilestoneId"];
            TempData.Keep();
            if (milestoneid == 0)
            {
                ViewBag.milestoneid = Convert.ToInt32(Session["MilestoneId"]);
                Session["MilestoneId"] = ViewBag.milestoneid;
            }
            else
            {
                ViewBag.milestoneid = milestoneid;
                Session["MilestoneId"] = milestoneid;
            }
            //GetTask(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]), milestoneid);
            return View();
        }

        // GET: Task
        [Authorize]
        [SessionTimeout]
        public ActionResult List(string activityid)
        {
            TempData["ActivityId"] = activityid;
            ViewBag.activityid = TempData["ActivityId"];
            TempData.Keep();
            if (activityid == null)
            {
                ViewBag.activityid = Convert.ToInt32(Session["ActivityId"]);
                Session["ActivityId"] = ViewBag.activityid;
            }
            else
            {
                ViewBag.activityid = activityid;
                Session["ActivityId"] = activityid;
            }
            return View();
        }

        public async Task<JsonResult> tasklist(string activityid)
        {
            List<ProjectTask> list = new List<ProjectTask>();
            try
            {
                int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);


                var gremlinScript = "g.V().has('task','activityid','" + activityid + "').project('id','taskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdusername','createdon','activityid')" +
                                    ".by(id()).by(values('taskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('tasktype')).by(values('taskstatus')).by(values('assignedto')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('activityid'))";
                if (Convert.ToInt16(Session["RoleId"]) == (int)Role.User)
                {
                    gremlinScript = "g.V().has('task','assignedto','" + pmuserid + "').project('id','taskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdusername','createdon','activityid')" +
                                    ".by(id()).by(values('taskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('tasktype')).by(values('taskstatus')).by(values('assignedto')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('activityid'))";
                }
                try
                {
                    //var results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result in results)
                    {
                        ProjectTask task = new ProjectTask();
                        task.taskid = result["id"].ToString();
                        task.taskname = result["taskname"].ToString();
                        task.startdate = Convert.ToDateTime(result["startdate"]);
                        task.enddate = Convert.ToDateTime(result["enddate"]);
                        //task.tasktype = result["tasktype"];
                        //task.taskstatus = result["taskstatus"];
                        task.durations = Convert.ToInt64(result["durations"]);
                        task.activityid = result["activityid"].ToString();

                        gremlinScript = "g.V().has('activity','id','" + result["activityid"] + "').project('activityname').by(values('activityname'))";
                        //var results1 = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                        var results1 = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        foreach (var result1 in results1)
                        {
                            task.activityname = result1["activityname"];
                        }

                        task.assignedto = result["assignedto"].ToString();

                        int userid = Convert.ToInt32(result["assignedto"]);
                        var username = db.tbl_user.Where(x => x.userid == userid).FirstOrDefault();

                        if (username != null)
                        {
                            task.assignedusername = username.firstname + ' ' + username.lastname;
                        }

                        task.createdon = Convert.ToDateTime(result["createdon"]);

                        list.Add(task);
                    }
                    var taskresult = this.Json(new { data = list, recordsTotal = list.Count(), recordsFiltered = list.Count() }, JsonRequestBehavior.AllowGet);
                    return taskresult;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public ActionResult Addrecord(int Id)
        {
            ViewBag.Message = "Edit Task";
            ProjectTask task = new ProjectTask();

            //task = db.ProjectTask.Find(Id);
            return RedirectToAction("Create", task);
        }

        public ActionResult AddEditrecord(string Id)
        {
            ViewBag.Message = "Edit Task";

            var task = gettaskbyid(Id);
            return RedirectToAction("AddEditTask", task.Result);
        }

        public async Task<ProjectTask> gettaskbyid(string id)
        {
            ProjectTask projecttask = new ProjectTask();
            try
            {

                var gremlinScript = "g.V().has('task','id','" + id + "').project('id','taskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdusername','createdon','activityid')" +
                    ".by(id()).by(values('taskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('tasktype')).by(values('taskstatus')).by(values('assignedto')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('activityid'))";
                try
                {
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result in results)
                    {
                        projecttask.taskid = result["id"].ToString();
                        projecttask.activityid = result["activityid"].ToString();
                        projecttask.taskname = result["taskname"].ToString();
                        projecttask.startdate = Convert.ToDateTime(result["startdate"]);
                        projecttask.enddate = Convert.ToDateTime(result["enddate"]);
                        projecttask.tasktype = Convert.ToInt16(result["tasktype"]);
                        projecttask.taskstatus = Convert.ToInt16(result["taskstatus"]);
                        projecttask.assignedto = result["assignedto"];
                        projecttask.durations = Convert.ToInt64(result["durations"]);
                        //activity.Predecessors = result["predecessors"].ToString();
                        projecttask.createdby = result["createdby"].ToString();
                        projecttask.createdusername = result["createdusername"].ToString();
                        projecttask.createdon = Convert.ToDateTime(result["createdon"]);
                    }
                    return projecttask;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }



        [Authorize]
        [SessionTimeout]
        public ActionResult AddEditTask(int? taskid, string activityid, ProjectTask projecttask)
        {
            if (activityid == null)
            {
                ViewBag.activityid = Convert.ToInt32(Session["ActivityId"]);
                Session["ActivityId"] = ViewBag.activityid;
            }
            else
            {
                ViewBag.activityid = activityid;
                Session["ActivityId"] = activityid;
            }

            if (projecttask.taskid == null)
            {
                var gremlinScript = "g.V().has('task','activityid','" + activityid + "').order().by('createdon',decr).limit(1).project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
                var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        projecttask.startdate = Convert.ToDateTime(item["enddate"]);
                        projecttask.enddate = Convert.ToDateTime(item["enddate"]);
                    }
                }
                else
                {
                    projecttask.startdate = DateTime.Today;
                    projecttask.enddate = DateTime.Today;
                }

            }

            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid = Convert.ToInt16(Session["RoleId"]);
            List<SelectListItem> user = Filluser(pmuserid, roleid);
            ViewBag.assignedto = user;

            List<SelectListItem> tasktype = (from b in db.tbl_tasktype
                                             where b.isactive == 1
                                             select new SelectListItem
                                             {
                                                 Text = b.tasktypename,
                                                 Value = b.tasktypeid.ToString()
                                             }).Distinct().OrderBy(x => x.Text).ToList();
            ViewBag.tasktype = tasktype;

            List<SelectListItem> status = (from b in db.tbl_status
                                           where b.isactive == 1
                                           select new SelectListItem
                                           {
                                               Text = b.statusname,
                                               Value = b.statusid.ToString()
                                           }).Distinct().OrderBy(x => x.Text).ToList();
            ViewBag.taskstatus = status;

            return View(projecttask);
        }


        [Authorize]
        [SessionTimeout]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditTask([Bind(Include = "taskid,taskname,startdate,enddate,durations,tasktype,taskstatus,assignedto,createdby,createdon,activityid")] ProjectTask tbl_task)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                long duration = 0;
                if (DateTime.Now.Date < tbl_task.startdate.Date)
                {
                    duration = 0;
                }
                else if (DateTime.Now.Date > tbl_task.enddate.Date)
                {
                    duration = 100;
                }
                else
                {
                    double dt1 = (DateTime.Now.Date - tbl_task.startdate.Date).TotalDays + 1;
                    double dt2 = (tbl_task.enddate.Date - tbl_task.startdate.Date).TotalDays + 1;
                    if (dt2 != 0)
                        duration = Convert.ToInt64((dt1 / dt2) * 100);
                }


                long count = 0;
                if (tbl_task.taskid == null)
                {
                    var gremlinScript = "g.V().has('task','taskname','" + tbl_task.taskname + "').has('task','activityid','" + tbl_task.activityid + "').count()";
                    var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    count = objList.ToList()[0];

                    if (count > 0)
                    {
                        message = "Task name already exists.";
                        goto endloop;
                    }
                }
                else
                {
                    var gremlinScript = "g.V().has('task','taskname','" + tbl_task.taskname + "').has('task','activityid','" + tbl_task.activityid + "')";
                    var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result in objList)
                    {
                        if (result["id"] != tbl_task.taskid)
                        {
                            message = "Task name already exists.";
                            goto endloop;
                        }
                    }

                }

                if (tbl_task.taskid == null)
                {
                    tbl_task.createdby = Session["LoggedInUserId"].ToString();

                    string gremlinScript = $"g.addV('task').property('pk', '{tbl_task.taskname}')" +
                            $".property('taskname', '{tbl_task.taskname}')" +
                            $".property('startdate', '{tbl_task.startdate.ToString("yyyy-MM-dd")}')" +
                            $".property('enddate', '{tbl_task.enddate.ToString("yyyy-MM-dd")}')" +
                            $".property('activityid', '{tbl_task.activityid}')" +
                            $".property('durations', '{duration}')" +
                            $".property('assignedto', '{tbl_task.assignedto}')" +
                            $".property('tasktype', '{tbl_task.tasktype}')" +
                            $".property('taskstatus', '{tbl_task.taskstatus}')" +
                            $".property('createdby', '{Convert.ToInt32(tbl_task.createdby)}')" +
                            $".property('createdusername', '')" +
                            $".property('createdon', '{DateTime.Now}')" +
                            $".property('type', 'task')";

                    // Execute the Gremlin script                        
                    var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Added Successfully";


                    gremlinScript = "g.V().has('task','taskname','" + tbl_task.taskname + "').project('id').by(values('id'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result1 in result)
                    {
                        tbl_task.taskid = Convert.ToString(result1["id"]);
                    }


                    //Remove connection the activity to task
                    gremlinScript = $"\ng.V().has('task', 'id', '{tbl_task.taskid}').bothE().drop()";
                    // Execute the Gremlin script                    
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";
                    

                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_task.activityid}').addE('contains').to(g.V('{tbl_task.taskid}'))";
                    // Execute the Gremlin script
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";
                    

                    gremlinScript = $"\ng.V('{tbl_task.activityid}').addE('precedes').property('duration', '{tbl_task.durations}').to(g.V('{tbl_task.taskid}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";                    
                }
                else
                {
                    string gremlinScript = $"g.V('{tbl_task.taskid}').property('taskname', '{tbl_task.taskname}')" +
                                            $".property('startdate', '{tbl_task.startdate.ToString("yyyy-MM-dd")}')" +
                                            $".property('enddate', '{tbl_task.enddate.ToString("yyyy-MM-dd")}')" +
                                            $".property('activityid', '{tbl_task.activityid}')" +
                                            $".property('durations', '{duration}')" +
                                            $".property('assignedto', '{tbl_task.assignedto}')" +
                                            $".property('tasktype', '{tbl_task.tasktype}')" +
                                            $".property('taskstatus', '{tbl_task.taskstatus}')" +
                                            $".property('updatedon', '{DateTime.Now}')" +
                                            $".property('type', 'task')";

                    // Execute the Gremlin script
                    //var gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + database + "/colls/" + collection, password: authKey);
                    var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Updated Successfully";
                    

                    //connect the activity to task
                    gremlinScript = $"\ng.V('{tbl_task.activityid}').addE('contains').to(g.V('{tbl_task.taskid}'))";
                    // Execute the Gremlin script
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";
                    

                    //Connect the predeccesors to succesors
                    gremlinScript = $"\ng.V('{tbl_task.activityid}').addE('precedes').property('duration', '{tbl_task.durations}').to(g.V('{tbl_task.taskid}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";                   
                    
                }
                return RedirectToAction("List", new { activityid = tbl_task.activityid });

            }
            endloop:
            ViewBag.Message = message;
            ViewBag.Activityid = tbl_task.activityid;
            int pmuserid1 = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid1 = Convert.ToInt32(Session["RoleId"]);
            if (tbl_task.taskid == null)
            {
                return RedirectToAction("AddEditTask", new { activityid = tbl_task.activityid });
            }
            else
            {
                List<SelectListItem> user = Filluser(pmuserid1, roleid1);
                ViewBag.assignedto = user;

                List<SelectListItem> tasktype = (from b in db.tbl_tasktype
                                                 where b.isactive == 1
                                                 select new SelectListItem
                                                 {
                                                     Text = b.tasktypename,
                                                     Value = b.tasktypeid.ToString()
                                                 }).Distinct().OrderBy(x => x.Text).ToList();
                ViewBag.tasktype = tasktype;

                List<SelectListItem> status = (from b in db.tbl_status
                                               where b.isactive == 1
                                               select new SelectListItem
                                               {
                                                   Text = b.statusname,
                                                   Value = b.statusid.ToString()
                                               }).Distinct().OrderBy(x => x.Text).ToList();
                ViewBag.taskstatus = status;
            }
            return View(tbl_task);
        }

        // GET: Task/Create
        [Authorize]
        [SessionTimeout]
        public ActionResult Create(int? taskid, int? milestoneid, TaskViewModel Tasks)
        {
            if (milestoneid == 0)
            {
                ViewBag.milestoneid = Convert.ToInt32(Session["MilestoneId"]);
                Session["MilestoneId"] = ViewBag.milestoneid;
            }
            else
            {
                ViewBag.milestoneid = Tasks.milestoneid;
                Session["MilestoneId"] = milestoneid;
            }

            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid = Convert.ToInt16(Session["RoleId"]);
            List<SelectListItem> user = Filluser(pmuserid, roleid);
            ViewBag.assignedto = user;

            List<SelectListItem> tasktype = (from b in db.tbl_tasktype
                                             where b.isactive == 1
                                             select new SelectListItem
                                             {
                                                 Text = b.tasktypename,
                                                 Value = b.tasktypeid.ToString()
                                             }).Distinct().OrderBy(x => x.Text).ToList();
            ViewBag.tasktypeid = tasktype;

            List<SelectListItem> status = (from b in db.tbl_status
                                           where b.isactive == 1
                                           select new SelectListItem
                                           {
                                               Text = b.statusname,
                                               Value = b.statusid.ToString()
                                           }).Distinct().OrderBy(x => x.Text).ToList();
            ViewBag.statusid = status;

            if (taskid == null)
            {
                Tasks.startdate = DateTime.Today;
                Tasks.enddate = DateTime.Today;
            }
            return View(Tasks);
        }

        public List<SelectListItem> Filluser(int? pmuserid, int? roleid)
        {
            List<SelectListItem> user = new List<SelectListItem>();
            if (roleid == (int)Role.ProjectManager)
            {
                user = (from b in db.tbl_user
                        where b.pmuserid == pmuserid && b.isactive == 1 && b.roleid == 3
                        select new SelectListItem
                        {
                            Text = b.firstname + " " + b.lastname,
                            Value = b.userid.ToString()
                        }).Distinct().OrderBy(x => x.Text).ToList();
            }
            else if (roleid == (int)Role.Admin)
            {
                user = (from b in db.tbl_user
                        where b.roleid == 3 && b.isactive == 1
                        select new SelectListItem
                        {
                            Text = b.firstname + " " + b.lastname,
                            Value = b.userid.ToString()
                        }).Distinct().OrderBy(x => x.Text).ToList();
            }
            //user.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });
            return user;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
