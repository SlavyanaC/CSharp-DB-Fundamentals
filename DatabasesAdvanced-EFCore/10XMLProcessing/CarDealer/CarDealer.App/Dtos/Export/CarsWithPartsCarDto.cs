namespace CarDealer.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("car")]
    public class CarsWithPartsCarDto
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        public CarsWithPartsPartDto[] Parts { get; set; }
    }
}
