using AmazonSalesAnalysis.Model;
using AmazonSalesAnalysis.Repository;
using AmazonSalesAnalysis.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace AmazonSalesAnalysis.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    private readonly IFileRepository fileRepository;
    private readonly IDataRepository dataRepository;


    public WeatherForecastController(ILogger<WeatherForecastController> logger, IFileRepository fileRepository, IDataRepository dataRepository)
    {
        _logger = logger;
        this.fileRepository = fileRepository;
        this.dataRepository = dataRepository;
    }

    [HttpGet("GetCategories")]
    public IActionResult GetCategories()
    {
        return Ok(dataRepository.GetCategories());
    }

    [HttpGet("GetSubCategories")]
    public IActionResult GetSubCategories()
    {
        return Ok(dataRepository.GetSubCategories());
    }

    [HttpGet("GetSubCategories/{CategoryId}")]
    public IActionResult GetSubCategories(int CategoryId)
    {
        return Ok(dataRepository.GetSubCategories(CategoryId));
    }

    [HttpGet("GetCategoryAnalysis2/{CategoryId}")]
    public IActionResult GetCategoryAnalysis(int CategoryId)
    {
        return Ok(dataRepository.GetCategoryAnalysis(CategoryId));
    }

    [HttpGet("GetSubCategoryAnalysis/{CategoryId}")]
    public IActionResult GetSubCategoryAnalysis(int CategoryId)
    {
        return Ok(dataRepository.GetSubCategoryAnalysis(CategoryId));

    }

    [HttpGet("GetProducts/{SubCategoryId}")]
    public IActionResult GetProducts(int SubCategoryId)
    {
        return Ok(dataRepository.GetProducts(SubCategoryId));

    }

    [HttpGet("GetProductAnalysis/{ProductId}")]
    public IActionResult GetProductAnalysis(int ProductId)
    {
        return Ok(dataRepository.GetProductAnalysis(ProductId));

    }
}

