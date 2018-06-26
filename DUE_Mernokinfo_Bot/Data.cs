using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DUE_Mernokinfo_Bot
{
    public class Data
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String SubjectCode { get; set; }
        public String ClassCode { get; set; }
        public bool ZH { get; set; }
        public string Participants { get; set;}
    }
}
