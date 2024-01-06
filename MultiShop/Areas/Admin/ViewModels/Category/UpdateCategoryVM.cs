using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Admin.ViewModels
{
    public class UpdateCategoryVM
    {
        [Required(ErrorMessage ="Bu adda artiq movcuddur")]
        [MaxLength(15,ErrorMessage ="Max uzunluq 15 olmalidir")]
        public string Name { get; set; }
    }
}
