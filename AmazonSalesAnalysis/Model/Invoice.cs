using System;
using System.ComponentModel.DataAnnotations;

namespace AmazonSalesAnalysis.Model
{
    public class Invoice
    {
        [Key()]
        public int InvoiceNo { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double Price { get; set; }
        public int CustomerId { get; set; }
        public string Location { get; set; }

    }

}