using MyCellarApiCore.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCellarApi.Models
{
    public class StockInformation : BaseModel
    {
        [Required]
        public int WineId { get; set; }

        [ForeignKey(nameof(WineBottle))]
        public virtual WineBottle WineBottle { get; set; }

        [Required]
        public int CellarId { get; set; }

        [ForeignKey(nameof(Cellar))]
        public virtual Cellar Cellar { get; set; }
        [Required]
        public int Quantity { get; set; } = 0;
}
}
