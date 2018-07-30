﻿namespace ProductsShop.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlRoot("users")]
    public class UsersColletionDto
    {
        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement("user")]
        public UserDto[] Users { get; set; }
    }
}
