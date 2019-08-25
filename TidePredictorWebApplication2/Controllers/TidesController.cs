using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TidePredictorWebApplication2.Data;
using TidePredictorWebApplication2.Models;
using TidePredictorWebApplication2.ViewModel;

namespace TidePredictorWebApplication2.Controllers
{
    public class TidesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TidesController(ApplicationDbContext db)
        {
            _db = db;
            if (DatatableContainingTideDataOfNeededDates.Columns.Count == 0)
            {
                AddColumnsToDatatableContainingTideDataOfNeededDates();
            }
        }
        public void AddColumnsToDatatableContainingTideDataOfNeededDates()
        {
            List<string> ListOFColumnNamesForDatatable = new List<string>();
            ListOFColumnNamesForDatatable.Add("TideId");
            ListOFColumnNamesForDatatable.Add("TideDateTime");
            ListOFColumnNamesForDatatable.Add("TideHeight");
            ListOFColumnNamesForDatatable.Add("TideLevel");
            List<string> ListOFColumndatatypesForDatatable = new List<string>();
            ListOFColumndatatypesForDatatable.Add("System.Int32");
            ListOFColumndatatypesForDatatable.Add("System.DateTime");
            ListOFColumndatatypesForDatatable.Add("System.Double");
            ListOFColumndatatypesForDatatable.Add("System.String");
            int i = 0;
            foreach (var item in ListOFColumnNamesForDatatable)
            {
                DatatableContainingTideDataOfNeededDates.Columns.Add(
                   new DataColumn()
                   {
                       DataType = System.Type.GetType(ListOFColumndatatypesForDatatable[i].ToString()),//or other type
                       ColumnName = ListOFColumnNamesForDatatable[i].ToString()     //or other column name
                   }
                );
                i++;
            }
        }


        ////GET// TIDES
        //public IActionResult Index(Tide tide)
        //{

        //    return View(tide);
        //}

        //GET// TIDES
        public IActionResult Index(Tide tide)
        {
            var model = new TideViewModel
            {
                TideId = tide.TideId,
                TideDateTime = tide.TideDateTime,
                TideLevel = tide.TideLevel,
                TideHeight = tide.TideHeight,
                MeanLowerLowWaterLevelForSelectedRegion = 2,
                ListOfTideStations = _db.TideStations.ToList(),
                //TideStationSelected = "Hulhule",
            };
            return View(model);


            //return View();
        }



        //POST //CalculateTide
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalculateTide(TideViewModel tideViewModel)
        {

            //Get the data of the date and time entered. // Done 
            DateTime date = new DateTime();
            date = tideViewModel.TideDateTime;
            //Get all the necessary data from the db to make calculation
            int tideIdGienByUser = tideViewModel.TideId;
            var TideDataFromDb = await _db.Tides.SingleOrDefaultAsync(u => u.TideDateTime == tideViewModel.TideDateTime);
            //Make Calculation 
            //return view of POST//CalculateTide with the data
            //set up view of POST//CalculateTide to display data. Must have a back button to return to index
            RecordDatesNeededToSearchCorrectTideTime(tideViewModel);
            LoadTideDataOfSelectedAndNeighbouringDays(tideViewModel);
            RecordTidePropertiesForCalculation(tideViewModel);
            PredictTideHeightForGivenTime();
            PredictSeaLevelUsingCalculatedHeight(tideViewModel);
            //  return RedirectToAction(nameof(Index));
            return View(tideViewModel);
        }
        //public async Task<IActionResult> CalculateTide(DecoyViewModel dViewModel)
        //{
        //    int de = dViewModel.TestInt1;
        //    return View(dViewModel);
        //}


        public static DateTime SelectedDatetimeForTidePrediction = new DateTime();
        public DateTime PrecedingDaysDateTime = new DateTime();
        public DateTime FollowingDaysDateTime = new DateTime();
        public static DataTable DatatableContainingTideDataOfNeededDates = new DataTable();
        public static double FirstTideHeightNeededForCalculation { get; set; }
        public static double SecondTideHeightNeededForCalculation { get; set; }
        public static DateTime FirstTideTimeNeededForCalculation { get; set; }
        public static DateTime SecondTideTimetNeededForCalculation { get; set; }
        public static double h2_minus_h1 { get; set; }
        public static double t_minus_t1 { get; set; }
        public static double t2_minus_t1 { get; set; }
        public static double A { get; set; }
        public static double SeaLevel { get; set; }
        public static double PredictedTideHeight { get; set; }

