using MyCellarApiCore.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCellarApi.Models
{
    public class StockInformation : BaseModel
    {
        [Required]
        [ForeignKey(nameof(WineBottle))]
        public int WineId { get; set; }
        public virtual WineBottle WineBottle { get; set; }

        [Required]
        [ForeignKey(nameof(Cellar))]
        public int CellarId { get; set; }        
        public virtual Cellar Cellar { get; set; }
        [Required]
        public int Quantity { get; set; } = 0;
}
}
