namespace FastFood.DataProcessor.Dto.Import
{
    using FastFood.Models.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Order")]
    public class OrderDto
    {
        [XmlElement("Customer")]
        public string Customer { get; set; }

        [XmlElement("Employee")]
        [StringLength(30, MinimumLength = 3)]
        public string Employee { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlArray("Items")]
        public OrderItemDto[] Items { get; set; }
    }
}
