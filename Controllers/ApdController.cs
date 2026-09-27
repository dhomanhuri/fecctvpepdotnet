using System;
using System.Linq;
using System.Web.Mvc;
using MvcSmartCctv.Models;

namespace MvcSmartCctv.Controllers
{
    public class ApdController : BaseController
    {
        public ActionResult Index()
        {
            SetShell("apd");
            ViewBag.Title = "APD Detection";

            var cameras = DummyData.Cameras.Where(c => c.Category == "apd").ToList();
            var violations = DummyData.Violations.Where(v => v.Category == "apd").ToList();
            var since24h = DateTime.Now.AddHours(-24);

            var vm = new DetectionViewModel
            {
                Category = "apd",
                PageTitle = "APD Detection",
                PageDescription = "Deteksi kepatuhan alat pelindung diri (mis. helm) di kamera terkait.",
                AccentColor = "#B57712",
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
