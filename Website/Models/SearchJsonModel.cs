namespace Website.Models
{
    public class SearchJsonModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string NameAscci { get; set; }
        public string UrlAvatar { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string Type { get; set; }
        public string Html { get; set; }
    }
}