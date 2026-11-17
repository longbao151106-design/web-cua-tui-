using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TraSuaMoc.Data;
using TraSuaMoc.Models;

namespace TraSuaMoc.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly TeaContext Db = new TeaContext();
        protected List<CartEntry> Cart
        {
            get
            {
                var cart = Session["Cart"] as List<CartEntry>;
                if (cart == null) { cart = new List<CartEntry>(); Session["Cart"] = cart; }
                return cart;
            }
        }
        protected int CustomerId { get { return int.Parse(User.Identity.Name); } }
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.CartCount = Cart.Sum(x => x.Quantity);
            ViewBag.CustomerName = User.Identity.IsAuthenticated ? Db.Customers.Where(x => x.Id == CustomerId).Select(x => x.Name).FirstOrDefault() : null;
            base.OnActionExecuting(context);
        }
        protected override void Dispose(bool disposing) { if (disposing) Db.Dispose(); base.Dispose(disposing); }
    }
}
