using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Отчёт по продажам
namespace DBReport.Models
{
    public class ReportInfo
    {
        public int OrderId { get; set; }//Номер заказа
        public DateTime Date { get; set; }//Дата заказа
        //DateTime(Int32, Int32, Int32) Инициализирует новый экземпляр структуры DateTime заданными значениями года, месяца и дня.
        public int ProductType { get; set; }//Артикул товара
        public string ProductName { get; set; }//Название товара
        public int Quantity { get; set; }//Кол-во реализованных единиц
        public decimal UnitPrice { get; set; }//Цена реализации за единицу продукции
    }
}
