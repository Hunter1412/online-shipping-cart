using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Data.Entities;

namespace App.Areas.VoucherManage.Controllers
{
    [Area("VoucherManage")]
    public class VoucherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VoucherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VoucherManage/Voucher
        [HttpGet("/admin/voucher")]
        public async Task<IActionResult> Index()
        {
              return _context.Vouchers != null ?
                          View(await _context.Vouchers.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Vouchers'  is null.");
        }

        // GET: VoucherManage/Voucher/Details/5
        [HttpGet("/admin/voucher/details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Vouchers == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View(voucher);
        }

        // GET: VoucherManage/Voucher/Create
        [HttpGet("/admin/voucher/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: VoucherManage/Voucher/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/admin/voucher/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Code,Discount,MinimumBill,ExpDate,CreateAt")] Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(voucher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(voucher);
        }

        // GET: VoucherManage/Voucher/Edit/5
        [HttpGet("/admin/voucher/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Vouchers == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return View(voucher);
        }

        // POST: VoucherManage/Voucher/Edit/5
        [HttpPost("/admin/voucher/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Code,Discount,MinimumBill,ExpDate,CreateAt")] Voucher voucher)
        {
            if (id != voucher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voucher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoucherExists(voucher.Id))
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
            return View(voucher);
        }

        // GET: VoucherManage/Voucher/Delete/5
        [HttpGet("/admin/voucher/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Vouchers == null)
            {
                return NotFound();
            }

            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (voucher == null)
            {
                return NotFound();
            }

            return View(voucher);
        }

        // POST: VoucherManage/Voucher/Delete/5
        [HttpPost("/admin/voucher/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Vouchers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vouchers'  is null.");
            }
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher != null)
            {
                _context.Vouchers.Remove(voucher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoucherExists(string id)
        {
          return (_context.Vouchers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
