using DBReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DBReport.Controllers
{
    public class HomeController : Controller
    {
        List<ReportInfo> Report = new List<ReportInfo>();//Список с информацией

        // GET: /<controller>/
        public IActionResult Index()
        {            
            ViewBag.Report = Report;
            return View();
        }

        public IActionResult What(int a)
        {
            ViewBag.Days = a;
            return View(ViewBag.Days);
        }

        [HttpPost]
        public RedirectToActionResult Index(int ReportDate)
        {
            return RedirectToAction("What", "Home", new { a=ReportDate } );
        }
    }
}
