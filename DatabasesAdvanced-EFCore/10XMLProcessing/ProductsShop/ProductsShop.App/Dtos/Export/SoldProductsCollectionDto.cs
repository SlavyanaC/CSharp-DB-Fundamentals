namespace ProductsShop.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("sold-products")]
    public class SoldProductsCollectionDto
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement("product")]
        public SoldProductsAttributesDto[] SoldProducts { get; set; }
    }
}
