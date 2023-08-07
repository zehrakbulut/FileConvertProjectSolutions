using System.ComponentModel.DataAnnotations;

namespace FileConvertProject.Models
{
    public class FileConvert
    {

        [Key]
        public int Id { get; set; }
        public string Information { get; set; }
        public string SoftwareNumber { get; set; }
        public string HardwareNumber { get; set; }
        public DateTime Date { get; set; }
        public string? FilePath { get; set; }
    }
}
