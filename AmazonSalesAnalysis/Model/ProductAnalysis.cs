using System;
namespace AmazonSalesAnalysis.Model
{
	public class ProductAnalysis
	{
        public int ProductId { get; set; }
        public int AverageOrderQuantity { get; set; }
        public int RepeatPercentage { get; set; }
        public double MinimumPrice { get; set; }
        public double MaximumPrice { get; set; }
        public string MostSoldAtLocation { get; set; }
        public double AvgRating { get; set; }
    }
}

