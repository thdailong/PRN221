using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Finance.Model
{
    public class Support
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Boolean FromUser { get; set; } = false;
        public Account Account { get; set; } = null!;
    }
}