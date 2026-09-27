using System.Linq;
using System.Web.Mvc;
using MvcSmartCctv.Models;

namespace MvcSmartCctv.Controllers
{
    // Dummy session login — validates against the in-memory DummyData.Users
    // roster (no real Keycloak/backend, see ADR-style comments in DummyData.cs).
    // Deliberately plain Controller, not BaseController: this is the one
    // page that has to render without a session already existing.
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (Session[SessionKeys.User] != null) return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            var user = DummyData.FindUser(username, password);
            if (user == null)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.Username = username;
                ViewBag.Error = "Username atau password salah.";
                return View();
            }

            Session[SessionKeys.User] = new SessionUser
            {
                Id = user.Id,
                Username = user.Username,
                DisplayName = (user.FirstName + " " + user.LastName).Trim(),
                Roles = user.Roles,
                IsAdmin = user.Roles.Contains("admin"),
            };

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}
