namespace CarDealer.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("part")]
    public class CarsWithPartsPartDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}
