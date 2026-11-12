namespace Tooded_EFCore
{
    /// <summary>Tabel Kliendid: klient, kliendikaart ja boonuspunktid (1 punkt = 1 €).</summary>
    public class Klient
    {
        public int Id { get; set; }
        public string Nimi { get; set; } = "";
        public string? Epost { get; set; }
        public string? Telefon { get; set; }
        public string Kliendikaart { get; set; } = "";
        public decimal Boonus { get; set; }
        public DateTime Loodud { get; set; } = DateTime.Now;
    }
}
