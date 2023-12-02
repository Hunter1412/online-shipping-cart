using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models.DTOs;
using OnlineShoppingCart.Utils;
using ImageEntity = OnlineShoppingCart.Data.Entities.Image;

namespace OnlineShoppingCart.Areas.ProductManage.Controllers
{
    [Area("ProductManage")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ImageService _imageService;
        public ProductController(ILogger<ProductController> logger, IMapper mapper = null, IUnitOfWork unitOfWork = null, ImageService imageService = null)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }


        [HttpGet("/admin/product")]
        public async Task<IActionResult> Index()
        {
            var productList = await _unitOfWork.Products.GetAll("Inventories");
            var productDtoList = productList == null ? new List<ProductDto>()
                : productList.Select(p => _mapper.Map<ProductDto>(p)).OrderByDescending(p => p.CreateAt).ToList();
            return View(productDtoList);
        }

        [HttpGet("/admin/product/details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var product = await _unitOfWork.Products.Get(p => p.Id == id);
            return View(_mapper.Map<ProductDto>(product));
        }

        [HttpGet("/admin/product/create")]
        public async Task<IActionResult> Create()
        {

            ViewData["CategoryId"] = new SelectList(await _unitOfWork.Categories.GetAll("Parent,Children"), "Id", "Name");
            return View();
        }

        [HttpPost("/admin/product/create")]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError(">>>>>>error create");
                return View(productDto);
            }
            var qty = productDto.Quantity;
            productDto.Quantity = 0;
            //add information product
            var product = _mapper.Map<Product>(productDto);
            string _name = product.Name! + "-" + product.Id;
            product.Slug = Slug.GenerateSlug(_name);
            await _unitOfWork.Products.Add(product);
            //upload image list
            if (productDto.ImageFiles != null && productDto.ImageFiles.Count > 0)
            {
                foreach (var item in productDto.ImageFiles)
                {
                    string fileName = string.Empty;
                    fileName = _imageService.UpLoadImage(item, "product_");
                    var image = new ImageEntity
                    {
                        ProductId = productDto.Id,
                        ImageName = fileName,
                        ImagePath = fileName
                    };
                    await _unitOfWork.Images.Add(image);
                }
            }
            //add quantity
            if (qty >= 0)
            {
                var inventory = new Inventory
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = productDto.Id,
                    Quantity = qty,
                    DateAt = DateTime.Now,
                    Note = "Input product"
                };
                await _unitOfWork.Inventory.Add(inventory);
            }

            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}