using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TidePredictorWebApplication2.Data;
using TidePredictorWebApplication2.Models;
using TidePredictorWebApplication2.ViewModel;

namespace TidePredictorWebApplication2.Controllers
{
    public class TideDataManagementController : Controller
    {
        // public string Message { get; set; }
        public static int SelectedYear = int.Parse(DateTime.Now.Year.ToString());
        private readonly ApplicationDbContext _db;
        public TideDataManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(Tide tide)
        {
            var model = new TideViewModel
            {
                TideId = tide.TideId,
                InumerableOfHulhuleTidesObj = _db.HulhuleTides.ToList().OrderByDescending(s => s.HulhuleTideId).Take(5),
                InumerableOfHanimaadhooTidesObj = _db.HanimaadhooTides.ToList().OrderByDescending(s => s.HanimaadhooTideId).Take(5),
                InumerableOfGanTidesObj = _db.GanTides.ToList().OrderByDescending(s => s.GanTideId).Take(5),
                ListOfTideStations = _db.TideStations.ToList(),
                FeedbackMessage = FeedbackMessage
            };
            return View(model);
        }

        ////[HttpPost]
        //public async Task<IActionResult> EnterTideDataToDatabase(TideViewModel tideViewModel)
        //{
        ////int superday = tideViewModel.DayOfTide;
        ////make date into database date format 
        ////push date data and tide height data into database
        //int testint = tideViewModel.MonthOfTide;
        //DateTime DateToEnterToDb = GetDateTimeToEnterIntoDb(tideViewModel);
        ////SetIdInTideObjInModelView(tideViewModel);
        //SetDateInTideObjInModelView(tideViewModel, DateToEnterToDb);
        //SetTideHeightInTideObjInModelView(tideViewModel);
        //if (ModelState.IsValid)
        //{
        //    //tideViewModel.NewHulhuleTideObj.HulhuleTideId = tideViewModel.TideId;
        //    //tideViewModel.NewHulhuleTideObj.HulhuleTideDateTime = DateToEnterToDb;
        //    //tideViewModel.NewHulhuleTideObj.HulhuleTideHeight = tideViewModel.TideHeightForDataEntry;
        //    if (tideViewModel.TideStationSelected == "Hulhule")
        //    {
        //        _db.Add(tideViewModel.NewHulhuleTideObj);
        //    }
        //    else if (tideViewModel.TideStationSelected == "Hanimaadhoo")
        //    {
        //        _db.Add(tideViewModel.NewHanimaadhooTideObj);
        //    }
        //    else if (tideViewModel.TideStationSelected == "Gan")
        //    {
        //        _db.Add(tideViewModel.NewGanTideObj);
        //    }
        //    await _db.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //return View();


        ///////////////////////

        ///////////////////////
        //}

