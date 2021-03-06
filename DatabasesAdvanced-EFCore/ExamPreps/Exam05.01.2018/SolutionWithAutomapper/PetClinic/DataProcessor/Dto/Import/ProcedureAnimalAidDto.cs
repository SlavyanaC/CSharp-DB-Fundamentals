﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.Dto.Import
{
    [XmlType("AnimalAid")]
    public class ProcedureAnimalAidDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }
    }
}