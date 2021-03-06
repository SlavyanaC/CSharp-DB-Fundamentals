﻿namespace CarDealer.App
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using AutoMapper;

    using CarDealer.App.Dtos.Export;
    using CarDealer.App.Dtos.Import;
    using CarDealer.Data;
    using CarDealer.Models;

    public class XmlProcessor
    {
        private const string PathToXmlImportFolder = "../../../Xmls/Import/";
        private const string PathToXmlExportFolder = "../../../Xmls/Export/";

        public XmlProcessor()
        {
            this.Context = new CarDealerContext();
        }

        public CarDealerContext Context { get; set; }

        public void ImportData()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });
            IMapper mapper = config.CreateMapper();

            ImportSuppliers(mapper);
            ImportParts(mapper);
            ImportCars(mapper);
            GenerateCarParts();
            ImportCutomers(mapper);
            GenerateSales();
        }

        private void ImportSuppliers(IMapper mapper)
        {
            string xmlString = File.ReadAllText(PathToXmlImportFolder + "suppliers.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(SupplierDto[]), new XmlRootAttribute("suppliers"));
            SupplierDto[] deserializedSuppliers = (SupplierDto[])serializer.Deserialize(new StringReader(xmlString));

            List<Supplier> suppliers = new List<Supplier>();
            foreach (var supplierDto in deserializedSuppliers)
            {
                Supplier supplier = mapper.Map<Supplier>(supplierDto);
                suppliers.Add(supplier);
            }

            this.Context.Suppliers.AddRange(suppliers);
            this.Context.SaveChanges();
        }

        private void ImportParts(IMapper mapper)
        {
            string xmlString = File.ReadAllText(PathToXmlImportFolder + "parts.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(PartDto[]), new XmlRootAttribute("parts"));
            PartDto[] deserializedParts = (PartDto[])serializer.Deserialize(new StringReader(xmlString));

            Random random = new Random();
            List<Part> parts = new List<Part>();
            foreach (var partDto in deserializedParts)
            {
                Part part = mapper.Map<Part>(partDto);

                int supplierId = random.Next(1, 32);
                part.Supplier_Id = supplierId;
                parts.Add(part);
            }

            this.Context.Parts.AddRange(parts);
            this.Context.SaveChanges();
        }

        private void ImportCars(IMapper mapper)
        {
            string xmlString = File.ReadAllText(PathToXmlImportFolder + "cars.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(CarDto[]), new XmlRootAttribute("cars"));
            CarDto[] deserializedCars = (CarDto[])serializer.Deserialize(new StringReader(xmlString));

            List<Car> cars = new List<Car>();
            foreach (var carDto in deserializedCars)
            {
                Car car = mapper.Map<Car>(carDto);
                cars.Add(car);
            }

            this.Context.Cars.AddRange(cars);
            this.Context.SaveChanges();
        }

        private void GenerateCarParts()
        {
            List<PartCars> partCars = new List<PartCars>();

            Random random = new Random();
            for (int carId = 1; carId <= 358; carId++)
            {
                int partsToAddCount = random.Next(10, 21);

                List<int> addedPartsIds = new List<int>();

                for (int partCount = 0; partCount < partsToAddCount; partCount++)
                {
                    int randomPartId = random.Next(1, 132);

                    if (addedPartsIds.Contains(randomPartId))
                    {
                        partCount--;
                        continue;
                    }

                    PartCars partCar = new PartCars
                    {
                        Car_Id = carId,
                        Part_Id = randomPartId,
                    };

                    addedPartsIds.Add(randomPartId);

                    partCars.Add(partCar);
                }
            }

            this.Context.PartCars.AddRange(partCars);
            this.Context.SaveChanges();
        }

        private void ImportCutomers(IMapper mapper)
        {
            string xmlString = File.ReadAllText(PathToXmlImportFolder + "customers.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(CustomerDto[]), new XmlRootAttribute("customers"));
            CustomerDto[] deserializedCustomers = (CustomerDto[])serializer.Deserialize(new StringReader(xmlString));

            List<Customer> customers = new List<Customer>();
            foreach (var customerDto in deserializedCustomers)
            {
                Customer customer = mapper.Map<Customer>(customerDto);
                customers.Add(customer);
            }

            this.Context.Customers.AddRange(customers);
            this.Context.SaveChanges();
        }

        private void GenerateSales()
        {
            int[] discounts = new int[8] { 0, 5, 10, 15, 20, 30, 40, 50 };
            int youngDriverDiscount = 5;

            Random random = new Random();
            List<Sale> sales = new List<Sale>();

            List<int> customersWithCarIds = new List<int>();
            List<int> soldCarsIds = new List<int>();

            for (int i = 0; i < 150; i++)
            {
                int customerId = random.Next(1, 31);
                int carId = random.Next(1, 359);

                if (customersWithCarIds.Contains(customerId) && soldCarsIds.Contains(carId))
                {
                    i--;
                    continue;
                }

                int discountIndex = random.Next(8);
                int discount = discounts[discountIndex];

                bool isYoungDriver = this.Context.Customers
                    .Where(c => c.Id == customerId)
                    .Select(c => c.IsYoungDriver)
                    .SingleOrDefault();

                if (isYoungDriver)
                {
                    discount += youngDriverDiscount;
                }

                Sale sale = new Sale
                {
                    Customer_Id = customerId,
                    Car_Id = carId,
                    Discount = discount,
                };

                customersWithCarIds.Add(customerId);
                soldCarsIds.Add(carId);

                sales.Add(sale);
            }

            this.Context.Sales.AddRange(sales);
            this.Context.SaveChanges();
        }

        public void GetCarsWithDistance()
        {
            DistanceCarsDto[] cars = this.Context.Cars
                .Where(c => c.TravelledDistance > 2_000_000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Select(c => new DistanceCarsDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(DistanceCarsDto[]), new XmlRootAttribute("cars"));

            serializer.Serialize(new StringWriter(sb), cars, namespaces);

            File.WriteAllText(PathToXmlExportFolder + "cars.xml", sb.ToString());
        }

        public void GetCarsFromMakeFerrari()
        {
            FerrariCarDto[] cars = this.Context.Cars
                .Where(c => c.Make.ToLower() == "ferrari")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new FerrariCarDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(FerrariCarDto[]), new XmlRootAttribute("cars"));

            serializer.Serialize(new StringWriter(sb), cars, namespaces);

            File.WriteAllText(PathToXmlExportFolder + "ferrari-cars.xml", sb.ToString());
        }

        public void GetLocalSuppliers()
        {
            LocalSupplierDto[] suppliers = this.Context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new LocalSupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count(),
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(LocalSupplierDto[]), new XmlRootAttribute("suppliers"));

            serializer.Serialize(new StringWriter(sb), suppliers, namespaces);

            File.WriteAllText(PathToXmlExportFolder + "local-suppliers.xml", sb.ToString());
        }

        public void GetCarsWithTheirListParts()
        {
            CarsWithPartsCarDto[] cars = this.Context.Cars
                .Select(c => new CarsWithPartsCarDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(pc => new CarsWithPartsPartDto
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price,
                    }).ToArray()
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(CarsWithPartsCarDto[]), new XmlRootAttribute("cars"));

            serializer.Serialize(new StringWriter(sb), cars, namespaces);

            File.WriteAllText(PathToXmlExportFolder + "cars-and-parts.xml", sb.ToString());
        }

        public void GetTotalSalesByCustomer()
        {
            CustomerSalesDto[] customerSales = this.Context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new CustomerSalesDto
                {
                    Name = c.Name,
                    CarsBought = c.Sales.Count(),
                    MoneySpent = Math.Round(c.Sales
                        .Select(s => s.Car.PartCars.Sum(p => p.Part.Price) * ((100m - s.Discount) / 100m))
                        .DefaultIfEmpty(0)
                        .Sum(), 2)
                })
                .OrderByDescending(c => c.MoneySpent)
                .ThenByDescending(c => c.CarsBought)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(CustomerSalesDto[]), new XmlRootAttribute("customers"));

            serializer.Serialize(new StringWriter(sb), customerSales, namespaces);

            File.WriteAllText(PathToXmlExportFolder + "customers-total-sales.xml", sb.ToString());
        }

        public void GetSalesWithAppliedDiscount()
        {
            SalesDiscountSaleDto[] sales = this.Context.Sales
                .Select(s => new SalesDiscountSaleDto
                {
                    Car = new SalesDiscountCarDto
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance,
                    },
                    Discount = Math.Round((s.Discount / 100.0m), 2, MidpointRounding.AwayFromZero),
                    Price = Math.Round(s.Car.PartCars
                        .Select(pc => pc.Part.Price * ((100m - s.Discount) / 100m))
                        .DefaultIfEmpty(0)
                        .Sum(), 2),
                    PriceWithoutDiscount = s.Car.PartCars
                        .Select(pc => pc.Part.Price)
                        .DefaultIfEmpty(0)
                        .Sum(),
                }).ToArray();

            StringBuilder sb = new StringBuilder();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            XmlSerializer serializer = new XmlSerializer(typeof(SalesDiscountSaleDto[]), new XmlRootAttribute("sales"));

            serializer.Serialize(new StringWriter(sb), sales, namespaces);

            File.WriteAllText(PathToXmlExportFolder + "sales-discounts.xml", sb.ToString());
        }
    }
}
