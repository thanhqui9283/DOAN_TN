using AspNetCoreHero.ToastNotification.Abstractions;
using TMDTShop.Models;
using TMDTShop.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMDTShop.Controllers
{
    public class DonHangController : Controller
    {
        private readonly EcommerceVer2Context _context;
        public INotyfService _notyfService { get; } //Import services

        public DonHangController(EcommerceVer2Context context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        public Order Detail { get; set; }


        [HttpGet]
        [Route("chi-tiet-don-hang-{id}.html", Name = "Chitietdonhang")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var taikhoannID = HttpContext.Session.GetString("CustommerId");
            if (string.IsNullOrEmpty(taikhoannID))
                return RedirectToAction("Login", "Accounts");
            var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustommerId == Convert.ToInt32(taikhoannID));
            if (khachhang == null)
                return NotFound();
            var donhang = _context.Orders
                .Include(x => x.TransactionStatus)
                .Include(x => x.Customer)
                .FirstOrDefault(m => m.OrderId == id && Convert.ToInt32(taikhoannID) == m.CustomerId);
            if (donhang == null)
            {
                return NotFound();
            }
            var chitietdonhang = _context.OrderDetails
                .Include(x => x.Product).AsNoTracking()
                .Where(x => x.OrderId == id)
                .OrderBy(x => x.OrderDetailId).ToList(); 
            XemDonHang donHang = new XemDonHang(); 
            donHang.DonHang = donhang;
            donHang.ChiTietDonHang = chitietdonhang;
            ViewBag.donHang = donHang;
            return View(donHang);
            //return PartialView("Details", donHang);
        }

        //Hủy đơn hàng
        [HttpPost]
        [Route("api/donhang/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                var _Order = _context.Orders.AsNoTracking().Where(x => x.OrderId == id).SingleOrDefault();
                if (_Order != null)
                {
                    _Order.IsDeleted = true;
                    foreach (var item in _Order.OrderDetails)
                    {
                        var product = _context.Products.AsNoTracking().Where(x => x.ProductId == item.ProductId).SingleOrDefault();
                        product.UnitInStock = product.UnitInStock + item.Quantity;
                        _context.Update(product);
                    }
                    _context.Update(_Order);
                    _context.SaveChanges();
                }
                _notyfService.Success("Đã hủy thành công");
                return Json(new { succcess = true });
            }
            catch (Exception)
            {
                return Json(new { succcess = false });
            }
        }
    }
}
