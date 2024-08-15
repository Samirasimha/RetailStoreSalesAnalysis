using System;
using AmazonSalesAnalysis.ViewModel;

namespace AmazonSalesAnalysis.Repository
{
	public interface IDataRepository
	{
        List<SimpleEntity> GetCategories();
        List<SimpleEntityWithParent> GetSubCategories();
        List<SimpleEntityWithParent> GetSubCategories(int CategoryId);
        CategoryAnalysisModel GetCategoryAnalysis(int CategoryId);
        CategoryAnalysisModel GetSubCategoryAnalysis(int CategoryId);
        List<SimpleEntity> GetProducts(int SubCategoryId);
        ProductAnalysisModel GetProductAnalysis(int ProductId);
    }
}

