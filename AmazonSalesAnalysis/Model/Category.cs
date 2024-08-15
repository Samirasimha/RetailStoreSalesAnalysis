using System;
using System.ComponentModel.DataAnnotations;

namespace AmazonSalesAnalysis.Model
{
	public class Category
	{
		[Key()]
		public int Id { get; set; }
		public string CategoryName { get; set; }
	}
}

