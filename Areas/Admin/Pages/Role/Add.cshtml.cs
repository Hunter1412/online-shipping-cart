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
    public class AddModel : PageModel
    {
        protected readonly ILogger<AddModel> _logger;
        protected readonly RoleManager<IdentityRole> _roleManager;

        [TempData]
        public string StatusMessage { get; set; }

        public AddModel(ILogger<AddModel> logger, RoleManager<IdentityRole> roleManager = null)
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
        public bool IsUpdate { get; set; }


        //
        public IActionResult OnGet() => NotFound();
        public IActionResult OnPost() => NotFound();

        public IActionResult OnPostStartNewRole()
        {
            StatusMessage = null!;
            IsUpdate = false;
            ModelState.Clear(); //xoa model cu
            return Page();
        }

        public async Task<IActionResult> OnPostStartUpdate()
        {
            // StatusMessage = "Input role information to update new role";
            IsUpdate = true;
            if (Input.Id == null)
            {
                StatusMessage = "Error Role not found";
                return Page();
            }

            var rr = await _roleManager.FindByIdAsync(Input.Id);
            if (rr != null)
            {
                Input.Name = rr.Name;
                ModelState.Clear(); //xoa model cu
            }
            else
            {
                StatusMessage = $"Error Role {Input.Id} not found.";
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAddOrUpdate()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = null!;
                return Page();
            }

            if (IsUpdate)
            {
                if (Input.Id == null)
                {
                    StatusMessage = "Error Role not found";
                    return Page();
                }
                var rr = await _roleManager.FindByIdAsync(Input.Id);
                if (rr != null)
                {
                    rr.Name = Input.Name;
                    var result = await _roleManager.UpdateAsync(rr);
                    if (result.Succeeded)
                    {
                        StatusMessage = $"Update Role {Input.Name} successfully";
                        return RedirectToPage("./Index");
                    }
                    else
                    {
                        StatusMessage = "Error";
                        foreach (var item in result.Errors)
                        {
                            StatusMessage += item.Description;
                        }
                    }
                }
                else
                {
                    StatusMessage = $"Error update role {Input.Id} not found";
                }
            }
            else
            {
                var rr = new IdentityRole(Input.Name!);
                var result = await _roleManager.CreateAsync(rr);
                if (result.Succeeded)
                {
                    StatusMessage = $"Create new role {Input.Name} successfully";
                    return RedirectToPage("./Index");
                }
                else
                {
                    StatusMessage = "Error";
                    foreach (var item in result.Errors)
                    {
                        StatusMessage += item.Description;
                    }
                }
            }
            return Page();
        }

    }
}