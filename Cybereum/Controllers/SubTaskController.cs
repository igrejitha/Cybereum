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
            TempData["taskid"] = taskid;
            ViewBag.taskid = TempData["TaskId"];
            TempData.Keep();
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
            GetSubTask(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]), taskid);
            return View();
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult List(string taskid)
        {
            TempData["taskid"] = taskid;
            ViewBag.taskid = TempData["TaskId"];
            TempData.Keep();
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
            return View();
        }

        // GET: SubTask/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_subtask tbl_subtask = db.tbl_subtask.Find(id);
            if (tbl_subtask == null)
            {
                return HttpNotFound();
            }
            return View(tbl_subtask);
        }

        public ActionResult Addrecord(int Id)
        {
            ViewBag.Message = "Edit SubTask";
            tbl_subtask subtask = new tbl_subtask();

            subtask = db.tbl_subtask.Find(Id);
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
                var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + HttpUtility.UrlEncode(gremlinvariables.database) + "/colls/" + HttpUtility.UrlEncode(gremlinvariables.collection), password: gremlinvariables.authKey);
                using (var gremlinClient = new GremlinClient(
                    gremlinServer,
                    new GraphSON2Reader(),
                    new GraphSON2Writer(),
                    GremlinClient.GraphSON2MimeType,
                    gremlinvariables.connectionPoolSettings))
                {
                    var gremlinScript = "g.V().has('subtask','id','" + id + "').project('id','subtaskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdon','taskid')" +
                        ".by(id()).by(values('subtaskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('tasktype')).by(values('taskstatus')).by(values('assignedto')).by(values('createdby')).by(values('createdon')).by(values('taskid'))";
                    try
                    {
                        //var results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var results = task.Result;

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
                var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + HttpUtility.UrlEncode(gremlinvariables.database) + "/colls/" + HttpUtility.UrlEncode(gremlinvariables.collection), password: gremlinvariables.authKey);
                using (var gremlinClient = new GremlinClient(
                    gremlinServer,
                    new GraphSON2Reader(),
                    new GraphSON2Writer(),
                    GremlinClient.GraphSON2MimeType,
                    gremlinvariables.connectionPoolSettings))
                {
                    var gremlinScript = "g.V().has('subtask','taskid','" + taskid + "').project('id','subtaskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdon','taskid')" +
                                        ".by(id()).by(values('subtaskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('tasktype')).by(values('taskstatus')).by(values('assignedto')).by(values('createdby')).by(values('createdon')).by(values('taskid'))";
                    try
                    {
                        var results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);

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
                            var results1 = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                            foreach (var result1 in results1)
                            {
                                subtask.taskname = result1["taskname"];
                            }

                            subtask.assignedto = result["assignedto"].ToString();

                            int userid = Convert.ToInt32(result["assignedto"]);
                            var username = db.tbl_user.Where(x => x.userid == userid).FirstOrDefault();

                            if (username != null)
                            {
                                subtask.assignedusername = username.firstname + ' ' + username.lastname;
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
                Session["TaskId"] = ViewBag.taskid;
            }
            else
            {
                ViewBag.taskid = taskid;
                Session["TaskId"] = taskid;
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
            else if(roleid==(int)Role.Admin)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "subtaskid,subtaskname,isactive,startdate,enddate,statusid,createdby,createddate,modifiedby,modifieddate,taskid,priority,tasktypeid,assignedto")] SubTaskViewModel tbl_subtask)
        //public ActionResult AddUsers(FormCollection formval, UserLevelOne _mod)
        {
            //tbl_subtask.taskid = Convert.ToInt16(TempData["TaskId"]);
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                var objList = db.tbl_subtask.Find(tbl_subtask.subtaskid);
                if (tbl_subtask.subtaskid == 0)
                {
                    //objList = db.sp_FetchTaskExists(tbl_milestone.milestonename).FirstOrDefault();
                    objList = db.tbl_subtask.Where(x => x.subtaskname == tbl_subtask.subtaskname).FirstOrDefault();
                    //objList = db.tbl_milestone.Where(a=> a.milestonename == tbl_milestone.milestonename).FirstOrDefault();
                }
                else
                {
                    objList = db.tbl_subtask.Where(x => x.subtaskname == tbl_subtask.subtaskname && x.subtaskid != tbl_subtask.subtaskid).FirstOrDefault();
                }


                if (objList != null)
                {
                    message = "Subtask name already exists.";
                }
                else
                {
                    tbl_subtask subtask = new tbl_subtask();
                    subtask.subtaskname = tbl_subtask.subtaskname;
                    subtask.taskid = tbl_subtask.taskid;
                    subtask.startdate = tbl_subtask.startdate;
                    subtask.enddate = tbl_subtask.enddate;
                    subtask.priority = tbl_subtask.priority;
                    subtask.assignedto = tbl_subtask.assignedto;
                    subtask.tasktypeid = tbl_subtask.tasktypeid;
                    subtask.statusid = tbl_subtask.statusid;
                    subtask.isactive = 1;
                    if (tbl_subtask.subtaskid == 0)
                    {
                        subtask.createddate = DateTime.Now;
                        subtask.createdby = Convert.ToInt16(Session["LoggedInUserId"]);
                        db.tbl_subtask.Add(subtask);
                        db.SaveChanges();
                        message = "Saved Successfully";
                    }
                    else
                    {
                        tbl_subtask subtaskedit = db.tbl_subtask.Find(tbl_subtask.subtaskid);

                        subtaskedit.subtaskname = tbl_subtask.subtaskname;
                        subtaskedit.taskid = tbl_subtask.taskid;
                        subtaskedit.startdate = tbl_subtask.startdate;
                        subtaskedit.enddate = tbl_subtask.enddate;
                        subtaskedit.priority = tbl_subtask.priority;
                        subtaskedit.assignedto = tbl_subtask.assignedto;
                        subtaskedit.tasktypeid = tbl_subtask.tasktypeid;
                        subtaskedit.statusid = tbl_subtask.statusid;
                        subtaskedit.isactive = 1;
                        subtaskedit.modifieddate = DateTime.Now;
                        subtaskedit.modifiedby = Convert.ToInt16(Session["LoggedInUserId"]);
                        subtaskedit.isactive = 1;
                        db.Entry(subtaskedit).State = EntityState.Modified;
                        db.SaveChanges();
                        message = "Modified Successfully";
                    }

                    return RedirectToAction("Index", new { taskid = tbl_subtask.taskid });
                }
            }
            ViewBag.Message = message;
            //return RedirectToAction("Create", "SubTask", new { taskid = tbl_subtask.taskid });

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
            //status.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });
            ViewBag.statusid = status;
            //return View(tbl_subtask);
            return RedirectToAction("Create", "SubTask", new
            {
                taskid = tbl_subtask.taskid
            });
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult AddEditSubTask(int? subtaskid, string taskid, ProjectSubTask projectsubtask)
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

            if (projectsubtask.subtaskid == null)
            {
                var gremlinScript = "g.V().has('subtask','taskid','" + taskid + "').order().by('createdon',decr).limit(1).project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
                var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + gremlinvariables.database + "/colls/" + gremlinvariables.collection, password: gremlinvariables.authKey);
                using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                {
                    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    task.Wait();
                    var result = task.Result;
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
                long duration = 0;
                if (DateTime.Now.Date < tbl_subtask.startdate.Date)
                {
                    duration = 0;
                }
                else if (DateTime.Now.Date > tbl_subtask.enddate.Date)
                {
                    duration = 100;
                }
                else
                {
                    double dt1 = (DateTime.Now.Date - tbl_subtask.startdate.Date).TotalDays + 1;
                    double dt2 = (tbl_subtask.enddate.Date - tbl_subtask.startdate.Date).TotalDays + 1;
                    if (dt2 != 0)
                        duration = Convert.ToInt64((dt1 / dt2) * 100);
                }

                var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + gremlinvariables.database + "/colls/" + gremlinvariables.collection, password: gremlinvariables.authKey);
                long count = 0;
                if (tbl_subtask.subtaskid == null)
                {
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var gremlinScript = "g.V().has('subtask','subtaskname','" + tbl_subtask.subtaskname + "').has('subtask','taskid','" + tbl_subtask.taskid + "').count()";
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var objList = task.Result;
                        count = objList.ToList()[0];
                    }
                    if (count > 0)
                    {
                        message = "Sub-Task name already exists.";
                        goto endloop;
                    }
                }
                else
                {
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var gremlinScript = "g.V().has('subtask','subtaskname','" + tbl_subtask.subtaskname + "').has('subtask','taskid','" + tbl_subtask.taskid + "')";
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var objList = task.Result;
                        foreach (var result in objList)
                        {
                            if (result["id"] != tbl_subtask.subtaskid)
                            {
                                message = "Sub-Task name already exists.";
                                goto endloop;
                            }
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
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Added Successfully";
                    }

                    gremlinScript = "g.V().has('subtask','subtaskname','" + tbl_subtask.subtaskname + "').project('id').by(values('id'))";
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        foreach (var result1 in result)
                        {
                            tbl_subtask.subtaskid = Convert.ToString(result1["id"]);
                        }
                    }

                    //connect the task to subtask
                    gremlinScript = $"\ng.V('{tbl_subtask.taskid}').addE('contains').to(g.V('{tbl_subtask.subtaskid}'))";
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
                    gremlinScript = $"\ng.V('{tbl_subtask.taskid}').addE('precedes').property('duration', '{tbl_subtask.durations}').to(g.V('{tbl_subtask.subtaskid}'))";
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Gremlin script executed successfully";
                    }
                    //}
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
                    //var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: gremlinvariables.containerLink, password: gremlinvariables.authKey);
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Updated Successfully";
                    }


                    //Remove connection the task to subtask
                    gremlinScript = $"\ng.V().has('subtask', 'id', '{tbl_subtask.subtaskid}').bothE().drop()"; 
                    // Execute the Gremlin script
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Gremlin script executed successfully";
                    }

                    //connect the task to subtask
                    gremlinScript = $"\ng.V('{tbl_subtask.taskid}').addE('contains').to(g.V('{tbl_subtask.subtaskid}'))";
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
                    gremlinScript = $"\ng.V('{tbl_subtask.taskid}').addE('precedes').property('duration', '{tbl_subtask.durations}').to(g.V('{tbl_subtask.subtaskid}'))";
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Gremlin script executed successfully";
                    }
                    //}
                }
                return RedirectToAction("List", new { taskid = tbl_subtask.taskid });

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
                //status.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });
                ViewBag.taskstatus = status;
            }
            return View(tbl_subtask);
        }

        // GET: SubTask/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_subtask tbl_subtask = db.tbl_subtask.Find(id);
            if (tbl_subtask == null)
            {
                return HttpNotFound();
            }
            ViewBag.statusid = new SelectList(db.tbl_status, "statusid", "statusname", tbl_subtask.statusid);
            ViewBag.assignedto = new SelectList(db.tbl_user, "userid", "username", tbl_subtask.assignedto);
            ViewBag.taskid = new SelectList(db.tbl_task, "taskid", "taskname", tbl_subtask.taskid);
            ViewBag.tasktypeid = new SelectList(db.tbl_tasktype, "tasktypeid", "tasktypename", tbl_subtask.tasktypeid);
            return View(tbl_subtask);
        }

        // POST: SubTask/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "subtaskid,subtaskname,isactive,startdate,enddate,statusid,createdby,createddate,modifiedby,modifieddate,taskid,priority,tasktypeid,assignedto")] tbl_subtask tbl_subtask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_subtask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.statusid = new SelectList(db.tbl_status, "statusid", "statusname", tbl_subtask.statusid);
            ViewBag.assignedto = new SelectList(db.tbl_user, "userid", "username", tbl_subtask.assignedto);
            ViewBag.taskid = new SelectList(db.tbl_task, "taskid", "taskname", tbl_subtask.taskid);
            ViewBag.tasktypeid = new SelectList(db.tbl_tasktype, "tasktypeid", "tasktypename", tbl_subtask.tasktypeid);
            return View(tbl_subtask);
        }

        // GET: SubTask/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_subtask tbl_subtask = db.tbl_subtask.Find(id);
            if (tbl_subtask == null)
            {
                return HttpNotFound();
            }
            return View(tbl_subtask);
        }

        // POST: SubTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_subtask tbl_subtask = db.tbl_subtask.Find(id);
            db.tbl_subtask.Remove(tbl_subtask);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [SessionTimeout]
        [Authorize]
        [HttpPost]
        public ActionResult GetSubTask(int userid = 0, int roleid = 0, int? taskid = 0)
        {
            JsonResult result = new JsonResult();
            try
            {
                cybereumEntities entities = new cybereumEntities();

                //var RecCount = entities.tblfiles.Count();
                // Initialization.  
                var search = Request.Form.GetValues("search[value]")[0];
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                //var length = Request.Form.GetValues("length").FirstOrDefault() == "-1" ? RecCount.ToString() : Request.Form.GetValues("length").FirstOrDefault();
                var length = "";
                if (Request.Form.GetValues("length").FirstOrDefault() == "-1")
                {
                    var RecCount = entities.tbl_user.Count();
                    length = RecCount.ToString();
                }
                else
                {
                    length = Request.Form.GetValues("length").FirstOrDefault();
                }
                //Find Order Column  
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                if (skip == 0)
                {
                    System.Web.HttpContext.Current.Session["TotalRecordCount"] = 0;
                }

                //if (projectid == 0)
                //{
                //    ViewBag.projectid = Convert.ToInt32(Session["ProjectId"]);
                //    Session["ProjectId"] = ViewBag.projectid;
                //    projectid = Convert.ToInt32(Session["ProjectId"]);
                //}
                //else
                //{
                //    ViewBag.projectid = projectid;
                //    Session["ProjectId"] = projectid;
                //}

                var files = entities.sp_FetchSubTasks(userid, roleid, taskid, skip, pageSize, sortColumn, sortColumnDir).ToList();

                // Total record count.  
                //recordsTotal = files.Count > 0 ? files[0].TotalRecordCount : 0;
                if (Convert.ToInt32(System.Web.HttpContext.Current.Session["TotalRecordCount"]) == 0)
                {
                    recordsTotal = files.Count > 0 ? files[0].TotalRecordCount : 0;
                    System.Web.HttpContext.Current.Session["TotalRecordCount"] = recordsTotal;
                }
                else
                {
                    recordsTotal = Convert.ToInt32(System.Web.HttpContext.Current.Session["TotalRecordCount"]);
                }
                // Loading drop down lists.                  
                result = this.Json(new { data = files, recordsTotal = recordsTotal, recordsFiltered = recordsTotal }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
            }
            // Return info.     
            return result;
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
