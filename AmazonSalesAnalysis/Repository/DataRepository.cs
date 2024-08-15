using System;
using AmazonSalesAnalysis.Model;
using AmazonSalesAnalysis.ViewModel;
using Entity_Framework.Context;

namespace AmazonSalesAnalysis.Repository
{
	public class DataRepository: IDataRepository
    {
		private readonly DataContext dataContext;
		public DataRepository(DataContext dataContext)
		{
			this.dataContext = dataContext;
		}

		public List<SimpleEntity> GetCategories()
		{
			try
			{

				return dataContext.Category.Select(a => new SimpleEntity
				{
					Id = a.Id,
					Name = a.CategoryName
				}).ToList();

			}
			catch(Exception Ex)
			{
				throw Ex;
			}
        }

        public List<SimpleEntityWithParent> GetSubCategories()
        {
            try
            {

                return dataContext.SubCategory.Select(a => new SimpleEntityWithParent
                {
                    Id = a.Id,
                    Name = a.Name,
					ParentId = a.CategoryId
                }).ToList();

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public List<SimpleEntityWithParent> GetSubCategories(int CategoryId)
        {
            try
            {

                return dataContext.SubCategory
                    .Where(a=> a.CategoryId == CategoryId).Select(a => new SimpleEntityWithParent
                {
                    Id = a.Id,
                    Name = a.Name,
                    ParentId = a.CategoryId
                }).ToList();

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public CategoryAnalysisModel GetCategoryAnalysis(int CategoryId)
        {
            try
            {

                return dataContext
                    .categoryAnalysis
                    .Where(a => a.Id == CategoryId)
                    .Select(a => new CategoryAnalysisModel
                    {
                        MostSoldProduct = a.MostSoldProduct ?? "",
                        HighestRatedProduct = a.HighestRatedProduct ?? "",
                        QuantitySold = a.TotalSold,
                        Rating = (double)Decimal.Round((decimal)a.AverageRating, 1, MidpointRounding.AwayFromZero)
                    }).FirstOrDefault();
            }
            catch(Exception Ex)
            {
                throw Ex;
            }
        }

        public CategoryAnalysisModel GetSubCategoryAnalysis(int CategoryId)
        {
            try
            {
                return dataContext
                    .subCategoryAnalysis
                    .Where(a => a.Id == CategoryId)
                    .Select(a => new CategoryAnalysisModel
                    {
                        MostSoldProduct = a.MostSoldProduct ?? "",
                        HighestRatedProduct = a.HighestRatedProduct ?? "",
                        QuantitySold = a.TotalSold,
                        Rating = (double)Decimal.Round((decimal)a.AverageRating, 1, MidpointRounding.AwayFromZero)
                    }).FirstOrDefault();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public List<SimpleEntity> GetProducts(int SubCategoryId)
        {
            try
            {

                var analysis = dataContext.ProductAnalysis.Where(a => a.AvgRating > 0 && a.MaximumPrice > 0 && a.MinimumPrice > 0 && a.RepeatPercentage > 0 && a.AverageOrderQuantity > 0).ToList();

                return dataContext.Product
                    .Where(a => a.SubCategoryId == SubCategoryId)
                    .Where(a=> analysis.Select(a=> a.ProductId).ToList().Contains(a.Id))
                    .Select(a => new SimpleEntity
                    {
                        Id = a.Id,
                        Name = a.Name
                    }).Take(10).ToList();
            }
            catch(Exception Ex)
            {
                throw Ex;
            }
        }

        public ProductAnalysisModel GetProductAnalysis(int ProductId)
        {
            try
            {

                Product product = dataContext.Product.FirstOrDefault(a => a.Id == ProductId);
                ProductAnalysis analysis = dataContext.ProductAnalysis.FirstOrDefault(a => a.ProductId == product.Id);

                return new ProductAnalysisModel()
                {
                    Id = product.Id,
                    Analysis = dataContext.ProductAnalysis.FirstOrDefault(a => a.ProductId == product.Id),
                    ImageUrl = product.ImageUrl,
                    Name = product.Name,
                    Price = (analysis.MinimumPrice + analysis.MaximumPrice) / 2
                };


            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    }
}

