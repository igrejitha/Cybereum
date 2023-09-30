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

    public class tblkeyinternal
    {
        public string activityname { get; set; }
        public string progress { get; set; }
    }

    public class tbltaskcompletion
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        public string status { get; set; }
        public int taskcount { get; set; }
=======
        public string label { get; set; }
        public int value { get; set; }
>>>>>>> Stashed changes
=======
        public string label { get; set; }
        public int value { get; set; }
>>>>>>> Stashed changes
    }

}