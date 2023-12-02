using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;
using ContactModel = OnlineShoppingCart.Data.Entities.Contact;

namespace OnlineShoppingCart.Areas.ContactManage.Controllers
{
    [Authorize]
    [Area("ContactManage")]
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContactController> _logger;

        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ContactController(ApplicationDbContext context, ILogger<ContactController> logger, UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet("/admin/contact")]
        public async Task<IActionResult> Index()
        {
            return _context.Contacts != null ?
                        View(await _context.Contacts.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Contacts'  is null.");
        }

        [HttpGet("/admin/contact/detail/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        [HttpGet("/contact")]
        [AllowAnonymous] //khong can phan quyen van truy cap duoc
        public IActionResult SendContact()
        {
            return View();
        }

        [HttpPost("/contact")]
        [AllowAnonymous] //khong can phan quyen van truy cap duoc
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Subject,Content,Answer,UserId,AppUser,CreateAt,UpdateAt")] ContactModel contact)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation(contact?.AppUser?.Email);
                var email = contact.AppUser.Email;
                var user = await _userManager.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

                if (user == null)
                {
                    //create new user
                    var newUser = new AppUser
                    {
                        UserName = email,
                        Email = email
                    };
                    var resultNewUser = await _userManager.CreateAsync(newUser);
                    if (resultNewUser.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme
                        );

                        await _emailSender.SendEmailAsync(email, "Thank you for your feedback",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                        );

                        contact.UserId = userId;
                    }
                }
                contact.UserId = user!.Id;
                contact.Id = Guid.NewGuid().ToString();
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
            return View(contact);
        }

        [HttpGet("/admin/contact/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        [HttpPost("/admin/contact/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Subject,Content,Answer,UserId,CreateAt,UpdateAt")] ContactModel contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _userManager.Users.SingleOrDefault(u => u.Id == contact.UserId);
                    _logger.LogInformation(user.Email);
                    if (user != null && user.Email != null)
                    {
                        //send email
                        await _emailSender.SendEmailAsync(
                            user.Email,
                            "Answer for your question",
                            contact.Answer + "<p>Thanks</p>"
                        );
                    }
                    contact.UpdateAt = DateTime.Now;
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        [HttpGet("/admin/contact/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        [HttpPost("/admin/contact/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Contacts'  is null.");
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(string id)
        {
            return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
