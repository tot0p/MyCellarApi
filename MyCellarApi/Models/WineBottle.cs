using MyCellarApiCore.Models;
using System.ComponentModel.DataAnnotations;

namespace MyCellarApi.Models
{
    public class WineBottle : BaseModel
    {
        [Required]
        public string BrandName { get; set; }
        [Required]
        public string BottleName { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        // cépage
        public List<string> VarietalType { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string WineType { get; set; }
        [Required]
        public string Producer { get; set; }
        [Required]
        public float NetContent { get; set; }
        [Required]
        public float AlcoholContent { get; set; }
        public float UnitPrice { get; set; }
        public List<string> Labels { get; set; }
        [Required]
        public bool SulfiteWarning { get; set; } = true;
        [Required]
        public bool Vegan { get; set; } = false;
        // appellation
        public string Designation { get; set; }
        public string Description { get; set; }
    }
}
