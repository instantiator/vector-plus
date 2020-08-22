using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VectorPlus.Web.Models;
using VectorPlus.Web.Service;

namespace VectorPlus.Web.Controllers
{
    public class HomeController : AbstractVectorPlusController<HomeController>
    {
        public HomeController(ILogger<HomeController> logger, VectorPlusBackgroundService service) : base(logger, service)
        {
        }

        public IActionResult Index()
        {
            SetCoreViewData();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
