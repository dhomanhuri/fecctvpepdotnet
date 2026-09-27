using System;
using System.Linq;
using System.Web.Mvc;
using MvcSmartCctv.Models;

namespace MvcSmartCctv.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            SetShell("dashboard");
            ViewBag.Title = "Dashboard";

            var cameras = DummyData.Cameras;
            var violations = DummyData.Violations;
            var since24h = DateTime.Now.AddHours(-24);

            var vm = new DashboardViewModel
            {
                TotalCameras = cameras.Count,
                TotalViolations = violations.Count,
                OpenCases = violations.Count(v => v.IsCase && v.Status != "selesai"),
                Last24h = violations.Count(v => v.CreatedAt >= since24h),
                ApdCameraCount = cameras.Count(c => c.Category == "apd"),
                ApdViolationCount = violations.Count(v => v.Category == "apd"),
                VehicleCameraCount = cameras.Count(c => c.Category == "vehicle"),
                VehicleViolationCount = violations.Count(v => v.Category == "vehicle"),
                Daily = DummyData.DailyTrend(7),
                TopLocations = cameras.Select(c => new TopLocationRow
                {
                    CameraName = c.Name,
                    Location = c.Location,
                    Category = c.Category,
                    Count = violations.Count(v => v.CameraId == c.Id)
                }).OrderByDescending(r => r.Count).Take(5).ToList(),
                Feed = violations.Take(8).ToList(),
            };

            return View(vm);
        }
    }
}
