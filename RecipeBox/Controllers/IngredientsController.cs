using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecipeBox.Controllers
{
    public class IngredientsController : Controller
    {
        private readonly RecipeBoxContext _db;

        public IngredientsController(RecipeBoxContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            return View(_db.Ingredients.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Ingredient ingredient)
        {
            _db.Ingredients.Add(ingredient);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int id)
        {
            Ingredient thisIngredient = _db.Ingredients
                .Include(ingredient => ingredient.JoinEntitiesIngredients)
                .ThenInclude(join => join.Recipe)
                .FirstOrDefault(ingredient => ingredient.IngredientId == id);
            return View(thisIngredient);
        }
        public ActionResult AddRecipe(int id)
        {
            Ingredient thisIngredient = _db.Ingredients.FirstOrDefault(ingredients => ingredients.IngredientId == id);
            ViewBag.RecipeId = new SelectList(_db.Recipes, "RecipeId", "Description");
            return View(thisIngredient);
        }

        [HttpPost]
        public ActionResult AddRecipe(Ingredient ingredient, int recipeId)
        {
#nullable enable
            RecipeIngredient? joinEntity = _db.RecipeIngredients.FirstOrDefault(join => (join.RecipeId == recipeId && join.IngredientId == ingredient.IngredientId));
#nullable disable
            if (joinEntity == null && recipeId != 0)
            {
                _db.RecipeIngredients.Add(new RecipeIngredient() { RecipeId = recipeId, IngredientId = ingredient.IngredientId });
                _db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = ingredient.IngredientId });
        }





    }
}