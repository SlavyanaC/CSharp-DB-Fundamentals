namespace Stations.Models
{
    using Stations.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Train
    {
        public Train()
        {
            this.TrainSeats = new HashSet<TrainSeat>();
            this.Trips = new HashSet<Trip>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string TrainNumber { get; set; }

        public TrainType? Type { get; set; }

        public ICollection<TrainSeat> TrainSeats { get; set; }

        public ICollection<Trip> Trips { get; set; }

    }
}
