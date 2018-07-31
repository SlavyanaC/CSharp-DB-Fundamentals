namespace ProductsShop.App.Dtos.Xml.Export
{
    using System.Xml.Serialization;

    [XmlType("user")]
    public class UserSoldProductsDto
    {
        [XmlAttribute("first-name")]
        public string FirstName { get; set; }

        [XmlAttribute("last-name")]
        public string LastName { get; set; }

        [XmlArray("sold-products")]
        public SoldProductDto[] SoldProducts { get; set; }
    }
}
