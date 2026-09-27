using System;
using System.Linq;
using System.Web.Mvc;
using MvcSmartCctv.Models;

namespace MvcSmartCctv.Controllers
{
    public class VehicleController : BaseController
    {
        public ActionResult Index()
        {
            SetShell("vehicle");
            ViewBag.Title = "Vehicle Violation";

            var cameras = DummyData.Cameras.Where(c => c.Category == "vehicle").ToList();
            var violations = DummyData.Violations.Where(v => v.Category == "vehicle").ToList();
            var since24h = DateTime.Now.AddHours(-24);

            var vm = new DetectionViewModel
            {
                Category = "vehicle",
                PageTitle = "Vehicle Violation",
                PageDescription = "Deteksi penumpang di bak kendaraan pada zona kamera terkait.",
                AccentColor = "#B8021F",
                Cameras = cameras,
                Violations = violations,
                StatCams = cameras.Count,
                StatToday = violations.Count(v => v.CreatedAt >= since24h),
                StatOpen = violations.Count(v => v.IsCase && v.Status != "selesai"),
            };
            return View("~/Views/Shared/Detection.cshtml", vm);
        }
    }
}
