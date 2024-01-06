using MultiShop.Models;
using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Admin.ViewModels
{
    public class UpdateProductVM
    {
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
		public IFormFile? MainPhoto { get; set; }
		public List<IFormFile>? Photos { get; set; }
        public string Description { get; set; }
		[Required]
        public int? CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
        public List<int> ColorIds { get; set; }
        public List<Color>? Colors { get; set; }
        public List<int>? ImageIds { get; set; } 

        public List<ProductImage>? ProductImages { get; set; }
    }
}
