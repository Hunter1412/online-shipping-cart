using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Models;
using OnlineShoppingCart.Models.DTOs;

namespace OnlineShoppingCart.Controllers;

public class HomeController : Controller
{
    protected readonly ILogger<HomeController> _logger;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IMapper _mapper;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _unitOfWork.Products.GetAll("Inventories,Images,Feedbacks");
        // var productDtoList = products!.Select(p => _mapper.Map<ProductDto>(p)).OrderByDescending(x => x.CreateAt).ToList();
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    //tes
    public IActionResult Shop()
    {
        return View();
    }

    //test
    public IActionResult ShopSingle()
    {
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
