namespace ProductsShop.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("product")]
    public class SoldProductsAttributesDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}