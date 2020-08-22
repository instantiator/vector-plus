using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VectorPlus.Web.Models;
using VectorPlus.Web.Service;

namespace VectorPlus.Web.Controllers
{
    public class ConfigController : AbstractVectorPlusController<ConfigController>
    {
        public ConfigController(ILogger<ConfigController> logger, VectorPlusBackgroundService service) : base(logger, service)
        {
        }

        public IActionResult Index()
        {
            SetCoreViewData();
            ConfigViewModel model = ConfigViewModel.From(service);
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> ActivateBehaviourAsync(string id)
        {
            ViewData["Result"] = await service.ActivateBehaviourAsync(id);
            return Index();
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateBehaviourAsync(string id)
        {
            ViewData["Result"] = await service.DeactivateBehaviourAsync(id);
            return Index();
        }

        [HttpPost]
        public IActionResult RemoveModule(string id)
        {
            ViewData["Result"] = service.RemoveModule(id);
            return Index();
        }

        [HttpPost]
        public async Task<IActionResult> UploadModuleAsync(List<IFormFile> files)
        {
            // NB. Never trust the FileName property - it's useless.
            ActionResponseMessage result = null;
            long size = files.Sum(f => f.Length);
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        var zipBytes = stream.ToArray();
                        result = service.AddModule(zipBytes);
                    }
                    if (result.State == ActionResponseState.Failure)
                    {
                        logger.LogWarning("Service unable to parse file: " + result.Message, result.Exception);
                        break;
                    }
                }
                else
                {
                    logger.LogWarning("FormFile of length 0 encountered.");
                }
            }
            ViewData["Result"] = result ?? ActionResponseMessage.NOP;
            return Index();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
