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

namespace Cybereum.Controllers
{
    public class ProjectController : Controller
    {
        private cybereumEntities db = new cybereumEntities();

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
            return View(Projects);
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
                            project.createdby = Convert.ToInt16(Session["LoggedInUserId"]);
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
                            project.startdate = tbl_project.startdate;
                            project.enddate = tbl_project.enddate;
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
                return View(tbl_project);
            }
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
    }
}
