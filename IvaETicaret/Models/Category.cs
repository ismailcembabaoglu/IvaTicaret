using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IvaETicaret.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
