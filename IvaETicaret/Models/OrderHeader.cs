using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IvaETicaret.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        //public int BranchId { get; set; }
        //[ForeignKey("BranchId")]
        //public Branch Branch { get; set; }
        public DateTime OrderDate { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string SurName { get; set; }
        [Required]
        public int AdressId { get; set; }
        [ForeignKey("AdressId")]
        public Adress Adress { get; set; }
        public int OdemeTurId { get; set; }
        [ForeignKey("OdemeTurId")]
        public OdemeTur OdemeTur { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public Guid StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store Store { get; set; }
       
        public string? CartName { get; set; }
       
        public string? CartNumber { get; set; }

        public string? ExpirationMonth { get; set; }
    
        public string? ExpirationYear { get; set; }
        
        public string? Cvc { get; set; }
        public string? Odendimi { get; set; }

    }
}
