using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models.DTOs;

namespace OnlineShoppingCart.Areas.FeedbackManage.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("FeedbackManage")]
    public class FeedbackController : Controller
    {
        protected readonly IMapper _mapper;
        protected readonly ILogger<FeedbackController> _logger;
        protected readonly IUnitOfWork _unitOfWork;

        protected readonly UserManager<AppUser> _userManager;

        public FeedbackController(ILogger<FeedbackController> logger, IMapper mapper, IUnitOfWork unitOfWork, UserManager<AppUser> userManager = null)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
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



        [HttpPost("/feedback")]
        [AllowAnonymous] //khong can phan quyen van truy cap duoc
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection form)
        {
            _logger.LogInformation("Feedback - Create action");

            try
            {
                AppUser? user = await _userManager.GetUserAsync(User);

                var feedback = new Feedback
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = form["productId"],
                    Content = form["content"],
                    Rating = Convert.ToInt32(form["rating"]),
                    UserId = user!.Id,
                };

                await _unitOfWork.Feedbacks.Add(feedback);
                await _unitOfWork.CompleteAsync();
            }
            catch (System.Exception ex)
            {
                TempData["error"] = "Error input data invalid. Can not send feedback, try again...";
                _logger.LogError(ex, "Error Create method");
            }

            return RedirectToAction("ShopSingle", "Home", new { id = form["productSlug"] });
        }


        [HttpPost("/admin/feedback/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] string feedbackId, string productSlug)
        {
            if (feedbackId != null)
            {
                var feedback = await _unitOfWork.Feedbacks.Get(f => f.Id == feedbackId);
                if (feedback != null)
                {
                    _unitOfWork.Feedbacks.Delete(feedback);
                    await _unitOfWork.CompleteAsync();
                }
            }
            return RedirectToAction("ShopSingle", "Home", new { id = productSlug });
        }

    }
}