namespace Stations.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;

    public class TrainDto
    {
        public TrainDto()
        {
            this.Type = "HighSpeed";
            this.Seats = new SeatDto[0];
        }

        [Required]
        [MaxLength(10)]
        public string TrainNumber { get; set; }

        public string Type { get; set; }

        public SeatDto[] Seats { get; set; }
    }
}
