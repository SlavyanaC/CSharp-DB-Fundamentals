namespace Stations.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TrainSeat
    {
        public int Id { get; set; }

        public int TrainId { get; set; }
        [Required]
        public Train Train { get; set; }

        public int SeatingClassId { get; set; }
        [Required]
        public SeatingClass SeatingClass { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
