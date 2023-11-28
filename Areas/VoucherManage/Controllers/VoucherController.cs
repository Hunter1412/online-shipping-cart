using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Core.UnitOfWork;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;
using OnlineShoppingCart.Models.DTOs;

namespace App.Areas.VoucherManage.Controllers
{
    [Area("VoucherManage")]
    public class VoucherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VoucherController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public VoucherController(ApplicationDbContext context, ILogger<VoucherController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: VoucherManage/Voucher
        [HttpGet("/admin/voucher")]
        public async Task<IActionResult> Index()
        {
            var items = await _unitOfWork.Vouchers.All();
            var voucherList = _mapper.Map<IEnumerable<VoucherDto>>(items);
            return View(voucherList);
        }

        // GET: VoucherManage/Voucher/Details/5
        [HttpGet("/admin/voucher/details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var item = await _unitOfWork.Vouchers.GetById(id);
            return View(_mapper.Map<VoucherDto>(item));
        }

        // GET: VoucherManage/Voucher/Create
        [HttpGet("/admin/voucher/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: VoucherManage/Voucher/Create
        [HttpPost("/admin/voucher/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Code,Discount,MinimumBill,ExpDate,CreateAt")] VoucherDto voucherDto)
        {
            if (ModelState.IsValid)
            {
                var voucher = _mapper.Map<Voucher>(voucherDto);
                await _unitOfWork.Vouchers.Add(voucher);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(voucherDto);
        }

        // GET: VoucherManage/Voucher/Edit/5
        [HttpGet("/admin/voucher/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _unitOfWork.Vouchers == null)
            {
                return NotFound();
            }
            var voucher = await _unitOfWork.Vouchers.GetById(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return View(_mapper.Map<VoucherDto>(voucher));
        }

        // POST: VoucherManage/Voucher/Edit/5
        [HttpPost("/admin/voucher/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Code,Discount,MinimumBill,ExpDate,CreateAt")] VoucherDto voucherDto)
        {
            if (id != voucherDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var voucher = _mapper.Map<Voucher>(voucherDto);
                    await _unitOfWork.Vouchers.Upsert(voucher);
                    await _unitOfWork.CompleteAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoucherExists(voucherDto.Id))
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
            return View(voucherDto);
        }

        // GET: VoucherManage/Voucher/Delete/5
        [HttpGet("/admin/voucher/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _unitOfWork.Vouchers == null)
            {
                return NotFound();
            }
            var voucher = await _unitOfWork.Vouchers.GetById(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return View(_mapper.Map<VoucherDto>(voucher));
        }

        // POST: VoucherManage/Voucher/Delete/5
        [HttpPost("/admin/voucher/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_unitOfWork.Vouchers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vouchers'  is null.");
            }
            await _unitOfWork.Vouchers.Delete(id);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoucherExists(string id)
        {
            return (_context.Vouchers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
