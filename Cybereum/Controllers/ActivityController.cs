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
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cybereum.Controllers
{
    public class ActivityController : Controller
    {
        private cybereumEntities db = new cybereumEntities();

        //private const string hostname = "gremtest1.gremlin.cosmos.azure.com";
        //private const int port = 443;
        //private const string authKey = "lja6Gkeuf5nsnEg9TYyC79N1fvt4v1ZBb9JwkbWPNiNC1tEeBOSVu8vBHQZeKnSFguIKz9ziKjVEiPAjRAuf3w==";
        //private const string database = "graphdb";
        //private const string collection = "ProjectGraph";

        //ConnectionPoolSettings connectionPoolSettings = new ConnectionPoolSettings()
        //{
        //    MaxInProcessPerConnection = 10,
        //    PoolSize = 4,
        //    ReconnectionAttempts = 3,
        //    ReconnectionBaseDelay = TimeSpan.FromMilliseconds(100)
        //};

        //string containerLink = "/dbs/" + database + "/colls/" + collection;

        // GET: Activity
        [Authorize]
        [SessionTimeout]
        public ActionResult Index(string projectid)
        {
            TempData["ProjectId"] = projectid;
            ViewBag.projectid = TempData["ProjectId"];
            TempData.Keep();
            if (projectid == string.Empty)
            {
                ViewBag.projectid = Convert.ToInt32(Session["ProjectId"]);
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
            ViewBag.Message = "Edit Activity";
            //ProjectActivity activity = new ProjectActivity();

            var activity = getactivitybyid(Id);
            return RedirectToAction("Create", activity.Result);
        }


        public async Task<JsonResult> activitylist(string projectid)
        {
            List<ProjectActivity> list = new List<ProjectActivity>();
            try
            {
                //var gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + HttpUtility.UrlEncode(database) + "/colls/" + HttpUtility.UrlEncode(collection), password: authKey);
                //using (var gremlinClient = new GremlinClient(
                //    gremlinServer,
                //    new GraphSON2Reader(),
                //    new GraphSON2Writer(),
                //    GremlinClient.GraphSON2MimeType,
                //    connectionPoolSettings))
                //{
                var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').project('id','activityname','startdate','enddate','durations','ismilestone','createdby','createdusername','createdon','projectid')" +
                                    ".by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('ismilestone')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('projectid'))";
                try
                {
                    //var results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    string pList = JsonConvert.SerializeObject(results);
                    List<ProjectActivity> people = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);

                    list = people;

                    //foreach (var result in results)
                    //{
                    //    ProjectActivity activity = new ProjectActivity();
                    //    activity.id = result["id"].ToString();
                    //    activity.activityname = result["activityname"].ToString();
                    //    activity.startdate = Convert.ToDateTime(result["startdate"]);
                    //    activity.enddate = Convert.ToDateTime(result["enddate"]);
                    //    activity.ismilestone = Convert.ToBoolean(result["ismilestone"]);
                    //    activity.durations = Convert.ToInt64(result["durations"]);
                    //    activity.projectid = result["projectid"].ToString();
                    //    activity.createdby = result["createdby"].ToString();

                    //    int userid = Convert.ToInt32(result["createdby"]);
                    //    var username = db.tbl_user.Where(x => x.userid == userid).FirstOrDefault();

                    //    if (username != null)
                    //    {
                    //        activity.createdusername = username.firstname + ' ' + username.lastname;
                    //    }

                    //    gremlinScript = "g.V().has('project','id','" + projectid + "').project('projectname').by(values('projectname'))";
                    //    var results1 = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                    //    foreach (var result1 in results1)
                    //    {
                    //        activity.projectname = result1["projectname"];
                    //    }

                    //    activity.createdon = Convert.ToDateTime(result["createdon"]);

                    //    list.Add(activity);
                    //}
                    //var projectresult = this.Json(new { data = list, recordsTotal = list.Count(), recordsFiltered = list.Count() }, JsonRequestBehavior.AllowGet);
                    var projectresult = this.Json(new { data = list, recordsTotal = list.Count(), recordsFiltered = list.Count() }, JsonRequestBehavior.AllowGet);
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

        public async Task<ProjectActivity> getactivitybyid(string id)
        {
            ProjectActivity activity = new ProjectActivity();
            try
            {
                //var gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + HttpUtility.UrlEncode(database) + "/colls/" + HttpUtility.UrlEncode(collection), password: authKey);
                //using (var gremlinClient = new GremlinClient(
                //    gremlinServer,
                //    new GraphSON2Reader(),
                //    new GraphSON2Writer(),
                //    GremlinClient.GraphSON2MimeType,
                //    connectionPoolSettings))
                //{
                var gremlinScript = "g.V().has('activity','id','" + id + "').project('id','activityname','startdate','enddate','durations','ismilestone','createdby','createdusername','createdon','projectid','predecessors').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('ismilestone')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('projectid')).by(values('predecessors').fold())";
                //var gremlinScript = "g.V().has('activity','id','" + id + "').project('id','activityname','startdate','enddate','durations','ismilestone','createdby','createdusername','createdon','projectid').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('ismilestone')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('projectid'))";
                try
                {
<<<<<<< Updated upstream
                    var gremlinScript = "g.V().has('activity','id','" + id + "').project('id','activityname','startdate','enddate','durations','ismilestone','createdby','createdusername','createdon','projectid','predecessors').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('ismilestone')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('projectid')).by(values('predecessors'))";
                    //var gremlinScript = "g.V().has('activity','id','" + id + "').project('id','activityname','startdate','enddate','durations','ismilestone','createdby','createdusername','createdon','projectid').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('ismilestone')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('projectid'))";
                    try
=======
                    //var results = await gremlinClient.SubmitAsync<dynamic>(gremlinScript).ConfigureAwait(false);
                    //var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //task.Wait();
                    //var results = task.Result;
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result in results)
>>>>>>> Stashed changes
                    {
                        activity.id = result["id"].ToString();
                        activity.projectid = result["projectid"].ToString();
                        activity.activityname = result["activityname"].ToString();
                        activity.startdate = Convert.ToDateTime(result["startdate"]);
                        activity.enddate = Convert.ToDateTime(result["enddate"]);
                        activity.ismilestone = Convert.ToBoolean(result["ismilestone"]);
                        activity.durations = Convert.ToInt64(result["durations"]);

                        var predecessors = result["predecessors"];
                        var stringlist = JsonConvert.SerializeObject(predecessors);
                        var jArray = JArray.Parse(stringlist);
                        string tasks = string.Empty;
                        foreach (string item in jArray)
                        {
<<<<<<< Updated upstream
                            activity.id = result["id"].ToString();
                            activity.projectid = result["projectid"].ToString();
                            activity.activityname = result["activityname"].ToString();
                            activity.startdate = Convert.ToDateTime(result["startdate"]);
                            activity.enddate = Convert.ToDateTime(result["enddate"]);
                            activity.ismilestone = Convert.ToBoolean(result["ismilestone"]);
                            activity.durations = Convert.ToInt64(result["durations"]);

                            //var predecessors = result["predecessors"];
                            //var stringlist = JsonConvert.SerializeObject(predecessors);
                            //var jArray = JArray.Parse(stringlist);
                            //string tasks = string.Empty;
                            //foreach (string item in jArray)
                            //{
                            //    tasks = tasks + item + ",";
                            //}
                            //if (tasks != "") tasks = tasks.Remove(tasks.LastIndexOf(",")).ToString();
                            //if (tasks.ToString() != string.Empty)
                            //{
                            //    string[] ints = tasks.Split(',').ToArray();
                            //    activity.Predecessors = ints;
                            //}
                            activity.Predecessors = result["predecessors"].ToString();

                            activity.createdby = result["createdby"].ToString();
                            activity.createdusername = result["createdusername"].ToString();
                            activity.createdon = Convert.ToDateTime(result["createdon"]);
=======
                            tasks = tasks + item + ",";
>>>>>>> Stashed changes
                        }
                        if (tasks != "") tasks = tasks.Remove(tasks.LastIndexOf(",")).ToString();
                        if (tasks.ToString() != string.Empty)
                        {
                            string[] ints = tasks.Split(',').ToArray();
                            activity.Predecessors = ints;
                        }
                        //activity.Predecessors = result["predecessors"].ToString();

                        activity.createdby = result["createdby"].ToString();
                        activity.createdusername = result["createdusername"].ToString();
                        activity.createdon = Convert.ToDateTime(result["createdon"]);
                    }
                    return activity;
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

        [Authorize]
        [SessionTimeout]
        //[Route("Activity/Create/{projectid}/{activityid}")]
        public ActionResult Create(string activityid, string projectid, ProjectActivity Activity)
        {
<<<<<<< Updated upstream
            var result1 = getactivitybyid(Activity.id);
            Activity = result1.Result;
=======
            if (Activity.id != null)
            {
                var result1 = getactivitybyid(Activity.id);
                Activity = result1.Result;
            }
>>>>>>> Stashed changes

            ViewBag.HasDate = false;
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
            //if (activityid == "null") activityid = null;
            if (activityid != null)
<<<<<<< Updated upstream
            {
                var gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + HttpUtility.UrlEncode(database) + "/colls/" + HttpUtility.UrlEncode(collection), password: authKey);
                var gremlinScript = "g.V().has('activity','id','" + activityid + "').project('startdate','enddate').by(values('startdate')).by(values('enddate'))";                
                using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                {
                    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    task.Wait();
                    var result = task.Result;
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            ViewBag.HasDate = false;
                            DateTime dt1 = Convert.ToDateTime(item["enddate"]);
                            //dt1=dt1.AddDays(1);
                            Activity.startdate = dt1;
                            Activity.enddate = dt1;
                        }
                    }
                    else
                    {
                        gremlinScript = "g.V().has('project','id','" + projectid + "').project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
                        gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + database + "/colls/" + collection, password: authKey);
                        using (var gremlinClient1 = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                        {
                            var task1 = gremlinClient1.SubmitAsync<dynamic>(gremlinScript);
                            task.Wait();
                            var resultProject = task.Result;
                            if (resultProject.Count > 0)
                            {
                                foreach (var item in resultProject)
                                {
                                    ViewBag.HasDate = false;
                                    DateTime dt1 = Convert.ToDateTime(item["startdate"]);
                                    dt1=dt1.AddDays(1);
                                    Activity.startdate = dt1;
                                    Activity.enddate = dt1;
                                }
                            }
                            else
                            {
                                Activity.startdate = DateTime.Today;
                                Activity.enddate = DateTime.Today;
                            }
                        }
                    }
                }
            }
            else if (Activity.id == null)
=======
>>>>>>> Stashed changes
            {
                //var gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + HttpUtility.UrlEncode(database) + "/colls/" + HttpUtility.UrlEncode(collection), password: authKey);
                var gremlinScript = "g.V().has('activity','id','" + activityid + "').project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
                //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                //{
                //var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                //task.Wait();
                //var result = task.Result;
                var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        ViewBag.HasDate = false;
                        DateTime dt1 = Convert.ToDateTime(item["enddate"]);
                        dt1=dt1.AddDays(1);
                        Activity.startdate = dt1;
                        Activity.enddate = dt1;
                    }
                }
                else
                {
                    gremlinScript = "g.V().has('project','id','" + projectid + "').project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
                    //gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + database + "/colls/" + collection, password: authKey);
                    //using (var gremlinClient1 = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //var task1 = gremlinClient1.SubmitAsync<dynamic>(gremlinScript);
                    //task.Wait();
                    //var resultProject = task.Result;
                    var resultProject = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    if (resultProject.Count > 0)
                    {
                        foreach (var item in resultProject)
                        {
                            ViewBag.HasDate = false;
<<<<<<< Updated upstream
                            DateTime dt1 = Convert.ToDateTime(item["enddate"]);
                            dt1=dt1.AddDays(1);
=======
                            DateTime dt1 = Convert.ToDateTime(item["startdate"]);
                            //dt1 = dt1.AddDays(1);
>>>>>>> Stashed changes
                            Activity.startdate = dt1;
                            Activity.enddate = dt1;
                        }
                    }
                    else
                    {
                        Activity.startdate = DateTime.Today;
                        Activity.enddate = DateTime.Today;
                    }
                    //}
                }
                //}
            }
            else if (Activity.id == null)
            {
                var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').order().by('enddate',decr).limit(1).project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
                //var gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + database + "/colls/" + collection, password: authKey);
                //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                //{
                //var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                //task.Wait();
                //var result = task.Result;
                var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        ViewBag.HasDate = false;
                        DateTime dt1 = Convert.ToDateTime(item["enddate"]);
                        dt1 = dt1.AddDays(1);
                        Activity.startdate = dt1;
                        Activity.enddate = dt1;
                    }
                }
                else
                {
                    Activity.startdate = DateTime.Today;
                    Activity.enddate = DateTime.Today;
                }
                //}
            }

            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid = Convert.ToInt32(Session["RoleId"]);
            List<SelectListItem> user = Filluser(pmuserid, roleid);
            ViewBag.createdby = user;

            List<SelectListItem> predecessors = FillActivity(projectid, activityid, Activity);
            ViewBag.predecessors = predecessors;
<<<<<<< Updated upstream
            //if (activityid != null)
            //{
            //    Activity.Predecessors = activityid.ToString();
            //}
            return View(Activity);
        }

