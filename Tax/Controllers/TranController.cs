using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tax.BAL;
using Tax.Models;

namespace Tax.Controllers
{
    public class TranController : Controller
    {
        private readonly ILogger<TranController> _logger;
        private readonly TranHelper _tranHelper;
        private readonly IWebHostEnvironment _env;

        public TranController(ILogger<TranController> logger, TranHelper tranHelper, IWebHostEnvironment env)
        {
            _logger = logger;
            _tranHelper = tranHelper;
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

        // Show Data
        [HttpGet]
        public IActionResult ShowExpense()
        {
            int uid = HttpContext.Session.GetInt32("id") ?? 1;
            return Json(_tranHelper.Show(uid));
        }

        // Add Data
        [HttpPost]
        public IActionResult Add(t_tran tran)
        {
            tran.c_userid = HttpContext.Session.GetInt32("id") ?? 1;

            bool result = tran.t_tranid > 0
                ? _tranHelper.Update(tran)
                : _tranHelper.Add(tran);

            return Json(new { success = result });
        }

        // Delete Data
        [HttpPost]
        public IActionResult DeleteExpense(int id)
        {
            return Json(new { success = _tranHelper.Delete(id) });
        }


        // Logic

        // ================= SUMMARY (FIXED ROUTE) =================
        [HttpGet]
        public IActionResult Summary()
        {
            int id = HttpContext.Session.GetInt32("id") ?? 1;

            int income = _tranHelper.totalincome(id);
            int nontax = _tranHelper.nontax(id);
            int taxexpense = _tranHelper.tax(id);

            int taxableIncome = income - nontax;
            int tax = 0;

            if (taxableIncome <= 400000)
                tax = 0;
            else if (taxableIncome <= 800000)
                tax = (taxableIncome - 400000) * 5 / 100;
            else if (taxableIncome <= 1200000)
                tax = (400000 * 5 / 100) +
                      (taxableIncome - 800000) * 10 / 100;
            else if (taxableIncome <= 1600000)
                tax = (400000 * 5 / 100) +
                      (400000 * 10 / 100) +
                      (taxableIncome - 1200000) * 15 / 100;
            else
                tax = (400000 * 5 / 100) +
                      (400000 * 10 / 100) +
                      (400000 * 15 / 100) +
                      (taxableIncome - 1600000) * 30 / 100;

            int saving = taxableIncome - (nontax + taxexpense) - tax;

            return Json(new
            {
                success = true,
                income,
                nontax,
                taxable_income = taxableIncome,
                tax,
                saving
            });
        }
    }
}