﻿namespace CarDealer.Models
{
    using System.Collections.Generic;

    public class Part
    {
        public Part()
        {
            this.PartCars = new HashSet<PartCars>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int Supplier_Id { get; set; }
        public Supplier Supplier { get; set; }

        public ICollection<PartCars> PartCars { get; set; }
    }
}
