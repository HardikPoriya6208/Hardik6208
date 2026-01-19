using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using library.BAL;
using library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace library.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly libraryHelper _libraryHelper;

        public AdminController(ILogger<AdminController> logger, libraryHelper libraryHelper)
        {
            _logger = logger;
            _libraryHelper = libraryHelper;
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
        public async Task<JsonResult> AddTour([FromBody] t_book tour)
        {
            int row = await _libraryHelper.AdminAddTour(tour);
            return Json(new { success = row > 0 });
        }


        // JSON: Get all bookings
        public async Task<JsonResult> GetBookings()
        {
            var bookings = await _libraryHelper.AdminGetAllBookings();
            return Json(bookings);
        }

        // UPDATE TOUR
        [HttpPost]
        public async Task<JsonResult> UpdateTour([FromBody] t_book tour)
        {
            int row = await _libraryHelper.AdminUpdateTour(tour);
            return Json(new { success = row > 0 });
        }

        // DELETE TOUR
        [HttpPost]
        public async Task<JsonResult> DeleteTour(int id)
        {
            int row = await _libraryHelper.AdminDeleteTour(id);
            return Json(new { success = row > 0 });
        }

    }
}