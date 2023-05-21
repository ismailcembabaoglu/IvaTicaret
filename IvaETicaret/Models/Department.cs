using System.ComponentModel.DataAnnotations;

namespace IvaETicaret.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Image { get; set; }
        public virtual ICollection<Store>? Stores { get; set; }
    }
}
