namespace CarDealer.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("sale")]
    public class SalesDiscountSaleDto
    {
        [XmlElement("car")]
        public SalesDiscountCarDto Car { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal PriceWithoutDiscount { get; set; }
    }
}
