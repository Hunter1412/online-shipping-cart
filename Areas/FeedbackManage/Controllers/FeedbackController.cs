using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models.DTOs;

namespace OnlineShoppingCart.Areas.FeedbackManage.Controllers
{
    [Authorize]
    [Area("feedbackManage")]
    public class FeedbackController : Controller
    {
        protected readonly IMapper _mapper;
        protected readonly ILogger<FeedbackController> _logger;
        protected readonly IUnitOfWork _unitOfWork;

        public FeedbackController(ILogger<FeedbackController> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("/admin/feedback")]
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _unitOfWork.Contacts.GetAll("AppUser");
            var feedbackDtoList = feedbacks == null
                                ? new List<FeedbackDto>()
                                : feedbacks.Select(c => _mapper.Map<FeedbackDto>(c)).OrderByDescending(x => x.CreateAt).ToList();
            return View(feedbackDtoList);
        }


        [HttpGet("/admin/feedback/detail/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _unitOfWork.Feedbacks == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var feedback = await _unitOfWork.Feedbacks.Get(m => m.Id == id, "AppUser");
            var feedbackDto = _mapper.Map<FeedbackDto>(feedback) ?? new FeedbackDto();

            return View(feedbackDto);
        }



        [HttpPost("/contact")]
        [AllowAnonymous] //khong can phan quyen van truy cap duoc
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,Rating,ProductId,UserId,CreateAt")] FeedbackDto feedbackDto)
        {
            if (ModelState.IsValid)
            {
                var feedback = _mapper.Map<Feedback>(feedbackDto);
                //save db
                feedback.Id = Guid.NewGuid().ToString();
                await _unitOfWork.Feedbacks.Add(feedback);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
            return RedirectToAction("ShopSingle", "Home");
        }

    }
}