=======
            return View(Activity);
        }

        public JsonResult GetEnddate(DateTime startDate, int id)
        {
            var record = IGUtilities.CalculateDays(startDate, id);
            return Json(record, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPredecesdate(string[] id)
        {
            DateTime lastactivityenddate = DateTime.Now;
            string gremlinScript = string.Empty;
            if (id != null)
            {
                for (int i = 0; i <= id.Length - 1; i++)
                {
                    gremlinScript = "g.V().has('activity','id','" + id[i] + "').project('enddate').by(values('enddate'))";

                    var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    if (result.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            DateTime dt1 = Convert.ToDateTime(item["enddate"]);
                            dt1 = dt1.AddDays(1);
                            if (i == 0)
                            {
                                lastactivityenddate = dt1;
                            }
                            if (dt1 > lastactivityenddate)
                                lastactivityenddate = dt1;
                        }

                    }
                }
            }
            return Json(lastactivityenddate, JsonRequestBehavior.AllowGet);
        }

>>>>>>> Stashed changes
        public List<SelectListItem> FillActivity(string projectid, string activityid, ProjectActivity Activity)
        {
            List<SelectListItem> predecessors = new List<SelectListItem>();

<<<<<<< Updated upstream
            var gremlinScript1 = "g.V().has('activity','projectid','" + projectid + "').order().by('startdate',incr).project('id','activityname').by(values('id')).by(values('activityname'))";
            var gremlinServer1 = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + database + "/colls/" + collection, password: authKey);
            using (var gremlinClient = new GremlinClient(gremlinServer1, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
            {
                var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript1);
                task.Wait();
                var result = task.Result;
                string pList = JsonConvert.SerializeObject(result);
                List<ProjectActivity> projectlist = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);

                predecessors = (from b in projectlist
                                select new SelectListItem
                                {
                                    Text = b.activityname,
                                    Value = b.id.ToString()
                                }).ToList();
                predecessors.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });

            }
