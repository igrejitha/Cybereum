using Cybereum.Filters;
using Cybereum.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cybereum.Controllers
{
    [Authorize]
    [SessionTimeout]
    public class ProjectportfoliodashboardController : Controller
    {
        // GET: projectportfoliodashboard
        public ActionResult Projectportfoliodashboard(string projectid)
        {
            return View();
        }
        public ActionResult masterdashboard(string projectid)
        {
            ViewBag.projectid = projectid;
            Session["ProjectId"] = ViewBag.projectid;
            return View();
        }
        public ActionResult projectdashboard()
        {
            return View();
        }
        public ActionResult index(string projectid)
        {
            ViewBag.projectid = projectid;
            Session["ProjectId"] = ViewBag.projectid;
            ViewBag.Keyinternal = getkeyinternal(projectid);
            ViewBag.Taskcompletion = GettaskcompletionData(projectid);
            return View();
        }
        public ActionResult ProjectOverview()
        {
            return View();
        }
        public ActionResult TeamHub()
        {
            return View();
        }


        #region project dashboard

        public List<tblkeyinternal> getkeyinternal(string projectid)
        {
            try
            {
                var gremlinScript = $"g.V().has('activity','projectid','{projectid}').order().by('startdate',incr).order().by('enddate',incr).project('activityname','progress').by(values('activityname')).by(values('progress'))";
                var keyinternaldata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                string pList = JsonConvert.SerializeObject(keyinternaldata);
                List<tblkeyinternal> keyinternallist = JsonConvert.DeserializeObject<List<tblkeyinternal>>(pList);
                return keyinternallist;
                //return new JsonResult { Data = keyinternallist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }

        }

        public string GettaskcompletionData(string projectid)
        {
            try
            {
                var gremlinScript = $"g.V().has('activity','projectid','{projectid}').project('id').by(values('id'))";
                var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                List<tbltaskcompletion> taskcompletionlist = new List<tbltaskcompletion>();
                foreach (var result in activitydata)
                {
                    gremlinScript = $"g.V().has('task','activityid','{result["id"]}').project('taskstatus').by(values('taskstatus'))";
                    var res = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var item in res)
                    {
                        if (item.Count > 0)
                        {                            
                            tbltaskcompletion task = new tbltaskcompletion();
                            if (Convert.ToInt16(item["taskstatus"]) == (int)TaskSubTaskStatus.ToDo)
                            {
                                task.status = TaskSubTaskStatus.ToDo.ToString();
                            }
                            else if (Convert.ToInt16(item["taskstatus"]) == (int)TaskSubTaskStatus.Inprogress)
                            {
                                task.status = TaskSubTaskStatus.Inprogress.ToString();
                            }
                            else if (Convert.ToInt16(item["taskstatus"]) == (int)TaskSubTaskStatus.Completed)
                            {
                                task.status = TaskSubTaskStatus.Completed.ToString();
                            }
                            task.taskcount = 1;
                            taskcompletionlist.Add(task);
                        }
                    }
                }
                var numberGroups = taskcompletionlist.GroupBy(n => n.status).Select(c => new { Key = c.Key, total = c.Count() });
                taskcompletionlist = new List<tbltaskcompletion>();
                foreach (var grp in numberGroups)
                {
                    tbltaskcompletion task = new tbltaskcompletion();
                    task.status = grp.Key;
                    task.taskcount = grp.total;
                    taskcompletionlist.Add(task);
                }

                //return taskcompletionlist;                
                string pList = JsonConvert.SerializeObject(taskcompletionlist);
                return pList;
                //List<tbltaskcompletion> list = JsonConvert.DeserializeObject<List<tbltaskcompletion>>(pList);
                //return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }

        }

        #endregion
    }
}