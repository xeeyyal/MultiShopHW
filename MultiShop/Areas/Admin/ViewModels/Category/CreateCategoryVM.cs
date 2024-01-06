using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Admin.ViewModels
{
    public class CreateCategoryVM
    {
        [MaxLength(15, ErrorMessage = "Max uzunluq 15 olmalidir")]
        public string Name { get; set; }
    }
}
