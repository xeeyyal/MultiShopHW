using Microsoft.AspNetCore.Mvc;
using MultiShop.DAL;

namespace MultiShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Detail()
        {
            return View();
        }
    }
}
