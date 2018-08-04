namespace PetClinic.DataProcessor.Dtos.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Procedure")]
    public class ProcedureImportDto
    {
        [XmlElement("Vet")]
        public string VetName { get; set; }

        [XmlElement("Animal")]
        public string AnimalName { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public AnimalAidImportDto[] AnimalAids { get; set; }
    }
}
