using System.Linq;
using System.Web.Mvc;

namespace TraSuaMoc.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        public ActionResult Invoice(int id)
        {
            var order = Db.Orders.Include("Details").FirstOrDefault(x => x.Id == id && x.CustomerId == CustomerId);
            if (order == null) return HttpNotFound();
            return View(order);
        }
        public ActionResult Index()
        {
            var orders = Db.Orders.Where(x => x.CustomerId == CustomerId).OrderByDescending(x => x.CreatedAt).ToList();
            ViewBag.TotalSpent = Services.OrderStatisticsService.TotalSpent(System.Configuration.ConfigurationManager.ConnectionStrings["TeaContext"].ConnectionString, CustomerId);
            return View(orders);
        }
    }
}
