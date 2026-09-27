using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MvcSmartCctv.Models;

namespace MvcSmartCctv.Controllers
{
    public class ReportsController : BaseController
    {
        private const int MaxRangeDays = 31;

        // Date range picker + downloads are handled server-side (?from=&to=),
        // no client-side JS fetch loop — this mirrors reports.html's
        // behavior but the page re-requests instead of calling an API.
        public ActionResult Index(string from, string to, string quick)
        {
            SetShell("reports");
            ViewBag.Title = "Laporan & Analitik";

            DateTime start, end;
            var today = DateTime.Today;

            if (!string.IsNullOrEmpty(quick))
            {
                int days = quick == "30" ? 30 : 7;
                end = today;
                start = today.AddDays(-(days - 1));
            }
            else if (DateTime.TryParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out start)
                     && DateTime.TryParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
            {
                // parsed from query string
            }
            else
            {
                end = today;
                start = today.AddDays(-6);
                quick = "7";
            }

            var vm = new ReportsViewModel { Start = start, End = end, QuickActive = quick };

            if (start > end)
            {
                vm.RangeError = "Tanggal \"Dari\" harus sebelum atau sama dengan \"Sampai\".";
            }
            else
            {
                var spanDays = (end - start).Days + 1;
                if (spanDays > MaxRangeDays)
                {
                    vm.RangeError = string.Format("Rentang maksimal {0} hari (dipilih {1} hari).", MaxRangeDays, spanDays);
                }
            }

            if (vm.RangeError == null)
            {
                var endExclusive = end.AddDays(1);
                var rows = DummyData.Violations.Where(v => v.CreatedAt >= start && v.CreatedAt < endExclusive).ToList();
                var spanDays = (end - start).Days + 1;

                vm.RangeLabel = start == end
                    ? start.ToString("dd MMM", new CultureInfo("id-ID"))
                    : start.ToString("dd MMM", new CultureInfo("id-ID")) + " – " + end.ToString("dd MMM", new CultureInfo("id-ID"));
                vm.Total = rows.Count;
                vm.AvgPerDay = Math.Round((double)rows.Count / spanDays, 1);

                var caseRows = rows.Where(v => v.IsCase).ToList();
                vm.CaseTotal = caseRows.Count;
                vm.CompletionRate = caseRows.Count > 0
                    ? (double?)caseRows.Count(v => v.Status == "selesai") / caseRows.Count
                    : null;
                // No real response-time tracking in this mockup (would need
                // an "acknowledged_at" timestamp we don't persist) — shown
                // as a fixed illustrative placeholder instead of "-" so the
                // KPI tile isn't perpetually empty.
                vm.AvgResponseMinutes = caseRows.Count > 0 ? (double?)37 : null;

                vm.ApdTotal = rows.Count(v => v.Category == "apd");
                vm.VehicleTotal = rows.Count(v => v.Category == "vehicle");
                vm.ApdPct = vm.Total > 0 ? (int)Math.Round(100.0 * vm.ApdTotal / vm.Total) : 0;
                vm.VehiclePct = vm.Total > 0 ? 100 - vm.ApdPct : 0;

                vm.Daily = DummyData.DailyTrend(start, end);

                var maxCount = 1;
                vm.ByCamera = DummyData.Cameras.Select(c => new CameraReportRow
                {
                    CameraName = c.Name,
                    Location = c.Location,
                    Category = c.Category,
                    Count = rows.Count(v => v.CameraId == c.Id),
                }).OrderByDescending(r => r.Count).ToList();
                if (vm.ByCamera.Any()) maxCount = Math.Max(1, vm.ByCamera.Max(r => r.Count));
                ViewBag.MaxCameraCount = maxCount;
            }

            return View(vm);
        }
    }
}
