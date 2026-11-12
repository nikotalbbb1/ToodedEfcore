using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Rakendab puuduvad migratsioonid (kui oled teinud Add-Migration).
            try
            {
                using var db = new ToodedContext();
                db.Database.Migrate();
                Seemnes.Taida(db);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Andmebaasiga ühendamine ebaõnnestus:\n" + ex.GetBaseException().Message +
                                "\n\nKontrolli, et oled käivitanud Package Manager Console'is:\nAdd-Migration Kassa\nUpdate-Database",
                                "Viga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            while (true)
            {
                if (!Seanss.OnSisse)
                {
                    DialogResult tulemus;
                    using (var login = new LoginForm())
                        tulemus = login.ShowDialog();
                    if (tulemus == DialogResult.Retry) continue;   // kasutaja vahetas teemat: näita sisselogimist uuesti
                    if (tulemus != DialogResult.OK) break;
                }

                // Omanik näeb menüüd (Tooted, Kassa, Kliendid ...), Ostja saab ainult Kassa-vormi.
                Form pea = Seanss.OnOmanik ? new MenuForm() : new KassaForm();
                Application.Run(pea);
                pea.Dispose();

                // Teema vahetati menüüs: avame menüü uuesti, kasutaja jääb sisse.
                if (Teema.TeemaVahetatud) { Teema.TeemaVahetatud = false; continue; }

                // Kui kasutaja logis välja, on Seanss.Kasutaja tühi ja näitame uuesti sisselogimist.
                // Kui ta sulges akna ristist, lõpetame programmi.
                if (Seanss.OnSisse) break;
            }
        }
    }
}
