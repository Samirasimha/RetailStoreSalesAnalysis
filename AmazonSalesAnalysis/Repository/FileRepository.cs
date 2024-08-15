using System;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using AmazonSalesAnalysis.Context;
using AmazonSalesAnalysis.JsonModel;
using AmazonSalesAnalysis.Model;
using CsvHelper;
using Entity_Framework.Context;
using OfficeOpenXml;

namespace AmazonSalesAnalysis.Repository
{
	public class FileRepository: IFileRepository
	{
		private readonly DataContext dataContext;
		public FileRepository(DataContext dataContext)
		{
			this.dataContext = dataContext;
		}

		public void ParseCsvData() {
			try
			{

                string root = "/Users/samirasimha/Projects/AmazonSalesAnalysis/AmazonSalesAnalysis/Data";

                var files = from file in Directory.EnumerateFiles(root) select file;

                foreach (var file in files)
                {
					List<CsvParse> records = new List<CsvParse>();
                    using (var reader = new StreamReader(file))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
						records = csv.GetRecords<CsvParse>().ToList();
                    }

					records.ForEach(record =>
					{
						//Check if all data exists. 

						if(
						   record.actual_price.Length	> 0
						&& record.discount_price.Length > 0
						&& record.image.Length			> 0
						&& record.link.Length			> 0
                        && record.main_category.Length	> 0
                        && record.name.Length			> 0
                        && record.no_of_ratings.Length	> 0
                        && record.ratings.Length		> 0
                        && record.sub_category.Length	> 0)
						{
							//Insert Category if it does not exist.
							string categoryInput = record.main_category.Replace("\"", "")
							.Trim();

							Category category = new Category();

                            if (!dataContext
							.Category
							.Any(a => a.CategoryName == categoryInput))
							{
								category = new Category
								{
									CategoryName = categoryInput
								};

                                dataContext.Category.Add(category);
								dataContext.SaveChanges();
							}
							else
								category = dataContext.Category.FirstOrDefault(a => a.CategoryName == categoryInput);


                            //Insert Sub Category if it does not exist.
                            string subCategoryInput = record.sub_category.Replace("\"", "").Trim();

                            SubCategory subCategory = new SubCategory();

                            if (!dataContext
                            .SubCategory
                            .Any(a => a.Name == subCategoryInput))
                            {
                                subCategory = new SubCategory
                                {
                                    Name = subCategoryInput,
									CategoryId = category.Id
                                };

                                dataContext.SubCategory.Add(subCategory);
                                dataContext.SaveChanges();
                            }
                            else
                                subCategory = dataContext.SubCategory.FirstOrDefault(a => a.Name == subCategoryInput);


							//Insert Product with Category and Sub Category.

							Product product = new Product
							{
								SubCategoryId = subCategory.Id,
								DiscountPrice = double.Parse(record.discount_price.Replace("₹", "").Replace(",", "").Replace("\"", "").Trim()),
								ImageUrl = record.image.Replace("\"", ""),
								Name = record.name.Replace("\"", ""),
								Price = double.Parse(record.actual_price.Replace("₹", "").Replace(",", "").Replace("\"", "").Trim()),
								ProductUrl = record.link.Replace("\"", "")
							};

                            dataContext.Product.Add(product);
                            dataContext.SaveChanges();

							double ratingScore = 0;
                            int reviewCount = 0;


                            double.TryParse(record.ratings.Replace("\"", "").Trim(),out ratingScore);
                            int.TryParse(record.no_of_ratings.Replace("\"", "").Trim(), out reviewCount);

                            //Insert Ratings
                            Rating rating = new Rating
							{
								RatingScore = ratingScore,
								ProductId	= product.Id
							};

                            dataContext.Rating.Add(rating);
                            dataContext.SaveChanges();

                        }
                    });
				}

            }
			catch(Exception ex)
			{
				throw ex;
			}
		}

        public void ParseExcelData(string FilePath = "/Users/samirasimha/Projects/AmazonSalesAnalysis/AmazonSalesAnalysis/ExcelData/sales.xlsx")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            FileInfo existingFile = new FileInfo(FilePath);
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                //get the first worksheet in the workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                int rowCount = worksheet.Dimension.End.Row;     //get row count
                List<string> products = new();

                for (int row = 1; row <= rowCount; row++)
                {
                    string? name = worksheet.Cells[row, 3].Value?.ToString().ToLower().Trim();

                    if(name != null)
                        products.Add(name);
                }

                var allProducts = dataContext.Product.ToList();

                Dictionary<string, int> fakeProductsIds = new Dictionary<string, int>();
                var distinceProducts = products.Distinct().ToList();
                int count = distinceProducts.Count - 2;
                foreach (var product in allProducts)
                {
                    fakeProductsIds.Add(distinceProducts[count--],product.Id);
                }
                List<Invoice> allBills = new();
                for (int row = 1; row <= rowCount; row++)
                {
                    if (fakeProductsIds.ContainsKey(worksheet.Cells[row, 3].Value?.ToString().ToLower().Trim() ?? ""))
                    {
                        Invoice invoice = new();

                        invoice.ProductId = fakeProductsIds[worksheet.Cells[row, 3].Value?.ToString().ToLower().Trim()];
                        invoice.Quantity = int.Parse(worksheet.Cells[row, 4].Value?.ToString().ToLower().Trim());
                        invoice.InvoiceDate = DateTime.Parse(worksheet.Cells[row, 5].Value?.ToString().ToLower().Trim());
                        invoice.Price = Double.Parse(worksheet.Cells[row, 6].Value?.ToString().ToLower().Trim());
                        invoice.CustomerId = int.Parse(worksheet.Cells[row, 7].Value?.ToString().ToLower().Trim() ?? "69420");
                        invoice.Location = worksheet.Cells[row, 8].Value?.ToString().ToLower().Trim();
                        dataContext.Add(invoice);
                        dataContext.SaveChanges();

                        //allBills.Add(invoice);
                    }
                }

  
                    

            }
        }

        public void GenerateRating()
        {
            try
            {

                List<Product> allProducts = dataContext.Product.ToList();

                Random rnd = new Random();

                int totalRating = 300000;

                

                while (totalRating > 0)
                {


                    Product item = allProducts[rnd.Next(0, allProducts.Count())];
                    var minRating = 5 * item.Name.Length / 150;
                    dataContext.Rating.Add(
                        new Rating
                        {
                            ProductId = item.Id,
                            RatingScore = rnd.Next((minRating > 0 ? minRating : 1), 5)
                        });


                    totalRating--;
                }
                dataContext.SaveChanges();

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    }
}

