using System.Drawing.Imaging;
using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    /// <summary>
    /// Täidab tühjad tabelid algandmetega (kategooriad, tooted, kasutajad, kliendid).
    /// Töötab ainult siis, kui vastav tabel on tühi, seega olemasolevaid andmeid ei kirjutata üle.
    /// </summary>
    public static class Seemnes
    {
        public static void Taida(ToodedContext db)
        {
            // vana roll "Müüja" -> "Ostja" (olemasolevad kasutajad)
            db.Kasutajad.Where(k => k.Roll == "Müüja").ExecuteUpdate(s => s.SetProperty(k => k.Roll, "Ostja"));

            if (!db.Kategooriatabel.Any())
            {
                db.Kategooriatabel.AddRange(
                    new Kategooria { Kategooria_nimetus = "Joogid", Kirjeldus = "Karastus- ja energiajoogid, mahlad, vesi" },
                    new Kategooria { Kategooria_nimetus = "Puu- ja köögiviljad", Kirjeldus = "Värsked puu- ja köögiviljad" },
                    new Kategooria { Kategooria_nimetus = "Maiustused", Kirjeldus = "Šokolaad, kommid, küpsised" });
                db.SaveChanges();
            }

            if (!db.Toodetabel.Any())
            {
                int joogid = IdPoolt(db, "Joogid");
                int vili = IdPoolt(db, "Puu- ja köögiviljad");
                int maius = IdPoolt(db, "Maiustused");

                var tooted = new (string Nimi, int Kogus, decimal Hind, int Kat, Color Varv)[]
                {
                    ("Coca-Cola 0,5 l",   40, 1.49m, joogid, Color.FromArgb(200, 30, 30)),
                    ("Mineraalvesi 1,5 l", 60, 0.89m, joogid, Color.FromArgb(70, 140, 210)),
                    ("Apelsinimahl 1 l",   25, 2.19m, joogid, Color.FromArgb(235, 140, 20)),
                    ("Energiajook 0,25 l", 30, 1.79m, joogid, Color.FromArgb(40, 160, 90)),
                    ("Õunad 1 kg",         50, 1.99m, vili,   Color.FromArgb(180, 40, 40)),
                    ("Banaanid 1 kg",      45, 1.59m, vili,   Color.FromArgb(225, 190, 30)),
                    ("Tomatid 1 kg",       30, 3.49m, vili,   Color.FromArgb(210, 60, 40)),
                    ("Kurgid 1 kg",        35, 2.29m, vili,   Color.FromArgb(70, 150, 60)),
                    ("Piimašokolaad 100 g", 80, 1.29m, maius, Color.FromArgb(120, 70, 40)),
                    ("Kommid 200 g",       55, 2.49m, maius,  Color.FromArgb(200, 70, 150)),
                    ("Küpsised 150 g",     4,  1.99m, maius,  Color.FromArgb(190, 140, 70)),
                    ("Marmelaad 300 g",    0,  2.99m, maius,  Color.FromArgb(130, 40, 90)),
                };

                foreach (var t in tooted)
                {
                    db.Toodetabel.Add(new Toode
                    {
                        Toodenimetus = t.Nimi,
                        Kogus = t.Kogus,
                        Hind = t.Hind,
                        KategooriaId = t.Kat,
                        Pilt = LooPilt(t.Nimi, t.Varv)
                    });
                }
                db.SaveChanges();
            }

            if (!db.Kasutajad.Any())
            {
                var (h1, s1) = ParoolAbi.Loo("Omanik123");
                var (h2, s2) = ParoolAbi.Loo("Ostja123");
                db.Kasutajad.AddRange(
                    new Kasutaja { Kasutajanimi = "omanik", TaisNimi = "Pood Omanik", ParoolHash = h1, Sool = s1, Roll = Rollid.Omanik },
                    new Kasutaja { Kasutajanimi = "ostja", TaisNimi = "Mari Ostja", ParoolHash = h2, Sool = s2, Roll = Rollid.Ostja });
                db.SaveChanges();
            }

            if (!db.Kliendid.Any())
            {
                db.Kliendid.AddRange(
                    new Klient { Nimi = "Jaan Tamm",  Epost = "jaan.tamm@example.com",  Telefon = "5551 1111", Kliendikaart = "K0001", Boonus = 5.00m },
                    new Klient { Nimi = "Mari Mägi",  Epost = "mari.magi@example.com",  Telefon = "5552 2222", Kliendikaart = "K0002", Boonus = 12.50m },
                    new Klient { Nimi = "Peeter Kask", Epost = "peeter.kask@example.com", Telefon = "5553 3333", Kliendikaart = "K0003", Boonus = 0m });
                db.SaveChanges();
            }
        }

        private static int IdPoolt(ToodedContext db, string nimi) =>
            db.Kategooriatabel.First(k => k.Kategooria_nimetus == nimi).Id;

        /// <summary>Teeb lihtsa värvilise pildi toote nimega ja salvestab Images-kausta. Tagastab failinime.</summary>
        private static string? LooPilt(string nimi, Color varv)
        {
            try
            {
                Directory.CreateDirectory(Seaded.PildiKaust);
                string fail = new string(nimi.Where(c => char.IsLetterOrDigit(c)).ToArray()) + ".png";
                string tee = Path.Combine(Seaded.PildiKaust, fail);
                if (File.Exists(tee)) return fail;

                using var bmp = new Bitmap(300, 300);
                using (var g = Graphics.FromImage(bmp))
                using (var taust = new SolidBrush(varv))
                using (var font = new Font("Segoe UI", 20F, FontStyle.Bold))
                using (var valge = new SolidBrush(Color.White))
                {
                    g.Clear(Teema.Paneel2);
                    g.FillEllipse(taust, 30, 30, 240, 240);
                    var vorming = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(nimi, font, valge, new RectangleF(50, 50, 200, 200), vorming);
                }
                bmp.Save(tee, ImageFormat.Png);
                return fail;
            }
            catch
            {
                return null;   // pilt ei ole kohustuslik
            }
        }
    }
}
