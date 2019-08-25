using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TidePredictorWebApplication2.Models
{
    public class HanimaadhooTide
    {
        [Key]
        public int HanimaadhooTideId { get; set; }
        public DateTime HanimaadhooTideDateTime { get; set; }
        public double HanimaadhooTideHeight { get; set; }
    }
}
