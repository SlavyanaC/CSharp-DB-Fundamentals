namespace FastFood.DataProcessor.Dto.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Item")]
    public class OrderItemDto
    {
        [Required]
        [XmlElement("Name")]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [XmlElement("Quantity")]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
