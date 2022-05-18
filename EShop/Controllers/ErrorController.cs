using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMDTShop.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Errorr()
        {
            return View();
        }
    }
}
