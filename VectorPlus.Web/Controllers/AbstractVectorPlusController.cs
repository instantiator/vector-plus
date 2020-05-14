using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VectorPlus.Web.Service;
using static VectorPlus.Lib.IVectorControllerPlus;

namespace VectorPlus.Web.Controllers
{
    public abstract class AbstractVectorPlusController<C> : Controller
    {
        protected readonly ILogger<C> logger;
        protected readonly IVectorPlusBackgroundService service;

        protected AbstractVectorPlusController(ILogger<C> logger, VectorPlusBackgroundService service)
        {
            this.logger = logger;
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> ReconnectAsync(string id)
        {
            await service.ReconnectAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DisconnectAsync(string id)
        {
            await service.DisconnectAsync();
            return RedirectToAction("Index", "Home");
        }

        protected void SetCoreViewData()
        {
            if (service != null && service.Controller != null)
            {
                ViewData["Connection"] = service.Controller.Connection;
                ViewData["Connected"] = service.Controller.Connection == ConnectedState.Connected;
            }
            else
            {
                ViewData["Connected"] = false;
            }
        }
    }
}
