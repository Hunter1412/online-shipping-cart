using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace OnlineShoppingCart.Areas.Admin.Pages.Role
{
    public class Delete : PageModel
    {
        private readonly ILogger<AddModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        [TempData]
        public string? StatusMessage { get; set; }

        public Delete(ILogger<AddModel> logger, RoleManager<IdentityRole> roleManager = null)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        public class InputModel
        {

            [Display(Name = "Role Id")]
            public string? Id { get; set; }

            [Display(Name = "Role Name")]
            [Required(ErrorMessage = "Please input the role name")]
            [StringLength(100, ErrorMessage = "{0} from {2} to {1} characters", MinimumLength = 3)]
            public string? Name { get; set; }

        }

        [BindProperty]
        public InputModel Input { get; set; }


        [BindProperty]
        public bool IsConfirm { get; set; }

        public IActionResult OnGet() => NotFound();


        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                return NotFound("Delete Error");
            }

            var rr = await _roleManager.FindByIdAsync(Input.Id!);
            if (rr == null)
            {
                return NotFound("Role not found");
            }
            ModelState.Clear();
            if (IsConfirm)
            {
                var result = await _roleManager.DeleteAsync(rr);
                StatusMessage = result.Succeeded ? "Delete role successfully!" : "Error cannot delete this role";
                return RedirectToPage("./Index");
            }
            else
            {
                Input.Name = rr.Name;
                IsConfirm = true;
            }
            return Page();


        }




    }
}