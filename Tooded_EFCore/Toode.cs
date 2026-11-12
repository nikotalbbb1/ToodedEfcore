namespace Tooded_EFCore
{
    public class Toode
    {
        public int Id { get; set; }
        public string Toodenimetus { get; set; } = "";
        public int Kogus { get; set; }
        public decimal Hind { get; set; }
        public string? Pilt { get; set; }

        public int KategooriaId { get; set; }
        public Kategooria? Kategooria { get; set; }
    }
}
