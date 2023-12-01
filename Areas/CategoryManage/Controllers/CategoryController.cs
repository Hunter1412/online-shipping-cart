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

namespace OnlineShoppingCart.Areas.CategoryManage.Controllers
{
    [Area("CategoryManage")]
    public class CategoryController : Controller
    {
        //test


        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ImageService _imageService;

        public CategoryController(ApplicationDbContext context, IWebHostEnvironment env, IMapper mapper, ImageService imageService)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
            _imageService = imageService;
        }


        public async Task<IEnumerable<CategoryDto>> GetItemsSelectCategorie(string id)
        {

            var query = (from c in _context.Categories select c)
                    .Include(c => c.Parent)
                    .Include(c => c.Children);

            var items = (await query.ToListAsync())
                             .Where(c => c.Parent == null)
                             .ToList();

            List<CategoryDto> itemsDto = _mapper.Map<List<CategoryDto>>(items);


            List<CategoryDto> resultitems = new List<CategoryDto>() {
                new CategoryDto() {
                    Id = "-1",
                    Name = "Không có danh mục cha"
                }
            };
            Action<List<CategoryDto>, int> _ChangeTitleCategory = null!;
            Action<List<CategoryDto>, int> ChangeTitleCategory = (itemsDto, level) =>
            {
                string prefix = String.Concat(Enumerable.Repeat("—", level));
                foreach (var item in itemsDto)
                {

                    resultitems.Add(new CategoryDto()
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

            return resultitems;
        }

        // GET: Category
        [HttpGet("/admin/category")]
        public async Task<IActionResult> Index()
        {
            var query = (from c in _context.Categories select c)
                     .Include(c => c.Parent)               // load parent category
                     .Include(c => c.Children);              // load parent category

            var categories = (await query.ToListAsync())
                             .Where(c => c.Parent == null)
                             .OrderByDescending(c => c.CreateAt)
                             .ToList();

            var categoryList = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return View(categoryList);
        }

        [HttpGet("/admin/category/details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<CategoryDto>(category));
        }

        // GET: Category/Create
        [HttpGet("/admin/category/create")]
        public async Task<IActionResult> Create()
        {
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategorie(""), "Id", "Name", "-1");
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/admin/category/create")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (categoryDto.ParentId == "-1")
                        categoryDto.ParentId = null;

                    string filename = _imageService.UpLoadImage(categoryDto.ImageFile!);
                    categoryDto.Image = filename;
                    categoryDto.Slug = Slug.GenerateSlug(categoryDto.Name!);
                    _context.Add(_mapper.Map<Category>(categoryDto));
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    if (!CategoryExists(categoryDto.Id!))
                    {
                        return NotFound();
                    }

                    else
                    {
                        return View(categoryDto);
                    }
                }
            }
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategorie(""), "Id", "Name", categoryDto.ParentId);
            return View(categoryDto);
        }

        // GET: Categories/Edit/5
        [HttpGet("/admin/category/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategorie(id), "Id", "Name", category.ParentId);
            return View(_mapper.Map<CategoryDto>(category));
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/admin/category/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return NotFound();
            }

            if (categoryDto.ParentId == id)
            {
                return Json(new { code = 201, msg = "Can't select category !" });
            }

            string filename = string.Empty;

            if (ModelState.IsValid && (categoryDto.ParentId != categoryDto.Id))
            {
                try
                {
                    //Images
                    if (categoryDto.Image != null)
                    {
                        if (categoryDto.ImageFile != null)
                        {
                            _imageService.DeleteImage(categoryDto.Image);
                            categoryDto.Image = _imageService.UpLoadImage(categoryDto.ImageFile);
                        }
                        else
                        {
                            categoryDto.Image = categoryDto.Image;
                        }
                    }
                    else
                    {
                        categoryDto.Image = _imageService.UpLoadImage(categoryDto.ImageFile!);
                    }

                    //Category and ParentId
                    if (categoryDto.ParentId == "-1")
                        categoryDto.ParentId = null;

                    categoryDto.Slug = Slug.GenerateSlug(categoryDto.Name!);

                    _context.Update(_mapper.Map<Category>(categoryDto));
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(categoryDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategorie(id), "Id", "Name", categoryDto.ParentId);
            return View(categoryDto);
        }

        // GET: Category/Delete/5
        [HttpPost("/admin/category/delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            try
            {
                var category = await _context.Categories!
                    .Include(c => c.Children)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound();
                }

                var cateProduct = await _context.Products!.SingleOrDefaultAsync(c => c.Id == id);
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

                _context.Categories!.Remove(category);
                await _context.SaveChangesAsync();
                return Json(new { code = 200, msg = "Delete successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Delete fails. Error:" + ex.Message });

            }

        }

        [HttpGet("/admin/category/categoryexists")]
        public bool CategoryExists(string id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}