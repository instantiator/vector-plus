using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VectorPlus.Web.Models;
using VectorPlus.Web.Service;

namespace VectorPlus.Web.Controllers
{
    public class ConnectionController : AbstractVectorPlusController<ConnectionController>
    {
        public ConnectionController(ILogger<ConnectionController> logger, VectorPlusBackgroundService service) : base(logger, service)
        {
        }

        public IActionResult Index()
        {
            SetCoreViewData();
            var model = ConnectionViewModel.From(service);
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(ConnectionViewModel model)
        {
            var response = await service.SetRoboConfigAsync(
                model.RoboConfig.RobotName,
                model.RoboConfig.RobotSerial,
                model.RoboConfig.Email,
                model.RoboConfig.Password,
                model.RoboConfig.IpOverrideStr);

            ViewData["Result"] = response;
            return Index();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAsync()
        {
            await service.EraseRoboConfigAsync();
            ViewData["Result"] = ActionResponseMessage.Success("TODO: Erased connection configuration.");
            return Index();
        }
    }
}