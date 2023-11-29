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

namespace OnlineShoppingCart.Areas.Admin.Pages.Role
{
    // [Authorize("MyPolicy1")]
    public class User : PageModel
    {
        protected readonly ILogger<AddModel> _logger;
        protected readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public User(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AddModel> logger
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public class UserAndRole : AppUser
        {
            public string? ListRoles { get; set; }
        }

        public List<UserAndRole>? Users { get; set; }

        public IActionResult OnPost() => NotFound();

        public async Task<IActionResult> OnGet()
        {
            //lay danh sach user
            Users = await _userManager.Users
                .Select(u => new UserAndRole { Id = u.Id, UserName = u.UserName })
                .ToListAsync();
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