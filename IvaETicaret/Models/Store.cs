using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IvaETicaret.Models
{
    public class Store
    {
        [Key]
        public Guid Id { get; set; }
        public string? RootName { get; set; }
        public string CompanyName { get; set; }
        public string TaxNumber { get; set; }
        public string TaxOffice { get; set; }
        public bool ContractConfirmation { get; set; }
        public bool IsActive { get; set; }
        public bool StoreIsActive { get; set; }
        public string TaxPlate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        public ICollection<StoreAdress>? storeAdresses { get; set; }
        public ICollection<OrderHeader>? OrderHeaders { get; set; }
    }
}
