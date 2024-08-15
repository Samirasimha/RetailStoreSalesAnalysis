using System;
namespace AmazonSalesAnalysis.ViewModel
{
	public class CategoryAnalysisModel
	{
		public string MostSoldProduct { get; set; }
		public string HighestRatedProduct { get; set; }
		public int QuantitySold { get; set; }
		public double Rating { get; set; }
	}
}

