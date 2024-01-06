using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Admin.ViewModels;
using MultiShop.DAL;
using MultiShop.Models;
using MultiShop.Utilities.Extensions;

namespace MultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {

            List<Product> products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                .ToListAsync();

            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            CreateProductVM vm = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync(),
                Colors = await _context.Colors.ToListAsync()
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                return View();
            }
            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bu adli category movcuddur");
                return View();
            }

            foreach (int id in productVM.ColorIds)
            {
                bool colorResult = await _context.Colors.AnyAsync(t => t.Id == id);
                if (!colorResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("ColorIds", "Yanlish id melumati gonderilib");
                    return View(productVM);
                }
            }

            if (!productVM.MainPhoto.ValidateType("image/"))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("MainPhoto", "File tipi uygun deyil");
                return View();
            }

            if (!productVM.MainPhoto.ValidateSize(500))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("MainPhoto", "File olcusu uygun deyil:500Kb");
                return View();
            }

            ProductImage mainImage = new ProductImage
            {
                IsPrimary = true,
                Url = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "img")
            };

            Product product = new Product
            {
                Name = productVM.Name,
                Description = productVM.Description,
                Price = productVM.Price,
                CategoryId = (int)productVM.CategoryId,
                ProductColors = new List<ProductColor>(),
                ProductImages = new List<ProductImage> { mainImage }
            };

            foreach (int colorId in productVM.ColorIds)
            {
                ProductColor productColor = new ProductColor
                {
                    ColorId = colorId,
                };
                product.ProductColors.Add(productColor);
            }

            TempData["Message"] = "";
            foreach (IFormFile photo in productVM.Photos)
            {
                if (!photo.ValidateType("image/"))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uygun deyil</p>";
                    continue;
                }

                if (!photo.ValidateSize(500))
                {
                    TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uygun deyil</p>";
                    continue;
                }
                product.ProductImages.Add(new ProductImage
                {
                    IsPrimary = null,
                    Url = await photo.CreateFileAsync(_env.WebRootPath, "assets", "img")
                });
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.Include(p => p.ProductImages).Include(p => p.ProductColors).FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ColorIds = product.ProductColors.Select(pt => pt.ColorId).ToList(),
                ProductImages = product.ProductImages,
                Categories = await _context.Categories.ToListAsync(),
                Colors = await _context.Colors.ToListAsync()
            };

            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            Product existed = await _context.Products.Include(pi => pi.ProductImages).Include(p => p.ProductColors).FirstOrDefaultAsync(p => p.Id == id);
            productVM.ProductImages = existed.ProductImages;
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                return View(productVM);
            }

            if (existed is null) return NotFound();

            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bele bir category movcud deyil");
                return View(productVM);
            }
            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateType("image/"))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("MainPhoto", "File novu uygun deyil");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(600))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("MainPhoto", "File olcusu uygun deyil");
                    return View(productVM);
                }

            }
            existed.ProductColors.RemoveAll(pt => !productVM.ColorIds.Exists(colorId => colorId == pt.ColorId));

            List<int> creatable = productVM.ColorIds.Where(colorId => !existed.ProductColors.Exists(pc => pc.ColorId == colorId)).ToList();
            foreach (int colorId in creatable)
            {
                bool tagResult = await _context.Colors.AnyAsync(c => c.Id == colorId);
                if (!tagResult)
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("ColorIds", "Bele bir color movcud deyil");
                    return View(productVM);
                }
                existed.ProductColors.Add(new ProductColor
                {
                    ColorId = colorId
                });
            }

            if (productVM.MainPhoto is not null)
            {
                string fileName = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "img");

                ProductImage mainImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                mainImage.Url.DeleteFile(_env.WebRootPath, "assets", "img");
                _context.ProductImages.Remove(mainImage);

                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    Url = fileName
                });
            }
            
            if (productVM.ImageIds is null)
            {
                productVM.ImageIds = new List<int>();
            }
            List<ProductImage> removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (ProductImage pImage in removeable)
            {
                pImage.Url.DeleteFile(_env.WebRootPath, "assets", "img");
                existed.ProductImages.Remove(pImage);
            }

            TempData["Message"] = "";
            if (productVM.Photos is not null)
            {
                foreach (IFormFile photo in productVM.Photos)
                {
                    if (!photo.ValidateType("image/"))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file tipi uygun deyil</p>";
                        continue;
                    }
                    if (!photo.ValidateSize(600))
                    {
                        TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file olcusu uygun deyil</p>";
                        continue;
                    }

                    existed.ProductImages.Add(new ProductImage
                    {
                        IsPrimary = null,
                        Url = await photo.CreateFileAsync(_env.WebRootPath, "assets", "img")
                    });
                }
            }

            existed.Name = productVM.Name;
            existed.Description = productVM.Description;
            existed.Price = productVM.Price;
            existed.CategoryId = (int)productVM.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(c => c.Id == id);

            if (product is null) return NotFound();

            foreach (ProductImage image in product.ProductImages)
            {
                image.Url.DeleteFile(_env.WebRootPath, "assets", "img");
            };

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null) return NotFound();

            return View(product);
        }
    }
}