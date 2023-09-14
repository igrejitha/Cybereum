using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Cybereum.Models
{
    public class GanttRequest
    {        
        public enum GanttMode
        {
            Tasks,
            Links
        }

        public enum GanttAction
        {
            Inserted,
            Updated,
            Deleted,
            Error
        }
        
    }
}