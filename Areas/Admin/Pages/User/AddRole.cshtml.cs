using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShoppingCart.Data.Entities;

namespace OnlineShoppingCart.Areas.Admin.Pages.User
{
    [Authorize(Roles = "admin")]
    public class AddRole : PageModel
    {
        protected readonly ILogger<AddRole> _logger;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly UserManager<AppUser> _userManager;


        [TempData]
        public string? StatusMessage { get; set; }

        public AddRole(ILogger<AddRole> logger, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
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
            if (Input == null || Input.Id == null)
            {
                return NotFound("Not found user");
            }
            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            Input.Name = user.UserName;

            var userRs = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            var allRolesName = await _roleManager.Roles.Select(x => x.Name).ToListAsync();

            AllRoles = new SelectList(allRolesName);

            if (!IsConfirm)
            {
                IsConfirm = true;
                Input.RoleName = userRs;
                StatusMessage = "";
                ModelState.Clear();
            }
            else
            {
                if (Input.RoleName == null)
                {
                    Input.RoleName = Array.Empty<string>();
                }
                //add new role
                var deleteRoles = userRs.Where(r => !Input.RoleName.Contains(r));
                var addRoles = Input.RoleName.Where(r => !userRs.Contains(r));

                var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRoles);

                if (!resultDelete.Succeeded)
                {
                    resultDelete.Errors.ToList().ForEach(error =>
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    });
                    return Page();
                }

                var resultAdd = await _userManager.AddToRolesAsync(user, addRoles);

                if (!resultAdd.Succeeded)
                {
                    resultDelete.Errors.ToList().ForEach(error =>
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    });
                    return Page();
                }

                StatusMessage = $"Add role to user {user.UserName}";

                return RedirectToPage("./Index");
            }

            return Page();
        }




    }
}