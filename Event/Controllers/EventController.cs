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
    public class EventController : Controller
    {
        private readonly ILogger<EventController> _logger;
        private readonly EventHelper _eventHelper;
        private readonly IWebHostEnvironment _env;

        public EventController(ILogger<EventController> logger, EventHelper eventHelper, IWebHostEnvironment env)
        {
            _logger = logger;
            _eventHelper = eventHelper;
            _env = env;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
           string role = HttpContext.Session.GetString("role");
            if (role != "User")
                return RedirectToAction("Login", "Home");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        // ===========================
        // API → Get All Event
        // ===========================
        public async Task<IActionResult> GetAll()
        {
            var data = await _eventHelper.GetAllTours();
            return Json(data);
        }

        // ===========================
        // API → Get All Bookings
        // ===========================

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            int? uid = HttpContext.Session.GetInt32("c_userid");

            if (uid == null)
                return BadRequest("User logged in");

            var list = await _eventHelper.GetBookingsByUser(uid.Value);

            return Json(list);
        }


        // ===========================
        // API → Save Booking
        // ===========================
        [HttpPost]
        public async Task<IActionResult> SaveBooking([FromBody] t_eventbooking booking)
        {
            booking.c_eventdate = DateTime.Now.ToString("yyyy-MM-dd");

            booking.c_userid = HttpContext.Session.GetInt32("c_userid") ?? 0;

            int row = await _eventHelper.SaveBooking(booking);

            return Json(new { success = row > 0 });
        }

        // ===========================
        // API → Update Bokking
        // ===========================
        [HttpPost]
        public async Task<IActionResult> UpdateBooking([FromBody] t_eventbooking booking)
        {
            int result = await _eventHelper.UpdateBooking(booking);
            return Json(new { success = result > 0 });
        }


        // ===========================
        // API → Delete Booking
        // ===========================
        [HttpPost]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            int result = await _eventHelper.DeleteBooking(id);
            return Json(new { success = result > 0 });
        }
    }
}