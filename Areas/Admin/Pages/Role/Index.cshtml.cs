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

namespace OnlineShoppingCart.Areas.Admin.Pages.Role
{
    // [Authorize(Roles = "admin")]
    public class Index : PageModel
    {
        private readonly ILogger<Index> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        [TempData]
        public string StatusMessage { get; set; }

        public Index(ILogger<Index> logger, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        public List<IdentityRole>? Roles { get; set; } = default;

        public async Task<IActionResult> OnGetAsync()
        {
            Roles = await _roleManager.Roles.ToListAsync();
            return Page();
        }
    }
}