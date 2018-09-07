namespace VaporStore.DataProcessor.ExportDto
{
    using System.Collections.Generic;

    public class GameDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Developer { get; set; }

        public string Tags { get; set; }

        public int Players { get; set; }
    }
}
