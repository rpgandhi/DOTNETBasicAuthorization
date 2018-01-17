using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BasicAuthentication.Models;
using BasicAuthentication.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BasicAuthentication.Controllers
{
    public class RoleController : Controller
    {
		private readonly ApplicationDbContext _db;
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

		public RoleController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext db)
		{
			_db = db;
            _userManager = userManager;
		}

        public IActionResult Index()
        {
            var roles = _db.Roles.ToList();
            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateRoleViewModel model)
        {
            try
            {
                _db.Roles.Add(new IdentityRole() {
                    Name = model.RoleName
                });
                _db.SaveChanges();
                ViewBag.ResultMessage = "Role created!";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(string id)
        {
            var thisRole = _db.Roles.FirstOrDefault(role => role.Id == id);
            _db.Roles.Remove(thisRole);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ManageUserRoles()
        {
            List<SelectListItem> roleList = _db.Roles.OrderBy(r => r.Name)
                .ToList()
                .Select(r => new SelectListItem {
                    Value = r.Name.ToString(),
                    Text = r.Name
                }).ToList();
            ViewBag.Roles = roleList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RoleAddToUser(string UserName, string RoleName)
        {
			ApplicationUser user = _db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
			IdentityRole role = _db.Roles.Where(u => u.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            IdentityResult result = await _userManager.AddToRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                ViewBag.ResultMessage = "Role created successfully!";
            }
            else
            {
				ViewBag.ResultMessage = "Role creation failed!";
			}

            return RedirectToAction("ManageUserRoles");
        }
    }
}
