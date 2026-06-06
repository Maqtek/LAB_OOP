namespace Lab3.Models
{
    public class Manga
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }

        public int MangakaId { get; set; }
        public Mangaka Mangaka { get; set; } = null!;

        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}
