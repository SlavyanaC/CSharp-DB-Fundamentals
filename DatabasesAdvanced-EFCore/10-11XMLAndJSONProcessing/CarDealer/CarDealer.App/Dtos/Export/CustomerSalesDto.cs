namespace CarDealer.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("customer")]
    public class CustomerSalesDto
    {
        [XmlAttribute("full-name")]
        public string Name { get; set; }

        [XmlAttribute("bought-cars")]
        public int CarsBought { get; set; }

        [XmlAttribute("spent-money")]
        public decimal MoneySpent { get; set; }
    }
}
