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
    public class SubTaskController : Controller
    {
        private cybereumEntities db = new cybereumEntities();

        // GET: SubTask
        [Authorize]
        [SessionTimeout]
        public ActionResult Index(int? taskid)
        {
            //TempData["taskid"] = taskid;
            //ViewBag.taskid = TempData["TaskId"];
            //TempData.Keep();
            if (taskid == 0)
            {
                ViewBag.taskid = Convert.ToInt32(Session["TaskId"]);
                Session["TaskId"] = ViewBag.taskid;
            }
            else
            {
                ViewBag.taskid = taskid;
                Session["TaskId"] = taskid;
            }
            //GetSubTask(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]), taskid);
            return View();
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult List(string taskid,string activityid, string projectid)
        {
            ////TempData["taskid"] = taskid;
            ////ViewBag.taskid = TempData["TaskId"];
            ////TempData.Keep();
            if (taskid == null)
            {
                ViewBag.taskid = Convert.ToInt32(Session["TaskId"]);
                Session["TaskId"] = ViewBag.taskid;
            }
            else
            {
                ViewBag.taskid = taskid;
                Session["TaskId"] = taskid;                
            }
            if (activityid == null)
            {
                ViewBag.activityid = Session["ActivityId"];
                Session["ActivityId"] = ViewBag.activityid;
            }
            else
            {
                ViewBag.activityid = activityid;
                Session["ActivityId"] = activityid;
            }
            if (projectid == null)
            {
                ViewBag.projectid = Session["ProjectId"];
                Session["ProjectId"] = ViewBag.projectid;
            }
            else
            {
                ViewBag.projectid = projectid;
                Session["ProjectId"] = projectid;
            }
            return View();
        }

        public ActionResult Addrecord(int Id)
        {
            ViewBag.Message = "Edit SubTask";
            ProjectSubTask subtask = new ProjectSubTask();

            //subtask = db.ProjectSubTask.Find(Id);
            return RedirectToAction("Create", subtask);
        }

        public ActionResult AddEditrecord(string Id)
        {
            ViewBag.Message = "Edit SubTask";

            var task = getsubtaskbyid(Id);
            return RedirectToAction("AddEditSubTask", task.Result);
        }

        public async Task<ProjectSubTask> getsubtaskbyid(string id)
        {
            ProjectSubTask projectsubtask = new ProjectSubTask();
            try
            {

                var gremlinScript = "g.V().has('subtask','id','" + id + "').project('id','subtaskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdon','taskid')" +
                    ".by(id()).by(values('subtaskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('tasktype')).by(values('taskstatus')).by(values('assignedto')).by(values('createdby')).by(values('createdon')).by(values('taskid'))";
                try
                {
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    foreach (var result in results)
                    {
                        projectsubtask.subtaskid = result["id"].ToString();
                        projectsubtask.taskid = result["taskid"].ToString();
                        projectsubtask.subtaskname = result["subtaskname"].ToString();
                        projectsubtask.startdate = Convert.ToDateTime(result["startdate"]);
                        projectsubtask.enddate = Convert.ToDateTime(result["enddate"]);
                        projectsubtask.tasktype = Convert.ToInt16(result["tasktype"]);
                        projectsubtask.taskstatus = Convert.ToInt16(result["taskstatus"]);
                        projectsubtask.assignedto = result["assignedto"];
                        projectsubtask.durations = Convert.ToInt64(result["durations"]);
                        projectsubtask.createdby = result["createdby"].ToString();
                        //projectsubtask.createdusername = result["createdusername"].ToString();
                        projectsubtask.createdon = Convert.ToDateTime(result["createdon"]);
                    }
                    return projectsubtask;
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

        public async Task<JsonResult> subtasklist(string taskid)
        {
            List<ProjectSubTask> list = new List<ProjectSubTask>();
            try
            {

                var gremlinScript = "g.V().has('subtask','taskid','" + taskid + "').project('id','subtaskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdon','taskid')" +
                                    ".by(id()).by(values('subtaskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('tasktype')).by(values('taskstatus')).by(values('assignedto')).by(values('createdby')).by(values('createdon')).by(values('taskid'))";
                try
                {
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result in results)
                    {
                        ProjectSubTask subtask = new ProjectSubTask();
                        subtask.subtaskid = result["id"].ToString();
                        subtask.subtaskname = result["subtaskname"].ToString();
                        subtask.startdate = Convert.ToDateTime(result["startdate"]);
                        subtask.enddate = Convert.ToDateTime(result["enddate"]);
                        //subtask.tasktype = result["tasktype"];
                        //subtask.taskstatus = result["taskstatus"];
                        subtask.durations = Convert.ToInt64(result["durations"]);
                        subtask.taskid = result["taskid"].ToString();

                        gremlinScript = "g.V().has('task','id','" + taskid + "').project('taskname').by(values('taskname'))";
                        var results1 = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        foreach (var result1 in results1)
                        {
                            subtask.taskname = result1["taskname"];
                        }

                        subtask.assignedto = result["assignedto"].ToString();

                        if (subtask.assignedto != "")
                        {
                            int userid = Convert.ToInt32(result["assignedto"]);
                            var username = db.tbl_user.Where(x => x.userid == userid).FirstOrDefault();

                            if (username != null)
                            {
                                subtask.assignedusername = username.firstname + ' ' + username.lastname;
                            }
                        }

                        subtask.createdon = Convert.ToDateTime(result["createdon"]);

                        list.Add(subtask);
                    }
                    var subtaskresult = this.Json(new { data = list, recordsTotal = list.Count(), recordsFiltered = list.Count() }, JsonRequestBehavior.AllowGet);
                    return subtaskresult;
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

        // GET: Task/Create
        [Authorize]
        [SessionTimeout]
        public ActionResult Create(int? subtaskid, int? taskid, SubTaskViewModel SubTasks)
        {            
            if (taskid == 0)
            {
                ViewBag.taskid = Convert.ToInt32(Session["TaskId"]);
                //Session["TaskId"] = ViewBag.taskid;
            }
            else
            {
                ViewBag.taskid = taskid;
                //Session["TaskId"] = taskid;
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

            if (subtaskid == null)
            {
                SubTasks.startdate = DateTime.Today;
                SubTasks.enddate = DateTime.Today;
            }

            return View(SubTasks);
        }

        public List<SelectListItem> Filluser(int? pmuserid, int? roleid)
        {
            List<SelectListItem> user = new List<SelectListItem>();
            if (roleid == (int)Role.User)
            {
                user = (from b in db.tbl_user
                        where b.userid == pmuserid && b.isactive == 1
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
            else
            {
                user = (from b in db.tbl_user
                        where b.pmuserid == pmuserid && b.isactive == 1
                        select new SelectListItem
                        {
                            Text = b.firstname + " " + b.lastname,
                            Value = b.userid.ToString()
                        }).Distinct().OrderBy(x => x.Text).ToList();
            }

            return user;
        }


        [Authorize]
        [SessionTimeout]
        public ActionResult AddEditSubTask(int? subtaskid, string taskid,string activityid,string projectid, ProjectSubTask projectsubtask)
        {
            if (taskid == null)
            {
                ViewBag.taskid = Convert.ToInt32(Session["TaskId"]);
                Session["TaskId"] = ViewBag.taskid;
            }
            else
            {
                ViewBag.taskid = taskid;
                Session["TaskId"] = taskid;
            }

            if (projectid == null)
            {
                ViewBag.projectid = Session["ProjectId"];
                Session["ProjectId"] = ViewBag.projectid;
            }
            else
            {
                ViewBag.projectid = projectid;
                Session["ProjectId"] = projectid;
            }
            if (activityid == null)
            {
                ViewBag.activityid = Session["ActivityId"];
                Session["ActivityId"] = ViewBag.activityid;
            }
            else
            {
                ViewBag.activityid = activityid;
                Session["ActivityId"] = activityid;
            }

            if (projectsubtask.subtaskid == null)
            {
                var gremlinScript = "g.V().has('subtask','taskid','" + taskid + "').order().by('createdon',decr).limit(1).project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
                var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        projectsubtask.startdate = Convert.ToDateTime(item["enddate"]);
                        projectsubtask.enddate = Convert.ToDateTime(item["enddate"]);
                    }
                }
                else
                {
                    projectsubtask.startdate = DateTime.Today;
                    projectsubtask.enddate = DateTime.Today;
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

            List<SelectListItem> taskstatus = (from b in db.tbl_status
                                               where b.isactive == 1
                                               select new SelectListItem
                                               {
                                                   Text = b.statusname,
                                                   Value = b.statusid.ToString()
                                               }).Distinct().OrderBy(x => x.Text).ToList();
            ViewBag.taskstatus = taskstatus;

            return View(projectsubtask);
        }


        [Authorize]
        [SessionTimeout]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditSubTask([Bind(Include = "subtaskid,subtaskname,startdate,enddate,durations,tasktype,taskstatus,assignedto,createdby,createdon,taskid")] ProjectSubTask tbl_subtask)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                //long duration = 0;
                //if (DateTime.Now.Date < tbl_subtask.startdate.Date)
                //{
                //    duration = 0;
                //}
                //else if (DateTime.Now.Date > tbl_subtask.enddate.Date)
                //{
                //    duration = 100;
                //}
                //else
                //{
                //    double dt1 = (DateTime.Now.Date - tbl_subtask.startdate.Date).TotalDays + 1;
                //    double dt2 = (tbl_subtask.enddate.Date - tbl_subtask.startdate.Date).TotalDays + 1;
                //    if (dt2 != 0)
                //        duration = Convert.ToInt64((dt1 / dt2) * 100);
                //}
                int duration = Convert.ToInt16(tbl_subtask.durations);
                tbl_subtask.enddate = IGUtilities.CalculateDays(tbl_subtask.startdate, duration);

                long count = 0;
                if (tbl_subtask.subtaskid == null)
                {
                    var gremlinScript = "g.V().has('subtask','subtaskname','" + tbl_subtask.subtaskname + "').has('subtask','taskid','" + tbl_subtask.taskid + "').count()";
                    var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    count = objList.ToList()[0];

                    if (count > 0)
                    {
                        message = "Sub-Task name already exists.";
                        goto endloop;
                    }
                }
                else
                {
                    var gremlinScript = "g.V().has('subtask','subtaskname','" + tbl_subtask.subtaskname + "').has('subtask','taskid','" + tbl_subtask.taskid + "')";
                    var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result in objList)
                    {
                        if (result["id"] != tbl_subtask.subtaskid)
                        {
                            message = "Sub-Task name already exists.";
                            goto endloop;
                        }
                    }

                }

                if (tbl_subtask.subtaskid == null)
                {
                    tbl_subtask.createdby = Session["LoggedInUserId"].ToString();

                    string gremlinScript = $"g.addV('subtask').property('pk', '{tbl_subtask.subtaskname}')" +
                            $".property('subtaskname', '{tbl_subtask.subtaskname}')" +
                            $".property('startdate', '{tbl_subtask.startdate.ToString("yyyy-MM-dd")}')" +
                            $".property('enddate', '{tbl_subtask.enddate.ToString("yyyy-MM-dd")}')" +
                            $".property('taskid', '{tbl_subtask.taskid}')" +
                            $".property('durations', '{duration}')" +
                            $".property('assignedto', '{tbl_subtask.assignedto}')" +
                            $".property('tasktype', '{tbl_subtask.tasktype}')" +
                            $".property('taskstatus', '{tbl_subtask.taskstatus}')" +
                            $".property('createdby', '{Convert.ToInt32(tbl_subtask.createdby)}')" +
                            $".property('createdon', '{DateTime.Now}')" +
                            $".property('type', 'subtask')";

                    // Execute the Gremlin script
                    //var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: gremlinvariables.containerLink, password: gremlinvariables.authKey);
                    var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Added Successfully";
                    

                    gremlinScript = "g.V().has('subtask','subtaskname','" + tbl_subtask.subtaskname + "').project('id').by(values('id'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result1 in result)
                        {
                            tbl_subtask.subtaskid = Convert.ToString(result1["id"]);
                        }
                    

                    //connect the task to subtask
                    gremlinScript = $"\ng.V('{tbl_subtask.taskid}').addE('contains').to(g.V('{tbl_subtask.subtaskid}'))";
                    // Execute the Gremlin script
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";
                    

                    //Connect the predeccesors to succesors
                    gremlinScript = $"\ng.V('{tbl_subtask.taskid}').addE('precedes').property('duration', '{tbl_subtask.durations}').to(g.V('{tbl_subtask.subtaskid}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";
                    
                }
                else
                {
                    string gremlinScript = $"g.V('{tbl_subtask.subtaskid}').property('subtaskname', '{tbl_subtask.subtaskname}')" +
                                            $".property('subtaskname', '{tbl_subtask.subtaskname}')" +
                                            $".property('startdate', '{tbl_subtask.startdate.ToString("yyyy-MM-dd")}')" +
                                            $".property('enddate', '{tbl_subtask.enddate.ToString("yyyy-MM-dd")}')" +
                                            $".property('taskid', '{tbl_subtask.taskid}')" +
                                            $".property('durations', '{duration}')" +
                                            $".property('assignedto', '{tbl_subtask.assignedto}')" +
                                            $".property('tasktype', '{tbl_subtask.tasktype}')" +
                                            $".property('taskstatus', '{tbl_subtask.taskstatus}')" +
                                            $".property('updatedon', '{DateTime.Now}')" +
                                            $".property('type', 'subtask')";

                    // Execute the Gremlin script
                    var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Updated Successfully";
                    
                    //Remove connection the task to subtask
                    gremlinScript = $"\ng.V().has('subtask', 'id', '{tbl_subtask.subtaskid}').bothE().drop()";
                    // Execute the Gremlin script
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";
                    

                    //connect the task to subtask
                    gremlinScript = $"\ng.V('{tbl_subtask.taskid}').addE('contains').to(g.V('{tbl_subtask.subtaskid}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";
                    

                    //Connect the predeccesors to succesors
                    gremlinScript = $"\ng.V('{tbl_subtask.taskid}').addE('precedes').property('duration', '{tbl_subtask.durations}').to(g.V('{tbl_subtask.subtaskid}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";
                    
                }                
                return RedirectToAction("List", new { taskid = tbl_subtask.taskid, activityid = Session["Activityid"], projectid = Session["Projectid"] });

            }
            endloop:
            ViewBag.Message = message;
            ViewBag.taskid = tbl_subtask.taskid;

            if (tbl_subtask.subtaskid == null)
            {
                return RedirectToAction("AddEditSubTask", new { taskid = tbl_subtask.taskid });
            }
            else
            {
                int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
                int roleid = Convert.ToInt32(Session["RoleId"]);
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
            }
            return View(tbl_subtask);
        }

        public JsonResult GetEnddate(DateTime startDate, int id)
        {
            var record = IGUtilities.CalculateDays(startDate, id);
            return Json(record, JsonRequestBehavior.AllowGet);
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
