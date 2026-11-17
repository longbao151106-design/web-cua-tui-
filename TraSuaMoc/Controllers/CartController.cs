using System;
using System.Linq;
using System.Web.Mvc;
using TraSuaMoc.Models;

namespace TraSuaMoc.Controllers
{
    public class CartController : BaseController
    {
        private CartViewModel ReadCart()
        {
            var ids = Cart.Select(x => x.ProductId).ToList();
            var products = Db.Products.Include("Category").Where(x => ids.Contains(x.Id) && x.IsActive).ToList();
            Cart.RemoveAll(x => !products.Any(p => p.Id == x.ProductId));
            return new CartViewModel { Lines = Cart.Select(x => new CartLine { Product = products.First(p => p.Id == x.ProductId), Quantity = x.Quantity }).ToList(), CheckoutToken = Session["CheckoutToken"] as string };
        }
        public ActionResult Index()
        {
            if (Session["CheckoutToken"] == null) Session["CheckoutToken"] = Guid.NewGuid().ToString("N");
            var model = ReadCart(); ViewBag.CartCount = Cart.Sum(x => x.Quantity); return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(int productId, int quantity = 1)
        {
            if (quantity < 1 || quantity > 20) return new HttpStatusCodeResult(400);
            if (!Db.Products.Any(x => x.Id == productId && x.IsActive)) return HttpNotFound();
            var entry = Cart.FirstOrDefault(x => x.ProductId == productId);
            if (entry == null) Cart.Add(new CartEntry { ProductId = productId, Quantity = quantity });
            else entry.Quantity = Math.Min(20, entry.Quantity + quantity);
            TempData["Success"] = "Đã thêm món vào giỏ. Mộc chờ bạn chọn thêm nhé!";
            return RedirectToAction("Index");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Update(int productId, int quantity)
        {
            if (quantity < 1 || quantity > 20) { TempData["Error"] = "Số lượng mỗi món từ 1 đến 20 ly."; return RedirectToAction("Index"); }
            var entry = Cart.FirstOrDefault(x => x.ProductId == productId);
            if (entry != null) entry.Quantity = quantity;
            return RedirectToAction("Index");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Remove(int productId) { Cart.RemoveAll(x => x.ProductId == productId); return RedirectToAction("Index"); }
        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Checkout(string checkoutToken)
        {
            var prior = Db.Orders.FirstOrDefault(x => x.CheckoutToken == checkoutToken && x.CustomerId == CustomerId);
            if (prior != null) return RedirectToAction("Invoice", "Orders", new { id = prior.Id });
            if (string.IsNullOrEmpty(checkoutToken) || checkoutToken != Session["CheckoutToken"] as string) return new HttpStatusCodeResult(400, "Invalid checkout token");
            var cart = ReadCart();
            if (cart.Lines.Count == 0) { TempData["Error"] = "Giỏ hàng đang trống. Hãy chọn món trước khi thanh toán."; return RedirectToAction("Index"); }
            var customer = Db.Customers.Find(CustomerId);
            if (customer == null) { FormsAuthenticationSignOut(); return RedirectToAction("Login", "Account"); }
            using (var transaction = Db.Database.BeginTransaction())
            {
                var order = new Order { CustomerId = customer.Id, CustomerName = customer.Name, CreatedAt = DateTime.UtcNow, PaymentStatus = "Đã thanh toán (mô phỏng)", Total = cart.Total, CheckoutToken = checkoutToken };
                foreach (var line in cart.Lines) order.Details.Add(new OrderDetail { ProductId = line.Product.Id, ProductName = line.Product.Name, UnitPrice = line.Product.Price, Quantity = line.Quantity });
                Db.Orders.Add(order); Db.SaveChanges(); transaction.Commit();
                Cart.Clear(); Session.Remove("CheckoutToken");
                return RedirectToAction("Invoice", "Orders", new { id = order.Id });
            }
        }
        private void FormsAuthenticationSignOut() { System.Web.Security.FormsAuthentication.SignOut(); }
    }
}
