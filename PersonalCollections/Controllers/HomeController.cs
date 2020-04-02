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
            IQueryable<CollectionItem> users = db.CollectionItems.Include(x => x.Themas).Include(x => x.Items);
            return View(await users.ToListAsync());
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
