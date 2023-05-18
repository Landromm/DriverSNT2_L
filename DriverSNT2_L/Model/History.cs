using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverSNT2_L.Model
{
    [Keyless]
    public class History
    {
        public int HashId { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string? Value { get; set; }

        public DateTime DateTime { get; set; }
    }
}
