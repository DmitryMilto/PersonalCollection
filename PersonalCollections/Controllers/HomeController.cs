using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersonalCollections.Models;
using PersonalCollections.Models.ViewModel;

namespace PersonalCollections.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PersonalCollectionContext db;

        public HomeController(ILogger<HomeController> logger, PersonalCollectionContext context)
        {
            _logger = logger;
            db = context;
        }


        public async Task<IActionResult> Index()
        {
            HomeViewModel model = new HomeViewModel();
            List<CollectionItem> collectionItems = new List<CollectionItem>();
            List<Item> Items = new List<Item>();
            List<CollectionItem> collections = await db.CollectionItems.Include(x => x.Themas).Include(x => x.Items).OrderByDescending(x => x.Items.Count).ToListAsync();
            if(collections.Count() > 6)
            {
                for (int i = 0; i < 6; i++)
                    collectionItems.Add(collections[i]);
                model.CollectionItems = collectionItems;
            }
            else
            {
                model.CollectionItems = collections;
            }

            List<Item> items = await db.Items.Include(x => x.CollectionItems).Include(x => x.Likes).OrderByDescending(x => x.Likes.Count()).ToListAsync();
            if (items.Count() > 6)
            {
                for (int i = 0; i < 6; i++)
                    Items.Add(items[i]);
                model.Items = Items;
            }
            else
            {
                model.Items = items;
            }   
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
        public async Task<IActionResult> DetailsCollecton(int id)
        {
            CollectionItem item = await db.CollectionItems.Include(x => x.Themas).Include(x => x.Items).FirstOrDefaultAsync(x => x.IdCollection == id);
            return View(item);
        }
        private int pageSize = 6;
        public async Task<IActionResult> AllCollection(int page = 1)
        {
            IQueryable<CollectionItem> source = db.CollectionItems.Include(x => x.Themas);
            var count = await source.CountAsync();
            var Collections = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            Page pageViewModel = new Page(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            {
                Page = pageViewModel,
                CollectionItems = Collections
            };
            return View(viewModel);
        }
        public async Task<IActionResult> AllItem(int page = 1)
        {
            IQueryable<Item> source = db.Items.Include(x => x.CollectionItems);
            var count = await source.CountAsync();
            var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            Page pageViewModel = new Page(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            {
                Page = pageViewModel,
                Items = items
            };
            return View(viewModel);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
