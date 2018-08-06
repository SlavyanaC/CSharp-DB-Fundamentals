namespace Stations.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;

    public class TripDto
    {
        public TripDto()
        {
            this.Status = "OnTime";
        }

        [Required]
        [MaxLength(10)]
        public string Train { get; set; }

        [Required]
        [MaxLength(50)]
        public string OriginStation { get; set; }

        [Required]
        [MaxLength(50)]
        public string DestinationStation { get; set; }

        [Required]
        public string DepartureTime { get; set; }

        [Required]
        public string ArrivalTime { get; set; }

        public string Status { get; set; }

        public string TimeDifference { get; set; }
    }
}
