namespace Tooded_EFCore
{
    /// <summary>Poe (PoodForm) ostukorvi rida. Kassa-vorm kasutab oma OstuRida klassi.</summary>
    public class OstukorviRida
    {
        public Toode Toode { get; set; } = null!;
        public int Kogus { get; set; }
        public decimal Summa => Toode.Hind * Kogus;

        public override string ToString() =>
            $"{Toode.Toodenimetus} - {Kogus} tk x {Toode.Hind:F2} EUR = {Summa:F2} EUR";
    }

    /// <summary>Üks ostukorv kogu rakenduse peale: pood lisab ridu, arvevorm loeb neid.</summary>
    public static class Ostukorv
    {
        public static List<OstukorviRida> Ridad { get; } = new();
        public static decimal Kokku => Ridad.Sum(r => r.Summa);

        public static void Lisa(Toode toode, int kogus)
        {
            var olemas = Ridad.FirstOrDefault(r => r.Toode.Id == toode.Id);
            if (olemas != null) olemas.Kogus += kogus;
            else Ridad.Add(new OstukorviRida { Toode = toode, Kogus = kogus });
        }

        public static void Tyhjenda() => Ridad.Clear();
    }
}
