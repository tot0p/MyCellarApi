using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
