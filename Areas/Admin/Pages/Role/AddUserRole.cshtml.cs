using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Areas.Admin.Pages.Role
{
    public class AddUserRole : PageModel
    {
        protected readonly ILogger<AddUserRole> _logger;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly UserManager<AppUser> _userManager;


        [TempData]
        public string? StatusMessage { get; set; }

        public AddUserRole(ILogger<AddUserRole> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public class InputModel
        {
            [Display(Name = "Role Id")]
            public string? Id { get; set; }

            [Display(Name = "Role Name")]
            [Required(ErrorMessage = "Please input the role name")]
            [StringLength(100, ErrorMessage = "{0} from {2} to {1} characters", MinimumLength = 3)]
            public string? Name { get; set; }

            public string[]? RoleName { get; set; }

        }

        [BindProperty]
        public InputModel? Input { get; set; }


        [BindProperty]
        public bool IsConfirm { get; set; } //default = false
        public SelectList? AllRoles { get; set; }

        public IActionResult OnGet() => NotFound("Not found");



        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            Input.Name = user.UserName;

            var userRs = await _userManager.GetRolesAsync(user);

            var allRs = await _roleManager.Roles.ToListAsync();
            var allRolesName = allRs.Select(x => x.Name).ToList();

            AllRoles = new SelectList(allRolesName);
            // AllRoles = new SelectList(allRs, "Id", "Name");

            if (!IsConfirm)
            {
                IsConfirm = true;
                Input.RoleName = userRs.ToArray();
                StatusMessage = "";
                ModelState.Clear();
            }
            else
            {
                StatusMessage = "Add role to user";
                if (Input.RoleName == null)
                {
                    Input.RoleName = new string[] { };
                }

                //add new role
                foreach (var item in Input.RoleName)
                {
                    if (!userRs.Contains(item))
                    {
                        await _userManager.AddToRoleAsync(user, item);
                    }
                }

                //delete old role
                foreach (var item in userRs)
                {
                    if (!Input.RoleName.Contains(item))
                    {
                        await _userManager.RemoveFromRoleAsync(user, item);
                    }
                }

                return RedirectToPage("/Role/User");
            }

            return Page();
        }




    }
}