        //public void SetIdInTideObjInModelView(TideViewModel tideViewModel)
        //{
        //    if (tideViewModel.TideStationSelected == "Hulhule")
        //    {
        //        tideViewModel.NewHulhuleTideObj.HulhuleTideId = tideViewModel.TideId;
        //    }
        //    else if (tideViewModel.TideStationSelected == "Hanimaadhoo")
        //    {
        //        tideViewModel.NewHanimaadhooTideObj.HanimaadhooTideHeight = tideViewModel.TideHeightForDataEntry;
        //    }
        //    else if (tideViewModel.TideStationSelected == "Gan")
        //    {
        //        tideViewModel.NewGanTideObj.GanTideHeight = tideViewModel.TideHeightForDataEntry;
        //    }
        //}
        [TempData]
        public string FeedbackMessage { get; set; }
        //[HttpPost]
        public async Task<IActionResult> EnterTideDataToDatabase(TideViewModel tideViewModel)
        {
            //int num = tideViewModel.YearOfTide;
            if (ModelState.IsValid)
            {
                if (tideViewModel.TideStationSelected == "1")
                {
                    try
                    {
                        tideViewModel.NewHulhuleTideObj = new HulhuleTide();
                        tideViewModel.NewHulhuleTideObj.HulhuleTideHeight = tideViewModel.TideHeightForDataEntry;
                        tideViewModel.NewHulhuleTideObj.HulhuleTideDateTime = GetDateTimeToEnterIntoDb(tideViewModel);
                        _db.Add(tideViewModel.NewHulhuleTideObj);
                        await _db.SaveChangesAsync();
                        //  tideViewModel.FeedbackMessage = "Saved Successfully";
                        FeedbackMessage = "Saved Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception e)
                    {
                        FeedbackMessage = "Error: Couldn't save to database";
                        return RedirectToAction(nameof(Index));
                    }
                    //tideViewModel.NewHulhuleTideObj = new HulhuleTide();
                    //tideViewModel.NewHulhuleTideObj.HulhuleTideHeight = tideViewModel.TideHeightForDataEntry;
                    //tideViewModel.NewHulhuleTideObj.HulhuleTideDateTime = GetDateTimeToEnterIntoDb(tideViewModel);
                    //_db.Add(tideViewModel.NewHulhuleTideObj);
                    //await _db.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                }
                else if (tideViewModel.TideStationSelected == "2")
                {
                    try
                    {
                        tideViewModel.NewHanimaadhooTideObj = new HanimaadhooTide();
                        tideViewModel.NewHanimaadhooTideObj.HanimaadhooTideHeight = tideViewModel.TideHeightForDataEntry;
                        tideViewModel.NewHanimaadhooTideObj.HanimaadhooTideDateTime = GetDateTimeToEnterIntoDb(tideViewModel);
                        _db.Add(tideViewModel.NewHanimaadhooTideObj);
                        await _db.SaveChangesAsync();
                        FeedbackMessage = "Saved Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception e)
                    {
                        FeedbackMessage = "Error: Couldn't save to database";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else if (tideViewModel.TideStationSelected == "3")
                {
                    try
                    {
                        tideViewModel.NewGanTideObj = new GanTide();
                        tideViewModel.NewGanTideObj.GanTideHeight = tideViewModel.TideHeightForDataEntry;
                        tideViewModel.NewGanTideObj.GanTideDateTime = GetDateTimeToEnterIntoDb(tideViewModel);
                        _db.Add(tideViewModel.NewGanTideObj);
                        await _db.SaveChangesAsync();
                        FeedbackMessage = "Saved Successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception e)
                    {
                        FeedbackMessage = "Error: Couldn't save to database";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(nameof(Index));
        }



        public void SetTideHeightInTideObjInModelView(TideViewModel tideViewModel)
        {
            if (tideViewModel.TideStationSelected == "1")
            {
                tideViewModel.NewHulhuleTideObj.HulhuleTideHeight = tideViewModel.TideHeightForDataEntry;
            }
            else if (tideViewModel.TideStationSelected == "2")
            {
                tideViewModel.NewHanimaadhooTideObj.HanimaadhooTideHeight = tideViewModel.TideHeightForDataEntry;
            }
            else if (tideViewModel.TideStationSelected == "3")
            {
                tideViewModel.NewGanTideObj.GanTideHeight = tideViewModel.TideHeightForDataEntry;
            }
        }

        public void SetDateInTideObjInModelView(TideViewModel tideViewModel, DateTime DateToEnterToDb)
        {
            if (tideViewModel.TideStationSelected == "1")
            {
                tideViewModel.NewHulhuleTideObj.HulhuleTideDateTime = DateToEnterToDb;
            }
            else if (tideViewModel.TideStationSelected == "2")
            {
                tideViewModel.NewHanimaadhooTideObj.HanimaadhooTideDateTime = DateToEnterToDb;
            }
            else if (tideViewModel.TideStationSelected == "3")
            {
                tideViewModel.NewGanTideObj.GanTideDateTime = DateToEnterToDb;
            }
        }

        public DateTime GetDateTimeToEnterIntoDb(TideViewModel tideViewModel)
        {
            DateTime DateToEnterIntoDB = new DateTime();
            string time = SelectedYear + GetGivenTimeTwoDigitTimeValue(tideViewModel.MonthOfTide.ToString()) + GetGivenTimeTwoDigitTimeValue(tideViewModel.DayOfTide.ToString()) + tideViewModel.TimeOfTide.ToString();
            // string time = "2017" + "01" + "01"+ 12+54;
            DateToEnterIntoDB = DateTime.ParseExact(time, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None);
            return DateToEnterIntoDB;
        }
        public string GetGivenTimeTwoDigitTimeValue(string TimeValueGiven)
        {
            int LengthOfTimeValueGiven = TimeValueGiven.Length;
            if (LengthOfTimeValueGiven < 2)
            {
                return "0" + TimeValueGiven;
            }
            else
            {
                return TimeValueGiven;
            }
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