=======
            var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname').by(values('id')).by(values('activityname'))";
            //var gremlinServer1 = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + database + "/colls/" + collection, password: authKey);
            //using (var gremlinClient = new GremlinClient(gremlinServer1, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
            //{
            //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
            //    task.Wait();
            //    var result = task.Result;
            //    string pList = JsonConvert.SerializeObject(result);
            //    List<ProjectActivity> projectlist = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);

            //    predecessors = (from b in projectlist
            //                    select new SelectListItem
            //                    {
            //                        Text = b.activityname,
            //                        Value = b.id.ToString()
            //                    }).ToList();
            //    predecessors.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });

            //}
            var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
            string pList = JsonConvert.SerializeObject(result);
            List<ProjectActivity> projectlist = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);

            predecessors = (from b in projectlist
                            select new SelectListItem
                            {
                                Text = b.activityname,
                                Value = b.id.ToString()
                            }).ToList();
            //predecessors.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });

>>>>>>> Stashed changes

            if (Activity.Predecessors != null && predecessors.Count > 0)
            {
                foreach (var selectedItem in predecessors)
                {
                    foreach (var item in Activity.Predecessors)
                    {
                        if (selectedItem.Value.ToString() == item.ToString())
                        {
                            selectedItem.Selected = true;
                        }
                    }
                }
            }
            if (activityid != null && predecessors.Count > 0)
            {
                foreach (var selectedItem in predecessors)
                {
                    if (selectedItem.Value.ToString() == activityid.ToString())
                    {
                        selectedItem.Selected = true;
                    }
                }
            }
            return predecessors;
        }

        public List<SelectListItem> Filluser(int? pmuserid, int? roleid)
        {
            List<SelectListItem> user = new List<SelectListItem>();
            if (roleid == 1)
            {
                user = (from b in db.tbl_user
                        where b.roleid == 2 && b.isactive == 1
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
        public ActionResult Create([Bind(Include = "id,activityname,ismilestone,startdate,enddate,durations,predecessors,createdby,createddate,modifiedby,modifieddate,projectid,Predecessors")] ProjectActivity tbl_activity)
        {
            string message = string.Empty;

<<<<<<< Updated upstream
            //string predecessors = string.Empty;
            //if (tbl_activity.Predecessors != null)
            //{
            //    predecessors = String.Join(",", tbl_activity.Predecessors);
            //}
=======
            string predecessors = string.Empty;
            if (tbl_activity.Predecessors != null)
            {
                predecessors = String.Join(",", tbl_activity.Predecessors);
            }
>>>>>>> Stashed changes

            if (ModelState.IsValid)
            {
                //long duration = 0;
                //if (DateTime.Now.Date < tbl_activity.startdate.Date)
                //{
                //    duration = 0;
                //}
                //else if (DateTime.Now.Date > tbl_activity.enddate.Date)
                //{
                //    duration = 100;
                //}
                //else
                //{
                //    double dt1 = (DateTime.Now.Date - tbl_activity.startdate.Date).TotalDays;
                //    double dt2 = (tbl_activity.enddate.Date - tbl_activity.startdate.Date).TotalDays;
                //    if (dt2 != 0)
                //        duration = Convert.ToInt64((dt1 / dt2) * 100);
                //}
<<<<<<< Updated upstream
                int duration = IGUtilities.CalculateDays(tbl_activity.startdate, tbl_activity.enddate) * 8;

                string a = "";
                a = $".property('predecessors', '{tbl_activity.Predecessors}')";
=======
                int duration = IGUtilities.CalculateDays(tbl_activity.startdate, tbl_activity.enddate);

                string a = "";
                //a = $".property('predecessors', '{tbl_activity.Predecessors}')";
>>>>>>> Stashed changes

                //var gremlinServer = new GremlinServer(hostname, port, enableSsl: true, username: "/dbs/" + database + "/colls/" + collection, password: authKey);
                long count = 0;
                if (tbl_activity.id == null)
                {
<<<<<<< Updated upstream
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').has('activity','projectid','" + tbl_activity.projectid + "').count()";
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var objList = task.Result;
                        count = objList.ToList()[0];
                    }
=======
                    //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //    var gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').has('activity','projectid','" + tbl_activity.projectid + "').count()";
                    //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //    task.Wait();
                    //    var objList = task.Result;
                    //    count = objList.ToList()[0];
                    //}
                    var gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').has('activity','projectid','" + tbl_activity.projectid + "').count()";
                    var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    count = objList.ToList()[0];
>>>>>>> Stashed changes
                    if (count > 0)
                    {
                        message = "Activity name already exists.";
                        goto endloop;
                    }
                }
                else
                {
                    //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //    var gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').has('activity','projectid','" + tbl_activity.projectid + "')";
                    //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //    task.Wait();
                    //    var objList = task.Result;
                    //    foreach (var result in objList)
                    //    {
                    //        if (result["id"] != tbl_activity.id)
                    //        {
                    //            message = "Activity name already exists.";
                    //            goto endloop;
                    //        }
                    //    }
                    //}
                    var gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').has('activity','projectid','" + tbl_activity.projectid + "')";
                    var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result in objList)
                    {
                        if (result["id"] != tbl_activity.id)
                        {
                            message = "Activity name already exists.";
                            goto endloop;
                        }
                    }
                }

                if (tbl_activity.id == null)
                {
<<<<<<< Updated upstream
                    //if (tbl_activity.Predecessors != null)
                    //{
                    //    for (int i = 0; i < tbl_activity.Predecessors.Length; i++)
                    //    {
                    //        a = a + $".property(list,'predecessors', '{tbl_activity.Predecessors[i]}') ";
                    //    }
                    //}
                    //else
                    //{
                    //    a = a + $".property(list,'predecessors', '')";
                    //}

                    if (tbl_activity.createdby == null)
                        tbl_activity.createdby = Session["LoggedInUserId"].ToString();
                    
=======
                    if (tbl_activity.Predecessors != null)
                    {
                        for (int i = 0; i < tbl_activity.Predecessors.Length; i++)
                        {
                            if (tbl_activity.Predecessors[i] != "" && tbl_activity.Predecessors[i] != "System.String[]")
                            {
                                a = a + $".property(list,'predecessors', '{tbl_activity.Predecessors[i]}') ";
                            }
                        }
                    }
                    else
                    {
                        a = a + $".property(list,'predecessors', '')";
                    }

                    if (tbl_activity.createdby == null)
                        tbl_activity.createdby = Session["LoggedInUserId"].ToString();

>>>>>>> Stashed changes
                    string gremlinScript = $"g.addV('activity').property('pk', '{tbl_activity.activityname}')" +
                            $".property('activityname', '{tbl_activity.activityname}')" +
                            $".property('startdate', '{tbl_activity.startdate.ToString("yyyy-MM-dd")}')" +
                            $".property('enddate', '{tbl_activity.enddate.ToString("yyyy-MM-dd")}')" +
                            $".property('projectid', '{tbl_activity.projectid}')" +
                            $".property('durations', '{duration}')" +
                            $".property('ismilestone', '{tbl_activity.ismilestone}')" +
                            $".property('createdby', '{Convert.ToInt32(tbl_activity.createdby)}')" +
                            $".property('createdusername', '')" +
                            $".property('createdon', '{DateTime.Now}')" + a +
                            $".property('type', 'activity')";

                    // Execute the Gremlin script                    
                    //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //    task.Wait();
                    //    var result = task.Result;
                    //    message = "Added Successfully";
                    //}                    
                    var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Added Successfully";

                    gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').project('id').by(values('id'))";
                    //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //    task.Wait();
                    //    var result = task.Result;
                    //    foreach (var result1 in result)
                    //    {
                    //        tbl_activity.id = Convert.ToString(result1["id"]);
                    //    }
                    //}
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result1 in result)
                    {
                        tbl_activity.id = Convert.ToString(result1["id"]);
                    }

                    //Remove connection the project to activity
                    gremlinScript = $"\ng.V().has('activity', 'id', '{tbl_activity.id}').bothE().drop()";
                    // Execute the Gremlin script
                    //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //    task.Wait();
                    //    var result = task.Result;
                    //    message = "Gremlin script executed successfully";
                    //}
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";

                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('contains').to(g.V('{tbl_activity.id}'))";
                    // Execute the Gremlin script
                    //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //    task.Wait();
                    //    var result = task.Result;
                    //    message = "Gremlin script executed successfully";
                    //}
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    //Connect the predeccesors to succesors
                    //foreach (var predecessor in tbl_activity.Predecessors)
                    //{
                    if (tbl_activity.Predecessors != null)
                    {
<<<<<<< Updated upstream
                        //for (int i = 0; i < tbl_activity.Predecessors.Length; i++)
                        //{
                        gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('precedes').property('duration', '{tbl_activity.durations}').to(g.V('{tbl_activity.Predecessors}'))";
                        using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                        {
                            var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                            task.Wait();
                            var result = task.Result;
                            message = "Gremlin script executed successfully";
                        }
                        //}
=======
                        for (int i = 0; i < tbl_activity.Predecessors.Length; i++)
                        {
                            if (tbl_activity.Predecessors[i] != "" && tbl_activity.Predecessors[i] != "System.String[]")
                            {
                                gremlinScript = $"\ng.V('{tbl_activity.id}').addE('precedes').property('duration', '{tbl_activity.durations}').to(g.V('{tbl_activity.Predecessors[i]}'))";
                                //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                                //{
                                //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                                //    task.Wait();
                                //    var result = task.Result;
                                //    message = "Gremlin script executed successfully";
                                //}
                                result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                            }
                        }
>>>>>>> Stashed changes
                    }
                    //}
                }
                else
                {
<<<<<<< Updated upstream
                    //string gremlinscript = $"g.V().has('activity', 'id','{ tbl_activity.id }').properties('predecessors').drop()";
=======
                    string gremlinscript = $"g.V().has('activity', 'id','{ tbl_activity.id }').properties('predecessors').drop()";
                    var result = IGUtilities.ExecuteGremlinScript(gremlinscript);
>>>>>>> Stashed changes
                    //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinscript);
                    //    task.Wait();
                    //    var result = task.Result;
                    //    message = "Updated Successfully";
                    //}

                    //string a = "";
<<<<<<< Updated upstream
                    //if (tbl_activity.Predecessors != null)
                    //{
                    //    for (int i = 0; i < tbl_activity.Predecessors.Length; i++)
                    //    {
                    //        a = a + $".property(list,'predecessors', '{tbl_activity.Predecessors[i]}') ";
                    //    }
                    //}
                    //else
                    //{
                    //    a = a + $".property(list,'predecessors', '') ";
                    //}
=======
                    if (tbl_activity.Predecessors != null)
                    {
                        for (int i = 0; i < tbl_activity.Predecessors.Length; i++)
                        {
                            if (tbl_activity.Predecessors[i] != "" && tbl_activity.Predecessors[i] != "System.String[]")
                            {
                                a = a + $".property(list,'predecessors', '{tbl_activity.Predecessors[i]}') ";
                            }
                        }
                    }
                    else
                    {
                        a = a + $".property(list,'predecessors', '') ";
                    }
>>>>>>> Stashed changes

                    string gremlinScript = $"g.V('{tbl_activity.id}')" +
                                            $".property('activityname', '{tbl_activity.activityname}')" +
                                            $".property('startdate', '{tbl_activity.startdate.ToString("yyyy-MM-dd")}')" +
                                            $".property('enddate', '{tbl_activity.enddate.ToString("yyyy-MM-dd")}')" +
                                            $".property('projectid', '{tbl_activity.projectid}')" +
                                            $".property('durations', '{duration}')" +
                                            $".property('ismilestone', '{tbl_activity.ismilestone}')" + a +
                                            //$".property('createdby', '{Convert.ToInt32(tbl_activity.createdby)}')" +
                                            //$".property('createdusername', '')" +
                                            $".property('updatedon', '{DateTime.Now}')" +
                                            $".property('type', 'activity')";

                    // Execute the Gremlin script                    
                    //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //    task.Wait();
                    //    var result = task.Result;
                    //    message = "Updated Successfully";
                    //}
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    //Remove connection the task to subtask
                    gremlinScript = $"\ng.V().has('activity', 'id', '{tbl_activity.id}').bothE().drop()";
                    // Execute the Gremlin script
                    //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //    task.Wait();
                    //    var result = task.Result;
                    //    message = "Gremlin script executed successfully";
                    //}
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    //Remove connection the task to subtask
                    gremlinScript = $"\ng.V().has('activity', 'id', '{tbl_activity.id}').bothE().drop()";
                    // Execute the Gremlin script
                    using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    {
                        var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                        task.Wait();
                        var result = task.Result;
                        message = "Gremlin script executed successfully";
                    }

                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('contains').to(g.V('{tbl_activity.id}'))";
                    //gremlinScript = $"\ng.V().has('project','projectid','{tbl_activity.projectid}').as('a').V().has('activity','activityname','{tbl_activity.activityname}').addE('projectactivity').to('a')";
                    // Execute the Gremlin script
                    //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                    //{
                    //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                    //    task.Wait();
                    //    var result = task.Result;
                    //    message = "Gremlin script executed successfully";
                    //}
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    //Connect the predeccesors to succesors
                    //foreach (var predecessor in tbl_activity.Predecessors)
                    //{
                    if (tbl_activity.Predecessors != null)
                    {
<<<<<<< Updated upstream
                        //    for (int i = 0; i < tbl_activity.Predecessors.Length; i++)
                        //    {
                        gremlinScript = $"\ng.V('{tbl_activity.id}').addE('precedes').property('duration', '{duration}').to(g.V('{tbl_activity.Predecessors}'))";
                        using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                        {
                            var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                            task.Wait();
                            var result = task.Result;
                            message = "Gremlin script executed successfully";
                        }
                        //    }
=======
                        for (int i = 0; i < tbl_activity.Predecessors.Length; i++)
                        {
                            if (tbl_activity.Predecessors[i] != "" && tbl_activity.Predecessors[i] != "System.String[]")
                            {
                                gremlinScript = $"\ng.V('{tbl_activity.id}').addE('precedes').property('duration', '{duration}').to(g.V('{tbl_activity.Predecessors[i]}'))";
                                //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
                                //{
                                //    var task = gremlinClient.SubmitAsync<dynamic>(gremlinScript);
                                //    task.Wait();
                                //    var result = task.Result;
                                //    message = "Gremlin script executed successfully";
                                //}
                                result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                            }
                        }
>>>>>>> Stashed changes
                    }
                    //}
                }
                return RedirectToAction("Index", new { projectid = tbl_activity.projectid });
            }
            endloop:
            ViewBag.Message = message;
            ViewBag.Projectid = tbl_activity.projectid;
            int pmuserid1 = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid1 = Convert.ToInt32(Session["RoleId"]);
            if (tbl_activity.id == null)
            {
                return RedirectToAction("Create", new { projectid = tbl_activity.projectid });
            }
            return View(tbl_activity);
            //return RedirectToAction("Create", tbl_activity);
        }

        // GET: Milestone/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_milestone tbl_milestone = db.tbl_milestone.Find(id);
            if (tbl_milestone == null)
            {
                return HttpNotFound();
            }
            return View(tbl_milestone);
        }

        // POST: Milestone/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_milestone tbl_milestone = db.tbl_milestone.Find(id);
            db.tbl_milestone.Remove(tbl_milestone);
            db.SaveChanges();
            return RedirectToAction("Index");
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
