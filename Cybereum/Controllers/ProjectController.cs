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
            GetProject(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]));
            return View();
        }

        // GET: Project/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_project tbl_project = db.tbl_project.Find(id);
            if (tbl_project == null)
            {
                return HttpNotFound();
            }
            return View(tbl_project);
        }

        //// GET: Project/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Project/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "projectid,projectname,startdate,enddate,createdon,modifiedon,createdby,modifiedby,projectcost,noofresource,isactive")] tbl_project tbl_project)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.tbl_project.Add(tbl_project);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(tbl_project);
        //}

        public ActionResult Addrecord(int Id)
        {
            ViewBag.Message = "Edit Project";
            tbl_project project = new tbl_project();

            project = db.tbl_project.Find(Id);
            return RedirectToAction("Create", project);
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
        public ActionResult Create([Bind(Include = "projectid,projectname,startdate,enddate,createdon,modifiedon,createdby,modifiedby,projectcost,noofresource,isactive")] ProjectViewModel tbl_project)
        //public ActionResult AddUsers(FormCollection formval, UserLevelOne _mod)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                int? objList = 0;
                //objList = db.sp_FetchUserExists(logindetails.emailid, logindetails.password).FirstOrDefault();
                if (tbl_project.projectid == 0)
                {
                    objList = db.sp_FetchProjectExists(tbl_project.projectname).FirstOrDefault();
                    //var objList1 = db.tbl_user.Where(x => x.emailid == logindetails.emailid && x.isactive != 2).FirstOrDefault();
                    //objList = objList1.userid;
                }

                switch (objList)
                {
                    case 1:
                        message = "Project name already exists.";
                        break;
                    default:
                        tbl_project project = new tbl_project();
                        project.projectname = tbl_project.projectname;
                        project.projectcost = tbl_project.projectcost;
                        project.noofresource = tbl_project.noofresource;
                        project.startdate = tbl_project.startdate;
                        project.enddate = tbl_project.enddate;
                        project.isactive = 1;
                        if (tbl_project.projectid == 0)
                        {
                            project.createdon = DateTime.Now;
                            if (tbl_project.createdby == 0)
                                project.createdby = Convert.ToInt16(Session["LoggedInUserId"]);
                            else
                                project.createdby = tbl_project.createdby;

                            db.tbl_project.Add(project);
                            db.SaveChanges();
                            message = "Saved Successfully";
                        }
                        else
                        {
                            tbl_project projectedit = db.tbl_project.Find(tbl_project.projectid);

                            projectedit.projectname = tbl_project.projectname;
                            projectedit.projectcost = tbl_project.projectcost;
                            projectedit.noofresource = tbl_project.noofresource;
                            projectedit.startdate = tbl_project.startdate;
                            projectedit.enddate = tbl_project.enddate;
                            projectedit.modifiedon = DateTime.Now;
                            projectedit.modifiedby = Convert.ToInt16(Session["LoggedInUserId"]);
                            projectedit.isactive = 1;
                            db.Entry(projectedit).State = EntityState.Modified;
                            db.SaveChanges();
                            message = "Modified Successfully";
                        }
                        return RedirectToAction("Index");
                }
                ViewBag.Message = message;
                int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
                int roleid = Convert.ToInt32(Session["RoleId"]);
                List<SelectListItem> user = Filluser(pmuserid, roleid);
                ViewBag.createdby = user;
                return View(tbl_project);
            }
            int pmuserid1 = Convert.ToInt32(Session["LoggedInUserId"]);
            int roleid1 = Convert.ToInt32(Session["RoleId"]);
            List<SelectListItem> user1 = Filluser(pmuserid1, roleid1);
            ViewBag.createdby = user1;
            return View(tbl_project);
        }

        // GET: Project/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_project tbl_project = db.tbl_project.Find(id);
            if (tbl_project == null)
            {
                return HttpNotFound();
            }
            return View(tbl_project);
        }

        // POST: Project/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "projectid,projectname,startdate,enddate,createdon,modifiedon,createdby,modifiedby,projectcost,noofresource,isactive")] tbl_project tbl_project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_project);
        }

        // GET: Project/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_project tbl_project = db.tbl_project.Find(id);
            if (tbl_project == null)
            {
                return HttpNotFound();
            }
            return View(tbl_project);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_project tbl_project = db.tbl_project.Find(id);
            db.tbl_project.Remove(tbl_project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [SessionTimeout]
        [Authorize]
        [HttpPost]
        public ActionResult GetProject(int userid = 0, int roleid = 0)
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

                var files = entities.sp_FetchProjects(userid, roleid, skip, pageSize, sortColumn, sortColumnDir).ToList();

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

        private const string hostname = "wss://cosmosgremlinapi.documents.azure.com:443/";
        private const int port = 443;
        private const string authkey = "lja6Gkeuf5nsnEg9TYyC79N1fvt4v1ZBb9JwkbWPNiNC1tEeBOSVu8vBHQZeKnSFguIKz9ziKjVEiPAjRAuf3w==";
        private const string database = "graphdb";

        public JsonResult getchartvalue()
        {
            DashTest2Model chartmodel = new Models.DashTest2Model();

            //var files = db.sp_getganttchart(0, 0).ToList();
            //foreach (var project in files)
            //{
            //    DashTest2Model.Activity chartactivity1 = new DashTest2Model.Activity();
            //    chartactivity1.id = project.id;
            //    chartactivity1.name = project.name;
            //    chartactivity1.start = project.actualStart.ToString("yyyy-MM-dd");
            //    chartactivity1.end = project.actualEnd.ToString("yyyy-MM-dd");
            //    chartactivity1.predecessors = new List<int> { project.connectTo };
            //    chartactivity1.successors = new List<int> { project.connectTo };
            //    chartactivity1.parentid = project.connectTo;
            //    chartactivity1.parentname = project.parentname;
            //    chartmodel.Activities.Add(chartactivity1);
            //}

            int rowid = 1;
            var projectdata = db.sp_get_project(1, 1, 0).ToList();
            foreach (var project in projectdata)
            {
                DashTest2Model.Activity chartactivity1 = new DashTest2Model.Activity();
                chartactivity1.id = rowid++;
                chartactivity1.name = project.name;
                chartactivity1.start = project.startdate.ToString("yyyy-MM-dd");
                chartactivity1.end = project.enddate.ToString("yyyy-MM-dd");
                chartactivity1.predecessors = new List<int> { project.connectTo };
                chartactivity1.successors = new List<int> { project.connectTo };
                chartactivity1.parentid = project.connectTo;
                chartactivity1.parentname = project.parentname;
                chartmodel.Activities.Add(chartactivity1);

                var milestonedata = db.sp_get_milestone(1, 1, project.id, 0).ToList();
                foreach (var itemmilestone in milestonedata)
                {
                    DashTest2Model.Activity chartactivity2 = new DashTest2Model.Activity();
                    chartactivity2.id = rowid++;
                    chartactivity2.name = itemmilestone.name;
                    chartactivity2.start = itemmilestone.startdate.ToString("yyyy-MM-dd");
                    chartactivity2.end = itemmilestone.enddate.ToString("yyyy-MM-dd");
                    chartactivity2.predecessors = new List<int> { chartactivity1.id };
                    chartactivity2.successors = new List<int> { chartactivity1.id };
                    chartactivity2.parentid = chartactivity1.id;
                    chartactivity2.parentname = chartactivity1.name;
                    chartmodel.Activities.Add(chartactivity2);

                    var taskdata = db.sp_get_task(1, 1, itemmilestone.id, 0).ToList();
                    foreach (var itemtask in taskdata)
                    {
                        DashTest2Model.Activity chartactivity3 = new DashTest2Model.Activity();
                        chartactivity3.id = rowid++;
                        chartactivity3.name = itemtask.name;
                        chartactivity3.start = itemtask.startdate.ToString("yyyy-MM-dd");
                        chartactivity3.end = itemtask.enddate.ToString("yyyy-MM-dd");
                        chartactivity3.predecessors = new List<int> { chartactivity2.id };
                        chartactivity3.successors = new List<int> { chartactivity2.id };
                        chartactivity3.parentid = chartactivity2.id;
                        chartactivity3.parentname = chartactivity2.name;
                        chartmodel.Activities.Add(chartactivity3);

                        var subtaskdata = db.sp_get_subtask(1, 1, itemtask.id, 0).ToList();
                        foreach (var itemsubtask in subtaskdata)
                        {
                            DashTest2Model.Activity chartactivity4 = new DashTest2Model.Activity();
                            chartactivity4.id = rowid++;
                            chartactivity4.name = itemsubtask.name;
                            chartactivity4.start = itemsubtask.startdate.ToString("yyyy-MM-dd");
                            chartactivity4.end = itemsubtask.enddate.ToString("yyyy-MM-dd");
                            chartactivity4.predecessors = new List<int> { chartactivity3.id };
                            chartactivity4.successors = new List<int> { chartactivity3.id };
                            chartactivity4.parentid = chartactivity3.id;
                            chartactivity4.parentname = chartactivity3.name;
                            chartmodel.Activities.Add(chartactivity4);
                        }
                    }
                }
            }
            var x = JsonConvert.SerializeObject(chartmodel.Activities, Formatting.Indented);
            return Json(chartmodel.Activities, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [SessionTimeout]
        public ActionResult Graph()
        {
            return View();
        }


        [Authorize]
        [SessionTimeout]
        // GET: Project
        public ActionResult GANTT()
        {
            DashTest2Model chartmodel = new Models.DashTest2Model();

            var files = db.sp_FetchProjects(1, 1, 0, 100, "projectname", "asc").ToList();
            foreach (var project in files)
            {
                DashTest2Model.Activity chartactivity = new DashTest2Model.Activity();
                chartactivity.id = project.projectid;
                chartactivity.name = project.projectname;
                chartactivity.start = project.startdate.ToString("yyyy-MM-dd");
                chartactivity.end = project.enddate.ToString("yyyy-MM-dd");
                chartactivity.predecessors = new List<int> { 1 };
                chartactivity.successors = new List<int> { 3 };
                chartmodel.Activities.Add(chartactivity);
            };



            List<tbl_project> projects = new List<tbl_project>();
            //projects = files.ToList();
            JsonResult result = new JsonResult();
            result = this.Json(new { data = files, recordsTotal = 0, recordsFiltered = 0 }, JsonRequestBehavior.AllowGet);
            //projects = JsonConvert.DeserializeObject<tbl_project>(result.ToString());
            //using (var client = new DocumentClient(new Uri(hostname), authkey))
            //{
            //    var query = client.CreateGremlinQuery<dynamic>(database, "g.V()");
            //    while (query.HasMoreResults)
            //    {
            //        foreach (dynamic result in query.ExecuteNextAsync().Result)
            //        {
            //            var project = JsonConvert.DeserializeObject<Project>(result.ToString());
            //            projects.Add(project);
            //        }
            //    }
            //}
            return View(chartmodel);
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

    }
}
