using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalCollections.Models;
using PersonalCollections.Models.ViewModel;

namespace PersonalCollections.Controllers
{
    public class CollectionController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly PersonalCollectionContext db;
        private readonly IWebHostEnvironment _appEnvironment;
        public CollectionController(UserManager<User> userManager, PersonalCollectionContext context, IWebHostEnvironment appEnvironment)
        {
            _userManager = userManager;
            db = context;
            _appEnvironment = appEnvironment;
        }
        public IActionResult CreateCollection()
        {
            SelectList themas = new SelectList(db.Themas, "IdThema", "Name");
            ViewBag.Thema = themas;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCollection(CollectionItem collectionItem, IFormFile uploadedFile)
        {
            Thema thema = await db.Themas.FirstOrDefaultAsync(p => p.IdThema == collectionItem.IdThema);
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            collectionItem.Themas = thema;
            collectionItem.IdUser = user.Id;

            if (uploadedFile != null)
            {
                // путь к папке Files
                string path = "/Files/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                collectionItem.Image = path;
            }

            db.CollectionItems.Add(collectionItem);
            await db.SaveChangesAsync();
            return RedirectToAction("EditCollection", new { id = collectionItem.IdCollection});
        }
        public IActionResult CreateItem(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public async Task<IActionResult> EditItem(int id)
        {
            Item item = await db.Items.Include(x => x.CollectionItems).FirstOrDefaultAsync(x => x.IdItem == id);
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> CreateItem(Item item, int IdCollectionItem)
        {
            CollectionItem collection = await db.CollectionItems.FirstOrDefaultAsync(p => p.IdCollection == IdCollectionItem);
            item.IdCollectionItem = IdCollectionItem;
            item.CollectionItems = collection;
            db.Items.Add(item);
            await db.SaveChangesAsync();
            return RedirectToAction("EditCollection", new { id = IdCollectionItem});
        }
        [HttpPost]
        public async Task<IActionResult> EditItem(Item item)
        {
            CollectionItem collection = await db.CollectionItems.FirstOrDefaultAsync(p => p.IdCollection == item.IdCollectionItem);
            item.CollectionItems = collection;
            db.Items.Update(item);
            await db.SaveChangesAsync();
            return RedirectToAction("EditCollection", new { id = item.IdCollectionItem });
        }
        public async Task<IActionResult> DetailsItem(int id)
        {
            Item item = await db.Items.Include(p => p.CollectionItems).FirstOrDefaultAsync(i => i.IdItem == id);
            return View(item);
        }
        [HttpGet]
        public async Task<IActionResult> EditCollection(int id)
        {
            SelectList themas = new SelectList(db.Themas, "IdThema", "Name");
            ViewBag.Thema = themas;
            CollectionItem item = await db.CollectionItems.Include(x => x.Themas).Include(x => x.Items).FirstOrDefaultAsync(x => x.IdCollection == id);
            ViewBag.Image = item.Image;
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> EditCollection(CollectionItem item, IFormFile uploadedFile, string Image)
        {
            Thema thema = await db.Themas.FirstOrDefaultAsync(p => p.IdThema == item.IdThema);
            item.Themas = thema;
            string image = Image;
            if (uploadedFile != null)
            {
                // путь к папке Files
                string path = "/Files/" + uploadedFile.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                if (image != path)
                {
                    item.Image = path;
                    if (image != null)
                    {
                        string paths = _appEnvironment.WebRootPath + Image;
                        FileInfo fileInf = new FileInfo(paths);
                        if (fileInf.Exists)
                        {
                            fileInf.Delete();
                        }
                    }
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    item.Image = ViewBag.Image;
                }  
            }
            db.CollectionItems.Update(item);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Manage");
        }

        public async Task<IActionResult> DeleteCollection(int id)
        {
            CollectionItem collection = await db.CollectionItems.Include(x => x.Items).FirstOrDefaultAsync(x => x.IdCollection == id);
            if (collection != null)
            {
                foreach (Item item in collection.Items)
                {
                    Item item1 = await db.Items.Include(x => x.Likes).FirstOrDefaultAsync(x => x.IdItem == item.IdItem);
                    if (item1 != null)
                    {
                        foreach (Like like in item1.Likes)
                        {
                            db.Likes.Remove(like);
                        }
                        db.Items.Remove(item1);
                    }
                }
                db.CollectionItems.Remove(collection);
            }
            await db.SaveChangesAsync();
            return RedirectToAction("ListCollection", "Admin");
        }
        public async Task<IActionResult> Item(int id)
        {
            Item item = await db.Items.Include(p => p.CollectionItems).Include(x => x.Likes).FirstOrDefaultAsync(i => i.IdItem == id);
            LikeViewModel model = new LikeViewModel();
            if (item != null)
            {
                model.like = false;
                model.NameItem = item.NameItem;
                model.Description = item.Description;
                model.IdCollectionItem = item.IdCollectionItem;
                model.NameCollection = item.CollectionItems.NameCollection;
                model.IdItem = item.IdItem;
                model.Image = item.CollectionItems.Image;
                for (int i = 0; i < item.Likes.Count; i++)
                    if (item.Likes[i].UserName == User.Identity.Name)
                        model.like = true;
            }
            return View(model);
        }
        public async Task<IActionResult> LikeUp(int id)
        {
            Like like = new Like
            {
                IdItem = id,
                Item = await db.Items.FirstOrDefaultAsync(x => x.IdItem == id),
                UserName = User.Identity.Name
            };
            db.Likes.Add(like);
            await db.SaveChangesAsync();
            return RedirectToAction("Item", new { id = like.IdItem});
        }
        public async Task<IActionResult> LikeDelete(int id)
        {
            Like like = await db.Likes.FirstOrDefaultAsync(x => x.IdItem == id && x.UserName == User.Identity.Name);
            if(like != null)
            {
                db.Likes.Remove(like);
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Item", new { id = like.IdItem });
        }
        public async Task<IActionResult> UserCollection(string id)
        {
            ViewBag.Id = id;
            var collection = db.CollectionItems.Where(x => x.IdUser == id).Include(x => x.Themas).Include(x => x.Items);
            return View(collection);
        }
        public async Task<IActionResult> DeleteItem(int id)
        {
            
            Item item = await db.Items.Include(x => x.Likes).FirstOrDefaultAsync(x => x.IdItem == id);
            int idCollection = item.IdCollectionItem;
            if (item != null)
            {
                if (item.Likes != null)
                {
                    foreach (Like like in item.Likes)
                    {
                        db.Likes.Remove(like);
                    }
                }
                TempData["messageDelete"] = String.Format("Item {0} delete", item.NameItem);
                db.Items.Remove(item);
                await db.SaveChangesAsync();
            }
            
            return RedirectToAction("EditCollection", new { id = idCollection });
        }
    }
}