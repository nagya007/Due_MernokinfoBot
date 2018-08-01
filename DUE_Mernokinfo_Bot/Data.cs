using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUE_Mernokinfo_Bot
{
    public class Data
    {
        [Key]
        public int EventId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String SubjectCode { get; set; }
        public String ClassCode { get; set; }
        public bool ZH { get; set; }
        public bool IsEmpty()
        {
            if ( StartDate != null || EndDate != null || SubjectCode==null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
