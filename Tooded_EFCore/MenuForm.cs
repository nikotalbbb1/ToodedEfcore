namespace Tooded_EFCore
{
    /// <summary>Omaniku peamenüü: lihtne päis, kaardid ja teemavalija.</summary>
    public class MenuForm : Form
    {
        private int _i;

        public MenuForm()
        {
            Text = "Peamenüü";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            ClientSize = new Size(460, 580);
            BackColor = Teema.Taust;
            Teema.TumeTiitel(this);

            var pais = new GradPanel { Dock = DockStyle.Top, Height = 90 };
            pais.Controls.Add(new Pealkiri
            {
                Text = Seaded.PoeNimi, Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                Left = 20, Top = 10, Width = 400, Height = 42
            });
            pais.Controls.Add(new Label
            {
                Text = $"{Seanss.Nimi}  ·  {Seanss.Kasutaja?.Roll}", ForeColor = Color.FromArgb(230, 235, 245),
                BackColor = Color.Transparent, Font = new Font("Segoe UI", 10.5F), Left = 22, Top = 56, AutoSize = true
            });
            Controls.Add(pais);

            Kaart("Tooted", "Tooted ja pildid", Teema.Aksent, () => Ava(new Form1()));
            Kaart("Kassa", "Müük ja boonus", Teema.Aksent, () => Ava(new KassaForm()));
            Kaart("Kliendid", "Kaardid ja boonused", Teema.Aksent, () => Ava(new AdminForm()));
            Kaart("Müügiajalugu", "Tšekid ja PDF", Teema.Aksent, () => Ava(new MuugiForm()));
            Kaart("Pood", "Kategooriate kaupa", Teema.Aksent, () => Ava(new PoodForm()));
            Kaart("Ostukorv ja arve", "PDF ja e-post", Teema.Aksent, () => Ava(new ArveForm()));
            Kaart("Registreeri kasutaja", "Uus konto", Teema.Aksent, () => Ava(new RegisterForm()));
            Kaart("Logi välja", "Välju kontolt", Teema.Punane, () =>
            {
                Seanss.Kasutaja = null;
                Close();
            });

            // teemavalija: vahetab värvid ja avab menüü uuesti (kasutaja jääb sisse)
            foreach (var c in Teema.TeemaValija(20, 534, 134, 9, 34, () => { Teema.TeemaVahetatud = true; Close(); }))
                Controls.Add(c);
        }

        private void Kaart(string tekst, string alam, Color vari, Action tegevus)
        {
            int x = 20 + (_i % 2) * 220;
            int y = 110 + (_i / 2) * 100;
            var b = Teema.Nupp(tekst, vari, x, y, 200, 88, 12.5F, alam);
            b.Raadius = 10;
            b.Click += (s, e) => tegevus();
            Controls.Add(b);
            _i++;
        }

        private void Ava(Form vorm)
        {
            using (vorm) vorm.ShowDialog(this);
        }
    }
}
