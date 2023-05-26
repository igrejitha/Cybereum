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
using System.Configuration;

namespace Cybereum.Controllers
{
    public class ActivityController : Controller
    {
        private cybereumEntities db = new cybereumEntities();

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
                var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').project('id','activityname','startdate','enddate','durations','ismilestone','createdby','createdusername','createdon','projectid')" +
                                    ".by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('ismilestone')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('projectid'))";
                try
                {
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    string pList = JsonConvert.SerializeObject(results);
                    List<ProjectActivity> people = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);

                    list = people;
                    
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
                var gremlinScript = "g.V().has('activity','id','" + id + "').project('id','activityname','startdate','enddate','durations','ismilestone','createdby','createdusername','createdon','projectid','predecessors').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('ismilestone')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('projectid')).by(values('predecessors').fold())";                
                try
                {
                    var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result in results)
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
                            tasks = tasks + item + ",";
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
            if (Activity.id != null)
            {
                var result1 = getactivitybyid(Activity.id);
                Activity = result1.Result;
            }

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
            
            if (activityid != null)
            {
                var gremlinScript = "g.V().has('activity','id','" + activityid + "').project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
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
                    gremlinScript = "g.V().has('project','id','" + projectid + "').project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
                    var resultProject = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    if (resultProject.Count > 0)
                    {
                        foreach (var item in resultProject)
                        {
                            ViewBag.HasDate = false;
                            DateTime dt1 = Convert.ToDateTime(item["startdate"]);
                            //dt1 = dt1.AddDays(1);
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
            else if (Activity.id == null)
            {
                var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').order().by('enddate',decr).limit(1).project('startdate','enddate').by(values('startdate')).by(values('enddate'))";
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
            }

            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid = Convert.ToInt32(Session["RoleId"]);
            List<SelectListItem> user = Filluser(pmuserid, roleid);
            ViewBag.createdby = user;

            List<SelectListItem> predecessors = FillActivity(projectid, activityid, Activity);
            ViewBag.predecessors = predecessors;
            
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

        public List<SelectListItem> FillActivity(string projectid, string activityid, ProjectActivity Activity)
        {
            List<SelectListItem> predecessors = new List<SelectListItem>();

            var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname').by(values('id')).by(values('activityname'))";

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

            if (predecessors != null)
            {
                predecessors.Remove(predecessors.Find(m => m.Text == ConfigurationManager.AppSettings["EndActivity"]));
                if (predecessors.Count > 1)
                {
                    var item = predecessors.Find(m => m.Text == ConfigurationManager.AppSettings["StartActivity"]);
                    if (item != null)
                    {
                        if (item.Selected == false)
                        {
                            predecessors.Remove(predecessors.Find(m => m.Text == ConfigurationManager.AppSettings["StartActivity"]));
                        }
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

            string predecessors = string.Empty;
            if (tbl_activity.Predecessors != null)
            {
                predecessors = String.Join(",", tbl_activity.Predecessors);
            }

            if (ModelState.IsValid)
            {                
                int duration = Convert.ToInt16(tbl_activity.durations);
                tbl_activity.enddate = IGUtilities.CalculateDays(tbl_activity.startdate, duration);

                string a = "";
                long count = 0;
                if (tbl_activity.id == null)
                {                    
                    var gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').has('activity','projectid','" + tbl_activity.projectid + "').count()";
                    var objList = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    count = objList.ToList()[0];
                    if (count > 0)
                    {
                        message = "Activity name already exists.";
                        goto endloop;
                    }
                }
                else
                {                    
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

                                    
                    var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Added Successfully";

                    gremlinScript = "g.V().has('activity','activityname','" + tbl_activity.activityname + "').project('id').by(values('id'))";
                    
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var result2 in result)
                    {
                        tbl_activity.id = Convert.ToString(result2["id"]);
                    }

                    //Remove connection the project to activity
                    gremlinScript = $"\ng.V().has('activity', 'id', '{tbl_activity.id}').bothE().drop()";                    
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    message = "Gremlin script executed successfully";

                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('contains').to(g.V('{tbl_activity.id}'))";                    
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    //Connect the predeccesors to succesors                    
                    if (tbl_activity.Predecessors != null)
                    {
                        for (int i = 0; i < tbl_activity.Predecessors.Length; i++)
                        {
                            if (tbl_activity.Predecessors[i] != "" && tbl_activity.Predecessors[i] != "System.String[]")
                            {
                                gremlinScript = $"\ng.V('{tbl_activity.id}').addE('precedes').property('duration', '{tbl_activity.durations}').to(g.V('{tbl_activity.Predecessors[i]}'))";
                                result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                            }
                        }
                    }
                   
                }
                else
                {
                    string gremlinscript = $"g.V().has('activity', 'id','{ tbl_activity.id }').properties('predecessors').drop()";
                    var result = IGUtilities.ExecuteGremlinScript(gremlinscript);
                    
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

                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    //Remove connection the task to subtask
                    gremlinScript = $"\ng.V().has('activity', 'id', '{tbl_activity.id}').bothE().drop()";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    //connect the project to activity
                    gremlinScript = $"\ng.V('{tbl_activity.projectid}').addE('contains').to(g.V('{tbl_activity.id}'))";
                    result = IGUtilities.ExecuteGremlinScript(gremlinScript);

                    //Connect the predeccesors to succesors
                    //foreach (var predecessor in tbl_activity.Predecessors)
                    //{
                    if (tbl_activity.Predecessors != null)
                    {
                        for (int i = 0; i < tbl_activity.Predecessors.Length; i++)
                        {
                            if (tbl_activity.Predecessors[i] != "" && tbl_activity.Predecessors[i] != "System.String[]")
                            {
                                gremlinScript = $"\ng.V('{tbl_activity.id}').addE('precedes').property('duration', '{duration}').to(g.V('{tbl_activity.Predecessors[i]}'))";
                                result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                            }
                        }
                    }
                    //}
                }

                //****************************Update End Activity date****************************
                string gremlinScript1 = $"g.V().has('activity','projectid','{tbl_activity.projectid}').order().by('enddate',decr).project('startdate','enddate').by(values('startdate')).by(values('enddate')).limit(1)";
                var result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                DateTime dt1=DateTime.Now;
                if (result1.Count > 0)
                {
                    foreach (var item in result1)
                    {
                        ViewBag.HasDate = false;
                        dt1 = Convert.ToDateTime(item["enddate"]);
                        //dt1 = dt1.AddDays(1);
                    }
                    gremlinScript1 = $"g.V().has('activity','activityname','{ ConfigurationManager.AppSettings["EndActivity"] }').has('activity','projectid','{tbl_activity.projectid}').project('id','startdate','enddate').by(values('id')).by(values('startdate')).by(values('enddate'))";
                    result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                    if (result1.Count > 0)
                    {
                        foreach (var item in result1)
                        {
                            if (dt1 > Convert.ToDateTime(item["enddate"]))
                            {
                                gremlinScript1 = $"g.V('{item["id"]}')" +
                                                $".property('startdate', '{dt1.ToString("yyyy-MM-dd")}')" +
                                                $".property('enddate', '{dt1.ToString("yyyy-MM-dd")}')" +
                                                $".property('projectid', '{tbl_activity.projectid}')" +
                                                $".property('durations', '{1}')" +
                                                $".property('updatedon', '{DateTime.Now}')" +
                                                $".property('type', 'activity')";
                                result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                            }
                        }
                    }

                    //****************************Update project End date****************************
                    gremlinScript1 = $"g.V().has('project','id','{tbl_activity.projectid}').project('id','startdate','enddate').by(values('id')).by(values('startdate')).by(values('enddate'))";
                    result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                    if (result1.Count > 0)
                    {
                        foreach (var item in result1)
                        {
                            if (dt1 > Convert.ToDateTime(item["enddate"]))
                            {
                                gremlinScript1 = $"g.V('{item["id"]}')" +                                                
                                                $".property('enddate', '{dt1.ToString("yyyy-MM-dd")}')" +                                                
                                                $".property('updatedon', '{DateTime.Now}')" +
                                                $".property('type', 'project')";
                                result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                            }
                        }
                    }
                }
                //****************************End****************************
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
