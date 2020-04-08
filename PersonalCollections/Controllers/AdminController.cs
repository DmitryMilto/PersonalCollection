using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalCollections.Models;
using PersonalCollections.Models.Admin;
using PersonalCollections.Models.Context;

namespace PersonalCollections.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PersonalCollectionContext db;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, PersonalCollectionContext context)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
            db = context;
        }
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        //[HttpPost]
        public async Task<IActionResult> BlockUser(string id)
        {
            IdentityResult result = null;
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                result = await _userManager.RemoveFromRoleAsync(user, "User");
                if (result.Succeeded)
                {
                    user.Status = false;
                    result = await _userManager.UpdateAsync(user);
                    result = await _userManager.AddToRoleAsync(user, "Blocked");
                    if (result.Succeeded)
                        return RedirectToAction("ListUsers");
                }
            }
            return RedirectToAction("ListUsers");
        }
        public async Task<IActionResult> UnBlockUser(string id)
        {
            IdentityResult result = null;
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                result = await _userManager.RemoveFromRoleAsync(user, "Blocked");
                if (result.Succeeded)
                {
                    user.Status = true;
                    result = await _userManager.UpdateAsync(user);
                    result = await _userManager.AddToRoleAsync(user, "User");
                    if (result.Succeeded)
                        return RedirectToAction("ListUsers");
                }
            }
            return RedirectToAction("ListUsers");
        }
        public async Task<IActionResult> AppointAdmin(string id)
        {
            IdentityResult result = null;
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                result = await _userManager.RemoveFromRoleAsync(user, "User");
                if (result.Succeeded)
                {
                    result = await _userManager.UpdateAsync(user);
                    result = await _userManager.AddToRoleAsync(user, "Admin");
                    if (result.Succeeded)
                        return RedirectToAction("ListUsers");
                }
            }
            return RedirectToAction("ListUsers");
        }
        public async Task<IActionResult> TakeAdmin(string id)
        {
            IdentityResult result = null;
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                result = await _userManager.RemoveFromRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    result = await _userManager.UpdateAsync(user);
                    result = await _userManager.AddToRoleAsync(user, "User");
                    if (result.Succeeded)
                        return RedirectToAction("ListUsers");
                }
            }
            return RedirectToAction("ListUsers");
        }
        public async Task<IActionResult> DeleteUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("ListUsers");
        }
        [HttpGet]
        public async Task<IActionResult> ListThemas()
        {
            return View(await db.Themas.ToListAsync());
        }
        [HttpGet]
        public IActionResult CreateThema()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateThema(Thema thema)
        {
            db.Themas.Add(thema);
            await db.SaveChangesAsync();
            return RedirectToAction("ListThemas");
        }
        public async Task<IActionResult> ListCollection()
        {
            IQueryable<CollectionItem> users = db.CollectionItems.Include(x => x.Themas).Include(x => x.Items);
            return View(await users.ToListAsync());
        }
        public async Task<IActionResult> Delete(int id)
        {
            Thema thema = await db.Themas.Include(x => x.CollectionItems).FirstOrDefaultAsync(x => x.IdThema == id);
            foreach (CollectionItem collection in thema.CollectionItems)
            {
                CollectionItem collectionItem = await db.CollectionItems.Include(x => x.Items).FirstOrDefaultAsync(x => x.IdCollection == collection.IdCollection);
                if (collectionItem != null)
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
            }
            db.Themas.Remove(thema);
            await db.SaveChangesAsync();
            return RedirectToAction("ListThemas");
        }
    }
}