using System.Web.Mvc;
using MvcSmartCctv.Models;

namespace MvcSmartCctv.Controllers
{
    public class SettingsController : BaseController
    {
        public ActionResult Index()
        {
            SetShell("settings");
            ViewBag.Title = "Pengaturan";

            // Admin-gated in the original via a client-side JWT role check;
            // here ShellViewModel.IsAdmin (derived from the logged-in
            // SessionUser, see BaseController.SetShell) plays that role —
            // a non-admin logged-in user really does see "Akses Terbatas".
            var shell = ViewBag.Shell as ShellViewModel;
            ViewBag.IsAdmin = shell != null && shell.IsAdmin;

            return View(DummyData.Cameras);
        }
    }
}
