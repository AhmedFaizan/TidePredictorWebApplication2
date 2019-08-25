using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TidePredictorWebApplication2.Models;

namespace TidePredictorWebApplication2.ViewModel
{
    public class TideViewModel
    {
        //public enum TideStation { Hulhule, Hanimaadhoo, Gan };

        public string TideStationSelected { get; set; }
        public double MeanTideLevelOfSelectedRegion { get; set; }
        public List<TideStation> ListOfTideStations { get; set; }
        public double MeanLowerLowWaterLevelForSelectedRegion { get; set; }
        public int TideId { get; set; }
        public DateTime TideDateTime { get; set; }
        public string TideLevel { get; set; }
        public int TideHeight { get; set; }
        public double SeaLevelCalculated { get; set; }
        public int YearOfTide { get; set; }
        public int MonthOfTide { get; set; }
        public int DayOfTide { get; set; }
        public int TimeOfTide { get; set; }
        public double TideHeightForDataEntry { get; set; }
        public TideViewModel()
        {
            TideStationSelected = "1";
            //TideId = tide.TideId;
            //TideDateTime = tide.TideDateTime;
            //TideLevel = tide.TideLevel;
            //TideHeight = tide.TideHeight;
            //MeanLowerLowWaterLevelForSelectedRegion = 2;
        }

      
        public string FeedbackMessage { get; set; }


        //Inumerables to load db tide datas into html table
        public IEnumerable<HulhuleTide> InumerableOfHulhuleTidesObj { get; set; }
        public IEnumerable<HanimaadhooTide> InumerableOfHanimaadhooTidesObj { get; set; }
        public IEnumerable<GanTide> InumerableOfGanTidesObj { get; set; }

        //Tide objects To save tide data to database 
        public HulhuleTide NewHulhuleTideObj { get; set; }
        public HanimaadhooTide NewHanimaadhooTideObj { get; set; }
        public GanTide NewGanTideObj { get; set; }



    }
}
