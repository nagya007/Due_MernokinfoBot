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
    public class User
    {   [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string ChatId { get; set; }
    }
}