        public void PredictTideHeightForGivenTime()
        {
            DateTime t = SelectedDatetimeForTidePrediction;
            DateTime t1 = FirstTideTimeNeededForCalculation;
            DateTime t2 = SecondTideTimetNeededForCalculation;
            double h1 = FirstTideHeightNeededForCalculation;
            double h2 = SecondTideHeightNeededForCalculation;
            h2_minus_h1 = h2 - h1;
            t_minus_t1 = (t - t1).TotalHours;
            t2_minus_t1 = (t2 - t1).TotalHours;
            double MainCalculationForA = t_minus_t1 / t2_minus_t1;
            A = Math.PI * (MainCalculationForA + 1);
            PredictedTideHeight = h1 + (h2 - h1) * ((Math.Cos(A) + 1) / 2);
        }

        public void PredictSeaLevelUsingCalculatedHeight(TideViewModel tideViewModel)
        {
            double MeanTideLevelOfSelectedRegion = double.Parse(tideViewModel.MeanTideLevelOfSelectedRegion.ToString());
            double MeanLowerLowWaterLevelOfSelectedRegion = double.Parse(tideViewModel.MeanLowerLowWaterLevelForSelectedRegion.ToString());
            MeanTideLevelOfSelectedRegion -= MeanLowerLowWaterLevelOfSelectedRegion;
            SeaLevel = PredictedTideHeight - MeanTideLevelOfSelectedRegion;
            tideViewModel.SeaLevelCalculated = SeaLevel;
        }

        public void RecordTidePropertiesForCalculation(TideViewModel tide)
        {

            //foreach (var row in DatatableContainingTideDataOfNeededDates.Rows)
            //{
            for (int i = 1; i < DatatableContainingTideDataOfNeededDates.Rows.Count; i++)
            {
                DateTime FirstDatetimeValue = DateTime.Parse(DatatableContainingTideDataOfNeededDates.Rows[i - 1][1].ToString());
                DateTime SecondDatetimeValue = DateTime.Parse(DatatableContainingTideDataOfNeededDates.Rows[i][1].ToString());
                if (SelectedDatetimeForTidePrediction > FirstDatetimeValue && SelectedDatetimeForTidePrediction < SecondDatetimeValue)
                {
                    FirstTideHeightNeededForCalculation = double.Parse(DatatableContainingTideDataOfNeededDates.Rows[i - 1][2].ToString());
                    SecondTideHeightNeededForCalculation = double.Parse(DatatableContainingTideDataOfNeededDates.Rows[i][2].ToString());
                    FirstTideTimeNeededForCalculation = FirstDatetimeValue;
                    SecondTideTimetNeededForCalculation = SecondDatetimeValue;
                }
            }
            tide.MeanTideLevelOfSelectedRegion = SetMeanTideLevelForSelectedSatation(tide);
            tide.MeanLowerLowWaterLevelForSelectedRegion = SetLowWaterLevelForSelectedSatation(tide);
        }

        public double SetMeanTideLevelForSelectedSatation(TideViewModel tide)
        {
            if (tide.TideStationSelected == "1")
            {
                return 1.892;
            }
            else if (tide.TideStationSelected == "2")
            {
                return 1.051;
            }
            else if (tide.TideStationSelected == "3")
            {
                return 1.573;
            }
            return 0;
        }

        public double SetLowWaterLevelForSelectedSatation(TideViewModel tide)
        {
            if (tide.TideStationSelected == "1")
            {
                return 1.532;
            }
            else if (tide.TideStationSelected == "2")
            {
                return 0.652;
            }
            else if (tide.TideStationSelected == "3")
            {
                return 1.159;
            }
            return 0;
        }

        public void LoadTideDataOfSelectedAndNeighbouringDays(TideViewModel tide)
        {
            LoadLastTideDataOfPrecedingDayToDatatable(tide);
            LoadTideDataOfSearchedDayToDatatable(tide);
            LoadFirstTideDataOfFollowingDayToDatatable(tide);
        }

