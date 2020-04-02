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
    }
}