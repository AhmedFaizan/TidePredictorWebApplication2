using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TidePredictorWebApplication2.Models
{
    public class GanTide
    {
        [Key]
        public int GanTideId { get; set; }
        public DateTime GanTideDateTime { get; set; }
        public double GanTideHeight { get; set; }
    }
}
