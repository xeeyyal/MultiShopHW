using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.ViewModels;

namespace MultiShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.OrderBy(s => s.Order).ToListAsync();

            List<Product> products = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();

            List<Category> categories = await _context.Categories.ToListAsync();

            HomeVM vM = new HomeVM
            {
                Slides = slides,
                Products = products,
                Categories = categories,
            };
            return View(vM);
        }
    }
}
