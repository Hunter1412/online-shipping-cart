using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models;
using OnlineShoppingCart.Models.DTOs;
using X.PagedList;

namespace OnlineShoppingCart.Controllers;

[Route("[controller]")]
public class ShopController : Controller
{
    private readonly ILogger<ShopController> _logger;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly ApplicationDbContext _context;
    private IMemoryCache _cache;
    protected readonly IMapper _mapper;

    public ShopController(ILogger<ShopController> logger, ApplicationDbContext context, IUnitOfWork unitOfWork, IMemoryCache cache, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _unitOfWork = unitOfWork;
        _cache = cache;
        _mapper = mapper;
    }

    [Route("/shop/{categorySlug?}")]
    public async Task<IActionResult> Index(string categoryslug, int? page)
    {
        var categories = GetCategories();

        Category category = null;
        if (!string.IsNullOrEmpty(categoryslug))
        {

            category = FindCategoryBySlug(categories, categoryslug);
            if (category == null)
            {
                return NotFound("Not fount Category");
            }

        }

        var products = await _unitOfWork.Products.GetAll("Inventories,Images,Feedbacks");
        var productDtoList = products!.Select(p => _mapper.Map<ProductDto>(p)).OrderByDescending(x => x.CreateAt).ToList();


        if(category!=null){
            var categoryDto = _mapper.Map<CategoryDto>(category);
            var ids = new List<string>();
            categoryDto.ChildrenCategoryIDs(null, ids);
            ids.Add(categoryDto.Id!);

            productDtoList = productDtoList
                .Where(p => ids.Contains(p.CategoryId)).OrderByDescending(d=>d.CreateAt)
                .ToList();
        }

        ViewBag.categories = categories;
        ViewBag.categorySlug = categoryslug;
        ViewBag.currentCategory = category;


        if (page == null) page = 1;
        int pageSize = 2;
        int pageNumber = (page ?? 1);

        return View(productDtoList.ToPagedList(pageNumber, pageSize));
    }


    // [HttpGet("/product/{id}")]
    [Route("/product/{id}.html", Name = "ShopSingle")]
    public async Task<IActionResult> ShopSingle([FromRoute] string id)
    {
        var products = await _unitOfWork.Products.GetAll("Inventories,Images,Feedbacks");
        var productDtoList = products!.Select(p => _mapper.Map<ProductDto>(p)).OrderByDescending(x => x.CreateAt).ToList();
        var productDto = productDtoList.Find(x => x.Slug == id);
        if (productDto != null)
        {
            var item = new ProductItem(productDto);
            return View(item);
        }
        return RedirectToAction(nameof(Index));
    }


    /// Lấy danh các Categories - có dùng cache
    [NonAction]
    List<Category> GetCategories()
    {

        List<Category> categories;

        string keycacheCategories = "_listallcategories";

        // Phục hồi categories từ Memory cache, không có thì truy vấn Db
        if (!_cache.TryGetValue(keycacheCategories, out categories))
        {

            categories = _context.Categories
                .Include(c => c.Children)
                .AsEnumerable()
                .Where(c => c.Parent == null)
                .ToList();

            // Thiết lập cache - lưu vào cache
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(300));
            _cache.Set("_GetCategories", categories, cacheEntryOptions);
        }

        return categories;
    }


    // Tìm (đệ quy) trong cây, một Category theo Slug
    [NonAction]
    Category FindCategoryBySlug(List<Category> categories, string Slug)
    {

        foreach (var c in categories)
        {
            if (c.Slug == Slug) return c;
            var c1 = FindCategoryBySlug(c.Children.ToList(), Slug);
            if (c1 != null)
                return c1;
        }

        return null;
    }


}
