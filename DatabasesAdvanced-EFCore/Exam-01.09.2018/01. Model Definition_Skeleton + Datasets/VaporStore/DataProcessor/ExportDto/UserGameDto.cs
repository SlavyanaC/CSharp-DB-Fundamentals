namespace VaporStore.DataProcessor.ExportDto
{
    using System.Xml.Serialization;

    [XmlType("Game")]
    public class UserGameDto
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        public string Genre { get; set; }

        public decimal Price { get; set; }
    }
}
