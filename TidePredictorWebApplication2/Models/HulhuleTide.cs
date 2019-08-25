using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TidePredictorWebApplication2.Models
{
    public class HulhuleTide
    {
        [Key]
        public int HulhuleTideId { get; set; }
        public DateTime HulhuleTideDateTime { get; set; }
        public double HulhuleTideHeight { get; set; }
    }
}
