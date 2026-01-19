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
    public class TourController : Controller
    {
        private readonly ILogger<TourController> _logger;
        private readonly TourHelper _tourHelper;
        private readonly IWebHostEnvironment _env;

        public TourController(ILogger<TourController> logger, TourHelper tourHelper, IWebHostEnvironment env)
        {
            _logger = logger;
            _tourHelper = tourHelper;
            _env = env;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            //  string role = HttpContext.Session.GetString("role");
            // if (HttpContext.Session.GetString("role") != "User")
            //     return RedirectToAction("Login");

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
        // API → Get All Tours
        // ===========================
        public async Task<IActionResult> GetAll()
        {
            var data = await _tourHelper.GetAllTours();
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

            var list = await _tourHelper.GetBookingsByUser(uid.Value);

            return Json(list);
        }


        // ===========================
        // API → Save Booking
        // ===========================
        [HttpPost]
        public async Task<IActionResult> SaveBooking([FromBody] t_tourbooking booking)
        {
            booking.t_tourdate = DateTime.Now.ToString("yyyy-MM-dd");

            booking.c_userid = HttpContext.Session.GetInt32("c_userid") ?? 0;

            int row = await _tourHelper.SaveBooking(booking);

            return Json(new { success = row > 0 });
        }

        // ===========================
        // API → Update Bokking
        // ===========================
        [HttpPost]
        public async Task<IActionResult> UpdateBooking([FromBody] t_tourbooking booking)
        {
            int result = await _tourHelper.UpdateBooking(booking);
            return Json(new { success = result > 0 });
        }


        // ===========================
        // API → Delete Booking
        // ===========================
        [HttpPost]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            int result = await _tourHelper.DeleteBooking(id);
            return Json(new { success = result > 0 });
        }
    }
}