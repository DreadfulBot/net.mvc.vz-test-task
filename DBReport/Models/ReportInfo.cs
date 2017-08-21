using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Отчёт по продажам
namespace DBReport.Models
{
    public class ReportInfo
    {
        public int ProductId { get; set; }//Номер заказа
        public DateTime Date { get; set; }//Дата заказа
        //DateTime(Int32, Int32, Int32) Инициализирует новый экземпляр структуры DateTime заданными значениями года, месяца и дня.
        public int Type { get; set; }//Артикул товара
        public string Name { get; set; }//Название товара
        public int Quantity { get; set; }//Кол-во реализованных единиц
        public decimal Price { get; set; }//Цена реализации за единицу продукции
    }
}
