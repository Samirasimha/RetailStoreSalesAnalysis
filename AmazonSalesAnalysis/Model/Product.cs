using System;
using System.ComponentModel.DataAnnotations;

namespace AmazonSalesAnalysis.Model
{
	public class Product
	{
		[Key()]
		public int Id { get; set; }
		public string Name { get; set; }
		public int SubCategoryId { get; set; }
		public string ImageUrl { get; set; }
		public string ProductUrl { get; set; }
		public double Price { get; set; }
		public double DiscountPrice { get; set; }
	}
}

