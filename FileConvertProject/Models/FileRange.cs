using System.ComponentModel.DataAnnotations;

namespace FileConvertProject.Models
{
    public class FileRange
    {
        [Key]
        public int Id { get; set; }
        public string FirstAddress { get; set; }
        public string LastAddress { get; set; }
    }
}
