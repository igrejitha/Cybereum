using Newtonsoft.Json.Converters;
using static Cybereum.Models.Activity;
using System.Globalization;

namespace Cybereum.Models
{
    class JsonDateConverter : IsoDateTimeConverter
    {
        public JsonDateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";            
        }

    }


}