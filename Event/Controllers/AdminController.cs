using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Event.BAL;
using Event.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Event.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly EventHelper _eventHelper;

        public AdminController(ILogger<AdminController> logger, EventHelper eventHelper)
        {
            _logger = logger;
            _eventHelper = eventHelper;
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
        public async Task<JsonResult> AddTour([FromBody] t_event tour)
        {
            int row = await _eventHelper.AdminAddTour(tour);
            return Json(new { success = row > 0 });
        }


        // JSON: Get all bookings
        public async Task<JsonResult> GetBookings()
        {
            var bookings = await _eventHelper.AdminGetAllBookings();
            return Json(bookings);
        }

        // UPDATE TOUR
        [HttpPost]
        public async Task<JsonResult> UpdateTour([FromBody] t_event tour)
        {
            int row = await _eventHelper.AdminUpdateTour(tour);
            return Json(new { success = row > 0 });
        }

        // DELETE TOUR
        [HttpPost]
        public async Task<JsonResult> DeleteTour(int id)
        {
            int row = await _eventHelper.AdminDeleteTour(id);
            return Json(new { success = row > 0 });
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