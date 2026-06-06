namespace Lab3.Models
{
    public class Mangaka
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public List<Manga> Mangas { get; set; } = new List<Manga>();
    }
}
