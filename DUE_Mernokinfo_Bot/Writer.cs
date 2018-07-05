using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUE_Mernokinfo_Bot
{
    public class Writer
    {
        public int WEventId { get; set; }
        public int WUserId { get; set; }
        public DateTime WStartDate { get; set; }
        public DateTime WEndDate { get; set; }
        public string WSubjectCode { get; set; }
        public string WClassCode { get; set; }
        public bool WZh { get; set; }
        public const string wSubjectcode = "Esemény: ";
        public const string wStartdate = "Kezdődátum: ";
        public const string wEnddate = "Végdátum: ";
        public const string wClasscode = "Csoport: ";
        public const string wZh = "Zh-e? : ";

    }
}
