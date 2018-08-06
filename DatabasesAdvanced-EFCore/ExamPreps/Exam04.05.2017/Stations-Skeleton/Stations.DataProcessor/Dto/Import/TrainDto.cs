namespace Stations.DataProcessor.Dto.Import
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TrainDto
    {
        public TrainDto()
        {
            this.Type = "HighSpeed";
            this.Seats = new HashSet<TrainSeatDto>();
        }

        [Required]
        [MaxLength(10)]
        public string TrainNumber { get; set; }

        public string Type { get; set; }

        public ICollection<TrainSeatDto> Seats { get; set; }
    }
}
