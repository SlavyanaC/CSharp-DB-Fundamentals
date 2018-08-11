using System.Xml.Serialization;

namespace Instagraph.DataProcessor.Dto.Export
{
    public class PopularUserDto
    {
        public string Username { get; set; }

        public int Followers { get; set; }
    }
}
