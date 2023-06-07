using System.ComponentModel.DataAnnotations;

namespace IvaETicaret.Models
{
    public class SiteSetting
    {
        [Key]
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string UserEntryLogo { get; set; }
        public string MainLogo { get; set; }
    }
}
