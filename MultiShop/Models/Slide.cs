using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MultiShop.Models
{
    public class Slide
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title mutleq daxil edilmelidir")]
        [MaxLength(25, ErrorMessage = "Title max uzunlugu: 25")]
        public string Title { get; set; }
        [MinLength(15, ErrorMessage = "Description min uzunlugu: 15")]
        [MaxLength(250, ErrorMessage = "Description max uzunlugu: 250")]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
    }
}
