namespace VaporStore.DataProcessor.ImportDto
{
    using System.Xml.Serialization;
    using System.ComponentModel.DataAnnotations;

    [XmlType("Purchase")]
    public class PurchaseDto
    {
        [Required]
        [XmlAttribute("title")]
        public string Title { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")]
        public string Key { get; set; }

        [Required]
        [RegularExpression(@"^\d{4}\s\d{4}\s\d{4}\s\d{4}$")]
        public string Card { get; set; }

        [Required]
        public string Date { get; set; }
    }
}
