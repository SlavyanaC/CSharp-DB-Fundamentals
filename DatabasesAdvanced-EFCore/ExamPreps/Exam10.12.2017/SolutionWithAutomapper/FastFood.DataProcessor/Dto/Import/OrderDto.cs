namespace FastFood.DataProcessor.Dto.Import
{
    using FastFood.Models.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Order")]
    public class OrderDto
    {
        public string Customer { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string Employee { get; set; }

        public string DateTime { get; set; }

        public string Type { get; set; }

        [XmlArray("Items")]
        public OrderItemDto[] Items { get; set; }
    }
}
