using DBReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;


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

        public IActionResult What()
        {
            //Строка соединения
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True";
            //Соединение с базой данных
            using (SqlConnection Northwind = new SqlConnection(connectionString))
            {
                Northwind.Open();
            }
            
            return View();
        }

        //ПРинимает интересующее количество дней
        [HttpPost]
        public IActionResult Index(int ReportDate)
        {
            ViewBag.Days = ReportDate;
            return View("What");
        }
    }
}
