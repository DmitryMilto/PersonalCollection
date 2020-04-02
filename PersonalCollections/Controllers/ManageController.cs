using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PersonalCollections.Models;
using Microsoft.AspNetCore.Authorization;
using PersonalCollections.Models.ViewModel;
using System.Data.Entity;

namespace PersonalCollections.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class ManageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private PersonalCollectionContext db;
        public ManageController(UserManager<User> userManager, PersonalCollectionContext context)
        {
            _userManager = userManager;
            db = context;
        }
        public async Task<IActionResult> Index(string id = null)
        {
            User users;
            if (id != null)
                users = await _userManager.FindByIdAsync(id);
            else
                users = await _userManager.FindByNameAsync(User.Identity.Name);
            return View(users);
        }
        public async Task<IActionResult> EditUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new EditUserViewModel 
            { 
                Id = user.Id, 
                LastName = user.LastName,
                FirstName = user.FirstName,
                City = user.City,
                DateBirth = user.DateBirth,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.LastName = model.LastName;
                    user.FirstName = model.FirstName;
                    user.City = model.City;
                    user.DateBirth = model.DateBirth;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", new { id = model.Id });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }
    }
}