using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Flight.BAL;
using Flight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Flight.Controllers
{
    public class FlightController : Controller
    {
        private readonly FlightHelper _helper;

        public FlightController(NpgsqlConnection conn)
        {
            _helper = new FlightHelper(conn);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("c_userid") == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        // ===========================
        // API → Get All Tours
        // ===========================
        public async Task<IActionResult> GetAll()
        {
            var data = await _helper.GetAllTours();
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

            var list = await _helper.GetBookingsByUser(uid.Value);

            return Json(list);
        }


        // ===========================
        // API → Save Booking
        // ===========================
        [HttpPost]
        public async Task<IActionResult> SaveBooking([FromBody] t_flightbooking booking)
        {
            booking.t_date = DateOnly.FromDateTime(DateTime.Now);

            booking.c_userid = HttpContext.Session.GetInt32("c_userid") ?? 0;

            int row = await _helper.SaveBooking(booking);

            return Json(new { success = row > 0 });
        }


        // ===========================
        // API → Delete Booking
        // ===========================
        [HttpPost]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            int result = await _helper.DeleteBooking(id);
            return Json(new { success = result > 0 });
        }


        // ===========================
        // API → Update Bokking
        // ===========================
        [HttpPost]
        public async Task<IActionResult> UpdateBooking([FromBody] t_flightbooking booking)
        {
            int result = await _helper.UpdateBooking(booking);
            return Json(new { success = result > 0 });
        }

    }
}