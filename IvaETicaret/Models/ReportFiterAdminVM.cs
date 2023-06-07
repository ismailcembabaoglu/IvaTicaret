namespace IvaETicaret.Models
{
    public class ReportFiterAdminVM
    {
        public string OrderStatus { get; set; }
        public string Name { get; set; }
      
        public string SurName { get; set; }
        public Guid StoreId { get; set; }
        public DateTime BaslangicDate { get; set; }
        public DateTime BitisDate { get; set; }
    }
}
