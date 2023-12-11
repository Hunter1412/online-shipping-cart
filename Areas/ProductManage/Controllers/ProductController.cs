using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models.DTOs;
using OnlineShoppingCart.Utils;
using ImageEntity = OnlineShoppingCart.Data.Entities.Image;

namespace OnlineShoppingCart.Areas.ProductManage.Controllers
{
    [Authorize(Roles = "admin")]
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
            var productList = await _unitOfWork.Products.GetAll("Category,Inventories");
            var productDtoList = productList == null ? new List<ProductDto>()
                : productList.Select(p => _mapper.Map<ProductDto>(p)).OrderByDescending(p => p.CreateAt).ToList();
            return View(productDtoList);
        }

        [HttpGet("/admin/product/details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var product = await _unitOfWork.Products.Get(p => p.Id == id, "Inventories,Images");
            if (product == null)
            {
                return View(new ProductDto());
            }
            return View(_mapper.Map<ProductDto>(product));
        }

        [HttpGet("/admin/product/create")]
        public async Task<IActionResult> Create()
        {

            ViewData["CategoryId"] = new SelectList(await GetItemsSelectCategories(), "Id", "Name");
            return View();
        }

        [HttpPost("/admin/product/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }
            var qty = productDto.Quantity ?? 0;
            productDto.Quantity = 0;
            //add information product
            if (productDto.CategoryId == "-1") productDto.CategoryId = null;

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
                        Id = Guid.NewGuid().ToString(),
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

        [HttpGet("/admin/product/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var product = await _unitOfWork.Products.Get(p => p.Id == id);
            if (product == null)
            {
                _logger.LogError($"The product with the {id} doesn't exist");
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(await GetItemsSelectCategories(id), "Id", "Name", product.CategoryId);


            return View(_mapper.Map<ProductDto>(product));
        }

        [HttpPost("/admin/product/edit/{id}")]
        public async Task<IActionResult> Edit(string id, ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return RedirectToAction(nameof(Edit));
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Edit));
            }
            try
            {
                //Images
                Product? productExist = await _unitOfWork.Products.Get(x => x.Id == id, "Images");
                List<ImageEntity> imageList = (productExist != null && productExist.Images != null) ? productExist.Images!.ToList() : null!;
                if (productDto.ImageFiles != null && productDto.ImageFiles.Count > 0)
                {
                    if (imageList.Count > 0)
                    {
                        //delete file old, delete on  the db
                        foreach (ImageEntity img in imageList)
                        {
                            _imageService.DeleteImage(img.ImageName!);
                            _unitOfWork.Images.Delete(img);
                        }
                    }
                    //add new
                    foreach (var item in productDto.ImageFiles)
                    {
                        string fileName = string.Empty;
                        //upload file
                        fileName = _imageService.UpLoadImage(item, "product_");
                        var image = new ImageEntity
                        {
                            Id = Guid.NewGuid().ToString(),
                            ProductId = productDto.Id,
                            ImageName = fileName,
                            ImagePath = fileName
                        };
                        //save db
                        await _unitOfWork.Images.Add(image);
                    }
                }

                //product info
                if (productDto.CategoryId == "-1") productDto.CategoryId = null;

                productDto.Slug = Slug.GenerateSlug(productDto.Name!);
                var product = _mapper.Map<Product>(productDto);
                await _unitOfWork.Products.Upsert(product);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error edit method");
                return RedirectToAction(nameof(Edit));
            }
        }



        [HttpGet("/admin/product/input-quantity")]
        public async Task<IActionResult> InputQuantity()
        {
            var productList = await _unitOfWork.Products.GetAll("Inventories");
            var productDtoList = productList == null ? new List<ProductDto>()
                : productList.Select(p => _mapper.Map<ProductDto>(p)).OrderByDescending(p => p.CreateAt).ToList();

            List<ProductDto> resultItems = new() {
                new ProductDto() {
                    Id = "-1",
                    Name = "Not product"
                }
            };
            foreach (var item in productDtoList)
            {
                resultItems.Add(new ProductDto
                {
                    Id = item.Id,
                    Name = item.Id + " - " + item.Name + " - Inventories: " + item.Inventories!.Select(x => x.Quantity).Sum() + " pcs"
                });
            }

            ViewData["ProductList"] = new SelectList(resultItems, "Id", "Name");

            return View();
        }

        [HttpPost("/admin/product/input-quantity")]
        public async Task<IActionResult> InputQuantityProduct([Bind("ProductId, Note, Quantity, DateAt")] InventoryDto inventoryDto)
        {
            if (ModelState.IsValid)
            {
                inventoryDto.Id = Guid.NewGuid().ToString();
                inventoryDto.DateAt = DateTime.Now;
                try
                {
                    await _unitOfWork.Inventory.Add(_mapper.Map<Inventory>(inventoryDto));
                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Error input quantity", typeof(ProductController));
                }
            }

            return RedirectToAction(nameof(InputQuantity));
        }

        [HttpGet("/admin/product/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _unitOfWork.Products == null)
            {
                return NotFound();
            }
            var product = await _unitOfWork.Products.Get(x => x.Id == id, "Images");
            if (product == null)
            {
                return NotFound();
            }
            return View(_mapper.Map<ProductDto>(product));
        }

        [Route("/admin/product/delete/{id}")]
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _unitOfWork.Products.Get(p => p.Id == id, "Inventories,Images");
            if (product == null)
            {
                return NotFound();
            }
            //delete image
            if (product.Images != null && product.Images.Count > 0)
            {
                foreach (var item in product.Images.ToList())
                {
                    _imageService.DeleteImage(item.ImageName!);
                    _unitOfWork.Images.Delete(item);
                }
            }
            //delete inventory
            if (product.Inventories != null && product.Inventories.Count > 0)
            {
                foreach (var qty in product.Inventories.ToList())
                {
                    _unitOfWork.Inventory.Delete(qty);
                }
            }
            //delete product
            _unitOfWork.Products.Delete(product);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IEnumerable<CategoryDto>> GetItemsSelectCategories(string? id = null)
        {

            var categories = await _unitOfWork.Categories.GetAll("Parent,Children");

            var itemsDto = categories == null
                ? new List<CategoryDto>()
                : categories.Select(c => _mapper.Map<CategoryDto>(c)).Where(c => c.Parent == null).ToList();


            List<CategoryDto> resultItems = new List<CategoryDto>() {
                new CategoryDto() {
                    Id = "-1",
                    Name = "Not parent category"
                }
            };
            Action<List<CategoryDto>, int> _ChangeTitleCategory = null!;
            Action<List<CategoryDto>, int> ChangeTitleCategory = (itemsDto, level) =>
            {
                string prefix = string.Concat(Enumerable.Repeat("—", level));
                foreach (var item in itemsDto)
                {
                    resultItems.Add(new CategoryDto()
                    {
                        Id = item.Id,
                        Name = prefix + " " + item.Name + "_" + item.Id
                    });
                    if ((item.Id != id) && (item.Children != null) && (item.Children.Count > 0))
                    {
                        _ChangeTitleCategory(item.Children.ToList(), level + 1);
                    }
                }

            };

            _ChangeTitleCategory = ChangeTitleCategory;
            ChangeTitleCategory(itemsDto, 0);

            return resultItems;
        }

    }
}