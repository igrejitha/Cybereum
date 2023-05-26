using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Gremlin.Net.Driver;
using System.Configuration;

namespace Cybereum.Models
{
    public class tblloginMetadata
    {
        public int userid { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        public string password { get; set; }

        [Required(ErrorMessage = "Firstname is required")]
        public string firstname { get; set; }

        public string lastname { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public Nullable<int> roleid { get; set; }

        public string gender { get; set; }
        public string phoneno { get; set; }
        public string profilepic { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<System.DateTime> createdon { get; set; }
        public Nullable<int> createdby { get; set; }
        public Nullable<System.DateTime> modifiedon { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<int> clientid { get; set; }
    }

    public class objFilterNew
    {
        public int stateid { get; set; }
        public int countyid { get; set; }
        public int leasetypeid { get; set; }
        public int leaseid { get; set; }
        public string filedate { get; set; }
        public string filedateto { get; set; }
        public string wellnessno { get; set; }
        public string searchtext { get; set; }
    }

    public class objFilesPathBytes
    {
        public byte[] filecontent { get; set; }
        public string filename { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-mail id")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Email { get; set; }


        //[Required]
        //[Display(Name = "User Name")]
        //public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
    }



    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPwdViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string email { get; set; }
    }

    public class ResetPwdViewModel
    {
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string code { get; set; }
    }


    public class RegisterViewModel
    {
        //[Required(ErrorMessage = "Please enter user name")]
        //[Display(Name = "User Name")]
        //public string username { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter email id")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        [Display(Name = "E-mail id")]
        public string email { get; set; }

        //[Required]
        //public int userid { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        [Display(Name = "First Name")]
        public string firstname { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        [Display(Name = "Last Name")]
        public string lastname { get; set; }

        [Required(ErrorMessage = "Please enter organization")]
        [Display(Name = "Organization")]
        public string organization { get; set; }
    }


    public class UserViewModel
    {
        [Required(ErrorMessage = "Please enter email id")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        [Display(Name = "E-mail id")]
        public string emailid { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int userid { get; set; }

        [Required(ErrorMessage = "Please enter first name")]
        [Display(Name = "First Name")]
        public string firstname { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        [Display(Name = "Last Name")]
        public string lastname { get; set; }

        [Display(Name = "Organization")]
        public string organization { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public Nullable<int> roleid { get; set; }

        public Nullable<System.DateTime> createddate { get; set; }
        public Nullable<bool> emailverification { get; set; }
        public string otp { get; set; }
        public string activationcode { get; set; }
        public Nullable<int> pmuserid { get; set; }
        public string username { get; set; }
        public Nullable<int> isactive { get; set; }

    }


    public class ProjectViewModel
    {
        [Required]
        public int projectid { get; set; }

        [Required]
        public string id { get; set; }

        [Required(ErrorMessage = "Please enter Project name")]
        [Display(Name = "Project Name")]
        public string projectname { get; set; }

        [Required(ErrorMessage = "Please enter Start Date")]
        [Display(Name = "Start Date")]
        public DateTime startdate { get; set; }

        [Required(ErrorMessage = "Please enter End Date")]
        [Display(Name = "End Date")]
        public DateTime enddate { get; set; }

        [Display(Name = "Cost")]
        public int projectcost { get; set; }

        [Display(Name = "No. of Resource")]
        public int noofresource { get; set; }

        public Nullable<System.DateTime> createdon { get; set; }
        public int createdby { get; set; }
        public Nullable<System.DateTime> modifiedon { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<int> isactive { get; set; }

    }

    public class TaskViewModel
    {
        [Required]
        public int taskid { get; set; }

        [Required]
        public int milestoneid { get; set; }

        //[Required(ErrorMessage = "Please enter Project name")]
        [Display(Name = "Milestone Name")]
        public string milestonename { get; set; }

        [Required(ErrorMessage = "Please enter Task name")]
        [Display(Name = "Task Name")]
        public string taskname { get; set; }

        [Required(ErrorMessage = "Please enter Start Date")]
        [Display(Name = "Start Date")]
        public DateTime startdate { get; set; }

        [Required(ErrorMessage = "Please enter End Date")]
        [Display(Name = "End Date")]
        public DateTime enddate { get; set; }


        public Nullable<System.DateTime> createddate { get; set; }
        public int createdby { get; set; }
        public Nullable<System.DateTime> modifieddate { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<int> isactive { get; set; }

        [Required(ErrorMessage = "Please enter status")]
        [Display(Name = "Status")]
        public int statusid { get; set; }

        public int priority { get; set; }

        [Required(ErrorMessage = "Please enter Task Type")]
        [Display(Name = "Task Type")]
        public int tasktypeid { get; set; }

        [Required(ErrorMessage = "Please select Assigned To")]
        public int assignedto { get; set; }

    }

    public class MilestoneViewModel
    {
        [Required]
        public int milestoneid { get; set; }

        [Required]
        public int projectid { get; set; }

        //[Required(ErrorMessage = "Please enter Project name")]
        [Display(Name = "Project Name")]
        public string projectname { get; set; }

        [Required(ErrorMessage = "Please enter Milestone name")]
        [Display(Name = "Milestone Name")]
        public string milestonename { get; set; }

        [Required(ErrorMessage = "Please enter Start Date")]
        [Display(Name = "Start Date")]
        public DateTime startdate { get; set; }

        [Required(ErrorMessage = "Please enter End Date")]
        [Display(Name = "End Date")]
        public DateTime enddate { get; set; }


        public Nullable<System.DateTime> createddate { get; set; }
        public int createdby { get; set; }
        public Nullable<System.DateTime> modifieddate { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<int> isactive { get; set; }
    }


    public class SubTaskViewModel
    {
        [Required]
        public int subtaskid { get; set; }

        [Required]
        public int taskid { get; set; }

        //[Required(ErrorMessage = "Please enter Project name")]
        [Display(Name = "Task Name")]
        public string taskname { get; set; }

        [Required(ErrorMessage = "Please enter SubTask name")]
        [Display(Name = "SubTask Name")]
        public string subtaskname { get; set; }

        [Required(ErrorMessage = "Please enter Start Date")]
        [Display(Name = "Start Date")]
        public DateTime startdate { get; set; }

        [Required(ErrorMessage = "Please enter End Date")]
        [Display(Name = "End Date")]
        public DateTime enddate { get; set; }


        public Nullable<System.DateTime> createddate { get; set; }
        public int createdby { get; set; }
        public Nullable<System.DateTime> modifieddate { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<int> isactive { get; set; }

        [Required(ErrorMessage = "Please enter status")]
        [Display(Name = "Status")]
        public int statusid { get; set; }

        public int priority { get; set; }

        [Required(ErrorMessage = "Please enter Task Type")]
        [Display(Name = "Task Type")]
        public int tasktypeid { get; set; }
        public int assignedto { get; set; }

    }


    public class Activity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan Duration { get; set; }
        public List<int> PrecedingTasks { get; set; }
        public int TimeDuration { get; set; }
        public int Progress { get; set; }
        public bool Achieved { get; set; }
        public bool Milestone { get; set; } //To determine if this activity is designated as a milestone or not
        public List<string> AssignedResources { get; set; }




        public struct DateTime
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }
            public int Hour { get; set; }
            public int Minute { get; set; }
            public int Second { get; set; }
            public int Millisecond { get; set; }
            public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
            {
                Year = year;
                Month = month;
                Day = day;
                Hour = hour;
                Minute = minute;
                Second = second;
                Millisecond = millisecond;
            }
        }
        //public override string ToString() => JsonSerializer.Serialize<Activity>(this);

    }

    public class ganttchartmodel
    {
        public string id { get; set; }
        public string taskid { get; set; }
        public string name { get; set; }
        public string actualStart { get; set; }
        public string actualEnd { get; set; }
        public string connectTo { get; set; }
        public string connecterType { get; set; }
        public string progressValue { get; set; }
        public List<Children> children { get; set; }
    }

    public class Children
    {
        public string id { get; set; }
        public string taskid { get; set; }
        public string name { get; set; }
        public string actualStart { get; set; }
        public string actualEnd { get; set; }
        public string connectTo { get; set; }
        public string connecterType { get; set; }
        public string progressValue { get; set; }
        public List<Children> children { get; set; }
        public List<chartconnector> connector { get; set; }
    }

    public class chartconnector
    {
        public string connectTo { get; set; }
        public string connectorType { get; set; }
    }


    public class TestModel
    {
        public string task { get; set; }
        public string type { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
    }

    public class d3_GANTT_ChartModel
    {
        public void OnGet()
        {
        }
    }

    public class DashTest2Model
    {
        public List<Activity> Activities { get; set; } = new List<Activity>();

        public void OnGet()
        {
            // Your code here
        }

        public class Activity
        {
            public int id { get; set; }
            public string name { get; set; }

            //[DataType(DataType.Date)]
            //[JsonConverter(typeof(JsonDateConverter))]
            public String start { get; set; }

            //[DataType(DataType.Date)]
            //[JsonConverter(typeof(JsonDateConverter))]
            public String end { get; set; }
            public List<int> predecessors { get; set; }
            public List<int> successors { get; set; }

            public int parentid { get; set; }
            public string parentname { get; set; }
        }

        public class Milestone
        {
            public string id { get; set; }
            public string Number { get; set; }
            public string name { get; set; }
            public string Start_Date { get; set; }
            public string Finish_Date { get; set; }
            public List<string> Predecessors { get; set; }
            public string Durations { get; set; }
        }
    }

    public class Project
    {
        public string projectid { get; set; }

        [Required(ErrorMessage = "Please enter Project name")]
        [Display(Name = "Project Name")]
        public string projectname { get; set; }

        [Required(ErrorMessage = "Please enter Start Date")]
        [Display(Name = "Start Date")]
        public DateTime startdate { get; set; }

        [Required(ErrorMessage = "Please enter End Date")]
        [Display(Name = "End Date")]
        public DateTime enddate { get; set; }
        public string type { get; set; }

        [Display(Name = "No. of Resource")]
        public string noofresource { get; set; }

        [Display(Name = "Cost")]
        public string projectcost { get; set; }
        public string createdby { get; set; }
        public string createdusername { get; set; }
        public DateTime createdon { get; set; }
        public List<ProjectActivity> activities { get; set; }

        public int[] projectmembers { get; set; }
        public string organization { get; set; }

        public string projectstatus { get; set; }
        public string projecttype { get; set; }
    }

    public class ProjectMembers
    {
        public int userid { get; set; }
        public string username { get; set; }
        public string userrole { get; set; }
        public string organization { get; set; }
        public int[] Members { get; set; }
    }

    public class ProjectActivity
    {
        public string id { get; set; }
        //public string activityid { get; set; }

        [Required(ErrorMessage = "Please enter Activity name")]
        [Display(Name = "Activity Name")]
        public string activityname { get; set; }

        [Required(ErrorMessage = "Please enter Start Date")]
        [Display(Name = "Start Date")]
        public DateTime startdate { get; set; }

        [Required(ErrorMessage = "Please enter End Date")]
        [Display(Name = "End Date")]
        public DateTime enddate { get; set; }


        public bool ismilestone { get; set; }
        public long durations { get; set; }
        public string[] Predecessors { get; set; }

        [Display(Name = "Predecessors List")]
        public System.Web.Mvc.MultiSelectList PredecessorsList { get; set; }

        public string projectid { get; set; }
        public string projectname { get; set; }
        public string createdby { get; set; }
        public string createdusername { get; set; }
        public DateTime createdon { get; set; }
        public List<ProjectTask> tasks { get; set; }
    }

    public class ProjectTask
    {
        public string taskid { get; set; }

        [Required(ErrorMessage = "Please enter Task name")]
        [Display(Name = "Task Name")]
        public string taskname { get; set; }

        [Required(ErrorMessage = "Please enter Start Date")]
        [Display(Name = "Start Date")]
        public DateTime startdate { get; set; }

        [Required(ErrorMessage = "Please enter End Date")]
        [Display(Name = "End Date")]
        public DateTime enddate { get; set; }


        public string assignedto { get; set; }
        public string assignedusername { get; set; }

        public int tasktype { get; set; }
        public int taskstatus { get; set; }
        //public string priority { get; set; }        
        public long durations { get; set; }
        public List<string> Predecessors { get; set; }
        public string activityid { get; set; }
        public string activityname { get; set; }

        public string createdby { get; set; }
        public string createdusername { get; set; }
        public DateTime createdon { get; set; }

        public List<ProjectSubTask> subtasks { get; set; }
    }

    public class ProjectSubTask
    {
        public string subtaskid { get; set; }

        [Required(ErrorMessage = "Please enter SubTask name")]
        [Display(Name = "SubTask Name")]
        public string subtaskname { get; set; }

        [Required(ErrorMessage = "Please enter Start Date")]
        [Display(Name = "Start Date")]
        public DateTime startdate { get; set; }

        [Required(ErrorMessage = "Please enter End Date")]
        [Display(Name = "End Date")]
        public DateTime enddate { get; set; }


        public string assignedto { get; set; }
        public string assignedusername { get; set; }

        public int tasktype { get; set; }
        public int taskstatus { get; set; }
        //public string priority { get; set; }        
        public long durations { get; set; }
        public List<string> Predecessors { get; set; }
        public string taskid { get; set; }
        public string taskname { get; set; }

        public string createdby { get; set; }
        public string createdusername { get; set; }
        public DateTime createdon { get; set; }
    }

    public class gremlinvariables
    {
        //private static string hostname = "gremtest1.gremlin.cosmos.azure.com";
        public static string hostname
        {
            get
            {
                return ConfigurationManager.AppSettings["gremlinhostname"];
            }
        }

        public static int port
        {
            get
            {
                return Convert.ToInt16(ConfigurationManager.AppSettings["gremlinport"]);
            }
        }
        public static string authKey
        {
            get
            {
                return ConfigurationManager.AppSettings["gremlinauthkey"];
            }
        }
        public static string database
        {
            get
            {
                return ConfigurationManager.AppSettings["gremlindatabase"];
            }
        }
        public static string collection
        {
            get
            {
                return ConfigurationManager.AppSettings["gremlincollection"];
            }
        }

        public static ConnectionPoolSettings connectionPoolSettings = new ConnectionPoolSettings()
        {
            MaxInProcessPerConnection = 10,
            PoolSize = 4,
            ReconnectionAttempts = 3,
            ReconnectionBaseDelay = TimeSpan.FromMilliseconds(100)
        };

        public static string containerLink
        {
            get
            {
                return "/dbs/" + database + "/colls/" + collection;
            }
        }
    }

    public enum Role
    {
        Admin = 1,
        ProjectManager,
        User
    }
}