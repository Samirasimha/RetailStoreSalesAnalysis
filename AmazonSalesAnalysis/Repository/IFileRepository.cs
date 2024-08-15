using System;
namespace AmazonSalesAnalysis.Repository
{
	public interface IFileRepository
	{
        void ParseCsvData();
        void ParseExcelData(string FilePath = "/Users/samirasimha/Projects/AmazonSalesAnalysis/AmazonSalesAnalysis/ExcelData/sales.xlsx");
        void GenerateRating();

    }
}

