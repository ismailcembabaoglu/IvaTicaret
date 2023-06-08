namespace IvaETicaret.Models
{
    public class ReportFiterAdminVM
    {
        public string? OrderStatus { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime BaslangicDate { get; set; }
        public DateTime BitisDate { get; set; }
    }
}
