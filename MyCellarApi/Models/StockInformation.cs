using MyCellarApiCore.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCellarApi.Models
{
    public class StockInformation : BaseModel
    {
        [Required]
        [ForeignKey("WineBottle")]
        public int WineBottleId { get; set; }

        [Required]
        [ForeignKey("Cellar")]
        public int CellarId { get; set; }

        [Required]
        public int Quantity { get; set; } = 0;
    }
}
