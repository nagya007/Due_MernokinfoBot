using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DUE_Mernokinfo_Bot
{
    public class UserEnrolled
    {
        [Key]
        public int UserEnrolledId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Data")]
        public int EventId { get; set; }
        public User User { get; set; }
        public Data Data { get; set; }
    }
}