        public void LoadFirstTideDataOfFollowingDayToDatatable(TideViewModel tide)
        {
            DateTime PreviousDayInShortDateFormAndDateFormat = DateTime.Parse(PrecedingDaysDateTime.ToShortDateString());
            DateTime SelectedDayInShortDateFormAndDateFormat = DateTime.Parse(SelectedDatetimeForTidePrediction.ToShortDateString());
            DateTime FollowingDayInShortDateFormAndDateFormat = DateTime.Parse(FollowingDaysDateTime.ToShortDateString());

            TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
            DateTime DayAfterFollowingDayInShortDateFormAndDateFormat = FollowingDayInShortDateFormAndDateFormat + oneDay;

            if (tide.TideStationSelected == "1")
            {
                var query = (from t in _db.HulhuleTides
                             where t.HulhuleTideDateTime > FollowingDayInShortDateFormAndDateFormat && t.HulhuleTideDateTime < DayAfterFollowingDayInShortDateFormAndDateFormat
                             orderby t.HulhuleTideDateTime
                             select t).First();
                var row = DatatableContainingTideDataOfNeededDates.NewRow();
                row[0] = query.HulhuleTideId.ToString();
                row[1] = query.HulhuleTideDateTime.ToString();
                row[2] = query.HulhuleTideHeight.ToString();
                //   row["HulhuleTideLevel"] = query.HulhuleTideLevel.ToString();
                DatatableContainingTideDataOfNeededDates.Rows.Add(row);
            }
            else if (tide.TideStationSelected == "2")
            {
                var query = (from t in _db.HanimaadhooTides
                             where t.HanimaadhooTideDateTime > FollowingDayInShortDateFormAndDateFormat && t.HanimaadhooTideDateTime < DayAfterFollowingDayInShortDateFormAndDateFormat
                             orderby t.HanimaadhooTideDateTime
                             select t).First();
                var row = DatatableContainingTideDataOfNeededDates.NewRow();
                row[0] = query.HanimaadhooTideId.ToString();
                row[1] = query.HanimaadhooTideDateTime.ToString();
                row[2] = query.HanimaadhooTideHeight.ToString();
                // row["HanimaadhooTideLevel"] = query.HanimaadhooTideLevel.ToString();
                DatatableContainingTideDataOfNeededDates.Rows.Add(row);
            }
            else if (tide.TideStationSelected == "3")
            {
                var query = (from t in _db.GanTides
                             where t.GanTideDateTime > FollowingDayInShortDateFormAndDateFormat && t.GanTideDateTime < DayAfterFollowingDayInShortDateFormAndDateFormat
                             orderby t.GanTideDateTime
                             select t).First();
                var row = DatatableContainingTideDataOfNeededDates.NewRow();
                row[0] = query.GanTideId.ToString();
                row[1] = query.GanTideDateTime.ToString();
                row[2] = query.GanTideHeight.ToString();
                // row["GanTideLevel"] = query.GanTideLevel.ToString();
                DatatableContainingTideDataOfNeededDates.Rows.Add(row);
            }

            //var query = (from t in _db.Tides
            //             where t.TideDateTime > FollowingDayInShortDateFormAndDateFormat && t.TideDateTime < DayAfterFollowingDayInShortDateFormAndDateFormat
            //             orderby t.TideDateTime
            //             select t).First();
            ////foreach (var element in query)
            ////{
            //var row = DatatableContainingTideDataOfNeededDates.NewRow();
            //row["TideId"] = query.TideId.ToString();
            //row["TideDateTime"] = query.TideDateTime.ToString();
            //row["TideHeight"] = query.TideHeight.ToString();
            //row["TideLevel"] = query.TideLevel.ToString();
            //DatatableContainingTideDataOfNeededDates.Rows.Add(row);


            //Testing
            List<string> asdad = new List<string>();
            for (int i = 0; i <= DatatableContainingTideDataOfNeededDates.Rows.Count - 1; i++)
            {
                asdad.Add(DatatableContainingTideDataOfNeededDates.Rows[i][2].ToString());
            }
            int test = 0;
        }

