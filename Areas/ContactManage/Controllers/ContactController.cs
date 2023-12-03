using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models.DTOs;
using ContactEntity = OnlineShoppingCart.Data.Entities.Contact;

namespace OnlineShoppingCart.Areas.ContactManage.Controllers
{
    [Authorize]
    [Area("ContactManage")]
    public class ContactController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<ContactController> _logger;

        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _unitOfWork;

        protected readonly UserManager<AppUser> _userManager;
        protected readonly IEmailSender _emailSender;

        public ContactController(ApplicationDbContext context, ILogger<ContactController> logger, UserManager<AppUser> userManager, IEmailSender emailSender, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _emailSender = emailSender;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("/admin/contact")]
        public async Task<IActionResult> Index()
        {
            var contacts = await _unitOfWork.Contacts.GetAll("AppUser");
            var contactsDto = _mapper.Map<List<ContactDto>>(contacts) ?? new List<ContactDto>();
            return View(contactsDto);
        }

        [HttpGet("/admin/contact/detail/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _unitOfWork.Contacts == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var contact = await _unitOfWork.Contacts.Get(m => m.Id == id);
            var contactDto = _mapper.Map<ContactDto>(contact) ?? new ContactDto();

            return View(contactDto);
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
        public async Task<IActionResult> Create([Bind("Id,Subject,Content,Answer,UserId,AppUser,CreateAt,UpdateAt")] ContactDto contactDto)
        {
            if (ModelState.IsValid)
            {
                var contact = _mapper.Map<Contact>(contactDto);
                _logger.LogInformation(contact?.AppUser?.Email);
                var email = contact.AppUser.Email;
                var user = await _userManager.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

                var id = string.Empty;
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
                        var userId = await _userManager.GetUserIdAsync(newUser);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme
                        );
                        //send mail
                        await _emailSender.SendEmailAsync(email!, "Thank you for your feedback",
                            $"<h3>Thank you for your feedback!</h3>We will contact with you as soon as! \n Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.\n<p>Thank you so much</p>"
                        );

                        id = userId;
                    }
                }
                else
                {
                    id = user.Id;
                    //send mail
                    await _emailSender.SendEmailAsync(email!, "Thank you for your feedback",
                            $"<h3>Thank you for your feedback!</h3>\n We will contact with you as soon as!</a>.\n<p>Thank you so much</p>"
                    );
                }
                //save db
                contact.UserId = id;
                contact.Id = Guid.NewGuid().ToString();
                _context.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
            return View("SendContact", contactDto);
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,Subject,Content,Answer,UserId,CreateAt,UpdateAt")] ContactEntity contact)
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
