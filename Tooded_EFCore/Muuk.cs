namespace Tooded_EFCore
{
    /// <summary>Üks müük (tšekk).</summary>
    public class Muuk
    {
        public int Id { get; set; }
        public DateTime Kuupaev { get; set; } = DateTime.Now;

        public int KasutajaId { get; set; }
        public Kasutaja? Kasutaja { get; set; }

        public int? KlientId { get; set; }
        public Klient? Klient { get; set; }

        public decimal Vahesumma { get; set; }
        public decimal BoonusKasutatud { get; set; }
        public decimal Kokku { get; set; }
        public decimal BoonusSaadud { get; set; }

        public string? PdfFail { get; set; }

        public List<MuugiRida> Read { get; set; } = new();
    }

    /// <summary>Müügi rida. Nimetus ja hind salvestatakse tšeki ajal, et ajalugu ei muutuks.</summary>
    public class MuugiRida
    {
        public int Id { get; set; }

        public int MuukId { get; set; }
        public Muuk? Muuk { get; set; }

        public int? ToodeId { get; set; }
        public Toode? Toode { get; set; }

        public string Nimetus { get; set; } = "";
        public int Kogus { get; set; }
        public decimal Hind { get; set; }
    }
}
