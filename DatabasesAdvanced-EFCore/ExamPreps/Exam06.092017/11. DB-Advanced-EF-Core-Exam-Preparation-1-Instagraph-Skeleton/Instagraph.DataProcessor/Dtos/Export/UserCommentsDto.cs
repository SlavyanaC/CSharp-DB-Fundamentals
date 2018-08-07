namespace Instagraph.DataProcessor.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("user")]
    public class UserCommentsDto
    {
        [XmlElement("Username")]
        public string Username { get; set; }

        [XmlElement("MostComments")]
        public int MostComments { get; set; }
    }
}
