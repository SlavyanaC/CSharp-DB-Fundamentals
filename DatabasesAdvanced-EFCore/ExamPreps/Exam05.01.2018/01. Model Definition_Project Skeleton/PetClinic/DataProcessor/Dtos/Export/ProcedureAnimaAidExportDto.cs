namespace PetClinic.DataProcessor.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("AnimalAid")]
    public class ProcedureAnimaAidExportDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
