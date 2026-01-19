using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC.BAL;
using MVC.Models;

namespace MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly TourHelper _tourHelper;

        public AdminController(ILogger<AdminController> logger, TourHelper tourHelper)
        {
            _logger = logger;
            _tourHelper = tourHelper;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            string role = HttpContext.Session.GetString("role");
            if (role != "Admin")
                return RedirectToAction("Login", "Home");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }


        // ================== ADMIN: ADD TOUR ==================
        [HttpPost]
        public async Task<JsonResult> AddTour([FromBody] t_tour tour)
        {
            int row = await _tourHelper.AdminAddTour(tour);
            return Json(new { success = row > 0 });
        }


        // JSON: Get all bookings
        public async Task<JsonResult> GetBookings()
        {
            var bookings = await _tourHelper.AdminGetAllBookings();
            return Json(bookings);
        }

        // UPDATE TOUR
        [HttpPost]
        public async Task<JsonResult> UpdateTour([FromBody] t_tour tour)
        {
            int row = await _tourHelper.AdminUpdateTour(tour);
            return Json(new { success = row > 0 });
        }

        // DELETE TOUR
        [HttpPost]
        public async Task<JsonResult> DeleteTour(int id)
        {
            int row = await _tourHelper.AdminDeleteTour(id);
            return Json(new { success = row > 0 });
        }



        [HttpGet]
        public IActionResult GetTourReport(int tourid)
        {
            var data = _tourHelper.GetTourReport(tourid);
            return Json(data);
        }


        // POST: Admin/UpdateBookingStatus

        //[HttpPost]
        //public async Task<JsonResult> UpdateBookingStatus([FromBody] t_eventbooking booking)
        //{
        // Validate the incoming booking object
        // if (booking == null)
        //     return Json(new { success = false, message = "Booking cannot be null" });

        // if (!booking.t_bookingId.HasValue)
        //     return Json(new { success = false, message = "Booking ID is required" });

        // if (string.IsNullOrWhiteSpace(booking.t_status))
        //    return Json(new { success = false, message = "Status cannot be empty" });

        //  try
        //  {
        //     int rowsUpdated = await _eventHelper.AdminUpdateBookingStatus(
        //        booking.t_bookingId.Value,
        //         booking.t_status
        //   );

        //  if (rowsUpdated > 0)
        //        return Json(new { success = true, message = "Status updated successfully" });
        //   else
        //   return Json(new { success = false, message = "Booking not found" });
        // }
        //  catch (Exception ex)
        //  {
        // Log the exception if you have a logger
        //     return Json(new { success = false, message = $"Error: {ex.Message}" });
        //  }
        //}
    }
}