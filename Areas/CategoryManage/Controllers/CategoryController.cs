using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Data;
using AutoMapper;
using OnlineShoppingCart.Models.DTOs;
using OnlineShoppingCart.Utils;
using OnlineShoppingCart.Core.UnitOfWork;
using Microsoft.AspNetCore.Authorization;

namespace OnlineShoppingCart.Areas.CategoryManage.Controllers
{
    [Authorize(Roles ="admin")]
    [Area("CategoryManage")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ImageService _imageService;


        public CategoryController(IMapper mapper, ImageService imageService, ILogger<CategoryController> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _imageService = imageService;
            _logger = logger;
            _unitOfWork = unitOfWork;
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

        // GET: Category
        [HttpGet("/admin/category")]
        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Categories.GetAll("Parent,Children");

            var categoryDto = categories == null
                ? new List<CategoryDto>()
                : categories.Select(c => _mapper.Map<CategoryDto>(c)).Where(c => c.Parent == null).ToList();

            return View(categoryDto);
        }

        [HttpGet("/admin/category/details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var category = await _unitOfWork.Categories.Get(x => x.Id == id, "Parent");
            if (category == null)
            {
                _logger.LogError($"The category with the {id} doesn't exist");
                return NotFound();
            }
            return View(_mapper.Map<CategoryDto>(category));
        }

        // GET: Category/Create
        [HttpGet("/admin/category/create")]
        public async Task<IActionResult> Create()
        {
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategories(), "Id", "Name", "-1");
            return View();
        }

        // POST: Category/Create
        [HttpPost("/admin/category/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (categoryDto.ParentId == "-1")
                        categoryDto.ParentId = null;

                    string filename = _imageService.UpLoadImage(categoryDto.ImageFile!, "category_");
                    categoryDto.Image = filename;
                    string _name = categoryDto.Name! + "-" + categoryDto.Id;
                    categoryDto.Slug = Slug.GenerateSlug(_name);
                    await _unitOfWork.Categories.Add(_mapper.Map<Category>(categoryDto));
                    await _unitOfWork.CompleteAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    if (!await CategoryExists(categoryDto.Id!))
                    {
                        _logger.LogError($"The category with the {categoryDto.Id} doesn't exist");
                        return NotFound();
                    }
                    else
                    {
                        return View(categoryDto);
                    }
                }
            }
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategories(), "Id", "Name", categoryDto.ParentId);
            return View(categoryDto);
        }

        // GET: Categories/Edit/5
        [HttpGet("/admin/category/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var category = await _unitOfWork.Categories.Get(x => x.Id == id, "Parent");
            if (category == null)
            {
                _logger.LogError($"The category with the {id} doesn't exist");
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategories(id), "Id", "Name", category.ParentId);
            return View(_mapper.Map<CategoryDto>(category));
        }

        // POST: Categories/Edit/5
        [HttpPost("/admin/category/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return NotFound();
            }

            if (categoryDto.ParentId == id || (categoryDto.ParentId == categoryDto.Id))
            {
                return Json(new { code = 201, msg = "Can't select category !" });
            }

            if (!ModelState.IsValid)
            {
                ViewData["ParentId"] = new SelectList(await GetItemsSelectCategories(id), "Id", "Name", categoryDto.ParentId);
                return View(categoryDto);
            }

            try
            {
                string fileName = categoryDto.Image!;
                //Images
                if (categoryDto.ImageFile != null)
                {
                    if (categoryDto.Image != null)
                    {
                        _imageService.DeleteImage(categoryDto.Image);
                    }
                    fileName = _imageService.UpLoadImage(categoryDto.ImageFile);
                }
                categoryDto.Image = fileName;

                //Category and ParentId
                if (categoryDto.ParentId == "-1")
                    categoryDto.ParentId = null;

                categoryDto.Slug = Slug.GenerateSlug(categoryDto.Name!);
                var category = _mapper.Map<Category>(categoryDto);
                await _unitOfWork.Categories.Upsert(category);
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CategoryExists(categoryDto.Id))
                {
                    _logger.LogError($"The category with the {categoryDto.Id} doesn't exist");
                    return NotFound();
                }
                else
                {
                    ViewData["ParentId"] = new SelectList(await GetItemsSelectCategories(id), "Id", "Name", categoryDto.ParentId);
                    return View(categoryDto);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Delete/5
        [HttpPost("/admin/category/delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _unitOfWork.Categories == null)
            {
                return BadRequest(new
                {
                    code = 404,
                    error = "Category doesn't exist"
                });
            }

            try
            {
                var category = await _unitOfWork.Categories.Get(c => c.Id == id, "Children");
                if (category == null)
                {
                    return BadRequest(new
                    {
                        code = 404,
                        error = "Category doesn't exist"
                    });
                }

                var cateProduct = await _unitOfWork.Products.Get(p => p.CategoryId == id);
                if (cateProduct != null)
                {
                    return Json(new { code = 202, msg = "Category already used. Can't delete!" });
                }

                foreach (var cCategory in category.Children!)
                {
                    cCategory.ParentId = category.ParentId;
                }

                if (category.Image != null)
                {
                    _imageService.DeleteImage(category.Image);
                }
                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.CompleteAsync();

                return Json(new { code = 200, msg = "Delete successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller Error delete method", typeof(CategoryController));
                return Json(new { code = 500, msg = "Delete fails. Error:" + ex.Message });
            }

        }

        [HttpGet("/admin/category/categoryexists")]
        public async Task<bool> CategoryExists(string id)
        {
            return (await _unitOfWork.Categories.Get(x => x.Id == id)) != null;
        }
    }
}