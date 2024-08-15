using System;
namespace AmazonSalesAnalysis.JsonModel
{
	public class CsvParse
	{
		public string name { get; set; }
		public string main_category { get; set; }
		public string sub_category { get; set; }
		public string image { get; set; }
        public string link { get; set; }
        public string ratings { get; set; }
        public string no_of_ratings { get; set; }
        public string discount_price { get; set; }
		public string actual_price { get; set; }
    }
}

