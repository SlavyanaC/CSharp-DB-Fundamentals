namespace CarDealer.App
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

    using CarDealer.Data;
    using CarDealer.Models;

    public class JsonProcessor
    {
        private const string PathToJsonImport = "../../../Jsons/Import/";
        private const string PathToJsonExport = "../../../Jsons/Export/";

        public JsonProcessor()
        {
            this.Context = new CarDealerContext();
        }

        public CarDealerContext Context { get; set; }

        public void ImportData()
        {
            this.ImportSuppliers();
            this.ImportParts();
            this.ImportCars();
            this.GenerateCarParts();
            this.ImportCustomers();
            this.GenerateSales();
        }

        private void ImportSuppliers()
        {
            string file = File.ReadAllText(PathToJsonImport + "suppliers.json");

            Supplier[] suppliers = JsonConvert.DeserializeObject<Supplier[]>(file);

            this.Context.Suppliers.AddRange(suppliers);
            this.Context.SaveChanges();
        }

        private void ImportParts()
        {
            string file = File.ReadAllText(PathToJsonImport + "parts.json");
            Part[] deserializedParts = JsonConvert.DeserializeObject<Part[]>(file);

            Random random = new Random();
            int maxSupplierIdRnd = this.Context.Suppliers.Count() + 1;
            List<Part> parts = new List<Part>();
            foreach (var part in deserializedParts)
            {
                int supplierId = random.Next(1, maxSupplierIdRnd);
                part.Supplier_Id = supplierId;
                parts.Add(part);
            }

            this.Context.Parts.AddRange(parts);
            this.Context.SaveChanges();
        }

        private void ImportCars()
        {
            string file = File.ReadAllText(PathToJsonImport + "cars.json");
            Car[] cars = JsonConvert.DeserializeObject<Car[]>(file);

            this.Context.Cars.AddRange(cars);
            this.Context.SaveChanges();
        }

        private void GenerateCarParts()
        {
            List<PartCars> partCars = new List<PartCars>();

            Random random = new Random();
            int partMaxRandom = this.Context.Parts.Count() + 1;

            int maxCarId = this.Context.Cars.Count();
            for (int carId = 1; carId <= maxCarId; carId++)
            {
                int countOfPartsToAdd = random.Next(10, 20);

                List<int> addedPartsIds = new List<int>();

                for (int partCount = 0; partCount < countOfPartsToAdd; partCount++)
                {
                    int partId = random.Next(1, partMaxRandom);

                    if (addedPartsIds.Contains(partId))
                    {
                        partCount--;
                        continue;
                    }

                    addedPartsIds.Add(partId);

                    PartCars partCar = new PartCars
                    {
                        Car_Id = carId,
                        Part_Id = partId,
                    };

                    partCars.Add(partCar);
                }
            }

            this.Context.PartCars.AddRange(partCars);
            this.Context.SaveChanges();
        }

        private void ImportCustomers()
        {
            string file = File.ReadAllText(PathToJsonImport + "customers.json");
            Customer[] customers = JsonConvert.DeserializeObject<Customer[]>(file);

            this.Context.Customers.AddRange(customers);
            this.Context.SaveChanges();
        }

        private void GenerateSales()
        {
            int[] discounts = new int[] { 0, 5, 10, 15, 20, 30, 40, 50 };
            int youngDriverDiscount = 5;

            List<Sale> sales = new List<Sale>();

            List<int> customersWithCarIds = new List<int>();
            List<int> soldCarsIds = new List<int>();

            Random random = new Random();
            int customerIdMaxRnd = this.Context.Customers.Count() + 1;
            int carIdMaxRnd = this.Context.Cars.Count() + 1;

            for (int i = 0; i < 150; i++)
            {
                int customerId = random.Next(1, customerIdMaxRnd);
                int carId = random.Next(1, carIdMaxRnd);

                if (customersWithCarIds.Contains(customerId) && soldCarsIds.Contains(carId))
                {
                    i--;
                    continue;
                }

                customersWithCarIds.Add(customerId);
                soldCarsIds.Add(carId);

                int discount = discounts[random.Next(8)];

                bool isYoungDriver = this.Context.Customers
                    .FirstOrDefault(c => c.Id == customerId)
                    .IsYoungDriver;

                if (isYoungDriver)
                {
                    discount += youngDriverDiscount;
                }

                Sale sale = new Sale
                {
                    Discount = discount,
                    Customer_Id = customerId,
                    Car_Id = carId,
                };

                sales.Add(sale);
            }

            this.Context.Sales.AddRange(sales);
            this.Context.SaveChanges();
        }

        public void ExportOrderedCustomers()
        {
            Customer[] customers = this.Context.Customers
                .OrderByDescending(c => c.BirthDate)
                .ToArray();

            string jsonString = JsonConvert.SerializeObject(customers, Formatting.Indented);

            File.WriteAllText(PathToJsonExport + "ordered-customers.json", jsonString);
        }

        public void ExportCarsFromMakeToyota()
        {
            var toyotaCars = this.Context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TravelledDistance
                }).ToArray();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            string jsonString = JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);

            File.WriteAllText(PathToJsonExport + "toyota-cars.json", jsonString);
        }

        public void ExportLocalSuppliers()
        {
            var localSuppliers = this.Context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count,
                })
                .ToArray();

            string jsonString = JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);

            File.WriteAllText(PathToJsonExport + "local-suppliers.json", jsonString);
        }

        public void ExportCarsWithListOfParts()
        {
            var cars = this.Context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TravelledDistance,
                    },
                    parts = c.PartCars.Select(pc => new
                    {
                        pc.Part.Name,
                        pc.Part.Price,
                    }).ToArray(),
                })
                .ToArray();

            string jsonString = JsonConvert.SerializeObject(cars, Formatting.Indented);

            File.WriteAllText(PathToJsonExport + "cars-and-parts.json", jsonString);
        }

        public void ExportTotalSalesByCustomer()
        {
            var buyers = this.Context.Customers
                .Where(b => b.Sales.Count > 0)
                .Select(b => new
                {
                    fullName = b.Name,
                    boughtCars = b.Sales.Count,
                    spentMoney = Math.Round(b.Sales.Select(s => s.Car.PartCars.Sum(p => p.Part.Price) * ((100m - s.Discount) / 100m))
                        .DefaultIfEmpty(0)
                        .Sum(), 2),
                })
                .OrderByDescending(b => b.spentMoney)
                .ThenByDescending(b => b.boughtCars)
                .ToArray();


            string jsonString = JsonConvert.SerializeObject(buyers, Formatting.Indented);

            File.WriteAllText(PathToJsonExport + "customers-total-sales.json", jsonString);
        }

        public void ExportSalesWithAppliedDiscount()
        {
            var sales = this.Context.Sales
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TravelledDistance,
                    },

                    customerName = s.Customer.Name,
                    Discount = Math.Round((s.Discount / 100m), 2, MidpointRounding.AwayFromZero),
                    price = Math.Round(s.Car.PartCars.Select(p => p.Part.Price * ((100m - s.Discount) / 100m)).DefaultIfEmpty(0).Sum(), 2),
                    priceWithDiscount = s.Car.PartCars.Sum(p => p.Part.Price),

                })
                .ToArray();

            string jsonString = JsonConvert.SerializeObject(sales, Formatting.Indented);

            File.WriteAllText(PathToJsonExport + "sales-discounts.json", jsonString);
        }
    }
}
