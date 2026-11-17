using System.Linq;
using System.Web.Mvc;
using TraSuaMoc.Models;

namespace TraSuaMoc.Controllers
{
    public class ProductsController : BaseController
    {
        public ActionResult Index(string search, int? categoryId)
        {
            var query = Db.Products.Include("Category").Where(x => x.IsActive);
            search = (search ?? "").Trim();
            if (search.Length > 100) search = search.Substring(0, 100);
            if (search.Length > 0) query = query.Where(x => x.Name.Contains(search));
            if (categoryId.HasValue) query = query.Where(x => x.CategoryId == categoryId.Value);
            return View(new MenuViewModel { Products = query.OrderBy(x => x.Id).ToList(), Categories = Db.Categories.OrderBy(x => x.Id).ToList(), Search = search, CategoryId = categoryId });
        }
        public ActionResult Details(int id)
        {
            var product = Db.Products.Include("Category").FirstOrDefault(x => x.Id == id && x.IsActive);
            if (product == null) return HttpNotFound();
            return View(product);
        }
    }
}
