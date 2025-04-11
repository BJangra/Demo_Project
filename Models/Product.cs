using System.ComponentModel.DataAnnotations;

namespace FirstProject_ECommerce.Models
{
    public class Product
    {
        public  int Id { get; set; }
        [Required]
        public  String Name { get; set; }
        [Required]
        public  string Description { get; set; }
        public  decimal Price { get; set; }
        public  String? ImageUrl { get; set; }
    }
}
