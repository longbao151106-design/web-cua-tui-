using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using TraSuaMoc.Models;
using TraSuaMoc.Services;

namespace TraSuaMoc.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet] public ActionResult Login(string returnUrl) { if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Products"); return View(new LoginViewModel { ReturnUrl = returnUrl }); }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var blockedUntil = Session["LoginBlockedUntil"] as DateTime?;
            if (blockedUntil.HasValue && blockedUntil.Value > DateTime.UtcNow) { ModelState.AddModelError("", "Bạn thử đăng nhập nhiều lần. Vui lòng đợi 1 phút."); return View(model); }
            var email = model.Email.Trim().ToLowerInvariant();
            var customer = Db.Customers.FirstOrDefault(x => x.Email == email);
            if (customer == null || !PasswordService.Verify(model.Password, customer.PasswordHash))
            {
                var failures = (int)(Session["LoginFailures"] ?? 0) + 1;
                Session["LoginFailures"] = failures;
                if (failures >= 5) { Session["LoginBlockedUntil"] = DateTime.UtcNow.AddMinutes(1); Session["LoginFailures"] = 0; }
                ModelState.AddModelError("", "Email hoặc mật khẩu chưa đúng.");
                return View(model);
            }
            Session.Remove("LoginFailures"); Session.Remove("LoginBlockedUntil");
            FormsAuthentication.SetAuthCookie(customer.Id.ToString(), false);
            if (Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl);
            return RedirectToAction("Index", "Products");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Logout() { FormsAuthentication.SignOut(); Session.Clear(); return RedirectToAction("Index", "Home"); }
    }
}
