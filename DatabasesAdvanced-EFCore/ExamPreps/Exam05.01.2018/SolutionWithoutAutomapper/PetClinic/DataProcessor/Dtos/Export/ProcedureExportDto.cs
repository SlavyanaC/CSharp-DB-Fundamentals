﻿namespace PetClinic.DataProcessor.Dtos.Export
{
    using System;
    using System.Xml.Serialization;

    [XmlType("Procedure")]
    public class ProcedureExportDto
    {
        [XmlElement("Passport")]
        public string Passport { get; set; }

        [XmlElement("OwnerNumber")]
        public string OwnerNumber { get; set; }

        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public ProcedureAnimaAidExportDto[] AnimalAids { get; set; }

        [XmlElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }
}
