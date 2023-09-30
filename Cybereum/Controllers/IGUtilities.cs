<<<<<<< Updated upstream
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.DirectoryServices;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Cybereum.Models;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class IGUtilities
{

    #region Converting Methods

    public static int ToInt(this object Value)
    {
        int Integer;
        int.TryParse(Convert.ToString(Value), out Integer);
        return Integer;
    }

    public static int ToNullInt(this int? Value)
    {
        if (!Value.HasValue)
        {
            return 0;
        }
        int Integer;
        int.TryParse(Convert.ToString(Value), out Integer);
        return Integer;
    }
    public static TimeSpan ToTimeSpan(this object Value)
    {
        TimeSpan Time;
        TimeSpan.TryParse(Convert.ToString(Value), out Time);
        return Time;
    }

    public static double ToDouble(this object Value)
    {
        double Double;
        double.TryParse(Convert.ToString(Value), out Double);
        return Double;
    }

    public static decimal ToDecimal(this object Value)
    {
        decimal Double;
        decimal.TryParse(Convert.ToString(Value), out Double);
        return Double;
    }
    public static DateTime TimeConversion(this string Value)
    {
        if (!string.IsNullOrEmpty(Value))
        {
            DateTime current = DateTime.Today;
            bool HasAM = true;
            HasAM = Value.Contains("am");
            Value = HasAM ? Value.Replace("am", "") : Value.Replace("pm", "");
            string[] splitarray = Value.Split(':');
            TimeSpan time = new TimeSpan(
                HasAM ? splitarray[0].ToInt() : splitarray[0].ToInt() + 12
                , splitarray[1].ToInt(), 0);
            current = current.Add(time);
            return current;
        }
        else
        {
            return DateTime.Now;
        }

    }

    public static float ToFloat(this object Value)
    {
        float Float;
        float.TryParse(Convert.ToString(Value), out Float);
        return Float;
    }

    public static DateTime ToDateTime(this object Value)
    {
        DateTime dateTime;
        DateTime.TryParse(Convert.ToString(Value), out dateTime);
        return dateTime;
    }
    public static DateTime ToDate(this object Value)
    {
        DateTime dateTime;
        DateTime.TryParse(Convert.ToString(Value), out dateTime);
        var date = dateTime.Date;
        return date;
    }

    public static bool IsValidDateTime(this object Value)
    {
        DateTime dateTime;
        DateTime.TryParse(Convert.ToString(Value), out dateTime);
        if (dateTime == DateTime.MinValue)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static DateTime ToDateTime(this object Value, string Format)
    {
        DateTime dateTime;
        DateTime.TryParseExact(Value.ToString(), Format, null, DateTimeStyles.None, out dateTime);
        return dateTime;
    }

    public static DateTime FirstDayOfMonth(this DateTime current)
    {
        return current.AddDays(1 - current.Day);
    }

    public static DateTime LastDayOfMonth(this DateTime current)
    {
        return current.AddMonths(1).AddDays(1 - current.Day).Date;
    }

    public static bool IsDate(this object Obj)
    {
        string strDate = Obj.ToString();
        try
        {
            DateTime dt = DateTime.Parse(strDate);
            if ((dt.Month != DateTime.Now.Month) || (dt.Day < 1 && dt.Day > 31) || dt.Year != DateTime.Now.Year)
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }


    #endregion

    #region Enumeration Methods
    public static List<T> GetEnumList<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }

    public static string GetEnumDescription(Enum EnumConstant)
    {
        FieldInfo fiEnum = EnumConstant.GetType().GetField(EnumConstant.ToString());
        DescriptionAttribute[] descAttribute = (DescriptionAttribute[])fiEnum.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (descAttribute.Length > 0)
        {
            return descAttribute[0].Description;
        }

        return EnumConstant.ToString();
    }

    public static string Description(this Enum Value)
    {
        FieldInfo field = Value.GetType().GetField(Value.ToString());
        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? Value.ToString() : attribute.Description;
    }

    public static T ToEnum<T>(this string value)
    {
        var type = typeof(T);
        if (!type.IsEnum) throw new InvalidOperationException();
        foreach (var field in type.GetFields())
        {
            var attribute = Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute != null)
            {
                if (attribute.Description == value)
                    return (T)field.GetValue(null);
            }
            else
            {
                if (field.Name == value)
                    return (T)field.GetValue(null);
            }
        }
        throw new ArgumentException("Not found.", "description");
        // or return default(T);
    }

    #endregion

    #region File Generation

    public static string GeneratedFileName(string FileName)
    {
        string guid = Guid.NewGuid().ToString();
        string validFile = guid + FileName.Substring(FileName.IndexOf('.'));
        return validFile;
    }
    #endregion

    #region Encription And Decription

    public static string EncryptString(string EncryptedString)
    {
        byte[] byteencrString = Encoding.ASCII.GetBytes(EncryptedString);
        string encryptedConnectionString = Convert.ToBase64String(byteencrString);
        return encryptedConnectionString;
    }

    public static string DecryptString(string String)
    {
        byte[] byteencrString = Convert.FromBase64String(String);
        string decryptedConnectionString = Encoding.ASCII.GetString(byteencrString);
        return decryptedConnectionString;
    }

    #endregion

    #region E-mail

    public static void SendEmail(string FromMailId, string ToEmailId, string Subject, List<string> AttachFile, string Message)
    {
        string[] Toaddress = ToEmailId.Split(';');

        MailMessage mail = new MailMessage();
        foreach (string Address in Toaddress)
        {
            if (string.IsNullOrEmpty(Address)) continue;
            mail.To.Add(Address);
        }

        mail.From = new MailAddress(FromMailId);
        mail.Subject = Subject;
        mail.Body = Message;
        mail.IsBodyHtml = true;

        if (AttachFile != null)
        {
            if (AttachFile.Count > 0)
            {
                foreach (string file in AttachFile)
                {
                    mail.Attachments.Add(new Attachment(file));
                }
            }
        }



        string SMTPServer = "";
        SMTPServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();

        string UserName = ConfigurationManager.AppSettings["SMTPUserName"].ToString();
        string Password = ConfigurationManager.AppSettings["SMTPPassword"].ToString();

        NetworkCredential loginInfo = new NetworkCredential(UserName, Password);
        SmtpClient smtpClient = new SmtpClient(SMTPServer);
        smtpClient.Port = 587; //25;            
        smtpClient.UseDefaultCredentials = true;
        smtpClient.Credentials = loginInfo;
        smtpClient.EnableSsl = true;

        //SmtpServer.Port = 587;
        //SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
        //SmtpServer.EnableSsl = true;

        //SmtpServer.Send(mail);
        //MessageBox.Show("mail Send");
        try
        {
            smtpClient.Send(mail);
        }
        catch (Exception ex)
        {
            throw ex;
        }


    }

    public static void SendEmail(string FromMailId, string ToEmailId, string Subject, List<string> AttachFile, string Message, AlternateView AlterViewContent, string CCEmailId, int ClientDeptId = 0)
    {
        string[] Toaddress = ToEmailId.Split(';');
        string[] CCaddress = CCEmailId.Split(';');

        MailMessage mail = new MailMessage();
        foreach (string Address in Toaddress)
        {
            if (string.IsNullOrEmpty(Address)) continue;
            mail.To.Add(Address);
        }

        mail.From = new MailAddress(FromMailId);
        mail.Subject = Subject;
        mail.Body = Message;
        mail.IsBodyHtml = true;

        foreach (string Address in CCaddress)
        {
            if (string.IsNullOrEmpty(Address)) continue;
            mail.CC.Add(Address);
        }

        if (ClientDeptId == 1)
            mail.CC.Add(ConfigurationManager.AppSettings["ForBPODeptMailID"].ToString());

        mail.CC.Add(ConfigurationManager.AppSettings["SMTPUserName"].ToString());

        if (AttachFile != null)
        {
            if (AttachFile.Count > 0)
            {
                foreach (string file in AttachFile)
                {
                    mail.Attachments.Add(new Attachment(file));
                }
            }
        }

        mail.AlternateViews.Add(AlterViewContent);


        string SMTPServer = "";
        SMTPServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();

        string UserName = ConfigurationManager.AppSettings["SMTPUserName"].ToString();
        string Password = ConfigurationManager.AppSettings["SMTPPassword"].ToString();

        NetworkCredential loginInfo = new NetworkCredential(UserName, Password);
        SmtpClient smtpClient = new SmtpClient(SMTPServer);
        smtpClient.Port = 587; //25;            
        smtpClient.UseDefaultCredentials = true;
        smtpClient.Credentials = loginInfo;
        smtpClient.EnableSsl = true;

        try
        {
            smtpClient.Send(mail);
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public static void SendEmail(string FromMailId, string ToEmailId, string Subject, List<string> AttachFile, string Message, AlternateView AlterViewContent, int ClientDeptId = 0)
    {
        string[] Toaddress = ToEmailId.Split(';');

        MailMessage mail = new MailMessage();
        foreach (string Address in Toaddress)
        {
            if (string.IsNullOrEmpty(Address)) continue;
            mail.To.Add(Address);
        }

        mail.From = new MailAddress(FromMailId);
        mail.Subject = Subject;
        mail.Body = Message;
        mail.IsBodyHtml = true;

        if (ClientDeptId == 1)
            mail.CC.Add(ConfigurationManager.AppSettings["ForBPODeptMailID"].ToString());

        mail.CC.Add(ConfigurationManager.AppSettings["SMTPUserName"].ToString());

        if (AttachFile != null)
        {
            if (AttachFile.Count > 0)
            {
                foreach (string file in AttachFile)
                {
                    mail.Attachments.Add(new Attachment(file));
                }
            }
        }

        mail.AlternateViews.Add(AlterViewContent);


        string SMTPServer = "";
        SMTPServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();

        string UserName = ConfigurationManager.AppSettings["SMTPUserName"].ToString();
        string Password = ConfigurationManager.AppSettings["SMTPPassword"].ToString();

        NetworkCredential loginInfo = new NetworkCredential(UserName, Password);
        SmtpClient smtpClient = new SmtpClient(SMTPServer);
        smtpClient.Port = 587; //25;            
        smtpClient.UseDefaultCredentials = true;
        smtpClient.Credentials = loginInfo;
        smtpClient.EnableSsl = true;

        //SmtpServer.Port = 587;
        //SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
        //SmtpServer.EnableSsl = true;

        //SmtpServer.Send(mail);
        //MessageBox.Show("mail Send");
        try
        {
            smtpClient.Send(mail);
        }
        catch (Exception ex)
        {
            WriteLog(ex.Message);
            throw ex;
        }

    }


    public static void SendRegisterEmailToUser(string link, string emailId, string activationCode, string name)
    {
        try
        {
            var fromMail = new MailAddress(ConfigurationManager.AppSettings["SMTPUserName"].ToString()); // set your email    
            var fromEmailpassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString(); // Set your password     
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);
            smtp.EnableSsl = true;

            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Welcome to Cybereum Project Management - Confirm Your email for Registration";
            Message.Body = "Dear " + name + "," +
                           "<br/> We're thrilled to have you on board the cybereum project management platform! " +
                           " We're excited to help you streamline your project management process with our cutting-edge data analytics and ML integration." +
                           "<br/> To complete your registration, we need you to confirm your email address. Simply click on the link below to verify your email:" +
                           "<br/><br/><a href=" + link + ">" + link + "</a>";
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }
        catch (Exception ex)
        {
            IGUtilities.WriteLog(ex.Message);
            IGUtilities.WriteLog(ex.Data.ToString());
            if (ex.InnerException != null) IGUtilities.WriteLog(ex.InnerException.Message);
            IGUtilities.WriteLog(ex.TargetSite.ToString());
            //throw ex;
        }
    }

    public static void SendConfirmationEmailToUser(string emailId, string name, string link)
    {
        try
        {
            var fromMail = new MailAddress(ConfigurationManager.AppSettings["SMTPUserName"].ToString()); // set your email    
            var fromEmailpassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString(); // Set your password     
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);
            smtp.EnableSsl = true;

            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Welcome to cybereum - Your One-Stop Project Management Solution with Cutting Edge Data Analytics and ML";
            Message.Body = "<br/> Dear " + name + "," +
                            "<br/> We are so excited to welcome you to cybereum! You are now a part of a community of innovative individuals who are transforming the way they manage their projects and tasks." +
                            "<br/><br/> Cybereum is a powerful project management platform that combines cutting edge data analytics and ML to deliver a truly unique and comprehensive solution. With its user-friendly interface and advanced features, you'll be able to manage your projects and tasks with ease, increase efficiency, and achieve your goals like never before." +
                            "<br/> We wanted to take a moment to thank you for confirming your email and granting access to our platform. Now that you're all set up, it's time to dive in and start exploring! Here's what you can expect:" +
                            "<br/> Access to a comprehensive project management dashboard" +
                            "<br/> The ability to create projects and tasks with ease" +
                            "<br/> Assign tasks to team members with ease" +
                            "<br/> Track progress and deadlines in real - time" +
                            "<br/> Collaborate with team members in real - time" +
                            "<br/> And much more!" +
                            "<br/> We believe that cybereum will have a profound impact on your work, and we're eager to see what you'll achieve. So why wait? Start exploring and experience the power of cybereum for yourself!" +
                            "<br/><br/> Best regards," +
                            "<br/> The cybereum team.";
            //+
            //"<br/> Click below link to login." +
            //"<br/><br/><a href=" + link + ">" + link + "</a>";
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }
        catch (Exception ex)
        {
            IGUtilities.WriteLog(ex.Message);
            IGUtilities.WriteLog(ex.Data.ToString());
            if (ex.InnerException != null) IGUtilities.WriteLog(ex.InnerException.Message);
            IGUtilities.WriteLog(ex.TargetSite.ToString());
            throw ex;
        }
    }
    #endregion

    #region log

    public static void WriteLog(string Message)
    {
        try
        {
            string FilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin") + "//";
            FilePath = FilePath + string.Format("{0}{1}_log.{2}", "", DateTime.Now.ToString("MM_dd_yyyy"), "txt");
            using (StreamWriter str = (File.Exists(FilePath)) ? File.AppendText(FilePath) : File.CreateText(FilePath))
            {
                str.WriteLine(string.Format("{0} {1}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"), Message));
                str.Flush();
                str.Close();
            }
        }
        catch
        { }
    }

    #endregion

    public static int CalculateDays(DateTime startDate, DateTime endDate)
    {
        DateTime[] arrayOfOrgHolidays = new DateTime[] { };//new DateTime(2023, 05, 01)

        int noOfDays = 0;
        int count = 0;
        if (DateTime.Compare(startDate, endDate) == 1)
        {
            return 1;
        }

        while (DateTime.Compare(startDate, endDate) <= 0)
        {
            if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
            {
                string holiday = (from date in arrayOfOrgHolidays where DateTime.Compare(startDate, date) == 0 select "Holiday").FirstOrDefault();

                if (holiday != "Holiday")
                {
                    noOfDays += 1;
                    count++;
                }
                startDate = startDate.AddDays(1);
            }
            else
                startDate = startDate.AddDays(1);
        }

        return noOfDays;
    }



    public static DateTime CalculateDays(DateTime startDate, int Days)
    {
        DateTime[] arrayOfOrgHolidays = new DateTime[] { };//new DateTime(2023, 05, 01)

        for (int i = 0; i < Days - 1; i++)
        {
            if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
            {
                string holiday = (from date in arrayOfOrgHolidays where DateTime.Compare(startDate, date) == 0 select "Holiday").FirstOrDefault();

                if (holiday != "Holiday")
                {
                    startDate = startDate.AddDays(1);
                }
            }
            else
            {
                startDate = startDate.AddDays(1);
                i--;
            }
        }
        //startDate = startDate.AddDays(1);
        if (startDate.DayOfWeek == DayOfWeek.Saturday)
        {
            startDate = startDate.AddDays(2);
        }
        else if (startDate.DayOfWeek == DayOfWeek.Sunday)
        {
            startDate = startDate.AddDays(1);
        }
        return startDate;
    }

    public static DateTime AddBusinessDays(this DateTime current, int days)
    {
        var sign = Math.Sign(days);
        var unsignedDays = Math.Abs(days);
        for (var i = 0; i < unsignedDays; i++)
        {
            do
            {
                current = current.AddDays(sign);
            } while (current.DayOfWeek == DayOfWeek.Saturday ||
                     current.DayOfWeek == DayOfWeek.Sunday);
        }
        return current;
    }

    public static DateTime SubractBusinessDays(this DateTime current, int days)
    {
        var sign = Math.Sign(days);
        var unsignedDays = Math.Abs(days);        
        for (var i = 1; i < unsignedDays; i++)
        {
            do
            {
                current = current.AddDays(- sign);
            } while (current.DayOfWeek == DayOfWeek.Saturday ||
                     current.DayOfWeek == DayOfWeek.Sunday);
        }
        return current;
    }

    public static string GeneratePassword()
    {
        string OTPLength = "4";
        string OTP = string.Empty;

        string Chars = string.Empty;
        Chars = "1,2,3,4,5,6,7,8,9,0";

        char[] seplitChar = { ',' };
        string[] arr = Chars.Split(seplitChar);
        string NewOTP = "";
        string temp = "";
        Random rand = new Random();
        for (int i = 0; i < Convert.ToInt32(OTPLength); i++)
        {
            temp = arr[rand.Next(0, arr.Length)];
            NewOTP += temp;
            OTP = NewOTP;
        }
        return OTP;
    }

    public static bool AuthenticateUser(string path, string user, string pass)
    {
        DirectoryEntry de = new DirectoryEntry(path, user, pass, AuthenticationTypes.Secure);
        try
        {
            // run a search using those credentials.  
            // If it returns anything, then you're authenticated
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.FindOne();
            return true;
        }
        catch
        {
            // otherwise, it will crash out so return false
            return false;
        }
    }

    public static GremlinServer gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + gremlinvariables.database + "/colls/" + gremlinvariables.collection, password: gremlinvariables.authKey);
    public static GremlinClient gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType);
    public static ResultSet<dynamic> ExecuteGremlinScript(string script)
    {
        //var gremlinScript = script;
        //var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + gremlinvariables.database + "/colls/" + gremlinvariables.collection, password: gremlinvariables.authKey);
        //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
        //{
        var task = gremlinClient.SubmitAsync<dynamic>(script);
        task.Wait();
        var result = task.Result;
        return result;
        //}
    }

    #region activity update
    public static void updateactivityprojectdate(string projectid)
    {
        //****************************Update End Activity date****************************
        string gremlinScript1 = $"g.V().has('activity','projectid','{projectid}').has('activityname',neq('{ ConfigurationManager.AppSettings["EndActivity"] }')).order().by('enddate',decr).project('startdate','enddate').by(values('startdate')).by(values('enddate')).limit(1)";
        var result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
        DateTime dt1 = DateTime.Now;
        //DateTime enddate = DateTime.Now.AddBusinessDays(1);
        if (result1.Count > 0)
        {
            foreach (var item in result1)
            {
                dt1 = Convert.ToDateTime(item["enddate"]);
                dt1 = IGUtilities.AddBusinessDays(dt1, 1); //dt1.AddDays(1);
                //enddate = IGUtilities.AddBusinessDays(dt1, 1); //dt1.AddDays(1);
            }
            gremlinScript1 = $"g.V().has('activity','activityname','{ ConfigurationManager.AppSettings["EndActivity"] }').has('activity','projectid','{projectid}').project('id','startdate','enddate').by(values('id')).by(values('startdate')).by(values('enddate'))";
            result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
            if (result1.Count > 0)
            {
                foreach (var item in result1)
                {
                    //if (dt1 > Convert.ToDateTime(item["enddate"]))
                    //{
                    gremlinScript1 = $"g.V('{item["id"]}')" +
                                    $".property('startdate', '{dt1.ToString("yyyy-MM-dd")}')" +
                                    $".property('enddate', '{dt1.ToString("yyyy-MM-dd")}')" +
                                    $".property('projectid', '{projectid}')" +
                                    $".property('durations', '{1}')" +
                                    $".property('updatedon', '{DateTime.Now}')" +
                                    $".property('type', 'activity')";
                    result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                    //}
                }
            }
            //****************************End****************************

            //****************************Update project End date****************************
            gremlinScript1 = $"g.V().has('project','id','{projectid}').project('id','startdate','enddate').by(values('id')).by(values('startdate')).by(values('enddate'))";
            result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
            if (result1.Count > 0)
            {
                foreach (var item in result1)
                {
                    //if (dt1 > Convert.ToDateTime(item["enddate"]))
                    //{
                    gremlinScript1 = $"g.V('{item["id"]}')" +
                                    $".property('enddate', '{dt1.ToString("yyyy-MM-dd")}')" +
                                    $".property('updatedon', '{DateTime.Now}')" +
                                    $".property('type', 'project')";
                    result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                    //}
                }
            }
        }
        //****************************End****************************
    }

    public static void updateactivitydatesbytype(string activityid)
    {
        try
        {
            string gremlinScript1 = $"g.V().has('activity','id','{activityid}').project('id','activityname','durations','startdate','enddate','predecessors','linktype').by(values('id')).by(values('activityname')).by(values('durations')).by(values('startdate')).by(values('enddate')).by(values('predecessors').fold()).by(values('linktype'))";
            var result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
            if (result1.Count > 0)
            {
                foreach (var item in result1)
                {
                    DateTime startdate = DateTime.Now;
                    int days = 0;
                    DateTime todate = DateTime.Now;
                    string linktype = item["linktype"];                    
                    var predecessors = item["predecessors"];
                    var stringlist = JsonConvert.SerializeObject(predecessors);
                    var jArray = JArray.Parse(stringlist);
                    string tasks = string.Empty;
                    string[] ints = new string []{ };

                    foreach (string precedor in jArray)
                    {
                        tasks = tasks + precedor + ",";
                    }
                    if (tasks != "") tasks = tasks.Remove(tasks.LastIndexOf(",")).ToString();

                    if (Convert.ToInt16(item["linktype"]) == (int)LinkType.Finish_to_start)
                    {
                        if (tasks.ToString() != string.Empty)
                        {
                            ints = tasks.Split(',').ToArray();
                            startdate = GetPredecesenddate(ints);
                        }
                        else
                        {
                            startdate = Convert.ToDateTime(item["enddate"]);
                        }
                        days = Convert.ToInt16(item["durations"]);
                        todate = IGUtilities.CalculateDays(startdate, days);
                    }
                    else if (Convert.ToInt16(item["linktype"]) == (int)LinkType.Start_to_start)
                    {
                        if (tasks.ToString() != string.Empty)
                        {
                            ints = tasks.Split(',').ToArray();
                            startdate = GetPredecesstartdate(ints);
                        }
                        else
                        {
                            startdate = Convert.ToDateTime(item["startdate"]);
                        }
                        days = Convert.ToInt16(item["durations"]);
                        todate = IGUtilities.CalculateDays(startdate, days);
                    }
                    else if (Convert.ToInt16(item["linktype"]) == (int)LinkType.Start_to_finish)
                    {
                        if (tasks.ToString() != string.Empty)
                        {
                            ints = tasks.Split(',').ToArray();
                            todate = GetPredecesstartdate(ints);
                        }
                        else
                        {
                            todate = Convert.ToDateTime(item["startdate"]);
                        }
                        days = Convert.ToInt16(item["durations"]);
                        startdate = SubractBusinessDays(todate, days);
                    }
                    else if (Convert.ToInt16(item["linktype"]) == (int)LinkType.Finish_to_finish)
                    {
                        if (tasks.ToString() != string.Empty)
                        {
                            ints = tasks.Split(',').ToArray();
                            todate = GetPredecesenddate(ints);
                        }
                        else
                        {
                            todate = Convert.ToDateTime(item["enddate"]);
                        }
                        days = Convert.ToInt16(item["durations"]);
                        startdate = SubractBusinessDays(todate, days);
                    }


                    var gremlinScripts = $"g.V('{item["id"]}')" +
                                $".property('startdate', '{startdate.ToString("yyyy-MM-dd")}')" +
                                $".property('enddate', '{todate.ToString("yyyy-MM-dd")}')" +
                                $".property('updatedon', '{DateTime.Now}')" +
                                $".property('type', 'activity')";
                    var result = IGUtilities.ExecuteGremlinScript(gremlinScripts);
                }
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static void updateprecedingactivitydates(string projectid, string activityid)
    {
        try
        {
            var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations','predecessors','linktype').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('predecessors').fold()).by(values('linktype'))";
            var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
            string pList = JsonConvert.SerializeObject(activitydata);
            List<ProjectActivity> Activitylist = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);
            Activitylist = Activitylist.OrderBy(a => a.startdate).ThenBy(a => a.enddate).ToList();

            // finding index
            int index = Activitylist.FindIndex(a => a.id == activityid);

            //Remove previous activities
            Activitylist.RemoveRange(0, index + 1);
            //Remove end activity
            Activitylist.Remove(Activitylist.Find(m => m.activityname == ConfigurationManager.AppSettings["EndActivity"]));

            //********update start and enddate of successor activities
            foreach (var itemactivity in Activitylist)
            {
                string gremlinScript1 = $"g.V().has('activity','id','{itemactivity.id}').project('id','activityname','durations','startdate','enddate','predecessors','linktype').by(values('id')).by(values('activityname')).by(values('durations')).by(values('startdate')).by(values('enddate')).by(values('predecessors').fold()).by(values('linktype'))";
                var result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                if (result1.Count > 0)
                {
                    foreach (var item in result1)
                    {
                        DateTime precedingenddate = DateTime.Now;

                        var predecessors = item["predecessors"];
                        var stringlist = JsonConvert.SerializeObject(predecessors);
                        var jArray = JArray.Parse(stringlist);
                        string tasks = string.Empty;

                        foreach (string precedor in jArray)
                        {
                            tasks = tasks + precedor + ",";
                        }
                        if (tasks != "") tasks = tasks.Remove(tasks.LastIndexOf(",")).ToString();
                        if (tasks.ToString() != string.Empty)
                        {
                            string[] ints = tasks.Split(',').ToArray();
                            precedingenddate = GetPredecesenddate(ints);
                        }
                        else
                        {
                            precedingenddate = Convert.ToDateTime(item["startdate"]);
                        }

                        int days = Convert.ToInt16(item["durations"]);
                        DateTime todate = IGUtilities.CalculateDays(precedingenddate, days);
                        var gremlinScripts = $"g.V('{item["id"]}')" +
                                    $".property('startdate', '{precedingenddate.ToString("yyyy-MM-dd")}')" +
                                    $".property('enddate', '{todate.ToString("yyyy-MM-dd")}')" +
                                    $".property('updatedon', '{DateTime.Now}')" +
                                    $".property('type', 'activity')";
                        var result = IGUtilities.ExecuteGremlinScript(gremlinScripts);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public static DateTime GetPredecesenddate(string[] id)
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
                        //dt1 = dt1.AddBusinessDays(1);
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
        return lastactivityenddate;
    }
    
    public static DateTime GetPredecesstartdate(string[] id)
    {
        DateTime lastactivitystartdate = DateTime.Now;
        string gremlinScript = string.Empty;
        if (id != null)
        {
            for (int i = 0; i <= id.Length - 1; i++)
            {
                gremlinScript = "g.V().has('activity','id','" + id[i] + "').project('startdate').by(values('startdate'))";

                var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        DateTime dt1 = Convert.ToDateTime(item["startdate"]);
                        //dt1 = dt1.AddBusinessDays(1);
                        if (i == 0)
                        {
                            lastactivitystartdate = dt1;
                        }
                        if (dt1 > lastactivitystartdate)
                            lastactivitystartdate = dt1;
                    }

                }
            }
        }
        return lastactivitystartdate;
    }
    #endregion
}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.DirectoryServices;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Cybereum.Models;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class IGUtilities
{

    #region Converting Methods

    public static int ToInt(this object Value)
    {
        int Integer;
        int.TryParse(Convert.ToString(Value), out Integer);
        return Integer;
    }

    public static int ToNullInt(this int? Value)
    {
        if (!Value.HasValue)
        {
            return 0;
        }
        int Integer;
        int.TryParse(Convert.ToString(Value), out Integer);
        return Integer;
    }
    public static TimeSpan ToTimeSpan(this object Value)
    {
        TimeSpan Time;
        TimeSpan.TryParse(Convert.ToString(Value), out Time);
        return Time;
    }

    public static double ToDouble(this object Value)
    {
        double Double;
        double.TryParse(Convert.ToString(Value), out Double);
        return Double;
    }

    public static decimal ToDecimal(this object Value)
    {
        decimal Double;
        decimal.TryParse(Convert.ToString(Value), out Double);
        return Double;
    }
    public static DateTime TimeConversion(this string Value)
    {
        if (!string.IsNullOrEmpty(Value))
        {
            DateTime current = DateTime.Today;
            bool HasAM = true;
            HasAM = Value.Contains("am");
            Value = HasAM ? Value.Replace("am", "") : Value.Replace("pm", "");
            string[] splitarray = Value.Split(':');
            TimeSpan time = new TimeSpan(
                HasAM ? splitarray[0].ToInt() : splitarray[0].ToInt() + 12
                , splitarray[1].ToInt(), 0);
            current = current.Add(time);
            return current;
        }
        else
        {
            return DateTime.Now;
        }

    }

    public static float ToFloat(this object Value)
    {
        float Float;
        float.TryParse(Convert.ToString(Value), out Float);
        return Float;
    }

    public static DateTime ToDateTime(this object Value)
    {
        DateTime dateTime;
        DateTime.TryParse(Convert.ToString(Value), out dateTime);
        return dateTime;
    }
    public static DateTime ToDate(this object Value)
    {
        DateTime dateTime;
        DateTime.TryParse(Convert.ToString(Value), out dateTime);
        var date = dateTime.Date;
        return date;
    }

    public static bool IsValidDateTime(this object Value)
    {
        DateTime dateTime;
        DateTime.TryParse(Convert.ToString(Value), out dateTime);
        if (dateTime == DateTime.MinValue)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static DateTime ToDateTime(this object Value, string Format)
    {
        DateTime dateTime;
        DateTime.TryParseExact(Value.ToString(), Format, null, DateTimeStyles.None, out dateTime);
        return dateTime;
    }

    public static DateTime FirstDayOfMonth(this DateTime current)
    {
        return current.AddDays(1 - current.Day);
    }

    public static DateTime LastDayOfMonth(this DateTime current)
    {
        return current.AddMonths(1).AddDays(1 - current.Day).Date;
    }

    public static bool IsDate(this object Obj)
    {
        string strDate = Obj.ToString();
        try
        {
            DateTime dt = DateTime.Parse(strDate);
            if ((dt.Month != DateTime.Now.Month) || (dt.Day < 1 && dt.Day > 31) || dt.Year != DateTime.Now.Year)
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }


    #endregion

    #region Enumeration Methods
    public static List<T> GetEnumList<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }

    public static string GetEnumDescription(Enum EnumConstant)
    {
        FieldInfo fiEnum = EnumConstant.GetType().GetField(EnumConstant.ToString());
        DescriptionAttribute[] descAttribute = (DescriptionAttribute[])fiEnum.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (descAttribute.Length > 0)
        {
            return descAttribute[0].Description;
        }

        return EnumConstant.ToString();
    }

    public static string Description(this Enum Value)
    {
        FieldInfo field = Value.GetType().GetField(Value.ToString());
        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? Value.ToString() : attribute.Description;
    }

    public static T ToEnum<T>(this string value)
    {
        var type = typeof(T);
        if (!type.IsEnum) throw new InvalidOperationException();
        foreach (var field in type.GetFields())
        {
            var attribute = Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute != null)
            {
                if (attribute.Description == value)
                    return (T)field.GetValue(null);
            }
            else
            {
                if (field.Name == value)
                    return (T)field.GetValue(null);
            }
        }
        throw new ArgumentException("Not found.", "description");
        // or return default(T);
    }

    #endregion

    #region File Generation

    public static string GeneratedFileName(string FileName)
    {
        string guid = Guid.NewGuid().ToString();
        string validFile = guid + FileName.Substring(FileName.IndexOf('.'));
        return validFile;
    }
    #endregion

    #region Encription And Decription

    public static string EncryptString(string EncryptedString)
    {
        byte[] byteencrString = Encoding.ASCII.GetBytes(EncryptedString);
        string encryptedConnectionString = Convert.ToBase64String(byteencrString);
        return encryptedConnectionString;
    }

    public static string DecryptString(string String)
    {
        byte[] byteencrString = Convert.FromBase64String(String);
        string decryptedConnectionString = Encoding.ASCII.GetString(byteencrString);
        return decryptedConnectionString;
    }

    #endregion

    #region E-mail

    public static void SendEmail(string FromMailId, string ToEmailId, string Subject, List<string> AttachFile, string Message)
    {
        string[] Toaddress = ToEmailId.Split(';');

        MailMessage mail = new MailMessage();
        foreach (string Address in Toaddress)
        {
            if (string.IsNullOrEmpty(Address)) continue;
            mail.To.Add(Address);
        }

        mail.From = new MailAddress(FromMailId);
        mail.Subject = Subject;
        mail.Body = Message;
        mail.IsBodyHtml = true;

        if (AttachFile != null)
        {
            if (AttachFile.Count > 0)
            {
                foreach (string file in AttachFile)
                {
                    mail.Attachments.Add(new Attachment(file));
                }
            }
        }



        string SMTPServer = "";
        SMTPServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();

        string UserName = ConfigurationManager.AppSettings["SMTPUserName"].ToString();
        string Password = ConfigurationManager.AppSettings["SMTPPassword"].ToString();

        NetworkCredential loginInfo = new NetworkCredential(UserName, Password);
        SmtpClient smtpClient = new SmtpClient(SMTPServer);        
        smtpClient.UseDefaultCredentials = true;
        smtpClient.Credentials = loginInfo;
        smtpClient.Port = ConfigurationManager.AppSettings["SMTPPort"].ToInt();
        smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPSSL"]);

        //SmtpServer.Port = 587;
        //SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
        //SmtpServer.EnableSsl = true;

        //SmtpServer.Send(mail);
        //MessageBox.Show("mail Send");
        try
        {
            smtpClient.Send(mail);
        }
        catch (Exception ex)
        {
            throw ex;
        }


    }

    public static void SendEmail(string FromMailId, string ToEmailId, string Subject, List<string> AttachFile, string Message, AlternateView AlterViewContent, string CCEmailId, int ClientDeptId = 0)
    {
        string[] Toaddress = ToEmailId.Split(';');
        string[] CCaddress = CCEmailId.Split(';');

        MailMessage mail = new MailMessage();
        foreach (string Address in Toaddress)
        {
            if (string.IsNullOrEmpty(Address)) continue;
            mail.To.Add(Address);
        }

        mail.From = new MailAddress(FromMailId);
        mail.Subject = Subject;
        mail.Body = Message;
        mail.IsBodyHtml = true;

        foreach (string Address in CCaddress)
        {
            if (string.IsNullOrEmpty(Address)) continue;
            mail.CC.Add(Address);
        }

        if (ClientDeptId == 1)
            mail.CC.Add(ConfigurationManager.AppSettings["ForBPODeptMailID"].ToString());

        mail.CC.Add(ConfigurationManager.AppSettings["SMTPUserName"].ToString());

        if (AttachFile != null)
        {
            if (AttachFile.Count > 0)
            {
                foreach (string file in AttachFile)
                {
                    mail.Attachments.Add(new Attachment(file));
                }
            }
        }

        mail.AlternateViews.Add(AlterViewContent);


        string SMTPServer = "";
        SMTPServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();

        string UserName = ConfigurationManager.AppSettings["SMTPUserName"].ToString();
        string Password = ConfigurationManager.AppSettings["SMTPPassword"].ToString();

        NetworkCredential loginInfo = new NetworkCredential(UserName, Password);
        SmtpClient smtpClient = new SmtpClient(SMTPServer);        
        smtpClient.UseDefaultCredentials = true;
        smtpClient.Credentials = loginInfo;
        smtpClient.Port = ConfigurationManager.AppSettings["SMTPPort"].ToInt();
        smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPSSL"]);

        try
        {
            smtpClient.Send(mail);
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public static void SendEmail(string FromMailId, string ToEmailId, string Subject, List<string> AttachFile, string Message, AlternateView AlterViewContent, int ClientDeptId = 0)
    {
        string[] Toaddress = ToEmailId.Split(';');

        MailMessage mail = new MailMessage();
        foreach (string Address in Toaddress)
        {
            if (string.IsNullOrEmpty(Address)) continue;
            mail.To.Add(Address);
        }

        mail.From = new MailAddress(FromMailId);
        mail.Subject = Subject;
        mail.Body = Message;
        mail.IsBodyHtml = true;

        if (ClientDeptId == 1)
            mail.CC.Add(ConfigurationManager.AppSettings["ForBPODeptMailID"].ToString());

        mail.CC.Add(ConfigurationManager.AppSettings["SMTPUserName"].ToString());

        if (AttachFile != null)
        {
            if (AttachFile.Count > 0)
            {
                foreach (string file in AttachFile)
                {
                    mail.Attachments.Add(new Attachment(file));
                }
            }
        }

        mail.AlternateViews.Add(AlterViewContent);


        string SMTPServer = "";
        SMTPServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();

        string UserName = ConfigurationManager.AppSettings["SMTPUserName"].ToString();
        string Password = ConfigurationManager.AppSettings["SMTPPassword"].ToString();

        NetworkCredential loginInfo = new NetworkCredential(UserName, Password);
        SmtpClient smtpClient = new SmtpClient(SMTPServer);        
        smtpClient.UseDefaultCredentials = true;
        smtpClient.Credentials = loginInfo;
        smtpClient.Port = ConfigurationManager.AppSettings["SMTPPort"].ToInt();
        smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPSSL"]);

        //SmtpServer.Port = 587;
        //SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
        //SmtpServer.EnableSsl = true;

        //SmtpServer.Send(mail);
        //MessageBox.Show("mail Send");
        try
        {
            smtpClient.Send(mail);
        }
        catch (Exception ex)
        {
            WriteLog(ex.Message);
            throw ex;
        }

    }


    public static void SendRegisterEmailToUser(string link, string emailId, string activationCode, string name)
    {
        try
        {
            var fromMail = new MailAddress(ConfigurationManager.AppSettings["SMTPUserName"].ToString()); // set your email    
            var fromEmailpassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString(); // Set your password     
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);
            smtp.Port = ConfigurationManager.AppSettings["SMTPPort"].ToInt();
            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPSSL"]);

            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Welcome to Cybereum Project Management - Confirm Your email for Registration";
            Message.Body = "Dear " + name + "," +
                           "<br/> We're thrilled to have you on board the cybereum project management platform! " +
                           " We're excited to help you streamline your project management process with our cutting-edge data analytics and ML integration." +
                           "<br/> To complete your registration, we need you to confirm your email address. Simply click on the link below to verify your email:" +
                           "<br/><br/><a href=" + link + ">" + link + "</a>";
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }
        catch (Exception ex)
        {
            IGUtilities.WriteLog(ex.Message);
            IGUtilities.WriteLog(ex.Data.ToString());
            if (ex.InnerException != null) IGUtilities.WriteLog(ex.InnerException.Message);
            IGUtilities.WriteLog(ex.TargetSite.ToString());
            //throw ex;
        }
    }

    public static void SendConfirmationEmailToUser(string emailId, string name, string link, string password = "")
    {
        try
        {
            var fromMail = new MailAddress(ConfigurationManager.AppSettings["SMTPUserName"].ToString()); // set your email    
            var fromEmailpassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString(); // Set your password     
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);
            smtp.Port = ConfigurationManager.AppSettings["SMTPPort"].ToInt();
            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPSSL"]);

            var Message = new MailMessage(fromMail, toEmail);
            string msgpassword = string.Empty;
            if (password != "")
            {
                msgpassword = "<br/><br/> To access the Cybereum portal, click the link and enter the credentials mentioned below." +
                    "<br/><a href=" + link + ">" + link + "</a>" +
                    "<br/> Emailid:" + emailId +
                    "</br> Password:" + password;
            }

            Message.Subject = "Welcome to cybereum - Your One-Stop Project Management Solution with Cutting Edge Data Analytics and ML";
            Message.Body = "<br/> Dear " + name + "," +
                            "<br/> We are so excited to welcome you to cybereum! You are now a part of a community of innovative individuals who are transforming the way they manage their projects and tasks." +
                            "<br/><br/> Cybereum is a powerful project management platform that combines cutting edge data analytics and ML to deliver a truly unique and comprehensive solution. With its user-friendly interface and advanced features, you'll be able to manage your projects and tasks with ease, increase efficiency, and achieve your goals like never before." +
                            "<br/> We wanted to take a moment to thank you for confirming your email and granting access to our platform. Now that you're all set up, it's time to dive in and start exploring! Here's what you can expect:" +
                            "<br/> Access to a comprehensive project management dashboard" +
                            "<br/> The ability to create projects and tasks with ease" +
                            "<br/> Assign tasks to team members with ease" +
                            "<br/> Track progress and deadlines in real - time" +
                            "<br/> Collaborate with team members in real - time" +
                            "<br/> And much more!" +
                            "<br/> We believe that cybereum will have a profound impact on your work, and we're eager to see what you'll achieve. So why wait? Start exploring and experience the power of cybereum for yourself!" +
                            msgpassword +
                            "<br/><br/> Best regards," +
                            "<br/> The cybereum team.";
            //+
            //"<br/> Click below link to login." +
            //"<br/><br/><a href=" + link + ">" + link + "</a>";
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }
        catch (Exception ex)
        {
            IGUtilities.WriteLog(ex.Message);
            IGUtilities.WriteLog(ex.Data.ToString());
            if (ex.InnerException != null) IGUtilities.WriteLog(ex.InnerException.Message);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
            IGUtilities.WriteLog(ex.TargetSite.ToString());
            throw ex;
        }
    }


    public static void SendRejectEmailToUser(string emailId, string name)
    {
        try
        {
            var fromMail = new MailAddress(ConfigurationManager.AppSettings["SMTPUserName"].ToString()); // set your email    
            var fromEmailpassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString(); // Set your password     
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);
            smtp.Port = ConfigurationManager.AppSettings["SMTPPort"].ToInt();
            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SMTPSSL"]);

            var Message = new MailMessage(fromMail, toEmail);
            
            Message.Subject = "Cybereum - Registration Rejected";
            Message.Body = "<br/> Hello " + name + "," +
                            "<br/> Your request for Cybereum platform access is rejected. Contact anand@cybereum.io for further information." +
                            "<br/><br/> Best regards," +
                            "<br/> The cybereum team.";
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }
        catch (Exception ex)
        {
            IGUtilities.WriteLog(ex.Message);
            IGUtilities.WriteLog(ex.Data.ToString());
            if (ex.InnerException != null) IGUtilities.WriteLog(ex.InnerException.Message);
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            IGUtilities.WriteLog(ex.TargetSite.ToString());
            throw ex;
        }
    }
    #endregion

    #region log

    public static void WriteLog(string Message)
    {
        try
        {
            string FilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin") + "//";
            FilePath = FilePath + string.Format("{0}{1}_log.{2}", "", DateTime.Now.ToString("MM_dd_yyyy"), "txt");
            using (StreamWriter str = (File.Exists(FilePath)) ? File.AppendText(FilePath) : File.CreateText(FilePath))
            {
                str.WriteLine(string.Format("{0} {1}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"), Message));
                str.Flush();
                str.Close();
            }
        }
        catch
        { }
    }

    #endregion

    public static int CalculateDays(DateTime startDate, DateTime endDate)
    {
        DateTime[] arrayOfOrgHolidays = new DateTime[] { };//new DateTime(2023, 05, 01)

        int noOfDays = 0;
        int count = 0;
        if (DateTime.Compare(startDate, endDate) == 1)
        {
            return 1;
        }

        while (DateTime.Compare(startDate, endDate) <= 0)
        {
            if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
            {
                string holiday = (from date in arrayOfOrgHolidays where DateTime.Compare(startDate, date) == 0 select "Holiday").FirstOrDefault();

                if (holiday != "Holiday")
                {
                    noOfDays += 1;
                    count++;
                }
                startDate = startDate.AddDays(1);
            }
            else
                startDate = startDate.AddDays(1);
        }

        return noOfDays;
    }



    public static DateTime CalculateDays(DateTime startDate, int Days)
    {
        DateTime[] arrayOfOrgHolidays = new DateTime[] { };//new DateTime(2023, 05, 01)

        for (int i = 0; i < Days - 1; i++)
        {
            if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
            {
                string holiday = (from date in arrayOfOrgHolidays where DateTime.Compare(startDate, date) == 0 select "Holiday").FirstOrDefault();

                if (holiday != "Holiday")
                {
                    startDate = startDate.AddDays(1);
                }
            }
            else
            {
                startDate = startDate.AddDays(1);
                i--;
            }
        }
        //startDate = startDate.AddDays(1);
        if (startDate.DayOfWeek == DayOfWeek.Saturday)
        {
            startDate = startDate.AddDays(2);
        }
        else if (startDate.DayOfWeek == DayOfWeek.Sunday)
        {
            startDate = startDate.AddDays(1);
        }
        return startDate;
    }

    public static DateTime AddBusinessDays(this DateTime current, int days)
    {
        var sign = Math.Sign(days);
        var unsignedDays = Math.Abs(days);
        for (var i = 0; i < unsignedDays; i++)
        {
            do
            {
                current = current.AddDays(sign);
            } while (current.DayOfWeek == DayOfWeek.Saturday ||
                     current.DayOfWeek == DayOfWeek.Sunday);
        }
        return current;
    }

    public static DateTime SubractBusinessDays(this DateTime current, int days)
    {
        var sign = Math.Sign(days);
        var unsignedDays = Math.Abs(days);
        for (var i = 1; i < unsignedDays; i++)
        {
            do
            {
                current = current.AddDays(-sign);
            } while (current.DayOfWeek == DayOfWeek.Saturday ||
                     current.DayOfWeek == DayOfWeek.Sunday);
        }
        return current;
    }

    public static string GeneratePassword()
    {
        string OTPLength = "4";
        string OTP = string.Empty;

        string Chars = string.Empty;
        Chars = "1,2,3,4,5,6,7,8,9,0";

        char[] seplitChar = { ',' };
        string[] arr = Chars.Split(seplitChar);
        string NewOTP = "";
        string temp = "";
        Random rand = new Random();
        for (int i = 0; i < Convert.ToInt32(OTPLength); i++)
        {
            temp = arr[rand.Next(0, arr.Length)];
            NewOTP += temp;
            OTP = NewOTP;
        }
        return OTP;
    }

    public static bool AuthenticateUser(string path, string user, string pass)
    {
        DirectoryEntry de = new DirectoryEntry(path, user, pass, AuthenticationTypes.Secure);
        try
        {
            // run a search using those credentials.  
            // If it returns anything, then you're authenticated
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.FindOne();
            return true;
        }
        catch
        {
            // otherwise, it will crash out so return false
            return false;
        }
    }

    public static GremlinServer gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + gremlinvariables.database + "/colls/" + gremlinvariables.collection, password: gremlinvariables.authKey);
    public static GremlinClient gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType);
    public static ResultSet<dynamic> ExecuteGremlinScript(string script)
    {
        //var gremlinScript = script;
        //var gremlinServer = new GremlinServer(gremlinvariables.hostname, gremlinvariables.port, enableSsl: true, username: "/dbs/" + gremlinvariables.database + "/colls/" + gremlinvariables.collection, password: gremlinvariables.authKey);
        //using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
        //{
        var task = gremlinClient.SubmitAsync<dynamic>(script);
        task.Wait();
        var result = task.Result;
        return result;
        //}
    }
    public static cybereumEntities db = new cybereumEntities();

    public static List<SelectListItem> FillPM(int userid)
    {
        List<SelectListItem> user = new List<SelectListItem>();

        var org = (from b in db.tbl_user
                   where b.userid == userid
                   select b.organization).Single();
        string organization = org.ToString();

        user = (from b in db.tbl_user
                join c in db.tbl_userrole on b.roleid equals c.roleid
                where b.isactive == 1 && (b.roleid == 2 || b.roleid == 3) && b.organization == organization
                select new SelectListItem
                {
                    Text = b.firstname + " " + b.lastname + "-" + c.rolename + "-" + b.organization,
                    Value = b.userid.ToString()
                }).Distinct().OrderBy(x => x.Text).ToList();
        return user;
    }

    public static string getprojectmembers(int userid)
    {
        var user = IGUtilities.FillPM(userid);
        string projectmember = "";
        foreach (var items in user.ToArray())
        {
            projectmember = (projectmember == "" ? projectmember : projectmember + ",") + "'" + items.Value + "'";
        }
        return projectmember;
    }

    #region activity update
    public static string getlastactivityid(string activityname, string projectid)
    {
        string id = string.Empty;
        var gremlinScript = "g.V().has('activity','activityname','" + activityname + "').has('activity','projectid','" + projectid + "').project('id').by(values('id'))";
        var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
        foreach (var result1 in result)
        {
            id = Convert.ToString(result1["id"]);
        }
        return id;
    }
    public static void updateactivityprojectdate(string projectid)
    {
        //****************************Update End Activity date****************************
        string gremlinScript1 = $"g.V().has('activity','projectid','{projectid}').has('activityname',neq('{ ConfigurationManager.AppSettings["EndActivity"] }')).order().by('enddate',decr).project('startdate','enddate').by(values('startdate')).by(values('enddate')).limit(1)";
        var result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
        DateTime dt1 = DateTime.Now;
        //DateTime enddate = DateTime.Now.AddBusinessDays(1);
        if (result1.Count > 0)
        {
            foreach (var item in result1)
            {
                dt1 = Convert.ToDateTime(item["enddate"]);
                dt1 = IGUtilities.AddBusinessDays(dt1, 1); //dt1.AddDays(1);                
            }
            gremlinScript1 = $"g.V().has('activity','activityname','{ ConfigurationManager.AppSettings["EndActivity"] }').has('activity','projectid','{projectid}').project('id','startdate','enddate').by(values('id')).by(values('startdate')).by(values('enddate'))";
            result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
            if (result1.Count > 0)
            {
                foreach (var item in result1)
                {
                    //if (dt1 > Convert.ToDateTime(item["enddate"]))
                    //{
                    gremlinScript1 = $"g.V('{item["id"]}')" +
                                    $".property('startdate', '{dt1.ToString("yyyy-MM-dd")}')" +
                                    $".property('enddate', '{dt1.ToString("yyyy-MM-dd")}')" +
                                    $".property('projectid', '{projectid}')" +
                                    $".property('durations', '{1}')" +
                                    $".property('updatedon', '{DateTime.Now}')" +
                                    $".property('type', 'activity')";
                    result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                    //}
                }
            }
            //****************************End****************************

            //****************************Update project End date****************************
            gremlinScript1 = $"g.V().has('project','id','{projectid}').project('id','startdate','enddate').by(values('id')).by(values('startdate')).by(values('enddate'))";
            result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
            if (result1.Count > 0)
            {
                foreach (var item in result1)
                {
                    //if (dt1 > Convert.ToDateTime(item["enddate"]))
                    //{
                    gremlinScript1 = $"g.V('{item["id"]}')" +
                                    $".property('enddate', '{dt1.ToString("yyyy-MM-dd")}')" +
                                    $".property('updatedon', '{DateTime.Now}')" +
                                    $".property('type', 'project')";
                    result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                    //}
                }
            }
        }
        //****************************End****************************
    }

    public static void updateactivitydatesbytype(string projectid, string activityid)
    {
        try
        {
            string gremlinScript1 = $"g.V().has('activity','id','{activityid}').project('id','activityname','durations','startdate','enddate','predecessors','linktype').by(values('id')).by(values('activityname')).by(values('durations')).by(values('startdate')).by(values('enddate')).by(values('predecessors').fold()).by(values('linktype'))";
            var result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
            if (result1.Count > 0)
            {
                foreach (var item in result1)
                {
                    DateTime startdate = DateTime.Now;
                    int days = 0;
                    DateTime todate = DateTime.Now;
                    string linktype = item["linktype"];
                    var predecessors = item["predecessors"];
                    var stringlist = JsonConvert.SerializeObject(predecessors);
                    var jArray = JArray.Parse(stringlist);
                    string tasks = string.Empty;
                    string[] ints = new string[] { };

                    foreach (string precedor in jArray)
                    {
                        tasks = tasks + precedor + ",";
                    }
                    if (tasks != "") tasks = tasks.Remove(tasks.LastIndexOf(",")).ToString();

                    if (Convert.ToInt16(item["linktype"]) == (int)LinkType.Finish_to_start)
                    {
                        if (tasks.ToString() != string.Empty)
                        {
                            ints = tasks.Split(',').ToArray();
                            startdate = GetPredecesenddate(ints);
                        }
                        else
                        {
                            startdate = Convert.ToDateTime(item["enddate"]);
                        }
                        days = Convert.ToInt16(item["durations"]);
                        todate = IGUtilities.CalculateDays(startdate, days);
                    }
                    else if (Convert.ToInt16(item["linktype"]) == (int)LinkType.Start_to_start)
                    {
                        if (tasks.ToString() != string.Empty)
                        {
                            ints = tasks.Split(',').ToArray();
                            startdate = GetPredecesstartdate(ints);
                        }
                        else
                        {
                            startdate = Convert.ToDateTime(item["startdate"]);
                        }
                        days = Convert.ToInt16(item["durations"]);
                        todate = IGUtilities.CalculateDays(startdate, days);
                    }
                    else if (Convert.ToInt16(item["linktype"]) == (int)LinkType.Start_to_finish)
                    {
                        if (tasks.ToString() != string.Empty)
                        {
                            ints = tasks.Split(',').ToArray();
                            todate = GetPredecesstartdate(ints);
                        }
                        else
                        {
                            todate = Convert.ToDateTime(item["startdate"]);
                        }
                        days = Convert.ToInt16(item["durations"]);
                        startdate = SubractBusinessDays(todate, days);
                    }
                    else if (Convert.ToInt16(item["linktype"]) == (int)LinkType.Finish_to_finish)
                    {
                        if (tasks.ToString() != string.Empty)
                        {
                            ints = tasks.Split(',').ToArray();
                            todate = GetPredecesenddate(ints);
                        }
                        else
                        {
                            todate = Convert.ToDateTime(item["enddate"]);
                        }
                        days = Convert.ToInt16(item["durations"]);
                        startdate = SubractBusinessDays(todate, days);
                    }

                    if (item["activityname"] != ConfigurationManager.AppSettings["StartActivity"].ToString())
                    {
                        //**********Checking for task start and end date*************
                        var enddate = Getstartactivitydate(projectid, startdate);
                        string pList = JsonConvert.SerializeObject(enddate);
                        ProjectActivity newtask = new ProjectActivity();
                        newtask = JsonConvert.DeserializeObject<ProjectActivity>(pList);
                        if (newtask.startdate != Convert.ToDateTime("01/01/0001"))
                        {
                            startdate = newtask.startdate;
                            todate = newtask.enddate;
                            days = newtask.durations.ToInt();
                        }
                        //**********End*********
                    }

                    var gremlinScripts = $"g.V('{item["id"]}')" +
                                $".property('startdate', '{startdate.ToString("yyyy-MM-dd")}')" +
                                $".property('enddate', '{todate.ToString("yyyy-MM-dd")}')" +
                                $".property('duration', '{days}')" +
                                $".property('updatedon', '{DateTime.Now}')" +
                                $".property('type', 'activity')";
                    var result = IGUtilities.ExecuteGremlinScript(gremlinScripts);
                }
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static ProjectActivity Getstartactivitydate(string projectid, DateTime startdate)
    {
        ProjectActivity activity = new ProjectActivity();
        var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').has('activity','activityname','" + ConfigurationManager.AppSettings["StartActivity"] + "').project('startdate','enddate','durations').by(values('startdate')).by(values('enddate')).by(values('durations'))";
        var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
        if (result.Count > 0)
        {
            foreach (var item in result)
            {
                if (startdate < Convert.ToDateTime(item["startdate"]))
                {
                    activity.startdate = Convert.ToDateTime(item["startdate"]);
                    activity.enddate = Convert.ToDateTime(item["enddate"]);
                    activity.durations = Convert.ToInt16(item["durations"]);
                }
            }
        }
        return activity;
    }

    public static void updateprecedingactivitydates(string projectid, string activityid)
    {
        try
        {
            var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').order().by('startdate',incr).order().by('enddate',incr).project('id','activityname','startdate','enddate','durations','predecessors','linktype').by(id()).by(values('activityname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('predecessors').fold()).by(values('linktype'))";
            var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
            string pList = JsonConvert.SerializeObject(activitydata);
            List<ProjectActivity> Activitylist = JsonConvert.DeserializeObject<List<ProjectActivity>>(pList);
            Activitylist = Activitylist.OrderBy(a => a.startdate).ThenBy(a => a.enddate).ToList();

            // finding index
            int index = Activitylist.FindIndex(a => a.id == activityid);

            //Remove previous activities
            Activitylist.RemoveRange(0, index + 1);
            //Remove end activity
            Activitylist.Remove(Activitylist.Find(m => m.activityname == ConfigurationManager.AppSettings["EndActivity"]));

            //********update start and enddate of successor activities
            foreach (var itemactivity in Activitylist)
            {
                string gremlinScript1 = $"g.V().has('activity','id','{itemactivity.id}').project('id','activityname','durations','startdate','enddate','predecessors','linktype').by(values('id')).by(values('activityname')).by(values('durations')).by(values('startdate')).by(values('enddate')).by(values('predecessors').fold()).by(values('linktype'))";
                var result1 = IGUtilities.ExecuteGremlinScript(gremlinScript1);
                if (result1.Count > 0)
                {
                    foreach (var item in result1)
                    {
                        DateTime precedingenddate = DateTime.Now;

                        var predecessors = item["predecessors"];
                        var stringlist = JsonConvert.SerializeObject(predecessors);
                        var jArray = JArray.Parse(stringlist);
                        string tasks = string.Empty;

                        foreach (string precedor in jArray)
                        {
                            tasks = tasks + precedor + ",";
                        }
                        if (tasks != "") tasks = tasks.Remove(tasks.LastIndexOf(",")).ToString();
                        if (tasks.ToString() != string.Empty)
                        {
                            string[] ints = tasks.Split(',').ToArray();
                            precedingenddate = GetPredecesenddate(ints);
                        }
                        else
                        {
                            precedingenddate = Convert.ToDateTime(item["startdate"]);
                        }

                        int days = Convert.ToInt16(item["durations"]);
                        DateTime todate = IGUtilities.CalculateDays(precedingenddate, days);
                        var gremlinScripts = $"g.V('{item["id"]}')" +
                                    $".property('startdate', '{precedingenddate.ToString("yyyy-MM-dd")}')" +
                                    $".property('enddate', '{todate.ToString("yyyy-MM-dd")}')" +
                                    $".property('updatedon', '{DateTime.Now}')" +
                                    $".property('type', 'activity')";
                        var result = IGUtilities.ExecuteGremlinScript(gremlinScripts);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static void updateTaskdates(string activityid)
    {
        try
        {
            var gremlinScript = "g.V().has('task','activityid','" + activityid + "').project('taskid','taskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdusername','createdon','activityid','progress')" +
                    ".by(id()).by(values('taskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('tasktype')).by(values('taskstatus')).by(values('assignedto')).by(values('createdby')).by(values('createdusername')).by(values('createdon')).by(values('activityid')).by(values('progress'))";
            var taskdata = IGUtilities.ExecuteGremlinScript(gremlinScript);
            string pList = JsonConvert.SerializeObject(taskdata);
            List<ProjectTask> Tasklist = JsonConvert.DeserializeObject<List<ProjectTask>>(pList);            
                        
            //********update start and enddate of task
            foreach (var itemtask in Tasklist)
            {
                //**********Checking for task start and end date*************
                //var enddate = CheckActivityEnddate(tbl_task.activityid, tbl_task.enddate);
                var newtask = CheckActivitydates(itemtask.activityid, itemtask.startdate, itemtask.enddate);                
                if (newtask.startdate != Convert.ToDateTime("01/01/0001"))
                {
                    itemtask.startdate = newtask.startdate;
                    itemtask.enddate = newtask.enddate;
                    itemtask.durations = newtask.durations;
                }
                var gremlinScripts = $"g.V('{itemtask.taskid}')" +
                                    $".property('startdate', '{itemtask.startdate.ToString("yyyy-MM-dd")}')" +
                                    $".property('enddate', '{itemtask.enddate.ToString("yyyy-MM-dd")}')" +
                                    $".property('duration', '{itemtask.durations}')" +
                                    $".property('updatedon', '{DateTime.Now}')" +
                                    $".property('type', 'task')";
                var result = IGUtilities.ExecuteGremlinScript(gremlinScripts);
                //**********End*********                
                ////***************update SubTask dates*************
                IGUtilities.updateSubTaskdates(itemtask.taskid);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static ProjectTask CheckActivitydates(string activityid, DateTime startdate, DateTime enddate)
    {
        ProjectTask task = new ProjectTask();
        var gremlinScript = "g.V().has('activity','id','" + activityid + "').project('startdate','enddate','durations').by(values('startdate')).by(values('enddate')).by(values('durations'))";
        var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
        if (result.Count > 0)
        {
            foreach (var item in result)
            {
                if (enddate > Convert.ToDateTime(item["enddate"]) || startdate < Convert.ToDateTime(item["startdate"]))
                {
                    task.startdate = Convert.ToDateTime(item["startdate"]);
                    task.enddate = Convert.ToDateTime(item["enddate"]);
                    task.durations = Convert.ToInt16(item["durations"]);
                }
            }
        }
        return task;
    }

    public static void updateSubTaskdates(string taskid)
    {
        try
        {
            var gremlinScript = "g.V().has('subtask','id','" + taskid + "').project('id','subtaskname','startdate','enddate','durations','tasktype','taskstatus','assignedto','createdby','createdon','taskid','progress')" +
                    ".by(id()).by(values('subtaskname')).by(values('startdate')).by(values('enddate')).by(values('durations')).by(values('tasktype')).by(values('taskstatus')).by(values('assignedto')).by(values('createdby')).by(values('createdon')).by(values('taskid')).by(values('progress'))";
            var taskdata = IGUtilities.ExecuteGremlinScript(gremlinScript);
            string pList = JsonConvert.SerializeObject(taskdata);
            List<ProjectSubTask> Tasklist = JsonConvert.DeserializeObject<List<ProjectSubTask>>(pList);

            //********update start and enddate of task
            foreach (var itemtask in Tasklist)
            {
                //**********Checking for task start and end date*************
                //var enddate = CheckActivityEnddate(tbl_task.activityid, tbl_task.enddate);
                var newtask = CheckTaskdates(itemtask.taskid, itemtask.startdate, itemtask.enddate);
                if (newtask.startdate != Convert.ToDateTime("01/01/0001"))
                {
                    itemtask.startdate = newtask.startdate;
                    itemtask.enddate = newtask.enddate;
                    itemtask.durations = newtask.durations;
                }
                var gremlinScripts = $"g.V('{itemtask.subtaskid}')" +
                                    $".property('startdate', '{itemtask.startdate.ToString("yyyy-MM-dd")}')" +
                                    $".property('enddate', '{itemtask.enddate.ToString("yyyy-MM-dd")}')" +
                                    $".property('duration', '{itemtask.durations}')" +
                                    $".property('updatedon', '{DateTime.Now}')" +
                                    $".property('type', 'subtask')";
                var result = IGUtilities.ExecuteGremlinScript(gremlinScripts);
                //**********End*********                
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static ProjectSubTask CheckTaskdates(string taskid, DateTime startdate, DateTime enddate)
    {
        ProjectSubTask subtask = new ProjectSubTask();
        var gremlinScript = "g.V().has('task','id','" + taskid + "').project('startdate','enddate','durations').by(values('startdate')).by(values('enddate')).by(values('durations'))";
        var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
        if (result.Count > 0)
        {
            foreach (var item in result)
            {
                if (enddate > Convert.ToDateTime(item["enddate"]) || startdate < Convert.ToDateTime(item["startdate"]))
                {
                    subtask.startdate = Convert.ToDateTime(item["startdate"]);
                    subtask.enddate = Convert.ToDateTime(item["enddate"]);
                    subtask.durations = Convert.ToInt16(item["durations"]);
                }
            }
        }
        return subtask;
    }

    public static DateTime GetPredecesenddate(string[] id)
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
                        //dt1 = dt1.AddBusinessDays(1);
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
        return lastactivityenddate;
    }

    public static DateTime GetPredecesstartdate(string[] id)
    {
        DateTime lastactivitystartdate = DateTime.Now;
        string gremlinScript = string.Empty;
        if (id != null)
        {
            for (int i = 0; i <= id.Length - 1; i++)
            {
                gremlinScript = "g.V().has('activity','id','" + id[i] + "').project('startdate').by(values('startdate'))";

                var result = IGUtilities.ExecuteGremlinScript(gremlinScript);
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                        DateTime dt1 = Convert.ToDateTime(item["startdate"]);
                        //dt1 = dt1.AddBusinessDays(1);
                        if (i == 0)
                        {
                            lastactivitystartdate = dt1;
                        }
                        if (dt1 > lastactivitystartdate)
                            lastactivitystartdate = dt1;
                    }

                }
            }
        }
        return lastactivitystartdate;
    }

    public static void updatetaskprogress(string taskid)
    {
        //Update Task progress
        var gremlinScript = "g.V().has('subtask','taskid','" + taskid + "').project('progress').by(values('progress'))";
        var results = ExecuteGremlinScript(gremlinScript);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        int progress = 0;
=======
        int progress = 0;        
>>>>>>> Stashed changes
=======
        int progress = 0;        
>>>>>>> Stashed changes
        foreach (var result in results)
        {
            progress += Convert.ToInt16(result["progress"]);
        }
        progress = (progress / results.Count);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        //if (progress > 0)
        {
            gremlinScript = $"g.V('{taskid}').property('progress', '{progress}')";
=======
=======
>>>>>>> Stashed changes
        
        //if (progress > 0)
        {
            if (progress == 100)
            {
                int taskstatus = (int)TaskSubTaskStatus.Completed;
                gremlinScript = $"g.V('{taskid}').property('progress', '{progress}').property('taskstatus','{taskstatus}')";
            }
            else
            {
                gremlinScript = $"g.V('{taskid}').property('progress', '{progress}')";
            }
            
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            var res = IGUtilities.ExecuteGremlinScript(gremlinScript);

            gremlinScript = $"g.V('task','id','{taskid}').project('id','activityid').by(id()).by(values('activityid'))";
            res = IGUtilities.ExecuteGremlinScript(gremlinScript);
            foreach (var result in res)
            {
                updateactivityprogress(result["activityid"]);
            }
        }
    }

    public static void updateactivityprogress(string activityid)
    {
        //Update Activity progress
        var gremlinScript = "g.V().has('task','activityid','" + activityid + "').project('progress').by(values('progress'))";
        var results = ExecuteGremlinScript(gremlinScript);
        int progress = 0;
        foreach (var result in results)
        {
            progress += Convert.ToInt16(result["progress"]);
        }
        progress = progress / results.Count;
        //if (progress > 0)
        {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            gremlinScript = $"g.V('{activityid}').property('progress', '{progress}')";
=======
=======
>>>>>>> Stashed changes
            if (progress == 100)
            {
                int taskstatus = (int)TaskSubTaskStatus.Completed;
                gremlinScript = $"g.V('{activityid}').property('progress', '{progress}').property('activitystatus','{taskstatus}')";
            }
            else
            {
                gremlinScript = $"g.V('{activityid}').property('progress', '{progress}')";
            }
            
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            var res = IGUtilities.ExecuteGremlinScript(gremlinScript);

            gremlinScript = $"g.V('activity','id','{activityid}').project('id','projectid').by(id()).by(values('projectid'))";
            res = IGUtilities.ExecuteGremlinScript(gremlinScript);
            foreach (var result in res)
            {
                updateprojectprogress(result["projectid"]);
            }
        }
    }

    public static void updateprojectprogress(string projectid)
    {
        //Update Activity progress
        var gremlinScript = "g.V().has('activity','projectid','" + projectid + "').project('progress').by(values('progress'))";
        var results = ExecuteGremlinScript(gremlinScript);
        int progress = 0;
        foreach (var result in results)
        {
            progress += Convert.ToInt16(result["progress"]);
        }
        progress = progress / results.Count;
        //if (progress > 0)
        {
            gremlinScript = $"g.V('{projectid}').property('progress', '{progress}')";
            var res = IGUtilities.ExecuteGremlinScript(gremlinScript);
        }
    }
    #endregion
}
>>>>>>> Stashed changes
