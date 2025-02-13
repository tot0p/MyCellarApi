using MyCellarApiCore.Models;
using System.ComponentModel.DataAnnotations;

namespace MyCellarApi.Models
{
    public class Cellar : BaseModel
    {
        [Required]
        public string Name { get; set; } = "";
        public string Location { get; set; }
    }
}
