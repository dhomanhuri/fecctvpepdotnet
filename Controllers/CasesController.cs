using System.Linq;
using System.Web.Mvc;
using MvcSmartCctv.Models;

namespace MvcSmartCctv.Controllers
{
    public class CasesController : BaseController
    {
        // Tab filter is done server-side via query string (?filter=apd) —
        // no client-side JS fetch loop exists in this mockup, so each tab
        // link is a plain <a> that re-requests the page.
        public ActionResult Index(string filter)
        {
            SetShell("cases");
            ViewBag.Title = "Case Violation";

            if (string.IsNullOrEmpty(filter)) filter = "all";

            // Only promoted cases show here (ADR-0012) — every raw AI
            // detection is an alert first; APD/Vehicle Detection's
            // "Riwayat Pelanggaran" is where the full unfiltered log lives.
            var rows = DummyData.Violations.Where(v => v.IsCase);
            if (filter == "apd" || filter == "vehicle") rows = rows.Where(v => v.Category == filter);
            if (filter == "open") rows = rows.Where(v => v.Status != "selesai");

            var vm = new CasesViewModel
            {
                ActiveFilter = filter,
                Rows = rows.ToList(),
            };
            return View(vm);
        }
    }
}