        public void LoadTideDataOfSearchedDayToDatatable(TideViewModel tide)
        {
            DateTime PreviousDayInShortDateFormAndDateFormat = DateTime.Parse(PrecedingDaysDateTime.ToShortDateString());
            DateTime SelectedDayInShortDateFormAndDateFormat = DateTime.Parse(SelectedDatetimeForTidePrediction.ToShortDateString());
            DateTime FollowingDayInShortDateFormAndDateFormat = DateTime.Parse(FollowingDaysDateTime.ToShortDateString());
            if (tide.TideStationSelected == "1")
            {
                var query = from t in _db.HulhuleTides
                            where t.HulhuleTideDateTime > SelectedDayInShortDateFormAndDateFormat && t.HulhuleTideDateTime < FollowingDayInShortDateFormAndDateFormat
                            orderby t.HulhuleTideDateTime
                            select t;
                foreach (var element in query)
                {
                    var row = DatatableContainingTideDataOfNeededDates.NewRow();
                    row[0] = element.HulhuleTideId.ToString();
                    row[1] = element.HulhuleTideDateTime.ToString();
                    row[2] = element.HulhuleTideHeight.ToString();
                    //   row["HulhuleTideLevel"] = element.HulhuleTideLevel.ToString();
                    DatatableContainingTideDataOfNeededDates.Rows.Add(row);
                }
            }
            else if (tide.TideStationSelected == "2")
            {
                var query = from t in _db.HanimaadhooTides
                            where t.HanimaadhooTideDateTime > SelectedDayInShortDateFormAndDateFormat && t.HanimaadhooTideDateTime < FollowingDayInShortDateFormAndDateFormat
                            orderby t.HanimaadhooTideDateTime
                            select t;
                foreach (var element in query)
                {
                    var row = DatatableContainingTideDataOfNeededDates.NewRow();
                    row[0] = element.HanimaadhooTideId.ToString();
                    row[1] = element.HanimaadhooTideDateTime.ToString();
                    row[2] = element.HanimaadhooTideHeight.ToString();
                    //   row["HulhuleTideLevel"] = element.HulhuleTideLevel.ToString();
                    DatatableContainingTideDataOfNeededDates.Rows.Add(row);
                }
            }
            else if (tide.TideStationSelected == "3")
            {
                var query = from t in _db.GanTides
                            where t.GanTideDateTime > SelectedDayInShortDateFormAndDateFormat && t.GanTideDateTime < FollowingDayInShortDateFormAndDateFormat
                            orderby t.GanTideDateTime
                            select t;
                foreach (var element in query)
                {
                    var row = DatatableContainingTideDataOfNeededDates.NewRow();
                    row[0] = element.GanTideId.ToString();
                    row[1] = element.GanTideDateTime.ToString();
                    row[2] = element.GanTideHeight.ToString();
                    //   row["HulhuleTideLevel"] = element.HulhuleTideLevel.ToString();
                    DatatableContainingTideDataOfNeededDates.Rows.Add(row);
                }
            }

            //    var query = from t in _db.Tides
            //            where t.TideDateTime > SelectedDayInShortDateFormAndDateFormat && t.TideDateTime < FollowingDayInShortDateFormAndDateFormat
            //            orderby t.TideDateTime
            //            select t;
            //foreach (var element in query)
            //{
            //    var row = DatatableContainingTideDataOfNeededDates.NewRow();
            //    row["TideId"] = element.TideId.ToString();
            //    row["TideDateTime"] = element.TideDateTime.ToString();
            //    row["TideHeight"] = element.TideHeight.ToString();
            //    row["TideLevel"] = element.TideLevel.ToString();
            //    DatatableContainingTideDataOfNeededDates.Rows.Add(row);
            //}
            ////just looping thorugh datatble to see ite contents because magnifing glass not working
            //for (int i = 0; i <= DatatableContainingTideDataOfNeededDates.Rows.Count-1; i++)
            //{
            //    string japan = DatatableContainingTideDataOfNeededDates.Rows[i][3].ToString();
            //}
        }

