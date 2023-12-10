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
using OnlineShoppingCart.Data;

namespace OnlineShoppingCart.Areas.Admin.Pages.Role
{
    [Authorize(Roles = "admin")]
    public class Index : RolePageModel
    {
        public Index(RoleManager<IdentityRole> roleManager, ApplicationDbContext context) : base(roleManager, context)
        {
        }

        public List<IdentityRole>? Roles { get; set; } = default;

        public async Task<IActionResult> OnGetAsync()
        {
            Roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            return Page();
        }

        public void OnPost() => RedirectToPage();
    }
}