using DBReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;



// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DBReport.Controllers
{
    public class HomeController : Controller
    {
        List<ReportInfo> Report = new List<ReportInfo>();//Список с информацией
        private void CreatingExcel()
        {
            //CreatingExcelFile
            // Create a spreadsheet document
            SpreadsheetDocument excelDoc = SpreadsheetDocument.Create(@"./wwwroot/ExcelReportFromNorthwind.xlsx", SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookPartNumberOne = excelDoc.AddWorkbookPart();
            workbookPartNumberOne.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPartNumberOne = workbookPartNumberOne.AddNewPart<WorksheetPart>();
            worksheetPartNumberOne.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets excelSheets = excelDoc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet excelSheetNumberOne = new Sheet() { Id = excelDoc.WorkbookPart.GetIdOfPart(worksheetPartNumberOne), SheetId = 1, Name = "ReportResults" };
            excelSheets.Append(excelSheetNumberOne);

            workbookPartNumberOne.Workbook.Save();

            excelDoc.Save();

            excelDoc.Close();
            // Other code goes here.
        }

        // GET: /<controller>/
        public IActionResult Index()
        {            
            return View();
        }

        


        public IActionResult What()
        {
            CreatingExcel();
            return View();
        }


        //ПРинимает интересующее количество дней
        [HttpPost]
        public IActionResult Index(int ReportDate)
        {
            //Строка соединения
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Database=Northwind;Integrated Security=True";//or Initial Catalog
            //SQL-запрос
            string CommandText = "SELECT TOP 5 Name FROM Product";

            //Соединение с базой данных
            using (SqlConnection Northwind = new SqlConnection(connectionString))
            {
                Northwind.Open();
                SqlCommand ReportRequest = new SqlCommand(CommandText, Northwind);
                SqlDataReader reader = ReportRequest.ExecuteReader();

                if (reader.HasRows)//если есть данные
                {

                    List<string> strings = new List<string>();
                    while (reader.Read())
                    {
                        strings.Add(reader.GetString(0));
                    }
                    ViewBag.Data = strings;
                }
            }

            return View("What");
        }
    }
}
