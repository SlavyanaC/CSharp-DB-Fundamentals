namespace ProductsShop.App
{
    class StartUp
    {
        public static void Main(string[] args)
        {
            XmlProcessor xmlProcessor = new XmlProcessor();
            xmlProcessor.ImportData();
            xmlProcessor.ExportProductsInRange();
            xmlProcessor.ExportSoldProducts();
            xmlProcessor.ExportCategoriesByProductsCount();
            xmlProcessor.ExportUsersAndProducts();

            JsonProcessor jsonProcessor = new JsonProcessor();
            jsonProcessor.ImportData();
            jsonProcessor.ExportProductsInRange();
            jsonProcessor.ExportSoldProducts();
            jsonProcessor.ExportCategoriesByProductsCount();
            jsonProcessor.ExportUsersAndProducts();
        }
    }
}
