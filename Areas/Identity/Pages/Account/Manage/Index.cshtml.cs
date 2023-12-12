// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Utils;

namespace OnlineShoppingCart.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly ImageService _imageService;

        protected readonly IUnitOfWork _unitOfWork;



        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ImageService imageService,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = null!;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Display(Name = "First name")]
            public string FirstName { get; set; } = null!;

            [Display(Name = "Last name")]
            public string LastName { get; set; } = null!;

            [Display(Name = "Male")]
            public bool Gender { get; set; }

            [DataType(DataType.Date)]
            [Range(typeof(DateTime), "1/1/1950", "1/1/2004", ErrorMessage = "Value for {0} must be between {1} and {2}")]
            public DateTime? BirthDay { get; set; }
            [Display(Name = "Upload avatar")]
            public IFormFile ImageFile { get; set; } = null!;

            public string Avatar { get; set; } = null!;
        }

        public AppUser AppUser { get; set; }
        public string Roles { get; set; } = null!;

        public virtual List<Order> GetOrdersBy { get; set; }
        public List<OrderDetail> GetOrderItemAll { get; set; }


        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var listRoles = await _userManager.GetRolesAsync(user);
            Roles = string.Join(", ", listRoles);

            Username = userName;
            AppUser = user;
            AppUser.Birthday = user.Birthday != null ? (DateTime)user.Birthday : DateTime.Now;
            AppUser.Avatar = _imageService.IsExist(user.Avatar) ? user.Avatar : "default-avatar-image.png";

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDay = user.Birthday != null ? (DateTime)user.Birthday : DateTime.Now,
                Avatar = user.Avatar
            };

            var orderItemsAll = await _unitOfWork.OrderDetails.GetAll("Order,Product");
            GetOrderItemAll = orderItemsAll;

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.FirstName != null)
            {
                user.FirstName = Input.FirstName;
            }
            if (Input.LastName != null)
            {
                user.LastName = Input.LastName;
            }
            if (Input.BirthDay != null)
            {
                user.Birthday = Input.BirthDay;
            }
            if (Input.ImageFile != null && Input.ImageFile.FileName != null)
            {
                if (user.Avatar != null)
                {
                    _imageService.DeleteImage(user.Avatar);
                }
                user.Avatar = _imageService.UpLoadImage(Input.ImageFile, "avatar_");
            }

            user.Gender = Input.Gender;

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

    }
}
