using System.Linq;
using System.Web.Mvc;

namespace TraSuaMoc.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index() { return View(Db.Products.Include("Category").Where(x => x.IsActive).OrderBy(x => x.Id).Take(3).ToList()); }
        public ActionResult Error() { Response.StatusCode = 500; return View(); }
    }
}
