using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using library.BAL;
using library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace library.Controllers
{
    public class libraryController : Controller
    {
        private readonly libraryHelper _helper;

        public libraryController(NpgsqlConnection conn)
        {
            _helper = new libraryHelper(conn);
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
        // API → Get All Library
        // ===========================
        public async Task<IActionResult> GetAll()
        {
            var data = await _helper.GetAllTours();
            return Json(data);
        }

        // ================== GET USER BOOKINGS ==================
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            int? uid = HttpContext.Session.GetInt32("c_userid");
            if (uid == null)
                return Unauthorized();

            var list = await _helper.GetBookingsByUser(uid.Value);
            return Json(list);
        }

        // ================== SAVE BOOKING ==================
        [HttpPost]
        public async Task<IActionResult> SaveBooking([FromBody] t_BookIssue booking)
        {
            booking.c_userid = HttpContext.Session.GetInt32("c_userid") ?? 0;
            booking.c_issueDate = DateTime.Now.ToString("yyyy-MM-dd");
            booking.c_dueDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
            booking.c_status = "Issued";


            //  Check duplicate booking
            bool alreadyBooked = await _helper.IsBookAlreadyBooked(
                booking.c_userid,
                booking.c_bookingid
            );

            if (alreadyBooked)
                return Json(new { success = false, message = "Book already issued" });

            //  Check & decrease stock
            bool stockOk = await _helper.DecreaseStock(booking.c_bookingid);
            if (!stockOk)
                return Json(new { success = false, message = "Out of stock" });

            //  Save booking
            int row = await _helper.SaveBooking(booking);
            return Json(new { success = row > 0 });
        }


        // ================== DELETE BOOKING ==================
        [HttpPost]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            //  Get bookId
            int? bookId = await _helper.GetBookIdByBooking(id);
            if (bookId == null)
                return Json(new { success = false });

            //  Delete booking
            int result = await _helper.DeleteBooking(id);

            // 3 Restore stock
            if (result > 0)
                await _helper.IncreaseStock(bookId.Value);

            return Json(new { success = result > 0 });
        }
    }
}