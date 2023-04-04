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
            List<SelectListItem> user = Filluser(pmuserid);
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

        public List<SelectListItem> Filluser(int? pmuserid)
        {
            List<SelectListItem> user = (from b in db.tbl_user
                                         where b.pmuserid == pmuserid && b.isactive == 1
                                         select new SelectListItem
                                         {
                                             Text = b.firstname + " " + b.lastname,
                                             Value = b.userid.ToString()
                                         }).Distinct().OrderBy(x => x.Text).ToList();
            //user.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });
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
            List<SelectListItem> user = Filluser(pmuserid);
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
