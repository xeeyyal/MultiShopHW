using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Admin.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Extensions;

namespace MultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SlideController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();

            return View(slides);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!slideVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "File tipi uygun deyil");
                return View();
            }
            if (!slideVM.Photo.ValidateSize(2 * 1024))
            {
                ModelState.AddModelError("Photo", "Sheklin hecmi 2 mb-den boyuk olmamalidir");
                return View();
            }

            string fileName = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img");

            Slide slide = new Slide
            {
                ImageUrl = fileName,
                Title = slideVM.Title,
                Description = slideVM.Description,
                Order = slideVM.Order
            };

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (existed is null) return NotFound();

            UpdateSlideVM slideVM = new UpdateSlideVM
            {
                Image = existed.ImageUrl,
                Title = existed.Title,
                Description = existed.Description,
                Order = existed.Order
            };

            return View(slideVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVM slideVM)
        {

            if (!ModelState.IsValid)
            {
                return View(slideVM);
            }
            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (existed is null) return NotFound();


            if (slideVM.Photo is not null)
            {
                if (!slideVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("Photo", "File tipi uygun deyil");
                    return View(slideVM);
                }
                if (!slideVM.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "Sheklin hecmi 2 mb-den boyuk olmamalidir");
                    return View(slideVM);
                }

                string newImage = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img");
                existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
                existed.ImageUrl = newImage;
            }

            existed.Title = slideVM.Title;
            existed.Description = slideVM.Description;
            existed.Order = slideVM.Order;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (slide is null) return NotFound();

            slide.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "slider");

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide == null) return NotFound();

            return View(slide);
        }
    }
}
