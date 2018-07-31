namespace CarDealer.App
{
    class StartUp
    {
        static void Main(string[] args)
        {
            XmlProcessor xmlProcessor = new XmlProcessor();
            xmlProcessor.ImportData();
            xmlProcessor.GetCarsWithDistance();
            xmlProcessor.GetCarsFromMakeFerrari();
            xmlProcessor.GetLocalSuppliers();
            xmlProcessor.GetCarsWithTheirListParts();
            xmlProcessor.GetTotalSalesByCustomer();
            xmlProcessor.GetSalesWithAppliedDiscount();

            JsonProcessor jsonProcessor = new JsonProcessor();
            jsonProcessor.ImportData();
            jsonProcessor.ExportOrderedCustomers();
            jsonProcessor.ExportCarsFromMakeToyota();
            jsonProcessor.ExportLocalSuppliers();
            jsonProcessor.ExportCarsWithListOfParts();
            jsonProcessor.ExportTotalSalesByCustomer();
            jsonProcessor.ExportSalesWithAppliedDiscount();
        }
    }
}
