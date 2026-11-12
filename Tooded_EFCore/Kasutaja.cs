namespace Tooded_EFCore
{
    /// <summary>Süsteemi kasutaja (Ostja või Omanik).</summary>
    public class Kasutaja
    {
        public int Id { get; set; }
        public string Kasutajanimi { get; set; } = "";
        public string? TaisNimi { get; set; }
        public string ParoolHash { get; set; } = "";
        public string Sool { get; set; } = "";
        public string Roll { get; set; } = Rollid.Ostja;
        public DateTime Loodud { get; set; } = DateTime.Now;
    }
}
