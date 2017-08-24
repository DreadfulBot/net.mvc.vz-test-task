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
using MimeKit;
using MailKit.Net.Smtp;



// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace DBReport.Controllers
{
    public class HomeController : Controller
    {
        private string CreatingExcel()
        {

            //CreatingExcelFile
            // Create a spreadsheet document
            string filename = @"./wwwroot/ExcelReportFromNorthwind"+DateTime.Today.ToString("d")+".xlsx";
            SpreadsheetDocument excelDoc = SpreadsheetDocument.Create(filename, SpreadsheetDocumentType.Workbook);

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
            return filename;
        }

        //Editing Excel file
        private void AddingDataToExcel(string filename)
        {
            using (SpreadsheetDocument currentDocument = SpreadsheetDocument.Open(filename, true))
            {
                // Retrieve a reference to the workbook part.
                WorkbookPart wbPart = currentDocument.WorkbookPart;
                // Find the first sheet and then use that Sheet object to retrieve a reference to the first worksheet.
                Sheet firstsheet = wbPart.Workbook.Descendants<Sheet>().First();
                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart = (WorksheetPart)wbPart.GetPartById(firstsheet.Id);
                //Документ пустой => существование строк не проверять
                int currentvalue = 124;
                Worksheet worksheet = wsPart.Worksheet;
                SheetData sheetData = worksheet.GetFirstChild<SheetData>();
                Row row = new Row() { RowIndex = 1 };
                Cell theCell = new Cell() { CellReference="A1", DataType=CellValues.Number, CellValue=new CellValue(currentvalue.ToString())};
                row.Append(theCell);
                sheetData.Append(row);

                wsPart.Worksheet.Save();
            }
        }

        //Let's work with emails
        private async void SendEmail(string address, string filename)
        {
            var emailMessage = new MimeMessage();
            //Создание приложения
            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "autoemail.Friar_s.web-proj@list.ru"));//адрес
            emailMessage.To.Add(new MailboxAddress("customer", address));
            emailMessage.Subject = "Запрошенная на Ваш адрес выборка из базы данных";

            var builder = new BodyBuilder();

            // Set the plain-text version of the message text
            builder.TextBody = @"Данные находятся в прикреплённом Excel файле.";
            //builder.HtmlBody;

            // We may also want to attach something
            builder.Attachments.Add(filename);

            // Now we just need to set the message body and we're done
            emailMessage.Body = builder.ToMessageBody();

            //Подключение к почте и отправка сообщения
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 25, false);
                await client.AuthenticateAsync("autoemail.Friar_s.web-proj@list.ru", "2714Qr55z`");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

        }

        //SQL request
        private void SQLRequest(DateTime starttime)
        {
            //Строка соединения
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Database=Northwind;Integrated Security=True";//or Initial Catalog
            //SQL-запрос         
            string CommandText = "SELECT OrderID, OrderDate, CategoryID, Name, Quantity, OrderDetail.UnitPrice FROM Product, \"Order\", OrderDetail WHERE (\"Order\".ID=OrderDetail.OrderID) AND (OrderDetail.ProductID=Product.ID) AND (\"Order\".OrderDate>='" + starttime.Year + "." + starttime.Month + "." + starttime.Day + "')";

            //Соединение с базой данных
            using (SqlConnection Northwind = new SqlConnection(connectionString))
            {
                Northwind.Open();
                SqlCommand ReportRequest = new SqlCommand(CommandText, Northwind);
                SqlDataReader reader = ReportRequest.ExecuteReader();

                if (reader.HasRows)//если есть данные
                {
                    List<ReportInfo> Report = new List<ReportInfo>();//Список с информацией
                    //List<string> strings = new List<string>();
                    while (reader.Read())
                    {
                        Report.Add(new ReportInfo()
                        {
                            ProductId=reader.GetInt32(0),
                            Date=reader.GetDateTime(1),
                            ProductType=reader.GetInt32(2),
                            ProductName=reader.GetString(3),
                            Quantity=reader.GetInt32(4),
                            UnitPrice=reader.GetDecimal(5)
                        }   );
                        //strings.Add(reader.GetString(0));
                    }
                    ViewBag.ReportData = Report;
                }
            }

        }



        // GET: /<controller>/
        public IActionResult Index()
        {            
            return View();
        }
        


        public IActionResult What()
        {
            string filename = CreatingExcel();
            AddingDataToExcel(filename);
            return View();
        }


        //ПРинимает интересующее количество дней
        [HttpPost]
        public IActionResult Index(DateTime starttime, string email)
        {
            SQLRequest(starttime);

            string filename = CreatingExcel();
            AddingDataToExcel(filename);
            SendEmail(email, filename);
            System.IO.File.Delete(filename);
          

            return View("What");
        }
    }
}
