using System.Security.Cryptography;

namespace Tooded_EFCore
{
    public static class Rollid
    {
        public const string Ostja = "Ostja";
        public const string Omanik = "Omanik";
    }

    public static class Seaded
    {
        public static readonly string PildiKaust = Path.Combine(AppContext.BaseDirectory, "Images");
        public static readonly string ArvedKaust = Path.Combine(AppContext.BaseDirectory, "Arved");
        public const string PoeNimi = "Pood";
        public const decimal BoonusProtsent = 0.05m;   // 5% ostusummast läheb kliendi boonusesse
        public const int MadalLaoseis = 5;             // kui kogus <= 5, märgime punasega
    }

    /// <summary>Praegu sisselogitud kasutaja.</summary>
    public static class Seanss
    {
        public static Kasutaja? Kasutaja { get; set; }
        public static bool OnSisse => Kasutaja != null;
        public static bool OnOmanik => Kasutaja?.Roll == Rollid.Omanik;
        public static string Nimi => Kasutaja == null
            ? ""
            : (string.IsNullOrWhiteSpace(Kasutaja.TaisNimi) ? Kasutaja.Kasutajanimi : Kasutaja.TaisNimi);
    }

    public static class ParoolAbi
    {
        private const int Iteratsioone = 100_000;

        public static (string Hash, string Sool) Loo(string parool)
        {
            byte[] sool = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(parool, sool, Iteratsioone, HashAlgorithmName.SHA256, 32);
            return (Convert.ToBase64String(hash), Convert.ToBase64String(sool));
        }

        public static bool Kontrolli(string parool, string hash, string sool)
        {
            byte[] arvutatud = Rfc2898DeriveBytes.Pbkdf2(parool, Convert.FromBase64String(sool),
                Iteratsioone, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(arvutatud, Convert.FromBase64String(hash));
        }
    }

    public static class Teade
    {
        public static void Viga(string tekst) =>
            MessageBox.Show(tekst, "Viga", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static void Info(string tekst) =>
            MessageBox.Show(tekst, "Teade", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static bool Kinnita(string tekst) =>
            MessageBox.Show(tekst, "Kinnitus", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        public static void AvaFail(string tee)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tee) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Viga("Faili ei õnnestunud avada:\n" + ex.Message);
            }
        }
    }

    /// <summary>
    /// Piltide käsitlemine. Parandab vead:
    ///  - Image.FromFile lukustas faili (uuendamisel tekkis IOException) -> laeme faili mällu ja sulgeme voo;
    ///  - kaust "Images" polnud alati olemas / sõltus töökaustast -> alati exe kõrval ja luuakse vajadusel;
    ///  - sama nimega erinev pilt kirjutati üle -> lisame nimele _2, _3 ...;
    ///  - sama faili kopeerimine iseendale andis veateate.
    /// </summary>
    public static class PildiHoidla
    {
        public static Image? LaeFailist(string tee)
        {
            try
            {
                using var voog = new FileStream(tee, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var originaal = Image.FromStream(voog);
                return new Bitmap(originaal);      // koopia -> fail ei jää lukku
            }
            catch
            {
                return null;
            }
        }

        public static Image? Lae(string? failinimi)
        {
            if (string.IsNullOrWhiteSpace(failinimi)) return null;
            string tee = Path.Combine(Seaded.PildiKaust, failinimi);
            return File.Exists(tee) ? LaeFailist(tee) : null;
        }

        /// <summary>Kopeerib pildi Images-kausta ja tagastab failinime, mis tuleb andmebaasi salvestada.</summary>
        public static string Salvesta(string allikas)
        {
            Directory.CreateDirectory(Seaded.PildiKaust);

            string nimi = Path.GetFileName(allikas);
            string baas = Path.GetFileNameWithoutExtension(nimi);
            string laiend = Path.GetExtension(nimi);
            int n = 1;

            while (true)
            {
                string siht = Path.Combine(Seaded.PildiKaust, nimi);
                if (!File.Exists(siht))
                {
                    File.Copy(allikas, siht, false);
                    return nimi;
                }
                if (SamaFail(allikas, siht)) return nimi;   // sama pilt on juba olemas
                n++;
                nimi = $"{baas}_{n}{laiend}";
            }
        }

        private static bool SamaFail(string a, string b)
        {
            if (string.Equals(Path.GetFullPath(a), Path.GetFullPath(b), StringComparison.OrdinalIgnoreCase)) return true;
            if (new FileInfo(a).Length != new FileInfo(b).Length) return false;
            return File.ReadAllBytes(a).SequenceEqual(File.ReadAllBytes(b));
        }
    }
}
