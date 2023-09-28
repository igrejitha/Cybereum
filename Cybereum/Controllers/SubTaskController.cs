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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cybereum.Controllers
{
    [Authorize]
    public class SubTaskController : Controller
    {
        private cybereumEntities db = new cybereumEntities();

        // GET: SubTask
        [Authorize]
        [SessionTimeout]
        public ActionResult Index(string taskid)
        {
            if (taskid == null)
            {
                ViewBag.taskid = Session["TaskId"];
                Session["TaskId"] = ViewBag.taskid;
            }
            else
            {
                ViewBag.taskid = taskid;
                Session["TaskId"] = taskid;
            }
            return View();
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult List(string taskid, string activityid, string projectid)
        {
            if (taskid == null)
            {
                ViewBag.taskid = Session["TaskId"];
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

        public ActionResult Addrecord(string Id)
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

                var gremlinScript = "g.V().has('subtask','id','" + id + "').project('id','subtaskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdon','taskid','progress')" +
                    ".by(id()).by(values('subtaskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('tasktype')).by(values('taskstatus')).by(values('assignedto')).by(values('createdby')).by(values('createdon')).by(values('taskid')).by(values('progress'))";
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
                        projectsubtask.progress = Convert.ToInt16(result["progress"]);
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
                int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
                var gremlinScript = "g.V().has('subtask','assignedto','" + pmuserid + "').project('id','subtaskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdon','taskid')" +
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

                        gremlinScript = "g.V().has('task','id','" + result["taskid"].ToString() + "').project('taskname','activityid').by(values('taskname')).by(values('activityid'))";
                        var results1 = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        foreach (var result1 in results1)
                        {
                            subtask.taskname = result1["taskname"];



                            gremlinScript = "g.V().has('activity','id','" + result1["activityid"] + "').project('activityname','projectid').by(values('activityname')).by(values('projectid'))";
                            var activityres = IGUtilities.ExecuteGremlinScript(gremlinScript);
                            foreach (var actres in activityres)
                            {
                                subtask.activityname = actres["activityname"];

                                gremlinScript = "g.V().has('project','id','" + actres["projectid"] + "').project('projectname').by(values('projectname'))";
                                var projresult = IGUtilities.ExecuteGremlinScript(gremlinScript);
                                foreach (var res in projresult)
                                {
                                    subtask.projectname = res["projectname"];
                                }
                            }

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
<<<<<<< Updated upstream
                //user = (from b in db.tbl_user
                //        where b.pmuserid == pmuserid && b.isactive == 1
                //        select new SelectListItem
                //        {
                //            Text = b.firstname + " " + b.lastname,
                //            Value = b.userid.ToString()
                //        }).Distinct().OrderBy(x => x.Text).ToList();

=======
                
>>>>>>> Stashed changes
                string projectid = Session["ProjectId"].ToString();
                var gremlinScript = "g.V().has('project','id','" + projectid + "').project('projectid','projectname','projectmembers').by(id()).by(values('projectname')).by(values('projectmembers').fold())";
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                int[] users = { };
                foreach (var result in results)
                {
                    var projectmembers = result["projectmembers"];
                    var stringlist = JsonConvert.SerializeObject(projectmembers);
                    var jArray = JArray.Parse(stringlist);
                    string Users = string.Empty;
                    foreach (string item in jArray)
                    {
                        Users = Users + item + ",";
                    }
                    Users = Users.Remove(Users.LastIndexOf(",")).ToString();
                    if (Users.ToString() != string.Empty)
                    {
                        users = Users.Split(',').Select(int.Parse).ToArray();
                    }
                }
<<<<<<< Updated upstream
                var query = users.Select((r, index) => new {
=======
                var query = users.Select((r, index) => new
                {
>>>>>>> Stashed changes
                    Text = index,
                    Value = r
                });
                var l = new List<SelectListItem>();
                foreach (var i in query)
                {
                    int userid = Convert.ToInt32(i.Value);
                    var username = (from b in db.tbl_user
                                    join c in db.tbl_userrole on b.roleid equals c.roleid
                                    where b.userid == userid
                                    select new { b.userid, b.firstname, b.lastname, c.rolename }).Take(1);

                    if (username != null)
                    {
                        var sli = new SelectListItem();
                        sli.Value = i.Value.ToString();
                        foreach (var item in username)
                        {
                            sli.Text = item.firstname + ' ' + item.lastname + " - " + item.rolename;
                        }
                        l.Add(sli);
                    }

                }
                user = new List<SelectListItem>(l);
            }

            return user;
        }


        [Authorize]
        [SessionTimeout]
        public ActionResult AddEditSubTask(string subtaskid, string taskid, string activityid, string projectid, ProjectSubTask projectsubtask)
        {
            if (taskid == null)
            {
                ViewBag.taskid = Session["TaskId"];
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
                //var gremlinScript = "g.V().has('subtask','taskid','" + taskid + "').order().by('createdon',decr).limit(1).project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
                var gremlinScript = "g.V().has('task','id','" + taskid + "').project('startdate','enddate','durations').by(values('startdate')).by(values('enddate')).by(values('durations'))";
                var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        projectsubtask.startdate = Convert.ToDateTime(item["startdate"]);
                        projectsubtask.enddate = Convert.ToDateTime(item["enddate"]);
                        projectsubtask.durations = Convert.ToInt16(item["durations"]);
                    }
                }
                else
                {
                    projectsubtask.startdate = DateTime.Today;
                    projectsubtask.enddate = DateTime.Today;
                    projectsubtask.durations = 1;
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
        public ActionResult AddEditSubTask([Bind(Include = "subtaskid,subtaskname,startdate,enddate,durations,tasktype,taskstatus,assignedto,createdby,createdon,taskid,progress")] ProjectSubTask tbl_subtask)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                int duration = Convert.ToInt16(tbl_subtask.durations);
                tbl_subtask.enddate = IGUtilities.CalculateDays(tbl_subtask.startdate, duration);

                long count = 0;
                //**********Checking for task start and end date*************
<<<<<<< Updated upstream
                var enddate = CheckTaskdates(tbl_subtask.taskid,tbl_subtask.startdate, tbl_subtask.enddate);
=======
                var enddate = CheckTaskdates(tbl_subtask.taskid, tbl_subtask.startdate, tbl_subtask.enddate);
>>>>>>> Stashed changes
                string pList = JsonConvert.SerializeObject(enddate.Data);
                ProjectSubTask newtask = new ProjectSubTask();
                newtask = JsonConvert.DeserializeObject<ProjectSubTask>(pList);
                if (newtask.startdate != Convert.ToDateTime("01/01/0001"))
                {
                    tbl_subtask.startdate = newtask.startdate;
                    tbl_subtask.enddate = newtask.enddate;
                    tbl_subtask.durations = newtask.durations;
                }
                //**********End*********

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
                    if (tbl_subtask.createdby == null)
                        tbl_subtask.createdby = Session["LoggedInUserId"].ToString();

                    string gremlinScript = $"g.addV('subtask').property('pk', '{tbl_subtask.subtaskname}')" +
                            $".property('subtaskname', '{tbl_subtask.subtaskname}')" +
                            $".property('startdate', '{tbl_subtask.startdate.ToString("yyyy-MM-dd")}')" +
                            $".property('enddate', '{tbl_subtask.enddate.ToString("yyyy-MM-dd")}')" +
                            $".property('taskid', '{tbl_subtask.taskid}')" +
                            $".property('durations', '{duration}')" +
                            $".property('progress', '{tbl_subtask.progress}')" +
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
                    gremlinScript = $"\ng.V('{tbl_subtask.subtaskid}').addE('contains').to(g.V('{tbl_subtask.taskid}'))";
                    // Execute the Gremlin script
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
                                            $".property('progress', '{tbl_subtask.progress}')" +
                                            $".property('assignedto', '{tbl_subtask.assignedto}')" +
                                            $".property('tasktype', '{tbl_subtask.tasktype}')" +
                                            $".property('taskstatus', '{tbl_subtask.taskstatus}')" +
                                            $".property('updatedon', '{DateTime.Now}')" +
                                            $".property('type', 'subtask')";

                    // Execute the Gremlin script
                    var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
<<<<<<< Updated upstream
                    message = "Updated Successfully";
                    
                    ////Remove connection the task to subtask
                    //gremlinScript = $"\ng.V().has('subtask', 'id', '{tbl_subtask.subtaskid}').bothE().drop()";
                    //// Execute the Gremlin script
                    //result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //message = "Gremlin script executed successfully";
                    

                    //connect the task to subtask
                    gremlinScript = $"\ng.V('{tbl_subtask.taskid}').addE('contains').to(g.V('{tbl_subtask.subtaskid}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";
                    

                    //Connect the predeccesors to succesors
                    gremlinScript = $"\ng.V('{tbl_subtask.taskid}').addE('precedes').property('duration', '{tbl_subtask.durations}').to(g.V('{tbl_subtask.subtaskid}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";
                    
                }                
=======
                    message = "Updated Successfully";                    
                }
                if (tbl_subtask.progress > 0)
                {
                    IGUtilities.updatetaskprogress(tbl_subtask.taskid);
                }
>>>>>>> Stashed changes
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

        public JsonResult GetEnddate(DateTime startDate, int id, string taskid)
        {
            var record = IGUtilities.CalculateDays(startDate, id);
            //return Json(record, JsonRequestBehavior.AllowGet);

            ProjectSubTask subtask = new ProjectSubTask();
            var gremlinScript = "g.V().has('task','id','" + taskid + "').project('startdate','enddate','durations').by(values('startdate')).by(values('enddate')).by(values('durations'))";
            var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    if (record > Convert.ToDateTime(item["enddate"]))
                    {
                        subtask.startdate = Convert.ToDateTime(item["startdate"]);
                        subtask.enddate = Convert.ToDateTime(item["enddate"]);
                        subtask.durations = Convert.ToInt16(item["durations"]);
                    }
                    else
                    {
                        subtask.enddate = record;
                    }
                }
                return Json(subtask, JsonRequestBehavior.AllowGet);
            }
            else
            {
                subtask.enddate = record;
            }
            return Json(subtask, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckTaskdates(string taskid, DateTime startdate, DateTime enddate)
        {
            ProjectSubTask subtask = new ProjectSubTask();
            var gremlinScript = "g.V().has('task','id','" + taskid + "').project('startdate','enddate','durations').by(values('startdate')).by(values('enddate')).by(values('durations'))";
            var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    if (enddate > Convert.ToDateTime(item["enddate"]) || startdate < Convert.ToDateTime(item["startdate"]))
                    {
                        subtask.startdate = Convert.ToDateTime(item["startdate"]);
                        subtask.enddate = Convert.ToDateTime(item["enddate"]);
                        subtask.durations = Convert.ToInt16(item["durations"]);
                    }
                }
            }
            return Json(subtask, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckTaskEnddate(string taskid, DateTime enddate)
        {
            ProjectSubTask subtask = new ProjectSubTask();
            var gremlinScript = "g.V().has('task','id','" + taskid + "').project('startdate','enddate','durations').by(values('startdate')).by(values('enddate')).by(values('durations'))";
            var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    if (enddate > Convert.ToDateTime(item["enddate"]))
                    {
                        subtask.startdate = Convert.ToDateTime(item["startdate"]);
                        subtask.enddate = Convert.ToDateTime(item["enddate"]);
                        subtask.durations = Convert.ToInt16(item["durations"]);
                    }
                }
            }
            return Json(subtask, JsonRequestBehavior.AllowGet);
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
