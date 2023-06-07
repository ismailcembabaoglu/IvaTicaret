using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IvaETicaret.Models
{
    public class StorePayment
    {
        [Key]
        public int Id { get; set; }
        public Guid StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public double PricePayable { get; set; }
    }
}