        public void LoadLastTideDataOfPrecedingDayToDatatable(TideViewModel tide)
        {
            DateTime PreviousDayInShortDateFormAndDateFormat = DateTime.Parse(PrecedingDaysDateTime.ToShortDateString());
            DateTime SelectedDayInShortDateFormAndDateFormat = DateTime.Parse(SelectedDatetimeForTidePrediction.ToShortDateString());
            //var query = (from t in _db.Tides
            //             where t.TideDateTime > PreviousDayInShortDateFormAndDateFormat && t.TideDateTime < SelectedDayInShortDateFormAndDateFormat
            //             orderby t.TideDateTime
            //             select t).Last();
            if (tide.TideStationSelected == "1")
            {
                var query = (from t in _db.HulhuleTides
                             where t.HulhuleTideDateTime > PreviousDayInShortDateFormAndDateFormat && t.HulhuleTideDateTime < SelectedDayInShortDateFormAndDateFormat
                             orderby t.HulhuleTideDateTime
                             select t).Last();
                var row = DatatableContainingTideDataOfNeededDates.NewRow();
                row[0] = query.HulhuleTideId.ToString();
                row[1] = query.HulhuleTideDateTime.ToString();
                row[2] = query.HulhuleTideHeight.ToString();
                //row["HulhuleTideLevel"] = query.HulhuleTideLevel.ToString();
                DatatableContainingTideDataOfNeededDates.Rows.Add(row);
            }
            else if (tide.TideStationSelected == "2")
            {
                var query = (from t in _db.HanimaadhooTides
                             where t.HanimaadhooTideDateTime > PreviousDayInShortDateFormAndDateFormat && t.HanimaadhooTideDateTime < SelectedDayInShortDateFormAndDateFormat
                             orderby t.HanimaadhooTideDateTime
                             select t).Last();
                var row = DatatableContainingTideDataOfNeededDates.NewRow();
                row[0] = query.HanimaadhooTideId.ToString();
                row[1] = query.HanimaadhooTideDateTime.ToString();
                row[2] = query.HanimaadhooTideHeight.ToString();
                //row["HulhuleTideLevel"] = query.HulhuleTideLevel.ToString();
                DatatableContainingTideDataOfNeededDates.Rows.Add(row);
            }
            else
            {
                var query = (from t in _db.GanTides
                             where t.GanTideDateTime > PreviousDayInShortDateFormAndDateFormat && t.GanTideDateTime < SelectedDayInShortDateFormAndDateFormat
                             orderby t.GanTideDateTime
                             select t).Last();
                var row = DatatableContainingTideDataOfNeededDates.NewRow();
                row[0] = query.GanTideId.ToString();
                row[1] = query.GanTideDateTime.ToString();
                row[2] = query.GanTideHeight.ToString();
                //row["HulhuleTideLevel"] = query.HulhuleTideLevel.ToString();
                DatatableContainingTideDataOfNeededDates.Rows.Add(row);
            }

            //var row = DatatableContainingTideDataOfNeededDates.NewRow();
            //row["TideId"] = query.TideId.ToString();
            //row["TideDateTime"] = query.TideDateTime.ToString();
            //row["TideHeight"] = query.TideHeight.ToString();
            //row["TideLevel"] = query.TideLevel.ToString();
            //DatatableContainingTideDataOfNeededDates.Rows.Add(row);

            ////Testing
            //List<string> asdad = new List<string>();
            //for (int i = 0; i <= DatatableContainingTideDataOfNeededDates.Rows.Count - 1; i++)
            //{
            //    asdad.Add(DatatableContainingTideDataOfNeededDates.Rows[i][3].ToString());
            //}
            //int test = 0;
        }

        //public HulhuleTide GetQueryToLoadLastTideDataOfPrecedingDayToDatatableForHulhumale(DateTime PreviousDayInShortDateFormAndDateFormat, DateTime SelectedDayInShortDateFormAndDateFormat))
        //{
        //HulhuleTide query = (from t in _db.HulhuleTides
        //                 where t.HulhuleTideDateTime > PreviousDayInShortDateFormAndDateFormat && t.HulhuleTideDateTime < SelectedDayInShortDateFormAndDateFormat
        //                 orderby t.HulhuleTideDateTime
        //                 select t).Last();
        //return query;

        //}


        public void RecordDatesNeededToSearchCorrectTideTime(TideViewModel tide)
        {
            SelectedDatetimeForTidePrediction = tide.TideDateTime;
            SelectedDatetimeForTidePrediction = new DateTime(
               SelectedDatetimeForTidePrediction.Ticks -
               (SelectedDatetimeForTidePrediction.Ticks
               % TimeSpan.TicksPerMinute),
               SelectedDatetimeForTidePrediction.Kind);

            PrecedingDaysDateTime = GetPrecedingDaysDate(SelectedDatetimeForTidePrediction);
            FollowingDaysDateTime = GetFollowingDaysDate(SelectedDatetimeForTidePrediction);
        }

        public DateTime GetFollowingDaysDate(DateTime RecievedSelectedDayForTidePrediction)
        {
            DateTime d1 = RecievedSelectedDayForTidePrediction;
            d1 = d1.AddDays(+1);
            return d1;
        }
        public DateTime GetPrecedingDaysDate(DateTime RecievedSelectedDayForTidePrediction)
        {
            DateTime d1 = RecievedSelectedDayForTidePrediction;
            d1 = d1.AddDays(-1);
            return d1;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
        }
    }
}