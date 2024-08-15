using System;
namespace AmazonSalesAnalysis.Model
{
	public class SubCategoryAnalysis
	{
        public int Id { get; set; }
        public int HighestRatedProductId { get; set; }
        public string HighestRatedProduct { get; set; }
        public float AverageRating { get; set; }
        public int MostSoldProductId { get; set; }
        public string MostSoldProduct { get; set; }
        public int TotalSold { get; set; }
    }
}

