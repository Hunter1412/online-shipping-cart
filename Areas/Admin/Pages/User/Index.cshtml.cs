using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Areas.Admin.Pages.User
{
    [Authorize(Roles = "admin")]
    public class Index : PageModel
    {
        const int USER_PER_PAGE = 10;
        protected readonly ILogger<Index> _logger;

        protected readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public Index(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<Index> logger
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; } = null!;
        [BindProperty(SupportsGet = true)]
        public int PageNumber { set; get; }

        public class UserAndRole : AppUser
        {
            public string? ListRoles { get; set; }
        }

        public List<UserAndRole>? Users { get; set; }
        public int TotalPages { set; get; }


        public IActionResult OnPost() => NotFound();

        public async Task<IActionResult> OnGet()
        {
            //lay danh sach user
            Users = await _userManager.Users.OrderBy(u => u.UserName)
                .Select(u => new UserAndRole { Id = u.Id, UserName = u.UserName, Email = u.Email })
                .ToListAsync();

            // int totalUsers = Users.Count;
            // TotalPages = (int)Math.Ceiling((double)totalUsers / USER_PER_PAGE);

            // Users = Users.Skip(USER_PER_PAGE * (PageNumber - 1)).Take(USER_PER_PAGE).ToList();


            foreach (var user in Users)
            {
                //lay role cua tung user
                var roles = await _userManager.GetRolesAsync(user);
                user.ListRoles = string.Join(",", roles);
            }

            return Page();
        }


    }
}