using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; //  will allow us to actually authorize users.
using Microsoft.AspNetCore.Identity; //  enables controller to interact with users from the DB
using System.Threading.Tasks; //  so we can call async methods
using System.Security.Claims; // important for using what is called Claim Based Authorization

namespace ToDoList.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ToDoListContext _db;
        private readonly UserManager<ApplicationUser> _userManager; // authentication

        public CategoriesController(UserManager<ApplicationUser> userManager, ToDoListContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<ActionResult> Index()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.FindByIdAsync(userId);
            var userCategories = _db.Categories.Where(entry => entry.User.Id == currentUser.Id);
            return View(userCategories);
        }

        // public ActionResult Index()
        // {
        //     List<Category> model = _db.Categories.ToList();
        //     return View(model);
        // }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Category category, int ItemId)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.FindByIdAsync(userId);
            category.User = currentUser;
            _db.Categories.Add(category);
            if (ItemId != 0)
            {
                _db.CategoryItem.Add(new CategoryItem() { ItemId = ItemId, CategoryId = category.CategoryId });
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // [HttpPost]
        // public ActionResult Create(Category category)
        // {
        //     _db.Categories.Add(category);
        //     _db.SaveChanges();
        //     return RedirectToAction("Index");
        // }


        public ActionResult Details(int id)
        {
            var thisCategory = _db.Categories
                .Include(category => category.Items)
                .ThenInclude(join => join.Item)
                .FirstOrDefault(category => category.CategoryId == id);
            return View(thisCategory);
        }


        public ActionResult Edit(int id)
        {
            var thisCategory = _db.Categories.FirstOrDefault(category => category.CategoryId == id);
            return View(thisCategory);
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            _db.Entry(category).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var thisCategory = _db.Categories.FirstOrDefault(category => category.CategoryId == id);
            return View(thisCategory);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var thisCategory = _db.Categories.FirstOrDefault(category => category.CategoryId == id);
            _db.Categories.Remove(thisCategory);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}