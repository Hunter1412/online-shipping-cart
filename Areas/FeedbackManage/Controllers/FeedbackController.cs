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

        public async Task<IEnumerable<FeedbackDto>> GetItemsSelectFeedbacks(string? id = null)
        {

            var feedbacks = await _unitOfWork.Feedbacks.GetAll("Parent,Children");

            var itemsDto = feedbacks == null
                ? new List<FeedbackDto>()
                : feedbacks.Select(c => _mapper.Map<FeedbackDto>(c)).Where(c => c.Parent == null).ToList();


            List<FeedbackDto> resultItems = new List<FeedbackDto>() {
                new FeedbackDto() {
                    Id = "-1",
                    Content = "Not parent feedback"
                }
            };
            Action<List<FeedbackDto>, int> _ChangeTitleFeedback = null!;
            Action<List<FeedbackDto>, int> ChangeTitleFeedback = (itemsDto, level) =>
            {
                string prefix = string.Concat(Enumerable.Repeat("—", level));
                foreach (var item in itemsDto)
                {
                    resultItems.Add(new FeedbackDto()
                    {
                        Id = item.Id,
                        Content = prefix + " " + item.Content + "_" + item.Id
                    });
                    if ((item.Id != id) && (item.Children != null) && (item.Children.Count > 0))
                    {
                        _ChangeTitleFeedback(item.Children.ToList(), level + 1);
                    }
                }

            };

            _ChangeTitleFeedback = ChangeTitleFeedback;
            ChangeTitleFeedback(itemsDto, 0);

            return resultItems;
        }


        [HttpGet("/admin/feedback")]
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _unitOfWork.Feedbacks.GetAll("Parent,Children");

            var feedbackDtoList = feedbacks == null
                ? new List<FeedbackDto>()
                : feedbacks.Select(c => _mapper.Map<FeedbackDto>(c)).Where(c => c.Parent == null).ToList();

            return View(feedbackDtoList);
        }


        [HttpGet("/admin/feedback/answer/{id}")]
        public async Task<IActionResult> Answer(string id)
        {
            if (id == null || _unitOfWork.Feedbacks == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var feedback = await _unitOfWork.Feedbacks.Get(m => m.Id == id, "AppUser");
            var feedbackDto = _mapper.Map<FeedbackDto>(feedback) ?? new FeedbackDto();

            return View(feedbackDto);
        }

        [HttpPost("/admin/feedback/admin-answer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminAnswer(IFormCollection form)
        {
            _logger.LogInformation("Feedback - Answer action");

            AppUser? user = await _userManager.GetUserAsync(User);
            try
            {
                var feedback = new Feedback()
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = form["content"],
                    Rating = 0,
                    Image = null,
                    UserId = user!.Id,
                    ProductId = form["productId"],
                    ParentId = form["parentId"]
                };

                await _unitOfWork.Feedbacks.Add(feedback);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error Answer method");
                return RedirectToAction("Answer", new { id = form["parentId"] });
            }
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

            return RedirectToAction("ShopSingle", "Shop", new { id = form["productSlug"] });
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
            return RedirectToAction("ShopSingle", "Shop", new { id = productSlug });
        }


        [HttpGet("/admin/feedback/remove")]
        public async Task<IActionResult> Remove(string id)
        {
            var feedback = await _unitOfWork.Feedbacks.Get(f => f.Id == id);
            return View(_mapper.Map<FeedbackDto>(feedback));
        }


        [HttpPost("/admin/feedback/remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminDelete(string id)
        {
            if (id != null)
            {
                var feedback = await _unitOfWork.Feedbacks.Get(f => f.Id == id);
                if (feedback != null)
                {
                    _unitOfWork.Feedbacks.Delete(feedback);
                    await _unitOfWork.CompleteAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

    }
}