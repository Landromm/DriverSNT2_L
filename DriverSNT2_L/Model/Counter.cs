using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriverSNT2_L.Model
{
    public class Counter
    {
        [MaxLength(20)]
        public string? CounterId { get; set; }

        [MaxLength(20)]
        public string? NameCounter { get; set; }

        public List<ListValue> ListValues { get; set; }

       
        public int ProjectObjectId { get; set; }
        public ProjectObject ProjectObject { get; set; }
    }
}