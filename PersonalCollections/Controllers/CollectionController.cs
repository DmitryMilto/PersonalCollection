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
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> EditCollection(CollectionItem item)
        {
            Thema thema = await db.Themas.FirstOrDefaultAsync(p => p.IdThema == item.IdThema);
            item.Themas = thema;
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
                    db.Items.Remove(item);
                }
                db.CollectionItems.Remove(collection);
            }
            await db.SaveChangesAsync();
            return RedirectToAction("ListCollection", "Admin");
        }
        public async Task<IActionResult> Item(int id)
        {
            ViewBag.AutorizeUser = await GetAutorizeUser();
            User autorizeUser = await GetAutorizeUser();
            Item item = await db.Items.Include(p => p.CollectionItems).FirstOrDefaultAsync(i => i.IdItem == id);
            return View(item);
        }
        [NonAction]
        public async Task<User> GetAutorizeUser()
        {
            string emailAutorizeUser = User.Identity.Name;
            return await db.Users.FirstOrDefaultAsync(u => u.UserName == emailAutorizeUser);
        }
    }
}