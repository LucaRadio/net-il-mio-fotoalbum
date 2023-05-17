using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    public class CategoriesController : Controller
    {

        public IActionResult Index(bool? isDel, bool? isAdd)
        {
            using (Context db = new())
            {
                List<Category> allCat = db.Categories.ToList();
                ViewBag.isDel = isDel;
                ViewBag.isAdd = isAdd ?? false;
                return View(allCat);
            }

        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category cat)
        {
            if (ModelState.IsValid)
            {
                Context db = new Context();
                db.Categories.Add(cat);
                db.SaveChanges();
                ViewBag.isAdd = true;
                return RedirectToAction("Index", new { isAdd = ViewBag.isAdd });
            }
            else
            {
                return View();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (Context db = new())
            {
                Category toDel = db.Categories.Where(c => c.Id == id).FirstOrDefault();
                if (toDel != null)
                {
                    db.Categories.Remove(toDel);
                    db.SaveChanges();
                    ViewBag.isDel = true;
                    return RedirectToAction("Index", new { isDel = ViewBag.isDel });
                }
                else
                {
                    ViewBag.isDel = false;
                    return RedirectToAction("Index", new { isDel = ViewBag.isDel });
                }

            }

        }

    }
}
