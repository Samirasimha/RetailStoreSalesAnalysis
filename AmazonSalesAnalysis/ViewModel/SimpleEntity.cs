using System;
namespace AmazonSalesAnalysis.ViewModel
{
	public class SimpleEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
    public class SimpleEntityWithParent : SimpleEntity
    {
        public int ParentId { get; set; }
    }
}

