namespace IvaETicaret.Models
{
    public class ReportVm
    {
        public List<OrderHeader> OrderHeaders { get; set; }
        public double ToplamSatisTutar { get; set; }
        public double AlinacakTutar { get; set; }
    }
}
