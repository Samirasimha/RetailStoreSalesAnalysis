using System;
using AmazonSalesAnalysis.Model;

namespace AmazonSalesAnalysis.ViewModel
{
	public class ProductAnalysisModel : SimpleEntity
	{
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public double Price { get; set; }
		public ProductAnalysis Analysis { get; set; } = new();
    }
}

