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

//using AppModelv2_WebApp_OpenIDConnect_DotNet.Models;
//using Microsoft.Azure.Documents;
//using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using System.IO;
using Gremlin.Net.Structure.IO.GraphSON;
using Newtonsoft.Json.Linq;
using Cybereum.Services;
using System.Configuration;
//using QuickGraph;


namespace Cybereum.Controllers
{
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
                        project.createdby = result["createdby"].ToString();
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
                int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
                var gremlinScript = "g.V().hasLabel('project').project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon'))";
                if (Convert.ToInt32(Session["RoleID"]) == (int)Role.ProjectManager)
                {
                    gremlinScript = "g.V().or(has('project','createdby','" + pmuserid + "'),has('project','projectmembers','" + pmuserid + "')).project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon'))";
                }

                try
                {
                    //var results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    string pList = JsonConvert.SerializeObject(results);
                    projectlist = JsonConvert.DeserializeObject<List<Project>>(pList);

                    foreach (var project in projectlist)
                    {
                        int userid = Convert.ToInt32(project.createdby);
                        var username = db.tbl_user.Where(x => x.userid == userid).FirstOrDefault();
                        if (username != null)
                        {
                            project.createdusername = username.firstname + ' ' + username.lastname;
                        }
                    }
                    
                    var projectresult = this.Json(new { data = projectlist, recordsTotal = projectlist.Count(), recordsFiltered = projectlist.Count() }, JsonRequestBehavior.AllowGet);
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
            List<SelectListItem> user = Filluser(pmuserid, roleid);
            ViewBag.createdby = user;
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
                        where b.roleid == 2 && b.isactive == 1 && b.userid != pmuserid
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
        public ActionResult AddEditProject([Bind(Include = "projectid,projectname,startdate,enddate,createdon,modifiedon,createdby,modifiedby,projectcost,noofresource,isactive,projectmembers,projectstatus,projecttype")] Project tbl_project)
        {
            string message = string.Empty;
            string members = string.Empty;
            if (tbl_project.projectmembers != null)
            {
                members = String.Join(",", tbl_project.projectmembers);
            }


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
                    if (tbl_project.createdby == null)
                    {
                        tbl_project.createdby = Session["LoggedInUserId"].ToString();
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
                            //$".property('projectmembers', '{members}')" +
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
                            $".property('type', 'activity')";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Activity Added Successfully";

                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('contains').to(g.V('{tbl_activity.id}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //******************* End ****************************
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
            List<SelectListItem> user = Filluser(pmuserid, roleid);
            ViewBag.createdby = user;
            List<SelectListItem> pm = Filluser(0, Convert.ToInt16(Role.Admin));
            ViewBag.projectmembers = pm;
            return View(tbl_project);
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

        public async Task<GraphData> getgannchart()
        {
            GraphService service = new GraphService(gremlinvariables.hostname,gremlinvariables.port,gremlinvariables.authKey,gremlinvariables.database,gremlinvariables.collection);
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
