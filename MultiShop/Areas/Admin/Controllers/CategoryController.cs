﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Admin.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;

namespace MultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.Include(c => c.Products).ToListAsync();

            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid) return View();

            bool result = _context.Categories.Any(c => c.Name.ToLower().Trim() == categoryVM.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Bu Kateqoriya artiq movcuddur.");
                return View();
            }

            Category category = new Category
            {
                Name = categoryVM.Name
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null) return NotFound();

            UpdateCategoryVM categoryVM = new UpdateCategoryVM 
            { 
                Name = category.Name 
            };

            return View(categoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid) return View();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            bool result = _context.Categories.Any(c => c.Name == categoryVM.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda category artiq movcuddur");
                return View();
            }
            existed.Name = categoryVM.Name;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            _context.Categories.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            Category category = await _context.Categories
            .Include(c => c.Products).ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(s => s.Id == id);
            if (category == null) return NotFound();

            return View(category);
        }
    }
}
