using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IvaETicaret.Models
{
    public class StoreAdress
    {
        [Key]
        public int Id { get; set; }
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }
        public int CityId { get; set; }
        public int CountyId { get; set; }
        public int DistrictId { get; set; }
        [ForeignKey("DistrictId")]
        public District District { get; set; }
    }
}
