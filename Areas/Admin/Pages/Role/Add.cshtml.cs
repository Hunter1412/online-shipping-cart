using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OnlineShoppingCart.Data;

namespace OnlineShoppingCart.Areas.Admin.Pages.Role
{
    [Authorize(Roles = "admin")]
    public class AddModel : RolePageModel
    {
        protected readonly ILogger<AddModel> _logger;

        public AddModel(ILogger<AddModel> logger, RoleManager<IdentityRole> roleManager, ApplicationDbContext context) : base(roleManager, context)
        {
            _logger = logger;
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
        public InputModel? Input { get; set; }

        [BindProperty]
        public bool IsUpdate { get; set; }

        public IActionResult OnGet() => NotFound();
        public IActionResult OnPost() => NotFound();

        public IActionResult OnPostStartNewRole()
        {
            // StatusMessage = "Input role information to update new role";
            IsUpdate = false;
            ModelState.Clear(); //xoa model cu
            return Page();
        }

        public async Task<IActionResult> OnPostStartUpdate()
        {
            StatusMessage = null!;
            IsUpdate = true;
            if (Input.Id == null)
            {
                StatusMessage = "Error Role not found";
                return Page();
            }

            var result = await _roleManager.FindByIdAsync(Input.Id);
            if (result != null)
            {
                Input.Name = result.Name;
                ViewData["Title"] = "Update role: " + Input.Name;
                ModelState.Clear(); //xoa model cu
            }
            else
            {
                StatusMessage = $"Error Role {Input.Id} not found.";
            }
            return Page();
        }

        //update or add --> isUpdate
        public async Task<IActionResult> OnPostAddOrUpdate()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = null!;
                return Page();
            }

            if (IsUpdate)
            {
                if (Input?.Id == null)
                {
                    StatusMessage = "Error Role not found";
                    return Page();
                }
                var result = await _roleManager.FindByIdAsync(Input.Id);
                if (result != null)
                {
                    result.Name = Input.Name;
                    //update role
                    var resultUpdateRo = await _roleManager.UpdateAsync(result);
                    if (resultUpdateRo.Succeeded)
                    {
                        StatusMessage = $"Update Role {Input.Name} successfully";
                        return RedirectToPage("./Index");
                    }
                    else
                    {
                        StatusMessage = "Error : ";
                        foreach (var item in resultUpdateRo.Errors)
                        {
                            StatusMessage += item.Description;
                        }
                    }
                }
                else
                {
                    StatusMessage = $"Error: Can't find this role {Input.Id} updated";
                }
            }
            else
            {
                var newRole = new IdentityRole(Input?.Name!);
                var result = await _roleManager.CreateAsync(newRole);
                if (result.Succeeded)
                {
                    StatusMessage = $"Create new role {Input?.Name} successfully";
                    return RedirectToPage("./Index");
                }
                else
                {
                    result.Errors.ToList().ForEach(error =>
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    });
                }
            }
            return Page();
        }

    }
}