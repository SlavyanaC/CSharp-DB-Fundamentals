namespace VaporStore.DataProcessor.ExportDto
{
    using System.Collections.Generic;

    public class GenreDto
    {
        public int Id { get; set; }

        public string Genre { get; set; }

        public ICollection<GameDto> Games { get; set; }

        public int TotalPlayers { get; set; }
    }
}
