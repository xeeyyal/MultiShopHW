﻿using System.ComponentModel.DataAnnotations;

namespace MultiShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ad mutleq daxil edilmelidir!")]
        [MaxLength(25, ErrorMessage = "Uzunlugu 25 xarakterden cox olmamalidir.")]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
