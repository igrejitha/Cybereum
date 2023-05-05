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
    public class MilestoneController : Controller
    {
        private cybereumEntities db = new cybereumEntities();

        // GET: Milestone
        [Authorize]
        [SessionTimeout]
        public ActionResult Index(int? projectid)
        {
            //var tbl_task = db.tbl_task.Include(t => t.tbl_project).Include(t => t.tbl_user).Include(t => t.tbl_tasktype);
            //return View(tbl_task.ToList());

            TempData["ProjectId"] = projectid;
            ViewBag.projectid = TempData["ProjectId"];
            TempData.Keep();
            if (projectid == 0)
            {
                ViewBag.projectid = Convert.ToInt32(Session["ProjectId"]);
                Session["ProjectId"] = ViewBag.projectid;
            }
            else
            {
                ViewBag.projectid = projectid;
                Session["ProjectId"] = projectid;
            }
            GetMilestone(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]), projectid);
            return View();
        }

        [SessionTimeout]
        [Authorize]
        [HttpPost]
        public ActionResult GetMilestone(int userid = 0, int roleid = 0, int? projectid = 0)
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

                var files = entities.sp_FetchMilestones(userid, roleid, projectid, skip, pageSize, sortColumn, sortColumnDir).ToList();

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

        public ActionResult Addrecord(int Id)
        {
            ViewBag.Message = "Edit Milestone";
            tbl_milestone milestone = new tbl_milestone();

            milestone = db.tbl_milestone.Find(Id);
            return RedirectToAction("Create", milestone);
        }

        // GET: Milestone/Details/5
        public ActionResult Details(int? id)
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

        // GET: Milestone/Create
        //public ActionResult Create()
        //{
        //    ViewBag.assignedto = new SelectList(db.tbl_user, "userid", "username");
        //    ViewBag.projectid = new SelectList(db.tbl_project, "projectid", "projectname");
        //    ViewBag.tasktypeid = new SelectList(db.tbl_tasktype, "tasktypeid", "tasktypename");
        //    return View();
        //}
        [Authorize]
        [SessionTimeout]
        public ActionResult Create(int? milestoneid, int? projectid, MilestoneViewModel Milestones)
        {
            if (projectid == 0)
            {
                ViewBag.projectid = Convert.ToInt32(Session["ProjectId"]);
                Session["ProjectId"] = ViewBag.projectid;
            }
            else
            {
                ViewBag.projectid = Milestones.projectid;
                Session["ProjectId"] = projectid;
            }

            //List<SelectListItem> tasktype = (from b in db.tbl_tasktype
            //                                 where b.isactive == 1
            //                                 select new SelectListItem
            //                                 {
            //                                     Text = b.tasktypename,
            //                                     Value = b.tasktypeid.ToString()
            //                                 }).Distinct().OrderBy(x => x.Text).ToList();
            //ViewBag.tasktypeid = tasktype;

            //List<SelectListItem> status = (from b in db.tbl_status
            //                               where b.isactive == 1
            //                               select new SelectListItem
            //                               {
            //                                   Text = b.statusname,
            //                                   Value = b.statusid.ToString()
            //                               }).Distinct().OrderBy(x => x.Text).ToList();
            //ViewBag.statusid = status;

            if (milestoneid == null)
            {
                Milestones.startdate = DateTime.Today;
                Milestones.enddate = DateTime.Today;
            }
            return View(Milestones);
        }

        [Authorize]
        [SessionTimeout]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "milestoneid,milestonename,isactive,startdate,enddate,statusid,createdby,createddate,modifiedby,modifieddate,projectid,priority,tasktypeid,assignedto")] MilestoneViewModel tbl_milestone)
        //public ActionResult AddUsers(FormCollection formval, UserLevelOne _mod)
        {
            tbl_milestone.projectid = Convert.ToInt16(TempData["ProjectId"]);
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                //int? objList = 0;
                var objList=db.tbl_milestone.Find(tbl_milestone.milestoneid);
                if (tbl_milestone.milestoneid == 0)
                {
                    //objList = db.sp_FetchTaskExists(tbl_milestone.milestonename).FirstOrDefault();
                    objList = db.tbl_milestone.Where(x => x.milestonename == tbl_milestone.milestonename).FirstOrDefault();
                    //objList = db.tbl_milestone.Where(a=> a.milestonename == tbl_milestone.milestonename).FirstOrDefault();
                }
                else
                {
                    objList = db.tbl_milestone.Where(x => x.milestonename == tbl_milestone.milestonename && x.projectid != tbl_milestone.projectid).FirstOrDefault();
                }

                if (objList != null)
                {
                    message = "Milestone name already exists.";                    
                }
                else
                {
                    tbl_milestone milestone = new tbl_milestone();
                    milestone.milestonename = tbl_milestone.milestonename;
                    milestone.projectid = tbl_milestone.projectid;
                    milestone.startdate = tbl_milestone.startdate;
                    milestone.enddate = tbl_milestone.enddate;
                    //milestone.priority = tbl_milestone.priority;
                    //milestone.assignedto = tbl_milestone.assignedto;
                    //milestone.tasktypeid = tbl_milestone.tasktypeid;
                    //milestone.statusid = tbl_milestone.statusid;
                    milestone.isactive = 1;
                    if (tbl_milestone.milestoneid == 0)
                    {
                        milestone.createddate = DateTime.Now;
                        milestone.createdby = Convert.ToInt16(Session["LoggedInUserId"]);
                        db.tbl_milestone.Add(milestone);
                        db.SaveChanges();
                        message = "Saved Successfully";
                    }
                    else
                    {
                        tbl_milestone milestoneedit = db.tbl_milestone.Find(tbl_milestone.milestoneid);

                        milestoneedit.milestonename = tbl_milestone.milestonename;
                        milestoneedit.projectid = tbl_milestone.projectid;
                        milestoneedit.startdate = tbl_milestone.startdate;
                        milestoneedit.enddate = tbl_milestone.enddate;
                        //milestoneedit.priority = tbl_milestone.priority;
                        //milestoneedit.assignedto = tbl_milestone.assignedto;
                        //milestoneedit.tasktypeid = tbl_milestone.tasktypeid;
                        //milestoneedit.statusid = tbl_milestone.statusid;
                        milestoneedit.isactive = 1;
                        milestoneedit.modifieddate = DateTime.Now;
                        milestoneedit.modifiedby = Convert.ToInt16(Session["LoggedInUserId"]);
                        milestoneedit.isactive = 1;
                        db.Entry(milestoneedit).State = EntityState.Modified;
                        db.SaveChanges();
                        message = "Modified Successfully";
                    }

                    return RedirectToAction("Index", new { projectid = tbl_milestone.projectid });
                }
                ViewBag.Message = message;
                return RedirectToAction("Create", "Milestone", new { projectid = tbl_milestone.projectid });
            }
            
            return RedirectToAction("Create", "Milestone", new { projectid = tbl_milestone.projectid });
        }

        // GET: Milestone/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.assignedto = new SelectList(db.tbl_user, "userid", "username", tbl_milestone.assignedto);
            ViewBag.projectid = new SelectList(db.tbl_project, "projectid", "projectname", tbl_milestone.projectid);
            ViewBag.tasktypeid = new SelectList(db.tbl_tasktype, "tasktypeid", "tasktypename", tbl_milestone.tasktypeid);
            return View(tbl_milestone);
        }

        // POST: Milestone/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "milestoneid,milestonename,isactive,startdate,enddate,statusid,createdby,createddate,modifiedby,modifieddate,projectid,priority,tasktypeid,assignedto")] tbl_milestone tbl_milestone)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_milestone).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.assignedto = new SelectList(db.tbl_user, "userid", "username", tbl_milestone.assignedto);
            ViewBag.projectid = new SelectList(db.tbl_project, "projectid", "projectname", tbl_milestone.projectid);
            ViewBag.tasktypeid = new SelectList(db.tbl_tasktype, "tasktypeid", "tasktypename", tbl_milestone.tasktypeid);
            return View(tbl_milestone);
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
