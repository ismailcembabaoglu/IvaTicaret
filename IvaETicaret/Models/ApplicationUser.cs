using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IvaETicaret.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string? Adress { get; set; }
        public string? PhoneNumber { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }
        public string? PostaKodu { get; set; }
        [NotMapped]
        public string Role { get; set; }
        public Guid? StoreId { get; set; }


    }
}
