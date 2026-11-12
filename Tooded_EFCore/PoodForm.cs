using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore
{
    /// <summary>Pood: kategooriad vahelehtedel, tootekaardid, ostukorvi nupp päises.</summary>
    public class PoodForm : Form
    {
        private readonly ToodedContext _db = new();
        private readonly TabControl tabPood = new();
        private readonly RoundButton _korv;

        public PoodForm()
        {
            Load += (s_, e_) => Teema.Rakenda(this);
            Text = "Pood";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(930, 690);

            tabPood.Dock = DockStyle.Fill;
            tabPood.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabPood.SizeMode = TabSizeMode.Fixed;
            tabPood.ItemSize = new Size(170, 38);
            tabPood.DrawItem += (s, e) =>
            {
                var leht = tabPood.TabPages[e.Index];
                bool valitud = e.Index == tabPood.SelectedIndex;
                using var taust = new SolidBrush(valitud ? Teema.Aksent : Teema.Paneel);
                e.Graphics.FillRectangle(taust, e.Bounds);
                using var fond = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                TextRenderer.DrawText(e.Graphics, leht.Text, fond, e.Bounds,
                    valitud ? Color.White : Teema.HallTekst,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            };
            Controls.Add(tabPood);

            // päis: pealkiri + ostukorvi nupp
            var pais = new Panel { Dock = DockStyle.Top, Height = 66, BackColor = Teema.Taust };
            pais.Controls.Add(new Pealkiri
            {
                Text = "Pood", Font = new Font("Segoe UI", 20F, FontStyle.Bold), ForeColor = Teema.Tume,
                Left = 18, Top = 12, Width = 240, Height = 40
            });
            _korv = Teema.Nupp("", Teema.Aksent, ClientSize.Width - 300, 12, 282, 42, 11F);
            _korv.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _korv.Click += (s, e) =>
            {
                using var arve = new ArveForm();
                arve.ShowDialog(this);
                UuendaKorv();
            };
            pais.Controls.Add(_korv);
            Controls.Add(pais);

            LaePood();
            UuendaKorv();
        }

        private void UuendaKorv()
        {
            int tk = Ostukorv.Ridad.Sum(r => r.Kogus);
            _korv.Text = $"Korv:  {tk} tk  ·  {Ostukorv.Kokku:F2} €";
        }

        private void LaePood()
        {
            tabPood.TabPages.Clear();
            var kategooriad = _db.Kategooriatabel
                .Include(k => k.Tooded)
                .OrderBy(k => k.Kategooria_nimetus)
                .ToList();

            foreach (var kat in kategooriad)
            {
                var leht = new TabPage(kat.Kategooria_nimetus)
                {
                    AutoScroll = true, UseVisualStyleBackColor = false, BackColor = Teema.Taust
                };
                int i = 0;
                foreach (var toode in kat.Tooded.OrderBy(t => t.Toodenimetus))
                {
                    var kaart = LooTootekaart(toode);
                    kaart.Location = new Point(14 + (i % 5) * 178, 14 + (i / 5) * 258);
                    leht.Controls.Add(kaart);
                    i++;
                }
                tabPood.TabPages.Add(leht);
            }
        }

        private Panel LooTootekaart(Toode t)
        {
            var kaart = new KaartPanel { Size = new Size(160, 244) };

            var pilt = new PictureBox
            {
                Bounds = new Rectangle(8, 8, 144, 126),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Teema.Paneel2,
                Image = PildiHoidla.Lae(t.Pilt)
            };
            var nimi = new Label
            {
                Bounds = new Rectangle(8, 138, 144, 22),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Teema.Tume,
                Text = t.Toodenimetus
            };
            var hind = new Label
            {
                Bounds = new Rectangle(8, 160, 144, 26),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = t.Kogus > 0 ? Teema.Aksent : Teema.Punane,
                Text = t.Kogus > 0 ? $"{t.Hind:F2} €" : "OTSAS"
            };
            var kogus = new NumericUpDown
            {
                Bounds = new Rectangle(8, 202, 60, 27),
                Minimum = 1,
                Maximum = Math.Max(1, t.Kogus),   // rohkem kui laos ei saa valida
                Value = 1
            };
            var nupp = Teema.Nupp("Lisa korvi", Teema.Aksent, 74, 198, 78, 34, 9F);
            nupp.Alus = Teema.Paneel;
            nupp.Raadius = 8;
            nupp.Enabled = t.Kogus > 0;
            nupp.Click += (s, e) =>
            {
                int juba = Ostukorv.Ridad.FirstOrDefault(r => r.Toode.Id == t.Id)?.Kogus ?? 0;
                if (juba + (int)kogus.Value > t.Kogus)
                {
                    Teade.Viga($"Laos on ainult {t.Kogus} tk (korvis juba {juba} tk).");
                    return;
                }
                Ostukorv.Lisa(t, (int)kogus.Value);
                UuendaKorv();
                Teema.Teavita(this, $"{t.Toodenimetus} × {kogus.Value} lisati korvi");
            };
            pilt.Click += (s, e) =>
                Teade.Info($"{t.Toodenimetus}\nHind: {t.Hind:F2} €\nLaos: {t.Kogus} tk");

            kaart.Controls.AddRange(new Control[] { pilt, nimi, hind, kogus, nupp });
            return kaart;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _db.Dispose();
            base.OnFormClosed(e);
        }
    }
}
