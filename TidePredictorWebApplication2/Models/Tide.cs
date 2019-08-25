using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TidePredictorWebApplication2.Models
{
    public class Tide
    {
        public int TideId { get; set; }
        public DateTime TideDateTime { get; set; }
        public string TideLevel { get; set; }
        public int TideHeight { get; set; }
    }
}
