using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace net_il_mio_fotoalbum.Controllers
{
    public class PhotosController : Controller
    {

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult All()
        {

            Context db = new Context();
            List<Photo> photos = db.Photos.Include(c => c.Category).ToList();
            return View(photos);
        }
        public IActionResult Index(string? search, bool? isSuccess)
        {


            Context db = new Context();
            if (string.IsNullOrEmpty(search))
            {
                List<Photo> photos = db.Photos.Include(c => c.Category).Where(p => p.Visibility == true).ToList();
                ViewBag.isSuccess = isSuccess;
                return View(photos);
            }
            else
            {
                List<Photo> photos = db.Photos.Include(c => c.Category).Where(p => p.Visibility == true && p.Title.Contains(search)).ToList();
                ViewBag.isSuccess = isSuccess;
                return View(photos);

            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Context db = new Context();
            PhotoData data = new PhotoData();
            List<Category> categories = db.Categories.ToList();
            data.Categories = categories;



            return View(data);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhotoData data)
        {
            Context db = new();


            if (!ModelState.IsValid)
            {
                List<Category> categories = db.Categories.ToList();
                data.Categories = categories;
                return View(data);
            }
            else
            {
                Photo photoToCreate = new()
                {
                    Title = data.Photo.Title,
                    Description = data.Photo.Description,
                    Visibility = data.Photo.Visibility,
                    UserEmail = data.Photo.UserEmail

                };

                refreshCategoriesSelected(data.SelectedCategory, photoToCreate, db);


                using (var ms = new MemoryStream())
                {
                    data.Photo.Upload.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    photoToCreate.Content = fileBytes;
                }

                db.Add(photoToCreate);
                db.SaveChanges();


                return RedirectToAction("Index", new { isSuccess = ViewBag.isSuccess = true });

            }
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Context db = new();
            Photo photoDetail = db.Photos.Include(p => p.Category).Where(p => p.Id == id).FirstOrDefault();
            if (photoDetail != null)
            {
                return View("Show", photoDetail);

            }
            else
            {
                return View("NotFound");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {

            Context db = new();
            PhotoData data = new();
            List<Category> allCat = db.Categories.ToList();
            data.Categories = allCat;


            Photo photoToEdit = db.Photos.Include(p => p.Category).Where(p => p.Id == id).FirstOrDefault();
            if (User.Identity.Name == photoToEdit.UserEmail || User.IsInRole("SuperAdmin"))
            {
                if (photoToEdit != null)
                {
                    data.Photo = photoToEdit;
                    return View(data);

                }
                else
                {
                    return View(data);
                }

            }
            else
            {
                return View("Unauthorize");
            }


        }



        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(PhotoData data, int id)
        {


            if (data.Photo.Upload == null)
            {
                ModelState.Remove("Photo.Upload");
            }

            Context db = new();
            if (!ModelState.IsValid)
            {
                List<Category> allCat = db.Categories.ToList();
                data.Categories = allCat;


                refreshCategoriesSelected(data.SelectedCategory, data.Photo, db);



                return View(data);

            }

            Photo photoToEdit = db.Photos.Include(p => p.Category).Where(p => p.Id == id).FirstOrDefault();

            if (photoToEdit != null)
            {
                photoToEdit.Title = data.Photo.Title;
                photoToEdit.Description = data.Photo.Description;
                photoToEdit.Visibility = data.Photo.Visibility;

                refreshCategoriesSelected(data.SelectedCategory, photoToEdit, db);

                //if (data.SelectedCategory != null)
                //{
                //    photoToEdit.Category.Clear();
                //    List<Category> newCategories = new();
                //    foreach (int i in data.SelectedCategory)
                //    {
                //        var toAdd = db.Categories.Where(c => c.Id == i).FirstOrDefault();
                //        if (toAdd != null)
                //        {
                //            newCategories.Add(toAdd);
                //        }
                //    }
                //    photoToEdit.Category = newCategories;
                //}


                if (data.Photo.Upload != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        data.Photo.Upload.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        photoToEdit.Content = fileBytes;
                    }
                }
                db.SaveChanges();

            }
            return RedirectToAction("Index", new { isSuccess = ViewBag.isSuccess = true });

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            Context db = new();
            Photo photoToDelete = db.Photos.Where(p => p.Id == id).FirstOrDefault();
            if (photoToDelete != null)
            {
                db.Photos.Remove(photoToDelete);
                db.SaveChanges();
                return RedirectToAction("Index", new { isSuccess = ViewBag.isSuccess = true });
            }
            return RedirectToAction("NotFound", new { isSuccess = ViewBag.isSuccess = false });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeVis(int id, bool visibility)
        {
            Context db = new();
            Photo photo = db.Photos.Where(p => p.Id == id).FirstOrDefault();
            photo.Visibility = visibility;
            db.SaveChanges();
            return RedirectToAction("Index");

        }









        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public void refreshCategoriesSelected(List<int> s, Photo photo, Context db)
        {
            if (s != null)
            {
                photo.Category?.Clear();
                List<Category> newCategories = new();
                foreach (int i in s)
                {
                    var toAdd = db.Categories.Where(c => c.Id == i).FirstOrDefault();
                    if (toAdd != null)
                    {
                        newCategories.Add(toAdd);
                    }
                }
                photo.Category = newCategories;
            }
        }
    }
}
