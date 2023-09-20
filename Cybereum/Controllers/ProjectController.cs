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
using Newtonsoft.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using System.IO;
using Gremlin.Net.Structure.IO.GraphSON;
using Newtonsoft.Json.Linq;
using Cybereum.Services;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
//using QuickGraph;

namespace Cybereum.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private cybereumEntities db = new cybereumEntities();


        [Authorize]
        [SessionTimeout]
        // GET: Project
        public ActionResult Index()
        {
            //GetProject(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]));            

            return View();
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult List()
        {
            //var x = getproject();
            return View();
        }

        public async Task<Project> getprojectbyid(string id)
        {
            Project project = new Project();
            try
            {

                //var gremlinScript = "g.V().has('project','id','" + id + "').project('id','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','projectmembers','projectstatus','projecttype').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('projectmembers').fold()).by(values('projectstatus')).by(values('projecttype'))";
                try
                {
                    //var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    var results = GetProjectbyID(id);

                    foreach (var result in results)
                    {
                        project.projectid = result["id"].ToString();
                        project.projectname = result["projectname"].ToString();
                        project.startdate = Convert.ToDateTime(result["startdate"]);
                        project.enddate = Convert.ToDateTime(result["enddate"]);
                        //project.type = result["type"].ToString();
                        project.noofresource = result["noofresource"].ToString();
                        project.projectcost = result["projectcost"].ToString();
                        project.createdby = Convert.ToInt16(result["createdby"].ToString());
                        project.createdusername = result["createdusername"].ToString();

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
                            int[] ints = Users.Split(',').Select(int.Parse).ToArray();
                            project.projectmembers = ints;
                        }

                        project.projectstatus = result["projectstatus"].ToString();
                        project.projecttype = result["projecttype"].ToString();

                        project.createdon = Convert.ToDateTime(result["createdon"]);
                    }
                    return project;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public async Task<JsonResult> projectlist()
        {
            List<Project> projectlist = new List<Project>();
            try
            {
                //int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
                string pmuserid = Session["LoggedInUserId"].ToString();
                var gremlinScript = "g.V().hasLabel('project').project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','hashcode').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('hashcode'))";
                if (Convert.ToInt32(Session["RoleID"]) == (int)Role.ProjectManager)
                {
                    //gremlinScript = "g.V().or(has('project','createdby','" + pmuserid + "'),has('project','projectmembers','" + pmuserid + "')).project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon'))";
                    gremlinScript = "g.V().has('project','createdby','" + pmuserid + "').project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','hashcode').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('hashcode'))";
                }

                try
                {
                    //var results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    string pList = JsonConvert.SerializeObject(results);
                    projectlist = JsonConvert.DeserializeObject<List<Project>>(pList);

                    var joinedData = from m in projectlist
                                     join r in db.tbl_user on m.createdby equals r.userid
                                     select new
                                     {
                                         projectid = m.projectid,
                                         projectname = m.projectname,
                                         startdate = m.startdate,
                                         enddate = m.enddate,
                                         type = m.type,
                                         noofresource = m.noofresource,
                                         projectcost = m.projectcost,
                                         createdby = m.createdby,
                                         createdusername = r.firstname + " " + r.lastname,
                                         createdon = m.createdon,
                                         projectmembers = m.projectmembers,
                                         organization = m.organization,
                                         projectstatus = m.projectstatus,
                                         projecttype = m.projecttype,
                                         hashcode = m.hashcode
                                     };

                    //foreach (var project in projectlist)
                    //{
                    //    int userid = Convert.ToInt32(project.createdby);
                    //    var username = db.tbl_user.Where(x => x.userid == userid).FirstOrDefault();
                    //    if (username != null)
                    //    {
                    //        project.createdusername = username.firstname + ' ' + username.lastname;
                    //    }
                    //}
                    var projectresult = this.Json(new { data = joinedData, recordsTotal = projectlist.Count(), recordsFiltered = projectlist.Count() }, JsonRequestBehavior.AllowGet);
                    return projectresult;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public JsonResult Getprojectmember(string term)
        {
            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            var userlist = (
                from b in db.tbl_user
                join c in db.tbl_userrole on b.roleid equals c.roleid
                where b.firstname.StartsWith(term) && b.roleid != 1 && b.isactive == 1 && b.userid != pmuserid
                select new SelectListItem
                {
                    Text = b.firstname + " " + b.lastname + "-" + c.rolename + "-" + b.organization,
                    Value = b.userid.ToString()
                }
                ).Distinct().OrderBy(x => x.Text).ToList();

            return Json(userlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Addrecord(int Id)
        {
            ViewBag.Message = "Edit Project";
            Project project = new Project();

            //project = db.Project.Find(Id);
            return RedirectToAction("Create", project);
        }

        public ActionResult AddEditrecord(string Id)
        {
            ViewBag.Message = "Edit Project";
            Project project1 = new Project();

            var result = getprojectbyid(Id);
            project1 = result.Result;
            return RedirectToAction("AddEditProject", project1);
        }

        [Authorize]
        [SessionTimeout]
        public ViewResult Create(int? id, ProjectViewModel Projects)
        {
            if (id == null)
            {
                Projects.startdate = DateTime.Today;
                Projects.enddate = DateTime.Today;
            }

            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid = Convert.ToInt32(Session["RoleId"]);
            //List<SelectListItem> user = Filluser(pmuserid, roleid);
            //ViewBag.createdby = user;
            return View(Projects);
        }

        [Authorize]
        [SessionTimeout]
        public ViewResult AddEditProject(int? id, Project Projects)
        {
            var result = getprojectbyid(Projects.projectid);
            Projects = result.Result;

            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid = Convert.ToInt32(Session["RoleId"]);

            //List<SelectListItem> user = Filluser(pmuserid, roleid);
            //ViewBag.createdby = user;

            List<SelectListItem> members = Filluser(pmuserid, Convert.ToInt16(Role.Admin));
            if (Projects.projectmembers != null && members.Count > 0)
            {
                foreach (var selectedItem in members)
                {
                    foreach (var item in Projects.projectmembers)
                    {
                        if (selectedItem.Value.ToString() == item.ToString())
                        {
                            selectedItem.Selected = true;
                        }
                    }
                }
            }
            ViewBag.projectmembers = members;
            ViewBag.projectmembersNew = members;
            ViewBag.projectmembersSelect = members;

            if (Projects.projectid == null)
            {
                Projects.startdate = DateTime.Today;
                Projects.enddate = DateTime.Today;
            }

            //Projects.projectmembers = null;
            return View(Projects);
        }

        public List<SelectListItem> Filluser(int? pmuserid, int? roleid)
        {
            List<SelectListItem> user = new List<SelectListItem>();
            if (roleid == (int)Role.Admin)
            {
                user = (from b in db.tbl_user
                        join c in db.tbl_userrole on b.roleid equals c.roleid
                        where b.roleid != 1 && b.isactive == 1 && b.userid != pmuserid
                        select new SelectListItem
                        {
                            Text = b.firstname + " " + b.lastname + "-" + c.rolename + "-" + b.organization,
                            Value = b.userid.ToString()
                        }).Distinct().OrderBy(x => x.Text).ToList();
            }
            else
            {
                user = (from b in db.tbl_user
                        join c in db.tbl_userrole on b.roleid equals c.roleid
                        where b.pmuserid == pmuserid && b.isactive == 1
                        select new SelectListItem
                        {
                            Text = b.firstname + " " + b.lastname + "-" + c.rolename + "-" + b.organization,
                            Value = b.userid.ToString()
                        }).Distinct().OrderBy(x => x.Text).ToList();
            }
            return user;
        }

        [Authorize]
        [SessionTimeout]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditProject([Bind(Include = "projectid,projectname,startdate,enddate,createdon,modifiedon,createdby,modifiedby,projectcost,noofresource,isactive,projectmembers,projectstatus,projecttype,members")] Project tbl_project)
        {
            string message = string.Empty;

            //if (tbl_project.members != null)
            //{
            //    tbl_project.projectmembers = tbl_project.members.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
            //}

            //var gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + database + "/colls/" + collection, password: authKey);

            if (ModelState.IsValid)
            {
                //Checking for project name existence
                //var objList = db.sp_FetchProjectExists(tbl_project.projectname).FirstOrDefault();
                long count = 0;
                if (tbl_project.projectid == null)
                {
                    var gremlinScript = "g.V().has('project','projectname','" + tbl_project.projectname + "').count()";
                    var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    count = objList.ToList()[0];
                    //}
                    if (count > 0)
                    {
                        message = "Project name already exists.";
                        goto endloop;
                    }
                }
                else
                {
                    var gremlinScript = "g.V().has('project','projectname','" + tbl_project.projectname + "')";
                    var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result in objList)
                    {
                        if (result["id"] != tbl_project.projectid)
                        {
                            message = "Project name already exists.";
                            goto endloop;
                        }
                    }
                }

                string a = "";
                if (tbl_project.projectmembers != null)
                {
                    for (int i = 0; i < tbl_project.projectmembers.Length; i++)
                    {
                        a = a + $".property(list,'projectmembers', '{tbl_project.projectmembers[i]}') ";
                    }
                }
                else
                {
                    a = a + $".property(list,'projectmembers', '')";
                }

                //
                if (tbl_project.projectid == null)
                {
                    if (tbl_project.createdby == 0 || tbl_project.createdby == null)
                    {
                        tbl_project.createdby = Convert.ToInt16(Session["LoggedInUserId"].ToString());
                        tbl_project.createdusername = Session["Username"].ToString();
                    }
                    else
                    {
                        tbl_project.createdby = tbl_project.createdby;
                    }

                    string gremlinScript = $"g.addV('project').property('pk', '{tbl_project.projectname}')" +
                            $".property('projectname', '{tbl_project.projectname}')" +
                            $".property('startdate', '{tbl_project.startdate.ToString("yyyy-MM-dd")}')" +
                            $".property('enddate', '{tbl_project.enddate.ToString("yyyy-MM-dd")}')" +
                            $".property('noofresource', '{tbl_project.noofresource}')" +
                            $".property('projectcost', '{tbl_project.projectcost}')" +
                            $".property('projectstatus', '{tbl_project.projectstatus}')" +
                            $".property('projecttype', '{tbl_project.projecttype}')" + a +
                            $".property('hashcode', '')" +
                            $".property('createdby', '{Convert.ToInt32(tbl_project.createdby)}')" +
                            $".property('createdusername', '')" +
                            $".property('createdon', '{DateTime.Now}')" +
                            $".property('type', 'project')";

                    // Execute the Gremlin script                    
                    var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Added Successfully";
                    //}


                    //**************Get Last added project id***********
                    gremlinScript = "g.V().has('project','projectname','" + tbl_project.projectname + "').project('id').by(values('id'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result1 in result)
                    {
                        tbl_project.projectid = Convert.ToString(result1["id"]);
                    }
                    //**************End***********

                    ////Add start and end activities
                    //******************* Start Activity****************************
                    ProjectActivity tbl_activity = new ProjectActivity();
                    tbl_activity.createdby = Session["LoggedInUserId"].ToString();
                    tbl_activity.activityname = ConfigurationManager.AppSettings["StartActivity"];//"Start Activity"
                    tbl_activity.startdate = tbl_project.startdate;
                    tbl_activity.enddate = tbl_project.startdate;
                    tbl_activity.projectid = tbl_project.projectid;
                    a = $".property(list,'predecessors', '')";

                    gremlinScript = $"g.addV('activity').property('pk', '{tbl_activity.activityname}')" +
                            $".property('activityname', '{tbl_activity.activityname}')" +
                            $".property('startdate', '{tbl_activity.startdate.ToString("yyyy-MM-dd")}')" +
                            $".property('enddate', '{tbl_activity.enddate.ToString("yyyy-MM-dd")}')" +
                            $".property('projectid', '{tbl_activity.projectid}')" +
                            $".property('durations', '{1}')" +
                            $".property('ismilestone', '{false}')" +
                            $".property('createdby', '{Convert.ToInt32(tbl_activity.createdby)}')" +
                            $".property('linktype','0')" +
                            $".property('createdusername', '')" +
                            $".property('createdon', '{DateTime.Now}')" + a +
                            $".property('type', 'activity')";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Activity Added Successfully";

                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('contains').to(g.V('{tbl_activity.id}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //******************* End ****************************

                    //******************* End Activity****************************
                    tbl_activity = new ProjectActivity();
                    tbl_activity.createdby = Session["LoggedInUserId"].ToString();
                    tbl_activity.activityname = ConfigurationManager.AppSettings["EndActivity"];
                    tbl_activity.startdate = tbl_project.enddate;
                    tbl_activity.enddate = tbl_project.enddate;
                    tbl_activity.projectid = tbl_project.projectid;
                    a = $".property(list,'predecessors', '')";

                    gremlinScript = $"g.addV('activity').property('pk', '{tbl_activity.activityname}')" +
                            $".property('activityname', '{tbl_activity.activityname}')" +
                            $".property('startdate', '{tbl_activity.startdate.ToString("yyyy-MM-dd")}')" +
                            $".property('enddate', '{tbl_activity.enddate.ToString("yyyy-MM-dd")}')" +
                            $".property('projectid', '{tbl_activity.projectid}')" +
                            $".property('durations', '{1}')" +
                            $".property('ismilestone', '{false}')" +
                            $".property('createdby', '{Convert.ToInt32(tbl_activity.createdby)}')" +
                            $".property('createdusername', '')" +
                            $".property('createdon', '{DateTime.Now}')" + a +
                            $".property('linktype','0')" +
                            $".property('type', 'activity')";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Activity Added Successfully";

                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('contains').to(g.V('{tbl_activity.id}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //******************* End ****************************

                    //*************Nodejs API Call*************
                    Senddatatoapi(tbl_project);
                }
                else
                {
                    string gremlinscript = $"g.V().has('project', 'id','{ tbl_project.projectid }').properties('projectmembers').drop()";
                    var result = IGUtilities.ExecuteGremlinScript(gremlinscript);
                    message = "Updated Successfully";
                    //}

                    string gremlinScript = $"g.V('{tbl_project.projectid}').property('projectname', '{tbl_project.projectname}')" +
                                                $".property('projectname', '{tbl_project.projectname}')" +
                                                $".property('startdate', '{tbl_project.startdate.ToString("yyyy-MM-dd")}')" +
                                                $".property('enddate', '{tbl_project.enddate.ToString("yyyy-MM-dd")}')" +
                                                $".property('noofresource', '{tbl_project.noofresource}')" +
                                                $".property('projectcost', '{tbl_project.projectcost}')" +
                                                $".property('projectstatus', '{tbl_project.projectstatus}')" +
                                                $".property('projecttype', '{tbl_project.projecttype}')" + a +
                                                $".property('updatedon', '{DateTime.Now}')" +
                                                $".property('type', 'project')";


                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Updated Successfully";
                    //}
                }
                return RedirectToAction("List");
            }

            endloop:
            ViewBag.Message = message;
            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid = Convert.ToInt32(Session["RoleId"]);
            //List<SelectListItem> user = Filluser(pmuserid, roleid);
            //ViewBag.createdby = user;
            List<SelectListItem> pm = Filluser(pmuserid, Convert.ToInt16(Role.Admin));
            if (tbl_project.projectmembers != null && pm.Count > 0)
            {
                foreach (var selectedItem in pm)
                {
                    foreach (var item in tbl_project.projectmembers)
                    {
                        if (selectedItem.Value.ToString() == item.ToString())
                        {
                            selectedItem.Selected = true;
                        }
                    }
                }
            }
            ViewBag.projectmembers = pm;
            ViewBag.projectmembersNew = pm;
            ViewBag.projectmembersSelect = pm;
            return View(tbl_project);
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult GanttChart(string projectid)
        {
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


        public JsonResult CreateTask(GanttTask1 task)
        {
            string createdby = Session["LoggedInUserId"].ToString();
            string projectid = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["projectid"].ToString();
            if (task.parent == "0")
            {
                Boolean ismilestone = false;
                ActivityController activity = new ActivityController();
                ProjectActivity act = new ProjectActivity();
                act.activityname = task.text;
                act.startdate = task.start_date;
                act.enddate = task.end_date;
                act.projectid = projectid;
                act.durations = task.duration;
                act.ismilestone = ismilestone;
                act.createdby = createdby;
                activity.Create(act);
            }
            else
            {
                string gremlinScript = $"g.V('id', '{task.parent}').project('id','type').by(values('id')).by(values('type'))";
                var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                foreach (var item in result)
                {
                    if(item["type"]=="activity")
                    {
                        TaskController Taskcontroller = new TaskController();
                        ProjectTask projecttask = new ProjectTask();
                        projecttask.taskname = task.text;
                        projecttask.startdate = task.start_date;
                        projecttask.enddate = task.end_date;
                        projecttask.activityid = task.parent;
                        projecttask.durations = task.duration;
                        projecttask.createdby = createdby;
                        projecttask.taskstatus = 1;
                        projecttask.tasktype = 1;
                        Taskcontroller.AddEditTask(projecttask);
                    }
                    else if (item["type"] == "task")
                    {
                        SubTaskController Taskcontroller = new SubTaskController();
                        ProjectSubTask projecttask = new ProjectSubTask();
                        projecttask.subtaskname = task.text;
                        projecttask.startdate = task.start_date;
                        projecttask.enddate = task.end_date;
                        projecttask.taskid = task.parent;
                        projecttask.durations = task.duration;
                        projecttask.createdby = createdby;
                        projecttask.taskstatus = 1;
                        projecttask.tasktype = 1;
                        Taskcontroller.AddEditSubTask(projecttask);
                    }
                }
            }

            return GetchartData(projectid);
        }

        public JsonResult CreateLink(GanttLink1 task)
        {
            string projectid = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["projectid"].ToString();
            string[] predecessor = new string[] { task.source };
            //string[]  predecessor = predecessor.Concat(new string[] { task.target }).ToArray();
            string a = string.Empty;
            if (predecessor != null)
            {
                for (int i = 0; i < predecessor.Length; i++)
                {
                    if (predecessor[i] != "" && predecessor[i] != "System.String[]")
                    {
                        a = a + $".property(list,'predecessors', '{predecessor[i]}')";
                    }
                }
            }
            var gremlinScript = $"g.V('{task.target}')" +
                        a +
                        $".property('linktype', '{task.type}')" +
                        $".property('updatedon', '{DateTime.Now}')" +
                        $".property('type', 'activity')";
            var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
            IGUtilities.updateactivitydatesbytype(task.target);
            IGUtilities.updateactivityprojectdate(task.target);            
            return GetchartData(projectid);
        }

        public dynamic getdata(string projectid)
        {
            try
            {
                DateTime startdate;
                DateTime enddate;
                string connection = string.Empty;

                var gremlinScript = "g.V().has('project','id','" + projectid + "').project('id','projectname','startdate','enddate').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate'))";
                var projectdata = IGUtilities.ExecuteGremlinScript(gremlinScript);

                List<GanttTask> ganttchartmodellist = new List<GanttTask>();
                List<GanttLink> ganttchartmodellink = new List<GanttLink>();
                int i = 1;
                int linkid = 1;
                foreach (var project in projectdata)
                {
                    GanttTask ganttchart = new GanttTask();
                    GanttLink ganttlink = new GanttLink();
                    int duration = 0;
                    //ganttchart.GanttTaskId = project["id"].ToString();// i;
                    //i++;
                    //ganttchart.taskid = project["id"].ToString();
                    //ganttchart.Text = project["projectname"].ToString();
                    //startdate = Convert.ToDateTime(project["startdate"].ToString());
                    //enddate = Convert.ToDateTime(project["enddate"].ToString());
                    //duration = IGUtilities.CalculateDays(startdate.Date, enddate.Date);
                    //ganttchart.Duration = duration;
                    //ganttchart.StartDate = startdate;
                    //ganttchart.EndDate = enddate;
                    //ganttchart.SortOrder = 10;
                    //ganttchart.Progress = (decimal)0.60;
                    ////ganttchart.ParentId = t.ParentId;
                    //ganttchart.Type = "Project";

                    //ganttchartmodellist.Add(ganttchart);

                    //Activity
                    gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations','predecessors','linktype').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('predecessors').fold()).by(values('linktype'))";
                    var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    string pList = JsonConvert.SerializeObject(activitydata);
                    List<ProjectActivity> Activitylist = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);
                    Activitylist = Activitylist.OrderBy(a=> a.activityname !=ConfigurationManager.AppSettings["StartActivity"].ToString()).ThenBy(a => a.startdate).ThenBy(a => a.enddate).ToList();
                    
                    foreach (var itemactivity in Activitylist)
                    {
                        ganttchart = new GanttTask();
                        ganttchart.GanttTaskId = itemactivity.id;// i;
                        i++;
                        ganttchart.taskid = itemactivity.id;
                        ganttchart.Text = itemactivity.activityname;
                        startdate = Convert.ToDateTime(itemactivity.startdate);
                        enddate = Convert.ToDateTime(itemactivity.enddate);
                        enddate=enddate.AddHours(23);
                        duration = IGUtilities.CalculateDays(startdate.Date, enddate.Date);
                        ganttchart.Duration = duration;
                        ganttchart.StartDate = startdate;
                        ganttchart.EndDate = enddate;
                        ganttchart.SortOrder = 10;
                        ganttchart.Progress = (decimal)0.60;
                        ganttchart.Type = "Activity";                        
                        ganttchartmodellist.Add(ganttchart);
                                                
                        gremlinScript = "g.V().has('task','activityid','" + itemactivity.id + "').project('taskid','taskname','startdate','enddate','durations','activityid').by(id()).by(values('taskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('activityid'))";
                        var taskdata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        pList = JsonConvert.SerializeObject(taskdata);
                        List<ProjectTask> Tasklist = JsonConvert.DeserializeObject<List<ProjectTask>>(pList);
                        Tasklist = Tasklist.OrderBy(a => a.startdate).ThenBy(a => a.enddate).ToList();                        
                        foreach (var itemtask in Tasklist)
                        {
                            ganttchart = new GanttTask();
                            ganttchart.GanttTaskId = itemtask.taskid;// i;
                            i++;
                            ganttchart.taskid = itemtask.taskid;
                            ganttchart.Text = itemtask.taskname;
                            startdate = Convert.ToDateTime(itemtask.startdate);
                            enddate = Convert.ToDateTime(itemtask.enddate);
                            enddate = enddate.AddHours(23);
                            duration = IGUtilities.CalculateDays(startdate.Date, enddate.Date);
                            ganttchart.Duration = duration;
                            ganttchart.StartDate = startdate;
                            ganttchart.EndDate = enddate;
                            ganttchart.SortOrder = 10;
                            ganttchart.Progress = (decimal)0.60;
                            ganttchart.Type = "Task";
                            ganttchart.ParentId = itemactivity.id;
                            ganttchartmodellist.Add(ganttchart);

                            
                            gremlinScript = "g.V().has('subtask','taskid','" + itemtask.taskid + "').project('subtaskid','subtaskname','startdate','enddate','durations').by(id()).by(values('subtaskname')).by(values('startdate')).by(values('enddate')).by(values('durations'))";
                            var subtaskdata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                            pList = JsonConvert.SerializeObject(subtaskdata);
                            List<ProjectSubTask> SubTasklist = JsonConvert.DeserializeObject<List<ProjectSubTask>>(pList);
                            SubTasklist = SubTasklist.OrderBy(a => a.startdate).ThenBy(a => a.enddate).ToList();
                            foreach (var itemsubtask in SubTasklist)
                            {
                                ganttchart = new GanttTask();
                                ganttchart.GanttTaskId = itemsubtask.subtaskid;// i;
                                i++;
                                ganttchart.taskid = itemsubtask.subtaskid;
                                ganttchart.Text = itemsubtask.subtaskname;
                                startdate = Convert.ToDateTime(itemsubtask.startdate);
                                enddate = Convert.ToDateTime(itemsubtask.enddate);
                                enddate = enddate.AddHours(23);
                                duration = IGUtilities.CalculateDays(startdate.Date, enddate.Date);
                                ganttchart.Duration = duration;
                                ganttchart.StartDate = startdate;
                                ganttchart.EndDate = enddate;
                                ganttchart.SortOrder = 10;
                                ganttchart.Progress = (decimal)0.60;
                                ganttchart.Type = "SubTask";
                                ganttchart.ParentId = itemtask.taskid;
                                ganttchartmodellist.Add(ganttchart);
                            }                            
                        }
                    }


                    //Activity Link
                    string prevtaskid = "";
                    foreach (var itemactivity in Activitylist)
                    {
                        ganttchart = new GanttTask();
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
                            if (ints.Count() > 0)
                            {
                                for (int j = 0; j <= ints.Count() - 1; j++)
                                {
                                    var connector = ganttchartmodellist.Find(a => a.taskid == ints[j]);
                                    chartconnector conn = new chartconnector();
                                    if (connector != null)
                                    {
                                        ganttlink = new GanttLink();
                                        ganttlink.GanttLinkId = linkid++;
                                        ganttlink.SourceTaskId = connector.GanttTaskId;
                                        ganttlink.TargetTaskId = itemactivity.id;
                                        ganttlink.taskid = itemactivity.id;
                                        ganttlink.Type = itemactivity.linktype;
                                        ganttchartmodellink.Add(ganttlink);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (itemactivity.activityname == ConfigurationManager.AppSettings["endactivity"].ToString())
                            {
                                ganttlink = new GanttLink();
                                ganttlink.GanttLinkId = linkid++;
                                ganttlink.SourceTaskId = prevtaskid;
                                ganttlink.TargetTaskId = itemactivity.id;
                                ganttlink.taskid = itemactivity.id;
                                ganttlink.Type = itemactivity.linktype;
                                ganttchartmodellink.Add(ganttlink);
                            }
                        }
                        prevtaskid = itemactivity.id;
                    }
                }

                int counter = 1;
                var jsonData = new
                {
                    // create tasks array
                    tasks = (
                        from t in ganttchartmodellist
                        select new
                        {
                            id = t.GanttTaskId,
                            text = t.Text,
                            start_date = t.StartDate.ToString("u"),
                            end_date = t.EndDate.ToString("u"),
                            duration = t.Duration,
                            order = t.SortOrder,
                            progress = t.Progress,
                            parent = t.ParentId,
                            type = t.Type,
                            taskid = t.taskid
                        }
                    ).ToArray(),
                    // create links array
                    links = (
                        from l in ganttchartmodellink
                        select new
                        {
                            id = (counter++).ToString(),
                            task = l.taskid,
                            source = l.SourceTaskId,
                            target = l.TargetTaskId,// == null ? counter++ : l.ParentId,
                            type = l.Type
                        }
                    ).ToArray()
                };
                //var x = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
                return jsonData;
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
        }
        
        [HttpPost]
        public ActionResult DeleteLink(string id)
        {
            string projectid = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["projectid"].ToString();
            string[] predecessor = new string[] { };
            try
            {
                var data = getdata(projectid);
                var jsondata = JsonConvert.SerializeObject(data, Formatting.Indented);
                JObject jsonObj = JObject.Parse(jsondata);
                JArray myArray = (JArray)jsonObj["links"];
                List<GanttLink1> numberList = myArray.ToObject<List<GanttLink1>>();

                GanttLink1 task = numberList.Find(x => x.id == id);
                var gremlinScript = "g.V().has('activity','id','" + task.task + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations','predecessors').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('predecessors').fold())";
                var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                string pList = JsonConvert.SerializeObject(activitydata);
                List<ProjectActivity> Activitylist = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);
                foreach (var result in Activitylist)
                {
                    predecessor = result.Predecessors;
                }
                predecessor = predecessor.Where(w => w != task.source).ToArray();
                gremlinScript = $"g.V().has('activity', 'id','{ task.task}').properties('predecessors').drop()";
                var result1 = IGUtilities.ExecuteGremlinScript(gremlinScript);

                string a = string.Empty;
                if (predecessor != null)
                {
                    for (int i = 0; i < predecessor.Length; i++)
                    {
                        if (predecessor[i] != "" && predecessor[i] != "System.String[]")
                        {
                            a = a + $".property(list,'predecessors', '{predecessor[i]}')";
                        }
                    }
                }
                gremlinScript = $"g.V('{task.task}')" +
                            a +
                            $".property('updatedon', '{DateTime.Now}')" +
                            $".property('type', 'activity')";
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                //IGUtilities.updateactivitydatesbytype(task.task);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
            return GetchartData(projectid);
        }

        public JsonResult UpdateTask(GanttTask1 task)
        {
            string projectid = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["projectid"].ToString();
            //try
            //{                
                Boolean ismilestone = false;
                if (task.type == "Activity")
                {
                    //string gremlinScript = $"g.V('{task.taskid}')" +
                    //        $".property('activityname', '{task.text}')" +
                    //        $".property('startdate', '{task.start_date.ToString("yyyy-MM-dd")}')" +
                    //        $".property('enddate', '{task.end_date.ToString("yyyy-MM-dd")}')" +
                    //        $".property('projectid', '{projectid}')" +
                    //        $".property('durations', '{task.duration}')" +
                    //        $".property('ismilestone', '{ismilestone}')" +
                    //        $".property('updatedon', '{DateTime.Now}')" +
                    //        $".property('type', 'activity')";
                    //var result = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    ActivityController activity = new ActivityController();
                    var result1 = activity.getactivitybyid(task.taskid);
                    ProjectActivity act = new ProjectActivity();
                    act = result1.Result;
                    act.id = task.taskid;
                    act.activityname = task.text;
                    act.startdate = task.start_date;
                    act.enddate = task.end_date;
                    act.projectid = projectid;
                    act.durations = task.duration;
                    act.ismilestone = ismilestone;
                    activity.Create(act);

                    ////***************update preceding activity dates*************
                    //IGUtilities.updateprecedingactivitydates(projectid, task.taskid);
                    ////****************************End****************************
                    //IGUtilities.updateactivityprojectdate(projectid);
                }
                else if (task.type == "Task")
                {
                    TaskController activity = new TaskController();
                    var result1 = activity.gettaskbyid(task.taskid);
                    ProjectTask act = new ProjectTask();
                    act = result1.Result;
                    act.taskid = task.taskid;
                    act.taskname = task.text;
                    act.startdate = task.start_date;
                    act.enddate = task.end_date;
                    act.activityid = task.parent;
                    act.durations = task.duration;
                    activity.AddEditTask(act);
                }
                else if (task.type == "SubTask")
                {
                    SubTaskController activity = new SubTaskController();
                    var result1 = activity.getsubtaskbyid(task.id);
                    ProjectSubTask act = new ProjectSubTask();
                    act = result1.Result;
                    act.taskid = task.taskid;
                    act.taskname = task.text;
                    act.startdate = task.start_date;
                    act.enddate = task.end_date;
                    act.taskid = task.parent;
                    act.durations = task.duration;
                    activity.AddEditSubTask(act);
                }
            
            return GetchartData(projectid);
        }

        public JsonResult GetchartData(string projectid)
        {
            try
            {
                var jsonData = getdata(projectid);                
                return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult Graph()
        {
            return View();
        }

        public void Senddatatoapi(Project project)
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["nodejsapi"].ToString();// + "addProject";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["nodejsapi"].ToString());
                    client.DefaultRequestHeaders.Accept.Clear();
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        id = project.projectid,
                        name = project.projectname
                    });
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    client.Timeout = TimeSpan.FromMinutes(120);

                    var response = client.PostAsync("addProject", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content.ReadAsStringAsync().Result;

                        Projectresponse projectlist = new Projectresponse();
                        projectlist = JsonConvert.DeserializeObject<Projectresponse>(responseContent);
                        string gremlinScript = $"g.V('{project.projectid}')" +
                                                    $".property('hashcode', '{projectlist.hash}')" +
                                                    $".property('type', 'project')";
                        var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        //IGUtilities.WriteLog(responseContent);
                    }
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                IGUtilities.WriteLog(ex.Message);
            }
        }


        public async Task<GraphData> getgannchart()
        {
            GraphService service = new GraphService(gremlinvariables.hostname, gremlinvariables.port, gremlinvariables.authKey, gremlinvariables.database, gremlinvariables.collection);
            GraphData x = await service.GetGraphData();
            //var x = JsonConvert.SerializeObject(projectdata, Formatting.Indented);
            return x;
        }

        [Authorize]
        [SessionTimeout]
        // GET: Project
        public ActionResult d3_GANTT_Chart()
        {
            return View();
        }


        [Authorize]
        [SessionTimeout]
        // GET: Project
        public ActionResult DashTest2()
        {
            return View();
        }

        private static ResultSet<dynamic> GetProjectbyID(string id)
        {
            var gremlinScript = "g.V().has('project','id','" + id + "').project('id','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','projectmembers','projectstatus','projecttype').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('projectmembers').fold()).by(values('projectstatus')).by(values('projecttype'))";
            var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
            return results;
        }

    }
}
