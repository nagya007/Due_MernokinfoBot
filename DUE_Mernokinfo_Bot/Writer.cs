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
        const string wSubjectcode = "Neve";
        const string wStartdate = "Kezdő dátum: ";
        const string wEnddate = "Vég dátum: ";
        const string wClasscode = "Csoport: ";
        const string wZh = "Zh-e? : ";

    }
}
