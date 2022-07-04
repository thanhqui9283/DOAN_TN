using AspNetCoreHero.ToastNotification.Abstractions;
using TMDTShop.Extension;
using TMDTShop.Models;
using TMDTShop.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMDTShop.Controllers
{
    public class ShopCartController : Controller
    {
        private readonly EcommerceVer2Context _context;
        public INotyfService _notyfService { get; } //Import services
        private readonly IConfiguration _config;

        private readonly string _clientId;
        private readonly string _sercretKey;

        public decimal TygiaUSD = 23000;

        public ShopCartController(EcommerceVer2Context context, INotyfService notyfService, IConfiguration config)
        {
            _notyfService = notyfService;
            _context = context;
            _clientId = config["PayPalSettings: ClientId"];
            _sercretKey = config["PayPalSettings: SecretKey"];
        }

        #region Khởi tạo giỏ hàng
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang"); // Lấy từ trong Session
                if (gh == default(List<CartItem>)) //có thì thôi
                {
                    gh = new List<CartItem>(); // ko thì tạo
                    HttpContext.Session.Set<List<CartItem>>("GioHang", gh);
                }
                return gh;
            }
        }
        #endregion

        private int Exists(List<CartItem> carts, int id)
        {
            for (var i = 0; i < carts.Count; i++)
            {
                if (carts[i].product.ProductId == id)
                {
                    return i;
                }
            }
            return -1;
        }

        #region Thao tác trên giỏ hàng

        // 1. Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productId, int? qty)
        {
            List<CartItem> carts = GioHang;
            try
            {
                Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productId);
               
                CartItem item = GioHang.SingleOrDefault(x => x.product.ProductId == productId);
                if (carts == null)
                {

                    carts.Add(new CartItem
                    {
                        Qty = qty.HasValue ? qty.Value : 1,
                        product = hh
                    });
                }
                else
                {
                    int index = Exists(carts, productId);
                    if (index == -1)
                    {
                        carts.Add(new CartItem
                        {
                            Qty = qty.HasValue ? qty.Value : 1, // chưa có mặc định nó về 1
                            product = hh
                        });
                    }
                    else
                    {
                        carts[index].Qty += qty;
                    }
                }
                

                GetSession.Set(HttpContext.Session, "GioHang", carts);
                //HttpContext.Session.Set<List<CartItem>>("GioHang", carts);
                _notyfService.Success("Thêm thành công vào giỏ hàng");
                return Json(new { succcess = true });
            }
            catch
            {
                return Json(new { succcess = false });
            }
        }

        // 2. Cập nhật giỏ hàng
        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productId, int? qty)
        {
            List<CartItem> carts = GioHang;
            try
            {
                if (carts != null)
                {
                    //CartItem item = GioHang.SingleOrDefault(x => x.product.ProductId == productId);
                    //if (item != null && qty.HasValue) // giỏ hàng có đồ
                    //{
                    //    item.Qty = qty.Value; // số lượng = số lượng nhập vào
                    //}

                    Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productId);
                    int index = Exists(carts, productId);
                    if (index == -1)
                    {
                        carts.Add(new CartItem
                        {
                            Qty = qty.HasValue ? qty.Value : 1, // chưa có mặc định nó về 1
                            product = hh
                        });
                        
                    }
                    else
                    {
                        if (hh.UnitInStock >= qty.Value)
                            carts[index].Qty = qty.Value;
                        else return Json(new { succcess = false,mess = "Số lượng đặt lớn hơn số lượng tồn kho" });
                    }

                    GetSession.Set(HttpContext.Session, "GioHang", carts);
                    _notyfService.Success("Update thành công");
                    return Json(new { succcess = true });
                }
                return Json(new { succcess = false });
            }
            catch
            {
                return Json(new { succcess = false });
            }
        }

        // 3. xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult Remove(int productId)
        {
            try
            {
                List<CartItem> carts = GioHang;
                CartItem item = carts.SingleOrDefault(a => a.product.ProductId == productId);
                if (item != null)
                {
                    carts.Remove(item);
                }
                GetSession.Set(HttpContext.Session, "GioHang", carts);
                _notyfService.Success("Xóa thành công sản phẩm khỏi giỏ hàng");
                return Json(new { succcess = true });
            }
            catch (Exception)
            {
                return Json(new { succcess = false });
            }
        }
        //
        [HttpPost]
        [Route("api/cart/checktonkho")]
        public IActionResult CheckCart(int productId, int? qty)
        {
            List<CartItem> carts = GioHang;
            try
            {
                if (carts != null)
                {

                    //CartItem item = GioHang.SingleOrDefault(x => x.product.ProductId == productId);
                    //if (item != null && qty.HasValue) // giỏ hàng có đồ
                    //{
                    //    item.Qty = qty.Value; // số lượng = số lượng nhập vào
                    //}
                    
                    Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productId);
                    if(hh.UnitInStock>=qty.Value)
                        return Json(new { succcess = true, data = true }) ;
                    return Json(new { succcess = true, data = false });
                }
                return Json(new { succcess = false ,data=false});

            }
            catch
            {
                return Json(new { succcess = false });
            }
        }
        #endregion

        [Route("CartItem.html", Name = "GioHang")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}
