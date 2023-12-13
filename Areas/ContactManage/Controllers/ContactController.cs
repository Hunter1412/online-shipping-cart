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
    [Authorize(Roles = "admin")]
    [Area("ContactManage")]
    public class ContactController : Controller
    {
        protected readonly ILogger<ContactController> _logger;

        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _unitOfWork;

        protected readonly UserManager<AppUser> _userManager;
        protected readonly IEmailSender _emailSender;

        public ContactController(ILogger<ContactController> logger, UserManager<AppUser> userManager, IEmailSender emailSender, IMapper mapper, IUnitOfWork unitOfWork)
        {
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
            var contactsDto = contacts == null
                                ? new List<ContactDto>()
                                : contacts.Select(c => _mapper.Map<ContactDto>(c)).OrderByDescending(x => x.CreateAt).ToList();
            return View(contactsDto);
        }

        [HttpGet("/admin/contact/detail/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _unitOfWork.Contacts == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var contact = await _unitOfWork.Contacts.Get(m => m.Id == id, "AppUser");
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
                if (contactDto.AppUser!.Email == null)
                {
                    return RedirectToAction(nameof(SendContact));
                }
                var contact = _mapper.Map<Contact>(contactDto);
                var email = contact!.AppUser!.Email;
                AppUser? user = await _unitOfWork.Users.Get(x => x.Email == email);

                var id = string.Empty;
                if (user == null)
                {
                    //create new user
                    var newUser = new AppUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = contactDto.AppUser.FirstName,
                        LastName = contactDto.AppUser.LastName,
                        PhoneNumber = contactDto.AppUser.PhoneNumber,
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
                        var content = @$"<h3>Hi {contactDto.AppUser.FirstName}!</h3>
                            We will contact with you as soon as! \n
                            Please patience to waiting us, many thanks with best regards.\n
                            Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.\n
                            <p>Thank you so much</p>";

                        await _emailSender.SendEmailAsync(email!, "Thank you for your fe", content
                        );

                        id = userId;
                    }
                }
                else
                {
                    id = user.Id;
                    //send mail
                    await _emailSender.SendEmailAsync(email!, "Thank you for your feedback",
                            $"<h3>Thank you for your feedback!</h3>\n We will contact with you as soon as! Please patience to waiting us, many thanks with best regards."
                    );
                }
                //save db
                contact.UserId = id;
                contact.Id = Guid.NewGuid().ToString();
                await _unitOfWork.Contacts.Add(contact);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
            return View("SendContact", contactDto);
        }

        [HttpGet("/admin/contact/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _unitOfWork.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _unitOfWork.Contacts.Get(x => x.Id == id, "AppUser");
            if (contact == null)
            {
                return NotFound();
            }
            return View(_mapper.Map<ContactDto>(contact));
        }

        [HttpPost("/admin/contact/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Subject,Content,Answer,UserId,AppUser,CreateAt,UpdateAt")] ContactDto contactDto)
        {
            if (id != contactDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var contact = _mapper.Map<Contact>(contactDto);
                    _logger.LogInformation(contactDto.AppUser!.Email);
                    var email = contact!.AppUser!.Email;

                    if (email != null)
                    {
                        //send email
                        await _emailSender.SendEmailAsync(
                            email,
                            "Answer for your question",
                            contact.Answer + "<p>Thanks</p>"
                        );
                    }

                    contact.UpdateAt = DateTime.Now;
                    await _unitOfWork.Contacts.Upsert(contact);
                    await _unitOfWork.CompleteAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Error edit method", typeof(ContactController));
                    return RedirectToAction(nameof(Edit));
                }
                return RedirectToAction(nameof(Index));
            }
            return View(contactDto);
        }

        [HttpGet("/admin/contact/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _unitOfWork.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _unitOfWork.Contacts.Get(m => m.Id == id, "AppUser");
            if (contact == null)
            {
                return NotFound();
            }
            return View(_mapper.Map<ContactDto>(contact));
        }

        [HttpPost("/admin/contact/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_unitOfWork.Contacts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Contacts'  is null.");
            }
            var contact = await _unitOfWork.Contacts.Get(x => x.Id == id);
            if (contact != null)
            {
                _unitOfWork.Contacts.Delete(contact);
            }
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
