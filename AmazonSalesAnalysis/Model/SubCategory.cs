using System;
using System.ComponentModel.DataAnnotations;

namespace AmazonSalesAnalysis.Model
{
	public class SubCategory
	{
		[Key()]
		public int Id { get; set; }
		public string Name { get; set; }
		public int CategoryId { get; set; }
	}
}

