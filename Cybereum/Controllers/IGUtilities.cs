using System;
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
            IGUtilities.WriteLog(ex.InnerException.Message);
            IGUtilities.WriteLog(ex.TargetSite.ToString());
            throw ex;
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
            smtp.Port = 25;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

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
            IGUtilities.WriteLog(ex.InnerException.Message);
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

        if (startDate.DayOfWeek == DayOfWeek.Saturday )
        {
            startDate = startDate.AddDays(2);
        }
        else if (startDate.DayOfWeek == DayOfWeek.Sunday)
        {
            startDate = startDate.AddDays(1);
        }
        return startDate;
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
}
