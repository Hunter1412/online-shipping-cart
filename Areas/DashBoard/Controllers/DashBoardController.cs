using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Areas.DashBoard.Controllers
{
    [Area("Dashboard")]
    public class DashBoardController : Controller
    {

        private readonly IMapper _mapper;

        public DashBoardController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        private readonly IUnitOfWork _unitOfWork;


        [HttpGet("/admin/dashboard")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/api/category/", Name = "apicate")]
        public async Task<ActionResult<List<Category>>> GetCategoryAsync()
        {
            var categories = await _unitOfWork.Categories.GetAll("Parent,Children");

            string value = string.Empty!;
            value = JsonConvert.SerializeObject(categories, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value );
        }

    }
}