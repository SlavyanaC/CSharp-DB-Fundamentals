namespace ProductsShop.App.Dtos.Xml.Export
{
    using System.Xml.Serialization;

    [XmlType("category")]
    public class CategoryByProductDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("products-count")]
        public int Count { get; set; }

        [XmlElement("average-price")]
        public decimal AveragePrice { get; set; }

        [XmlElement("total-revenue")]
        public decimal TotalRevenue { get; set; }
    }
}
