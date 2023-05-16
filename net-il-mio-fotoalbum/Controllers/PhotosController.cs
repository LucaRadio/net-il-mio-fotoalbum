using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Models;
using System.Diagnostics;

namespace net_il_mio_fotoalbum.Controllers
{
    public class PhotosController : Controller
    {

        public IActionResult Index(string? search)
        {
            Context db = new Context();
            if (string.IsNullOrEmpty(search))
            {
                List<Photo> photos = db.Photos.Include(c => c.Category).Where(p => p.Visibility == true).ToList();
                return View(photos);
            }
            else
            {
                List<Photo> photos = db.Photos.Include(c => c.Category).Where(p => p.Visibility == true && p.Title.Contains(search)).ToList();
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
                    Visibility = data.Photo.Visibility

                };
                if (data.SelectedCategory != null)
                {
                    List<Category> categoriesToInsert = new();
                    foreach (int i in data.SelectedCategory)
                    {
                        Category toAdd = db.Categories.Where(item => item.Id == i).FirstOrDefault();
                        categoriesToInsert.Add(toAdd);
                    }
                    photoToCreate.Category = categoriesToInsert;
                }

                using (var ms = new MemoryStream())
                {
                    data.Photo.Upload.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    photoToCreate.Content = fileBytes;
                }

                db.Add(photoToCreate);
                db.SaveChanges();


                return RedirectToAction("Index");

            }
        }

        [HttpGet]
        public IActionResult Show(int id)
        {
            Context db = new();
            Photo photoDetail = db.Photos.Include(p => p.Category).Where(p => p.Id == id).FirstOrDefault();
            if (photoDetail != null)
            {
                return View(photoDetail);

            }
            else
            {
                return View("NotFound");
            }
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {

            Context db = new();
            PhotoData data = new();
            List<Category> allCat = db.Categories.ToList();
            data.Categories = allCat;


            Photo photoToEdit = db.Photos.Include(p => p.Category).Where(p => p.Id == id).FirstOrDefault();
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
                return View(allCat);

            }

            Photo photoToEdit = db.Photos.Include(p => p.Category).Where(p => p.Id == id).FirstOrDefault();

            if (photoToEdit != null)
            {
                photoToEdit.Title = data.Photo.Title;
                photoToEdit.Description = data.Photo.Description;
                photoToEdit.Visibility = data.Photo.Visibility;

                if (data.SelectedCategory != null)
                {
                    photoToEdit.Category.Clear();
                    List<Category> newCategories = new();
                    foreach (int i in data.SelectedCategory)
                    {
                        var toAdd = db.Categories.Where(c => c.Id == i).FirstOrDefault();
                        if (toAdd != null)
                        {
                            newCategories.Add(toAdd);
                        }
                    }
                    photoToEdit.Category = newCategories;
                }


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
            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Context db = new();
            Photo photoToDelete = db.Photos.Where(p => p.Id == id).FirstOrDefault();
            if (photoToDelete != null)
            {
                db.Photos.Remove(photoToDelete);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFound");
        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
