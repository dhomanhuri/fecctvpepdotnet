using System.Linq;
using System.Web.Mvc;
using MvcSmartCctv.Models;

namespace MvcSmartCctv.Controllers
{
    // Standalone portal-style page, not using the shared shell/_Layout —
    // mirrors admin.html which has its own header rather than the sidebar
    // nav shell. Still inherits BaseController for the session-login gate
    // (OnActionExecuting) and CurrentUser — Index() builds its own
    // ShellViewModel directly instead of calling SetShell.
    public class AdminController : BaseController
    {
        public ActionResult Index()
        {
            var u = CurrentUser;
            if (!u.IsAdmin) return RedirectToAction("Index", "Home");

            ViewBag.Title = "Admin Dashboard";
            ViewBag.Shell = new ShellViewModel { Active = "admin", UserName = u.DisplayName, RoleLabel = "Admin", IsAdmin = true };
            return View(DummyData.Users);
        }

        [HttpPost]
        public ActionResult AddUser(string username, string firstName, string lastName, string email, string role, string password)
        {
            if (!CurrentUser.IsAdmin) return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(username) || DummyData.UsernameExists(username))
            {
                TempData["AddUserError"] = string.IsNullOrWhiteSpace(username)
                    ? "Username wajib diisi." : "Username sudah digunakan.";
                return RedirectToAction("Index");
            }

            DummyData.Users.Add(new PlatformUser
            {
                Id = DummyData.NextUserId(),
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Email = string.IsNullOrWhiteSpace(email) ? null : email,
                Roles = new System.Collections.Generic.List<string> { string.IsNullOrWhiteSpace(role) ? "operator" : role },
                Enabled = true,
                Password = string.IsNullOrWhiteSpace(password) ? "busDev123!" : password,
            });
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ToggleUser(string id)
        {
            if (!CurrentUser.IsAdmin) return RedirectToAction("Index", "Home");

            // Guard against locking yourself out — you can't disable the
            // account you're currently logged in as (no recovery path
            // exists in this mockup if you did).
            var target = DummyData.Users.FirstOrDefault(x => x.Id == id);
            if (target != null && target.Id != CurrentUser.Id)
            {
                target.Enabled = !target.Enabled;
            }
            return RedirectToAction("Index");
        }
    }
}
