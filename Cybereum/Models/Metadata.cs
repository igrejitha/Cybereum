using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
//using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

    public class leveloneMetaData
    {

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Desc { get; set; }
    }

    public class leveltwoMetaData
    {
        public int idLeveltwo { get; set; }

        [Required(ErrorMessage = "Level one is required")]
        public Nullable<int> idlevelone { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Desc { get; set; }
    }

    public class levelthreeMetaData
    {
        public int idLevelthree { get; set; }

        [Required(ErrorMessage = "Level two is required")]
        public Nullable<int> idleveltwo { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Desc { get; set; }

        public string LevelTwoName { get; set; }
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
        public int projectid { get; set; }

        [Required(ErrorMessage = "Please enter Project name")]
        [Display(Name = "Project Name")]
        public string projectname { get; set; }

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

    public class DashTest2Model
    {
        public List<Activity> Activities { get; set; } = new List<Activity>();

        public void OnGet()
        {
            // Your code here
        }

        public class Activity
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public List<int> Predecessors { get; set; }
            public List<int> Successors { get; set; }
        }
    }


    public enum Role
    {
        Admin = 1,
        ProjectManager,
        User
    }    
}