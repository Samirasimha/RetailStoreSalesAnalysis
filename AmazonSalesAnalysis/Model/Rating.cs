using System;
using System.ComponentModel.DataAnnotations;

namespace AmazonSalesAnalysis.Model
{
	public class Rating
	{
        [Key()]
        public int Id { get; set; }
		public int ProductId { get; set; }
		public double RatingScore { get; set; }
	}
}

