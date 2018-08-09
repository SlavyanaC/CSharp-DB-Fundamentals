namespace Stations.DataProcessor.Dto.Import
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class TripDto
    {
        public TripDto()
        {
            this.Status = "OnTime";
        }

        [Required]
        [MaxLength(10)]
        [JsonProperty("Train")]
        public string TrainNumber { get; set; }

        [Required]
        [MaxLength(50)]
        [JsonProperty("OriginStation")]
        public string OriginStationName { get; set; }

        [Required]
        [MaxLength(50)]
        [JsonProperty("DestinationStation")]
        public string DestinationStationName { get; set; }

        [Required]
        public string DepartureTime { get; set; }

        [Required]
        public string ArrivalTime { get; set; }

        public string TimeDifference { get; set; }

        public string Status { get; set; }
    }
}
