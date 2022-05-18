using TMDTShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMDTShop.ViewModel
{
    public class CartItem
    {
        public Product product { get; set; }
        public int? Qty { get; set; }
        public decimal? TotalMoney => Qty * product.SalesPrice.Value;
    }
}
