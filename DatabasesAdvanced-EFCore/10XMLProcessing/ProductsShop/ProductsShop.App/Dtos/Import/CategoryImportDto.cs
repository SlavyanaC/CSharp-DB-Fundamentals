namespace ProductsShop.App.Dtos.Import
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("category")]
    public class CategoryImportDto
    {
        [XmlElement("name")]
        [StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }
    }
}
