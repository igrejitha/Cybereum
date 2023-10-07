﻿using System;
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
using Microsoft.Graph;
using System.Net.Mime;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

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
            return View();
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult List()
        {
            return View();
        }

        [Authorize]
        //[Authorize(Roles ="Admin,ProjectManager,OrganizationManager,SeniorProjectManager")]
        [SessionTimeout]
        public ActionResult OrganizationProjectList(string organization)
        {
            //var x = getproject();
            Session["Organization"] = organization;
            return View();
        }

        public async Task<Project> getprojectbyid(string id)
        {
            Project project = new Project();
            try
            {
                try
                {
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
                        project.projectmanager = Convert.ToInt16(result["projectmanager"].ToString());

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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        public async Task<JsonResult> projectlist(string organization)
        {
            List<Project> projectlist = new List<Project>();
            try
            {
                var search = Request.Form.GetValues("search[value]")[0];
                int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
                int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
                string projectnamesearch = string.Empty;
                if (search != "")
                {
                    projectnamesearch = $".has('project','projectname',containing('{search}'))";
                }
                var gremlinScript = string.Empty;//= $"g.V().hasLabel('project').project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','hashcode').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('hashcode'))";
                if (Convert.ToInt32(Session["RoleID"]) == (int)Role.ProjectManager || Convert.ToInt32(Session["RoleID"]) == (int)Role.User)
                {
                    //gremlinScript = $"g.V().has('project','projectmanager','{pmuserid}').or().has('project','projectmembers','{ pmuserid }')" + projectnamesearch + ".project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','hashcode','projectmanager','projectstatus','progress').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('hashcode')).by(values('projectmanager')).by(values('projectstatus')).by(values('progress'))";
                    gremlinScript = $"g.V().or(has('project','projectmanager','{ pmuserid }'),has('project','projectmembers','" + pmuserid + "'))" + projectnamesearch + ".project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','hashcode','projectmanager','projectstatus','progress').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('hashcode')).by(values('projectmanager')).by(values('projectstatus')).by(values('progress'))";
                    //gremlinScript = $"g.V().has('project','projectmanager','{pmuserid}')" + projectnamesearch + ".project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','hashcode','projectmanager','projectstatus','progress').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('hashcode')).by(values('projectmanager')).by(values('projectstatus')).by(values('progress'))";
                }
                else
                {
                    string projectmember = IGUtilities.getprojectmembers(pmuserid);
                    if (projectmember == "") projectmember = "''";
                    gremlinScript = $"g.V().has('project','projectmanager',within({projectmember}))" + projectnamesearch + ".project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','hashcode','projectmanager','projectstatus','progress').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('hashcode')).by(values('projectmanager')).by(values('projectstatus')).by(values('progress'))";
                }

                try
                {
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    var pList = JsonConvert.SerializeObject(results);                    
                    projectlist = JsonConvert.DeserializeObject<List<Project>>(pList);

                    var joinedData = from m in projectlist
                                     join r in db.tbl_user on m.projectmanager equals r.userid
                                     select new
                                     {
                                         projectid = m.projectid,
                                         projectname = m.projectname,
                                         startdate = m.startdate,
                                         enddate = m.enddate,
                                         type = m.type,
                                         noofresource = m.noofresource,
                                         projectcost = m.projectcost,
                                         createdby = m.projectmanager,
                                         createdusername = r.firstname + " " + r.lastname,
                                         createdon = m.createdon,
                                         projectmembers = m.projectmembers,
                                         organization = m.organization,
                                         projectstatus = m.projectstatus,
                                         projecttype = m.projecttype,
                                         hashcode = m.hashcode,
                                         progress = m.progress
                                     };
                    joinedData = joinedData.Skip(startRec).Take(pageSize).ToList();
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

        public async Task<JsonResult> projectlistfororganization(string organization)
        {
            List<Project> projectlist = new List<Project>();
            try
            {
                int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
                var user = (from b in db.tbl_user
                            join c in db.tbl_userrole on b.roleid equals c.roleid
                            where b.isactive == 1 && (b.roleid == 2 || b.roleid == 3) && b.organization == organization
                            select new SelectListItem
                            {
                                Text = b.firstname + " " + b.lastname + "-" + c.rolename + "-" + b.organization,
                                Value = b.userid.ToString()
                            }).Distinct().OrderBy(x => x.Text).ToList();
                string projectmember = "''";
                foreach (var items in user.ToArray())
                {
                    projectmember = (projectmember == "" ? projectmember : projectmember + ",") + "'" + items.Value + "'";
                }

                var gremlinScript = string.Empty;//= $"g.V().hasLabel('project').project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','hashcode').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('hashcode'))";
                if (Convert.ToInt32(Session["RoleID"]) == (int)Role.ProjectManager || Convert.ToInt32(Session["RoleID"]) == (int)Role.User)
                {
                    gremlinScript = $"g.V().has('project','projectmanager','{pmuserid}').project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','hashcode','projectmanager','projectstatus').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('hashcode')).by(values('projectmanager')).by(values('projectstatus'))";
                }
                else
                {
                    gremlinScript = $"g.V().has('project','projectmanager',within({projectmember})).project('projectid','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','hashcode','projectmanager','projectstatus').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('hashcode')).by(values('projectmanager')).by(values('projectstatus'))";
                }

                try
                {
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    string pList = JsonConvert.SerializeObject(results);
                    projectlist = JsonConvert.DeserializeObject<List<Project>>(pList);

                    var joinedData = from m in projectlist
                                     join r in db.tbl_user on m.projectmanager equals r.userid
                                     select new
                                     {
                                         projectid = m.projectid,
                                         projectname = m.projectname,
                                         startdate = m.startdate,
                                         enddate = m.enddate,
                                         type = m.type,
                                         noofresource = m.noofresource,
                                         projectcost = m.projectcost,
                                         createdby = m.projectmanager,
                                         createdusername = r.firstname + " " + r.lastname,
                                         createdon = m.createdon,
                                         projectmembers = m.projectmembers,
                                         organization = m.organization,
                                         projectstatus = m.projectstatus,
                                         projecttype = m.projecttype,
                                         hashcode = m.hashcode
                                     };
                    var projectresult = this.Json(new { data = joinedData, recordsTotal = projectlist.Count(), recordsFiltered = projectlist.Count() }, JsonRequestBehavior.AllowGet);
                    return projectresult;
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
            return View(Projects);
        }

        //[Authorize(Roles = "ProjectManager,OrganizationManager,SeniorProjectManager")]
        [Authorize]
        [SessionTimeout]
        public ViewResult AddEditProject(int? id, Project Projects)
        {
            var result = getprojectbyid(Projects.projectid);
            Projects = result.Result;

            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid = Convert.ToInt32(Session["RoleId"]);

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
            List<SelectListItem> pm = IGUtilities.FillPM(pmuserid);
            if (Projects.projectmanager != null && pm.Count > 0)
            {
                foreach (var selectedItem in pm)
                {
                    if (selectedItem.Value.ToString() == Projects.projectmanager.ToString())
                    {
                        selectedItem.Selected = true;
                    }
                }
            }

            ViewBag.projectmanager = pm;
            ViewBag.projectmembers = members;
            ViewBag.projectmembersNew = members;
            ViewBag.projectmembersSelect = members;

            if (Projects.projectid == null)
            {
                Projects.startdate = DateTime.Today;
                Projects.enddate = DateTime.Today;
            }

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

        public List<SelectListItem> GetProjectMembers(int projectid, int userid)
        {
            List<SelectListItem> user = new List<SelectListItem>();

            var org = (from b in db.tbl_user
                       where b.userid == userid
                       select b.organization).Single();
            string organization = org.ToString();

            user = (from b in db.tbl_user
                    join c in db.tbl_userrole on b.roleid equals c.roleid
                    where b.isactive == 1 && (b.roleid == 2 || b.roleid == 3) && b.organization == organization
                    select new SelectListItem
                    {
                        Text = b.firstname + " " + b.lastname + "-" + c.rolename + "-" + b.organization,
                        Value = b.userid.ToString()
                    }).Distinct().OrderBy(x => x.Text).ToList();
            return user;
        }

        [Authorize]
        [SessionTimeout]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditProject([Bind(Include = "projectid,projectname,startdate,enddate,createdon,modifiedon,createdby,modifiedby,projectcost,noofresource,isactive,projectmembers,projectstatus,projecttype,members,projectmanager")] Project tbl_project)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //Checking for project name existence
                long count = 0;
                if (tbl_project.projectid == null)
                {
                    var gremlinScript = "g.V().has('project','projectname','" + tbl_project.projectname + "').count()";
                    var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    count = objList.ToList()[0];
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

                    if (tbl_project.projectmanager == 0)
                    {
                        tbl_project.projectmanager = Convert.ToInt16(Session["LoggedInUserId"].ToString());
                    }

                    string gremlinScript = $"g.addV('project').property('pk', '{tbl_project.projectname}')" +
                            $".property('projectname', '{tbl_project.projectname}')" +
                            $".property('startdate', '{tbl_project.startdate.ToString("yyyy-MM-dd")}')" +
                            $".property('enddate', '{tbl_project.enddate.ToString("yyyy-MM-dd")}')" +
                            $".property('noofresource', '{tbl_project.noofresource}')" +
                            $".property('projectcost', '{tbl_project.projectcost}')" +
                            $".property('projectstatus', '{tbl_project.projectstatus}')" +
                            $".property('projecttype', '{tbl_project.projecttype}')" + a +
                            $".property('projectmanager', '{tbl_project.projectmanager}')" +
                            $".property('organization', '{Session["Organization"].ToString()}')" +
                            $".property('hashcode', '')" +
                            $".property('progress', '0')" +
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
                    tbl_activity.activityname = ConfigurationManager.AppSettings["StartActivity"];
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
                            $".property('activitystatus', '{1}')" +
                            $".property('progress', '{0}')" +
                            $".property('ismilestone', '{false}')" +
                            $".property('createdby', '{Convert.ToInt32(tbl_activity.createdby)}')" +
                            $".property('linktype','0')" +
                            $".property('createdusername', '')" +
                            $".property('createdon', '{DateTime.Now}')" + a +
                            $".property('type', 'activity')";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Activity Added Successfully";

                    //**************Get Last added activity id***********
                    //gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').has('activity','projectid','" + tbl_activity.projectid + "').project('id').by(values('id'))";
                    //result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //foreach (var result1 in result)
                    //{
                    //    tbl_activity.id = Convert.ToString(result1["id"]);
                    //}
                    tbl_activity.id = IGUtilities.getlastactivityid(tbl_activity.activityname, tbl_activity.projectid);
                    //**************End***********

                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.id}').addE('contains').to(g.V('{tbl_activity.projectid}'))";
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
                            $".property('activitystatus', '{1}')" +
                            $".property('progress', '{0}')" +
                            $".property('ismilestone', '{false}')" +
                            $".property('createdby', '{Convert.ToInt32(tbl_activity.createdby)}')" +
                            $".property('createdusername', '')" +
                            $".property('createdon', '{DateTime.Now}')" + a +
                            $".property('linktype','0')" +
                            $".property('type', 'activity')";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Activity Added Successfully";

                    //**************Get Last added activity id***********
                    //gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').has('activity','projectid','" + tbl_activity.projectid + "').project('id').by(values('id'))";
                    //result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //foreach (var result1 in result)
                    //{
                    //    tbl_activity.id = Convert.ToString(result1["id"]);
                    //}
                    tbl_activity.id = IGUtilities.getlastactivityid(tbl_activity.activityname, tbl_activity.projectid);
                    //**************End***********

                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.id}').addE('contains').to(g.V('{tbl_activity.projectid}'))";
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

                    string gremlinScript = $"g.V('{tbl_project.projectid}').property('projectname', '{tbl_project.projectname}')" +
                                                $".property('projectname', '{tbl_project.projectname}')" +
                                                $".property('startdate', '{tbl_project.startdate.ToString("yyyy-MM-dd")}')" +
                                                $".property('enddate', '{tbl_project.enddate.ToString("yyyy-MM-dd")}')" +
                                                $".property('noofresource', '{tbl_project.noofresource}')" +
                                                $".property('projectcost', '{tbl_project.projectcost}')" +
                                                $".property('projectstatus', '{tbl_project.projectstatus}')" +
                                                $".property('projecttype', '{tbl_project.projecttype}')" + a +
                                                $".property('projectmanager', '{tbl_project.projectmanager}')" +
                                                $".property('updatedon', '{DateTime.Now}')" +
                                                $".property('type', 'project')";

                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Updated Successfully";


                    gremlinScript = $"g.V().has('activity','projectid','{tbl_project.projectid}').count()";
                    results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    if (results.Count == 2)
                    {                        
                        //***********Start activity********
                        gremlinScript = $"g.V().has('activity','projectid','{tbl_project.projectid}').has('activity','activityname','{ConfigurationManager.AppSettings["StartActivity"].ToString()}').project('id').by(id())";
                        results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        foreach (var item in results)
                        {
                            string activityid = item["id"].ToString();
                            gremlinScript = $"g.V('{activityid}').property('startdate', '{tbl_project.startdate.ToString("yyyy-MM-dd")}')" +
                                                    $".property('enddate', '{tbl_project.startdate.ToString("yyyy-MM-dd")}')" +
                                                    $".property('type', 'activity')";

                            var res = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        }
                        //***********End*****************
                        //***********End activity********
                        gremlinScript = $"g.V().has('activity','projectid','{tbl_project.projectid}').has('activity','activityname','{ConfigurationManager.AppSettings["EndActivity"].ToString()}').project('id').by(id())";
                        results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        foreach (var item in results)
                        {
                            string activityid = item["id"].ToString();
                            gremlinScript = $"g.V('{activityid}').property('startdate', '{tbl_project.startdate.ToString("yyyy-MM-dd")}')" +
                                                    $".property('enddate', '{tbl_project.startdate.ToString("yyyy-MM-dd")}')" +
                                                    $".property('type', 'activity')";

                            var res = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        }
                        //***********End*****************                        
                    }
                }
                return RedirectToAction("Projectportfoliodashboard", "Projectportfoliodashboard");
            }

            endloop:
            ViewBag.Message = message;
            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid = Convert.ToInt32(Session["RoleId"]);
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
            List<SelectListItem> manager = IGUtilities.FillPM(pmuserid);
            if (tbl_project.projectmanager != null && manager.Count > 0)
            {
                foreach (var selectedItem in manager)
                {
                    if (selectedItem.Value.ToString() == tbl_project.projectmanager.ToString())
                    {
                        selectedItem.Selected = true;
                    }
                }
            }
            ViewBag.projectmanager = manager;
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
                //Boolean ismilestone = false;
                ActivityController activity = new ActivityController();
                ProjectActivity act = new ProjectActivity();
                act.activityname = task.text;
                act.startdate = task.start_date;
                act.enddate = task.end_date;
                act.projectid = projectid;
                act.durations = task.duration;
                act.ismilestone = task.ismilestone;
                act.createdby = createdby;
                activity.Create(act);
            }
            else
            {
                string gremlinScript = $"g.V('id', '{task.parent}').project('id','type').by(values('id')).by(values('type'))";
                var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                foreach (var item in result)
                {
                    if (item["type"] == "activity")
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
                        projecttask.assignedto = task.user_id.ToString();
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
                        projecttask.assignedto = task.user_id.ToString();
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
            IGUtilities.updateactivitydatesbytype(projectid, task.target);
            IGUtilities.updateprecedingactivitydates(projectid, task.target);
            IGUtilities.updateactivityprojectdate(projectid);
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
                    gremlinScript = "g.V().has('activity','projectid','" + project["id"] + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations','predecessors','linktype','progress','ismilestone').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('predecessors').fold()).by(values('linktype')).by(values('progress')).by(values('ismilestone'))";
                    var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    string pList = JsonConvert.SerializeObject(activitydata);
                    List<ProjectActivity> Activitylist = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);
                    Activitylist = Activitylist.OrderBy(a => a.activityname != ConfigurationManager.AppSettings["StartActivity"].ToString()).ThenBy(a => a.startdate).ThenBy(a => a.enddate).ToList();

                    foreach (var itemactivity in Activitylist)
                    {
                        ganttchart = new GanttTask();
                        ganttchart.GanttTaskId = itemactivity.id;// i;
                        i++;
                        ganttchart.taskid = itemactivity.id;
                        ganttchart.Text = itemactivity.activityname;
                        startdate = Convert.ToDateTime(itemactivity.startdate);
                        enddate = Convert.ToDateTime(itemactivity.enddate);
                        enddate = enddate.AddHours(23);
                        duration = IGUtilities.CalculateDays(startdate.Date, enddate.Date);
                        ganttchart.Duration = duration;
                        ganttchart.StartDate = startdate;
                        ganttchart.EndDate = enddate;
                        ganttchart.SortOrder = 10;
                        ganttchart.Progress = Convert.ToDecimal(Convert.ToDecimal(itemactivity.progress) / 100);
                        ganttchart.Type = "Activity";
                        ganttchart.ismilestone = itemactivity.ismilestone;
                        //ganttchart.color = "blue";
                        ganttchartmodellist.Add(ganttchart);

                        gremlinScript = "g.V().has('task','activityid','" + itemactivity.id + "').project('taskid','taskname','startdate','enddate','durations','activityid','assignedto','progress','taskstatus').by(id()).by(values('taskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('activityid')).by(values('assignedto')).by(values('progress')).by(values('taskstatus'))";
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
                            ganttchart.Progress = Convert.ToDecimal(Convert.ToDecimal(itemtask.progress) / 100);
                            ganttchart.Type = "Task";
                            ganttchart.ParentId = itemactivity.id;
                            ganttchart.user_id = itemtask.assignedto == "" ? 0 : Convert.ToInt16(itemtask.assignedto);
                            if (itemtask.taskstatus == (int)TaskSubTaskStatus.Completed)
                            {
                                ganttchart.color = "#069e5c";
                            }
                            else if (itemtask.taskstatus == (int)TaskSubTaskStatus.Inprogress)
                            {
                                ganttchart.color = "#c7d156";
                            }
                            else
                            {
                                //ganttchart.color = "blue";
                            }
                            ganttchartmodellist.Add(ganttchart);


                            gremlinScript = "g.V().has('subtask','taskid','" + itemtask.taskid + "').project('subtaskid','subtaskname','startdate','enddate','durations','assignedto','progress','taskstatus').by(id()).by(values('subtaskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('assignedto')).by(values('progress')).by(values('taskstatus'))";
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
                                ganttchart.Progress = Convert.ToDecimal(Convert.ToDecimal(itemsubtask.progress) / 100);
                                ganttchart.Type = "SubTask";
                                ganttchart.ParentId = itemtask.taskid;
                                ganttchart.user_id = itemsubtask.assignedto == "" ? 0 : Convert.ToInt16(itemsubtask.assignedto);
                                if (itemsubtask.taskstatus == (int)TaskSubTaskStatus.Completed)
                                {
                                    ganttchart.color = "#069e5c";
                                }
                                else if (itemsubtask.taskstatus == (int)TaskSubTaskStatus.Inprogress)
                                {
                                    ganttchart.color = "#c7d156";
                                }
                                else
                                {
                                    //ganttchart.color = "blue";
                                }
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
                                        ganttlink.Type = itemactivity.linktype == "" ? "0" : itemactivity.linktype;
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
                                ganttlink.Type = itemactivity.linktype == "" ? "0" : itemactivity.linktype;
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
                            taskid = t.taskid,
                            user_id = t.user_id,
                            color = t.color,
                            ismilestone = t.ismilestone
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
            //Boolean ismilestone = false;
            if (task.type == "Activity")
            {
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
                act.ismilestone = task.ismilestone;
                decimal progress = task.progress * 100;
                act.progress = Convert.ToInt16(progress);
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
                act.assignedto = task.user_id.ToString();
                decimal progress = task.progress * 100;
                act.progress = Convert.ToInt16(progress);
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
                act.assignedto = task.user_id.ToString();
                decimal progress = task.progress * 100;
                act.progress = Convert.ToInt16(progress);
                activity.AddEditSubTask(act);
            }
            return GetchartData(projectid);
        }

        public JsonResult DeleteChartTask(GanttTask1 task)
        {
            string projectid = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["projectid"].ToString();
            if (task.type == "Activity")
            {
                deleteactivity(task.taskid);
            }
            else if (task.type == "Task")
            {
                deletetask(task.taskid);
            }
            else if (task.type == "SubTask")
            {
                deletesubtask(task.taskid);
            }
            return null;
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
            var gremlinScript = "g.V().has('project','id','" + id + "').project('id','projectname','startdate','enddate','noofresource','projectcost','createdby','createdusername','createdon','projectmembers','projectstatus','projecttype','projectmanager').by(id()).by(values('projectname')).by(values('startdate')).by(values('enddate')).by(values('noofresource')).by(values('projectcost')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('projectmembers').fold()).by(values('projectstatus')).by(values('projecttype')).by(values('projectmanager'))";
            var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
            return results;
        }

        public JsonResult FillProjectMember(string id)
        {
            List<GanttUser> user = new List<GanttUser>();
            var gremlinScript = "g.V().has('project','id','" + id + "').project('id','projectmembers').by(id()).by(values('projectmembers').fold())";
            var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
            string pList = JsonConvert.SerializeObject(results);
            List<Project> projectlist = JsonConvert.DeserializeObject<List<Project>>(pList);
            foreach (var project in projectlist)
            {
                var projectmembers = project.projectmembers;

                var result = from Q in db.tbl_user
                             where projectmembers.Contains(Q.userid)
                             select Q;
                //var outPutCount = db.tbl_user.Where(x => projectmembers.Contains(x.userid)).Count();
                user = (from b in result
                        join d in db.tbl_user on b.userid equals d.userid
                        join c in db.tbl_userrole on b.roleid equals c.roleid
                        where b.isactive == 1
                        select new GanttUser
                        {
                            label = b.firstname + " " + b.lastname + "-" + c.rolename + "-" + b.organization,
                            key = b.userid.ToString()
                        }).Distinct().OrderBy(x => x.label).ToList();
            }
            var jsonData = new
            {
                // create user array                
                items = (
                        from l in user
                        select new
                        {
                            key = l.key,
                            label = l.label
                        }
                    ).ToArray()
            };
            return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // POST: User/Delete/5
        [HttpPost]
        public ActionResult DeleteProject(string id)
        {
            try
            {
                var gremlinScript = $"g.V().has('activity','projectid','{id}').project('id').by(id())";
                var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                foreach (var result in activitydata)
                {
                    ////task 
                    //gremlinScript = $"g.V().has('task','activityid','{result["id"]}').project('taskid').by(id())";
                    //var res = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //string pList = JsonConvert.SerializeObject(res);
                    //List<ProjectTask> list = JsonConvert.DeserializeObject<List<ProjectTask>>(pList);
                    //foreach (var item in list)
                    //{
                    //    //Subtask 
                    //    gremlinScript = $"g.V().has('subtask','taskid','{item.taskid}').project('subtaskid').by(id())";
                    //    var ressubtask = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //    pList = JsonConvert.SerializeObject(ressubtask);
                    //    List<ProjectSubTask> listsubtask = JsonConvert.DeserializeObject<List<ProjectSubTask>>(pList);
                    //    foreach (var itemsubtask in listsubtask)
                    //    {
                    //        gremlinScript = "g.V().hasId('" + itemsubtask.subtaskid + "').drop()";
                    //        var delresult = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //    }
                    //    gremlinScript = "g.V().hasId('" + item.taskid + "').drop()";
                    //    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    //}
                    //gremlinScript = $"g.V().hasId('{result["id"]}').drop()";
                    //var result1 = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    deleteactivity(result["id"]);
                }
                gremlinScript = "g.V().hasId('" + id + "').drop()";
                var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Content("Project deleted successfully", MediaTypeNames.Text.Plain);
                //return RedirectToAction("Projectportfoliodashboard", "Projectportfoliodashboard");
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content("Project deletion failed", MediaTypeNames.Text.Plain);
            }
        }

        private void deleteactivity(string activityid)
        {
            //Subtask 
            var gremlinScript = $"g.V().has('task','activityid','{activityid}').project('taskid').by(id())";
            var ressubtask = IGUtilities.ExecuteGremlinScript(gremlinScript);
            string pList = JsonConvert.SerializeObject(ressubtask);
            List<ProjectTask> listsubtask = JsonConvert.DeserializeObject<List<ProjectTask>>(pList);
            foreach (var itemtask in listsubtask)
            {
                deletetask(itemtask.taskid);
            }
            gremlinScript = "g.V().hasId('" + activityid + "').drop()";
            var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
        }

        private void deletetask(string taskid)
        {
            //Subtask 
            var gremlinScript = $"g.V().has('subtask','taskid','{taskid}').project('subtaskid').by(id())";
            var ressubtask = IGUtilities.ExecuteGremlinScript(gremlinScript);
            string pList = JsonConvert.SerializeObject(ressubtask);
            List<ProjectSubTask> listsubtask = JsonConvert.DeserializeObject<List<ProjectSubTask>>(pList);
            foreach (var itemsubtask in listsubtask)
            {
                deletesubtask(itemsubtask.subtaskid);
            }
            gremlinScript = "g.V().hasId('" + taskid + "').drop()";
            var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
        }

        private void deletesubtask(string subtaskid)
        {
            var gremlinScript = "g.V().hasId('" + subtaskid + "').drop()";
            var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
        }

        public void XMLImport()
        {
            var postedFile = Request.Files[0];
            System.Xml.XmlDocument xdc = new System.Xml.XmlDocument();

            string filePath = string.Empty;
            string path = Server.MapPath("~/Uploads/");
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            filePath = path + Path.GetFileName(postedFile.FileName);
            string extension = Path.GetExtension(postedFile.FileName);
            postedFile.SaveAs(filePath);

            xdc.Load(filePath);
            System.Xml.XmlNodeList xnlNodes = xdc.LastChild.SelectNodes("/Tasks");
        }
    }
}
