using Cybereum.Filters;
using Cybereum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cybereum.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        [SessionTimeout]
        public ActionResult Index()
        {
            //using (DashboardContext _context = new DashboardContext())
            //{
            //    ViewBag.CountCustomers = _context.CustomerSet.Count();
            //    ViewBag.CountOrders = _context.OrderSet.Count();
            //    ViewBag.CountProducts = _context.ProductSet.Count();
            //}
            cybereumEntities entities = new cybereumEntities();
            var countapproved = (from p in entities.tbl_user
                          where p.emailverification == true && p.isactive == 1
                          select p.userid).Count();

            var countpending = (from p in entities.tbl_user
                          where p.emailverification == true && p.isactive == 0
                          select p.userid).Count();

            var countproject = (from p in entities.tbl_project
                                where p.isactive == 0
                                select p.projectid).Count();

            ViewBag.countApproved = countapproved;
            ViewBag.countPending = countpending;
            ViewBag.countProject = countproject;

            return View();
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult Dashboard()
        {
            int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
            cybereumEntities entities = new cybereumEntities();
            var countUser = (from p in entities.tbl_user
                                 where p.emailverification == true && p.isactive == 1 && p.pmuserid == pmuserid
                             select p.userid).Count();

            var countProject = (from p in entities.tbl_project
                                where p.isactive == 1 && p.createdby == pmuserid 
                                select p.projectid).Count();

            var countTask = (from p in entities.tbl_task
                                join a in entities.tbl_project on p.projectid equals a.projectid
                                where p.isactive == 1 && a.createdby == pmuserid
                                select p.projectid).Count();

            ViewBag.countUser = countUser;
            ViewBag.countProject = countProject;
            ViewBag.countTask = countTask;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
    }
}