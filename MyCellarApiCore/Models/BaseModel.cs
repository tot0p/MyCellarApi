using System.ComponentModel.DataAnnotations;

namespace MyCellarApiCore.Models
{
    public class BaseModel
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [Required]
        public DateTime UpdateAt { get; set; } = DateTime.Now;

        [Required]
        public bool Deleted { get; set; }
    }
}
