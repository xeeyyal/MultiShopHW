using System.ComponentModel.DataAnnotations;

namespace MultiShop.Models
{
    public class Color
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ad mutleq daxil edilmelidir")]
        [MaxLength(10, ErrorMessage = "Max uzunlugu 10 olmalidir")]
        public string Name { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
